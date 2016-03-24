using UnityEngine;

public class Fragment : MonoBehaviour {
    public string fragmentName;

	public RockFall rockTrigger;

    public int index;

    private void Start()
    {
        if (Application.isPlaying)
        {
            PlayerController.RegisterFragment(this); // We must wait for when the PlayerController will be initialized so we use Start
        }
    }

    void Update(){
        transform.Rotate(new Vector3(0, 30, 0) * Time.deltaTime);
    }
}
