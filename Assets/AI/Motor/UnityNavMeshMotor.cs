using RAIN.Motion;
using RAIN.Serialization;
using UnityEngine;

[RAINSerializableClass]
public class UnityNavMeshMotor : RAINMotor {
	[RAINSerializableField]
	private float defaultCloseEnoughDistance = 0.1F;
	[RAINSerializableField]
	private float defaultSpeed = 3F;
	[RAINSerializableField]
	private float defaultAcceleration = 3F;

	private Vector3 lastPosition = Vector3.zero;

	private NavMeshAgent agent = null;


	public float DefaultAcceleration {
		get { return defaultAcceleration; }
		set { defaultAcceleration = value; }
	}

	public float Acceleration { get; set; }

	public override float DefaultCloseEnoughDistance {
		get { return defaultCloseEnoughDistance; }
		set { defaultCloseEnoughDistance = value; }
	}

	public override float DefaultSpeed {
		get { return defaultSpeed; }
		set { defaultSpeed = value; }
	}

	// It seems like the NavMeshAgent forces positions on
	// the NavMesh, so 3D Movement doesn't make sense
	public override bool Allow3DMovement {
		get { return false; }
		set { }
	}

	// Don't support this at the moment, not sure if the NavMeshAgent
	// can go off of the mesh
	public override bool AllowOffGraphMovement {
		get { return false; }
		set { }
	}

	// 3D Rotation is technically doable, but not like
	// we support it with the BasicMotor
	public override bool Allow3DRotation {
		get { return false; }
		set { }
	}

	public override void Start() {
		base.Start();

		Acceleration = DefaultAcceleration;
	}

	public override void BodyInit() {
		base.BodyInit();

		if(AI.Body == null)
			agent = null;
		else {
			agent = AI.Body.GetComponent<NavMeshAgent>();
			if(agent == null)
				agent = AI.Body.AddComponent<NavMeshAgent>();
		}
	}

	public override void UpdateMotionTransforms() {
		// I don't believe the Unity Navigation Mesh can handle transforms, so this stays as identity
		AI.Kinematic.ParentTransform = Matrix4x4.identity;
		AI.Kinematic.Position = AI.Body.transform.position;
		AI.Kinematic.Orientation = AI.Body.transform.rotation.eulerAngles;

		// Velocities likely won't matter as we never actually use them in this motor
		AI.Kinematic.ResetVelocities();

		// Set our speed to zero, we'll set it when we are using it
		agent.speed = 0;
	}

	public override void ApplyMotionTransforms() {
	}

	public override bool Move() {
		if(!MoveTarget.IsValid)
			return false;

		// Set our speed
		agent.speed = Speed;

		// Set our acceleration
		agent.acceleration = Acceleration;

		// We'll just update these constantly as our value can change when the MoveTarget changes
		agent.stoppingDistance = Mathf.Max(DefaultCloseEnoughDistance, MoveTarget.CloseEnoughDistance);

		SetAreaCosts(AI.Kinematic.Position, MoveTarget.Position);

		// Have to make sure the target is still in the same place
		Vector3 tEndMoved = lastPosition - MoveTarget.Position;
		tEndMoved.y = 0;

		// If we don't have a path or our target moved
		if(!agent.hasPath || !Mathf.Approximately(tEndMoved.sqrMagnitude, 0)) {
			agent.destination = MoveTarget.Position;
			lastPosition = MoveTarget.Position;

			agent.GetComponent<Entity>().DrawPath();

			// We can return at least if we are at our destination at this point
			return IsAt(agent.destination);
		}

		agent.GetComponent<Entity>().DrawPath();

		// Still making a path or our path is invalid
		if(agent.pathPending || agent.pathStatus == NavMeshPathStatus.PathInvalid)
			return false;

		return agent.remainingDistance <= agent.stoppingDistance;
	}

	public override bool IsAt(Vector3 aPosition) {
		Vector3 tPosition = AI.Body.transform.position - aPosition;
		tPosition.y = 0;

		return tPosition.magnitude <= agent.stoppingDistance;
	}

	public override bool IsAt(MoveLookTarget aTarget) {
		return IsAt(aTarget.Position);
	}

	public override bool Face() {
		// Too tired to do this
		return true;
	}

	public override bool IsFacing(Vector3 aPosition) {
		// Too tired to do this
		return true;
	}

	public override bool IsFacing(MoveLookTarget aTarget) {
		// Too tired to do this
		return true;
	}

	public override void Stop() {
		agent.Stop();
	}


	protected virtual void SetAreaCosts(Vector3 position, Vector3 target) {
		StreamController.SetAreaCosts(position, target);
	}
}