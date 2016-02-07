using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
    public Rigidbody rigidBody;
    public float forceTurn;
    public float forceFoward;
    GameObject player;
    Animator animatorl;
    ArrayList memoryfragments;




	// Use this for initialization
	void Start () {
        memoryfragments = new ArrayList();
        rigidBody = GetComponent<Rigidbody>();


    }

    // Update is called once per frame
    void Update () {
	    
	}
    // fixed update
    void FixedUpdate()
    {
        playerMouvement();

    }

    void playerMouvement()
    {
        
        
        float moveFoward = -(Input.GetAxis("LeftJoystickY"));
        float turn = Input.GetAxis("LeftJoystickX");
        Vector3 movement;
        movement = new Vector3(0.0f, forceTurn*turn, 0.0f);

        if(moveFoward<1)
        {
            moveFoward = moveFoward*0.125f;
            turn = turn * -1;  
        }
        rigidBody.AddForce(transform.forward *moveFoward*forceFoward,ForceMode.Acceleration);


        
        rigidBody.AddTorque(movement, ForceMode.Force);
        float lift =  0.1f* rigidBody.velocity.sqrMagnitude;
        rigidBody.AddForceAtPosition(lift * transform.right, transform.position);


        float yVel = rigidBody.velocity.y;
        Vector3 localV = transform.InverseTransformDirection(rigidBody.velocity);
        localV.x = 0.0f;
        localV.y = 0.0f;
        localV.z = localV.z * 0.025f;
        Vector3 worldV = transform.TransformDirection(localV);
        worldV.y = yVel;
        rigidBody.velocity = worldV;
        //rigidBody.rotation.
    }

    void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("fragment"))
        {
            other.gameObject.SetActive(false);
            Fragments fragment;
            fragment = new Fragments();
            addFragment(fragment);
        }

    }
    

    void addFragment(Fragments fragments)
    {
        memoryfragments.Add(fragments);
    }

    ArrayList getFragments()
    {
        return memoryfragments;
    }

    void receiveInputEvent(Input inputEvent)
    {
        
    }
}
