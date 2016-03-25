using UnityEngine;
using System.Collections;
using RAIN.Core;
using RAIN.Perception;
using RAIN.Perception.Sensors;

public class StunEntity : Entity {

    //private Rigidbody rigidBody;
	private AIRig ai;
	private LineRenderer line;

	private bool coolDown = false;

    protected override void Start() {
		base.Start();
        //rigidBody = GetComponent<Rigidbody>();
		ai = GetComponentInChildren<AIRig>();

		if(debug) {
			SetupLineRenderer();
			wasDebugging = true;
		}
    }

	private void Update() {
		if(debug) {
			if(!wasDebugging)
				SetupLineRenderer();
			
			if(coolDown != ai.AI.WorkingMemory.GetItem<bool>("coolDown")){
				coolDown = !coolDown;
				if(coolDown) {
					line.SetColors(Color.red, Color.red);
				} else {
					line.SetColors(Color.green, Color.green);
				}
			}
		} else if(wasDebugging) {
			if(line != null) {
				line.enabled = false;
			}
			wasDebugging = false;
		}
	}

    public override bool ReceiveHit() {
		return false;
    }

    public override void ReceiveStun() {
    }

    protected override void OnTriggerStay(Collider other) {
    }

	protected override void SetupLineRenderer()
	{
		VisualSensor sight = ai.AI.Senses.GetSensor("Sight") as VisualSensor;
		float x;
		float y = 0f;
		float z;
		int segments = 20;
		float angle = 20f;

		line = GetComponent<LineRenderer>();
		if(line == null) {
			line = gameObject.AddComponent<LineRenderer>();
			line.material = new Material(Shader.Find("Sprites/Default")) { };
			line.SetWidth(0.1f, 0.1f);
			line.SetColors(Color.green, Color.green);
			line.SetVertexCount (segments + 1);
			line.useWorldSpace = false;
		} else {
			line.enabled = true;
		}

		for (int i = 0; i < (segments + 1); i++)
		{
			x = Mathf.Sin (Mathf.Deg2Rad * angle) * sight.Range;
			z = Mathf.Cos (Mathf.Deg2Rad * angle) * sight.Range;

			line.SetPosition (i,new Vector3(x,y,z) );

			angle += (360f / segments);
		}
	}
}
