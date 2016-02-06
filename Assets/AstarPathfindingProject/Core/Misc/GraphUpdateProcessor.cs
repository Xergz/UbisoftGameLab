using System.Collections.Generic;
using System.Collections;
using System.Threading;
using UnityEngine;

namespace Pathfinding {
	using Pathfinding;

#if NETFX_CORE
	using Thread = Pathfinding.WindowsStore.Thread;
	using ParameterizedThreadStart = Pathfinding.WindowsStore.ParameterizedThreadStart;
#else
	using Thread = System.Threading.Thread;
	using ParameterizedThreadStart = System.Threading.ParameterizedThreadStart;
#endif

	class GraphUpdateProcessor {

		public event System.Action OnGraphsUpdated;

		/** Holds graphs that can be updated */
		readonly AstarPath astar;

#if !UNITY_WEBGL
		/**
		 * Reference to the thread which handles async graph updates.
		 * \see ProcessGraphUpdatesAsync
		 */
		Thread graphUpdateThread;
#endif

		/**
		 * Stack containing all waiting graph update queries. Add to this stack by using \link UpdateGraphs \endlink
		 * \see UpdateGraphs
		 */
		readonly Queue<GraphUpdateObject> graphUpdateQueue = new Queue<GraphUpdateObject>();

		/** Queue of all async graph updates waiting to be executed */
		readonly Queue<GUOSingle> graphUpdateQueueAsync = new Queue<GUOSingle>();

		/** Queue of all non-async graph updates waiting to be executed */
		readonly Queue<GUOSingle> graphUpdateQueueRegular = new Queue<GUOSingle>();

		readonly System.Threading.AutoResetEvent graphUpdateAsyncEvent = new System.Threading.AutoResetEvent(false);

		readonly System.Threading.ManualResetEvent asyncGraphUpdatesComplete = new System.Threading.ManualResetEvent(true);

#if !UNITY_WEBGL
		readonly System.Threading.AutoResetEvent exitAsyncThread = new System.Threading.AutoResetEvent(false);
#endif

		/** Returns if any graph updates are waiting to be applied */
		public bool IsAnyGraphUpdateQueued { get { return graphUpdateQueue.Count > 0; }}

		/** The last area index which was used.
		 * Used for the \link FloodFill(GraphNode node) FloodFill \endlink function to start flood filling with an unused area.
		 * \see FloodFill(Node node)
		 */
		uint lastUniqueAreaIndex = 0;

		/**
		 * Stack used for flood-filling the graph.
		 * It is cached to reduce memory allocations
		 */
		Stack<GraphNode> floodStack;

		/** Order type for updating graphs */
		enum GraphUpdateOrder {
			GraphUpdate,
			FloodFill
		}

		/** Holds a single update that needs to be performed on a graph */
		struct GUOSingle {
			public GraphUpdateOrder order;
			public IUpdatableGraph graph;
			public GraphUpdateObject obj;
		}

		public GraphUpdateProcessor (AstarPath astar) {
			this.astar = astar;
		}

		/** Work item which can be used to apply all queued updates */
		public AstarWorkItem GetWorkItem () {
			return new AstarWorkItem(QueueGraphUpdatesInternal, ProcessGraphUpdates);
		}

		public void EnableMultithreading () {
#if !UNITY_WEBGL
			if (graphUpdateThread == null || !graphUpdateThread.IsAlive) {
				graphUpdateThread = new Thread (ProcessGraphUpdatesAsync);
				graphUpdateThread.IsBackground = true;

				// Set the thread priority for graph updates
				// Unless compiling for windows store or windows phone which does not support it
#if !UNITY_WINRT
				graphUpdateThread.Priority = System.Threading.ThreadPriority.Lowest;
#endif
				graphUpdateThread.Start (this);
			}
#endif
		}

		public void DisableMultithreading () {
#if !UNITY_WEBGL
			if (graphUpdateThread != null && graphUpdateThread.IsAlive) {
				//Resume graph update thread, will cause it to terminate
				exitAsyncThread.Set();

				if (!graphUpdateThread.Join(20*1000)) {
					Debug.LogError("Graph update thread did not exit in 20 seconds");
				}

				graphUpdateThread = null;
			}
#endif
		}

		/** Update all graphs using the GraphUpdateObject.
		 * This can be used to, e.g make all nodes in an area unwalkable, or set them to a higher penalty.
		 * The graphs will be updated as soon as possible (with respect to #limitGraphUpdates)
		 *
		 * \see FlushGraphUpdates
		*/
		public void UpdateGraphs (GraphUpdateObject ob) {
			//Put the GUO in the queue
			graphUpdateQueue.Enqueue (ob);
		}

		/** Schedules graph updates internally */
		void QueueGraphUpdatesInternal () {
			bool anyRequiresFloodFill = false;

			while (graphUpdateQueue.Count > 0) {
				GraphUpdateObject ob = graphUpdateQueue.Dequeue ();

				if (ob.requiresFloodFill) anyRequiresFloodFill = true;

				foreach (IUpdatableGraph g in astar.astarData.GetUpdateableGraphs ()) {
					NavGraph gr = g as NavGraph;
					if (ob.nnConstraint == null || ob.nnConstraint.SuitableGraph (astar.astarData.GetGraphIndex (gr),gr)) {
						var guo = new GUOSingle ();
						guo.order = GraphUpdateOrder.GraphUpdate;
						guo.obj = ob;
						guo.graph = g;
						graphUpdateQueueRegular.Enqueue (guo);
					}
				}
			}

			if (anyRequiresFloodFill) {
				var guo = new GUOSingle();
				guo.order = GraphUpdateOrder.FloodFill;
				graphUpdateQueueRegular.Enqueue (guo);
			}

			GraphModifier.TriggerEvent (GraphModifier.EventType.PreUpdate);
		}

		/** Updates graphs.
		 * Will do some graph updates, possibly signal another thread to do them.
		 * Will only process graph updates added by QueueGraphUpdatesInternal
		 *
		 * \param force If true, all graph updates will be processed before this function returns. The return value
		 * will be True.
		 *
		 * \returns True if all graph updates have been done and pathfinding (or other tasks) may resume.
		 * False if there are still graph updates being done or waiting in the queue.
		 *
		 *
		 */
		bool ProcessGraphUpdates (bool force) {

			if (force) {
				asyncGraphUpdatesComplete.WaitOne ();
			} else {
#if !UNITY_WEBGL
				if (!asyncGraphUpdatesComplete.WaitOne (0)) {
					return false;
				}
#endif
			}

			if (graphUpdateQueueAsync.Count != 0) throw new System.Exception ("Queue should be empty at this stage");

			while (graphUpdateQueueRegular.Count > 0) {

				GUOSingle s = graphUpdateQueueRegular.Peek ();

				GraphUpdateThreading threading = s.order == GraphUpdateOrder.FloodFill ? GraphUpdateThreading.SeparateThread : s.graph.CanUpdateAsync(s.obj);

#if !UNITY_WEBGL
				bool forceUnityThread = force;

				// When not playing or when not using a graph update thread (or if it has crashed), everything runs in the Unity thread
				if ( !Application.isPlaying || graphUpdateThread == null || !graphUpdateThread.IsAlive ) {
					forceUnityThread = true;
				}

				if (!forceUnityThread && (threading == GraphUpdateThreading.SeparateAndUnityInit)) {
					if (graphUpdateQueueAsync.Count > 0) {
						//Process async graph updates first.

						//Next call to this function will process this object so it is not dequeued now
						asyncGraphUpdatesComplete.Reset ();
						graphUpdateAsyncEvent.Set ();

						return false;
					}

					s.graph.UpdateAreaInit (s.obj);

					//Move GUO to async queue to be updated by another thread
					graphUpdateQueueRegular.Dequeue ();
					graphUpdateQueueAsync.Enqueue (s);

					//Next call to this function will process this object so it is not dequeued now
					asyncGraphUpdatesComplete.Reset ();
					graphUpdateAsyncEvent.Set ();

					return false;
				} else if (!forceUnityThread && (threading == GraphUpdateThreading.SeparateThread)) {
					//Move GUO to async queue to be updated by another thread
					graphUpdateQueueRegular.Dequeue ();
					graphUpdateQueueAsync.Enqueue (s);
				} else {
#endif
					//Everything should be done in the unity thread

					if (graphUpdateQueueAsync.Count > 0) {
						//Process async graph updates first.

						if (force) throw new System.Exception ("This should not happen");

						//Next call to this function will process this object so it is not dequeued now
						asyncGraphUpdatesComplete.Reset ();
						graphUpdateAsyncEvent.Set ();

						return false;
					}

					graphUpdateQueueRegular.Dequeue ();

					if (s.order == GraphUpdateOrder.FloodFill) {
						FloodFill ();
					} else {
						if (threading == GraphUpdateThreading.SeparateAndUnityInit) {
							try {
								s.graph.UpdateAreaInit (s.obj);
							} catch (System.Exception e) {
								Debug.LogError ("Error while initializing GraphUpdates\n" + e);
							}
						}
						try {
							s.graph.UpdateArea (s.obj);
						} catch (System.Exception e) {
							Debug.LogError ("Error while updating graphs\n"+e);
						}
					}
#if !UNITY_WEBGL
				}
#endif
			}

#if !UNITY_WEBGL
			if (graphUpdateQueueAsync.Count > 0) {

				//Next call to this function will process this object so it is not dequeued now
				asyncGraphUpdatesComplete.Reset ();
				graphUpdateAsyncEvent.Set ();

				return false;
			}
#endif

			GraphModifier.TriggerEvent (GraphModifier.EventType.PostUpdate);
			if (OnGraphsUpdated != null) OnGraphsUpdated();

			return true;
		}

#if !UNITY_WEBGL
		/** Graph update thread.
		 * Async graph updates will be executed by this method in another thread.
		 */
		void ProcessGraphUpdatesAsync () {
			var handles = new [] { graphUpdateAsyncEvent, exitAsyncThread };

			while(true) {
				// Wait for the next batch or exit event
				var handleIndex = WaitHandle.WaitAny(handles);

				if (handleIndex == 1) {
					// Exit even was fired
					//Abort thread and clear queue
					graphUpdateQueueAsync.Clear ();
					asyncGraphUpdatesComplete.Set ();
					return;
				}

				while (graphUpdateQueueAsync.Count > 0) {
					GUOSingle aguo = graphUpdateQueueAsync.Dequeue ();

					try {
						if (aguo.order == GraphUpdateOrder.GraphUpdate) {
							aguo.graph.UpdateArea (aguo.obj);
						} else if (aguo.order == GraphUpdateOrder.FloodFill) {
							FloodFill ();
						} else {
							throw new System.NotSupportedException ("" + aguo.order);
						}
					} catch (System.Exception e) {
						Debug.LogError ("Exception while updating graphs:\n"+e);
					}
				}

				// Done
				asyncGraphUpdatesComplete.Set ();
			}
		}
#endif

		/** Floodfills starting from the specified node */
		public void FloodFill (GraphNode seed) {
			FloodFill (seed, lastUniqueAreaIndex+1);
			lastUniqueAreaIndex++;
		}

		/** Floodfills starting from 'seed' using the specified area */
		public void FloodFill (GraphNode seed, uint area) {

			if (area > GraphNode.MaxAreaIndex) {
				Debug.LogError ("Too high area index - The maximum area index is " + GraphNode.MaxAreaIndex);
				return;
			}

			if (area < 0) {
				Debug.LogError ("Too low area index - The minimum area index is 0");
				return;
			}

			if (floodStack == null) {
				floodStack = new Stack<GraphNode> (1024);
			}

			Stack<GraphNode> stack = floodStack;

			stack.Clear ();

			stack.Push (seed);
			seed.Area = (uint)area;

			while (stack.Count > 0) {
				stack.Pop ().FloodFill (stack,(uint)area);
			}

		}

		/** Floodfills all graphs and updates areas for every node.
		 * The different colored areas that you see in the scene view when looking at graphs
		 * are called just 'areas', this method calculates which nodes are in what areas.
		 * \see Pathfinding.Node.area
		 */
		[ContextMenu("Flood Fill Graphs")]
		public void FloodFill () {


			if (astar.astarData.graphs == null) {
				return;
			}

			uint area = 0;

			lastUniqueAreaIndex = 0;

			if (floodStack == null) {
				floodStack = new Stack<GraphNode> (1024);
			}

			Stack<GraphNode> stack = floodStack;

			var graphs = astar.graphs;
			// Iterate through all nodes in all graphs
			// and reset their Area field
			for (int i = 0;i < graphs.Length;i++) {
				NavGraph graph = graphs[i];

				if (graph != null) {
					graph.GetNodes (node => {
						node.Area = 0;
						return true;
					});
				}
			}

			int smallAreasDetected = 0;

			bool warnAboutAreas = false;

			List<GraphNode> smallAreaList = Pathfinding.Util.ListPool<GraphNode>.Claim();

			for (int i = 0; i < graphs.Length; i++) {

				NavGraph graph = graphs[i];

				if (graph == null) continue;

				GraphNodeDelegateCancelable del = delegate (GraphNode node) {
					if (node.Walkable && node.Area == 0) {

						area++;

						uint thisArea = area;

						if (area > GraphNode.MaxAreaIndex) {
							if ( smallAreaList.Count > 0 ) {
								GraphNode smallOne = smallAreaList[smallAreaList.Count-1];
								thisArea = smallOne.Area;
								smallAreaList.RemoveAt (smallAreaList.Count-1);

								//Flood fill the area again with area ID GraphNode.MaxAreaIndex-1, this identifies a small area
								stack.Clear ();

								stack.Push (smallOne);
								smallOne.Area = GraphNode.MaxAreaIndex;

								while (stack.Count > 0) {
									stack.Pop ().FloodFill (stack,GraphNode.MaxAreaIndex);
								}

								smallAreasDetected++;
							} else {
								// Forced to consider this a small area
								area--;
								thisArea = area;
								warnAboutAreas = true;
							}
						}

						stack.Clear ();

						stack.Push (node);

						int counter = 1;

						node.Area = thisArea;

						while (stack.Count > 0) {
							counter++;
							stack.Pop ().FloodFill (stack,thisArea);
						}

						if (counter < astar.minAreaSize) {
							smallAreaList.Add ( node );
						}
					}
					return true;
				};

				graph.GetNodes (del);
			}

			lastUniqueAreaIndex = area;

			if (warnAboutAreas) {
				Debug.LogError ("Too many areas - The maximum number of areas is " + GraphNode.MaxAreaIndex +". Try raising the A* Inspector -> Settings -> Min Area Size value. Enable the optimization ASTAR_MORE_AREAS under the Optimizations tab.");
			}

			if (smallAreasDetected > 0) {
				astar.Log (smallAreasDetected +" small areas were detected (fewer than " + astar.minAreaSize + " nodes)," +
				          "these might have the same IDs as other areas, but it shouldn't affect pathfinding in any significant way (you might get All Nodes Searched as a reason for path failure)." +
				          "\nWhich areas are defined as 'small' is controlled by the 'Min Area Size' variable, it can be changed in the A* inspector-->Settings-->Min Area Size" +
				          "\nThe small areas will use the area id "+ GraphNode.MaxAreaIndex);
			}

			Pathfinding.Util.ListPool<GraphNode>.Release (smallAreaList);

		}
	}
}
