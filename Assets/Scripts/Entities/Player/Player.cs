using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : Entity
{
    public PlayerController PlayerController { get; set; } // The PlayerController linked to this player
    public UIManager ui { get; set; }

    public List<GameObject> objectsForBlink;

    public GameObject stunStars;

    [SerializeField]
    private bool isStuned = false;

    [SerializeField]
    private bool isInvincible = false;

    [SerializeField]
    private float stunTime = 2f;
    [SerializeField]
    private float invincibleTime = 1f;
    [SerializeField]
    private float delayBeforeCollisionSFX = 2f;
    private float beginStunTime;
    private float beginInvincibleTime;

    private float lastTimeCollisionSFX = 0.0f;

    [SerializeField]
    private ParticleSystem collisionSystem;

    private void Awake()
    {
        ui = FindObjectOfType<UIManager>();
    }

    private void Update()
    {
        if (isStuned && Time.time > beginStunTime + stunTime)
        {
            isStuned = false;
            stunStars.SetActive(false);
            PlayerController.PlayerCanBeMoved = true;
        }

        if (isInvincible && Time.time > beginInvincibleTime + invincibleTime)
        {
            isInvincible = false;
        }

    }

    public override void ReceiveHit()
    {
        if (!isInvincible)
        {
            audioController.PlayAudio(AudioController.soundType.receiveHit);
            PlayerController.DamagePlayer(10);
            isInvincible = true;
            beginInvincibleTime = Time.time;
            StartCoroutine(DoBlinks(invincibleTime, 0.2f));
        }
    }

    public override void ReceiveStun()
    {
        audioController.PlayAudio(AudioController.soundType.receiveStun);
        PlayerController.PlayerCanBeMoved = false;
        isStuned = true;
        stunStars.SetActive(true);
        beginStunTime = Time.time;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Environment"))
        {
            if (Time.time - lastTimeCollisionSFX > delayBeforeCollisionSFX)
            {
                audioController.PlayAudio(AudioController.soundType.collision, volume: 0.6f);
                lastTimeCollisionSFX = Time.time;
            }

            collisionSystem.Emit(Random.Range(30, 50));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Fragment"))
        { // Picked up a fragment
            audioController.PlayAudio(AudioController.soundType.collectFragment);
            other.gameObject.SetActive(false);
			Fragment frag = other.GetComponent<Fragment>();
			if(frag.rockTrigger != null)
				frag.rockTrigger.startFall();
			PlayerController.AddFragment(frag);
        }
        else if (other.CompareTag("Zone"))
        { // Entered a zone
            PlayerController.CurrentZone = other.GetComponent<Zone>().ZoneIndex;
            ui.EnterLevel(other.gameObject.name);
        }
        else if (other.CompareTag("Life"))
        {
			if(PlayerController.GetPlayerCurrentLife() < PlayerController.GetPlayerMaxLife()) {
				audioController.PlayAudio(AudioController.soundType.collectLife);
				PlayerController.AddLife(other.GetComponent<LifePickup>().Value);
				other.gameObject.SetActive(false);
			}
        }
    }

    protected override void OnTriggerStay(Collider other)
    {
        if (!isStuned)
        {
            if (other.CompareTag("Stream"))
            { // Is inside a stream
                Vector3 force = other.GetComponent<Stream>().GetForceAtPosition(transform.position);
                PlayerController.AddForce(force, other.GetComponent<Stream>());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Zone"))
        { // Left the zone to enter the open world
            if (Random.Range(0.0f, 1.0f) > 0.5f)
                audioController.PlayAudio(AudioController.soundType.enterOpenWorld);

            PlayerController.CurrentZone = EnumZone.OPEN_WORLD;
            ui.EnterLevel("Open World");
        }
    }

    IEnumerator DoBlinks(float duration, float blinkTime)
    {
        float beginTime = Time.time;
        while (Time.time - beginTime < duration)
        {
            foreach (GameObject element in objectsForBlink)
                element.SetActive(!element.activeSelf);

            yield return new WaitForSeconds(blinkTime);
        }

        foreach (GameObject element in objectsForBlink)
            element.SetActive(true);
    }
}
