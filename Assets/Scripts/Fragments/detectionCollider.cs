using UnityEngine;
using System.Collections;

public class detectionCollider : MonoBehaviour
{

    private bool triggered = false;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !triggered)
        {
            var player = other.GetComponent<Player>();
            player.audioController.PlayAudio(AudioController.soundType.spotFragment);
            triggered = true;
        }
    }
}
