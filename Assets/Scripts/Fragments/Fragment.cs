using UnityEngine;

public class Fragment : MonoBehaviour {
    public string fragmentName;

	public Sprite image;

    public int index;

    private void Start()
    {
        if (Application.isPlaying)
        {
            PlayerController.RegisterFragment(this); // We must wait for when the PlayerController will be initialized so we use Start
        }
    }
}
