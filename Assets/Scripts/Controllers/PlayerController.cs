using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class PlayerController : InputReceiver
{
    public static Rigidbody playerRigidbody;

    public static MusicController Music;

    public static LevelLoading loader;

    public int numberOfFragmentsToWin = 5;


    private static EnumZone c_currentZone;
    public static EnumZone CurrentZone
    {
        get
        {
            return c_currentZone;
        }
        set
        {
            c_currentZone = value;

            if (Music != null)
            {
                Music.OnZoneChanged(c_currentZone);
            }
        }
    }

    private bool canBeMoved = true;
    public bool PlayerCanBeMoved
    {
        get
        {
            return canBeMoved;
        }
        set
        {
            canBeMoved = value;
            if (!canBeMoved)
            {
                forceToApply = Vector3.zero;
            }
        }
    }

    public static bool HasWon { get; set; }
    public static bool isPlayerOnstream { get; set; }

    public static Stream streamPlayer { get; set; }

    [Tooltip("The force to apply to the player when it moves (multiplied by its movement speed multiplier)")]
    public float movementForce;
    [Tooltip("The time in seconds the player takes to rotate 1 unit")]
    public float rotationSpeed;
    [Tooltip("The maximum velocity the player can reach")]
    public float maximumVelocity;
    [Tooltip("The range the player's sight can reach. We should animate any objet within this distance")]
    public float sightRange = 60F;

    public Image lifeBarFill;
    public Image lifeBarRim;

    private static Image lifeBarFillStatic;
    private static Image lifeBarRimStatic;

    public ParticleSystem switchParticles;

    private static ParticleSystem switchParticlesStatic;

    //public PowerController powerController;

    private static float maxFill;
    private static int maxLife;

    public static List<Fragment> memoryFragments; // The list of all the fragments in the player's possession.

    //public Transform Fragments;
    public static List<Transform> fragmentsList; //List of every fragments

    //public static Transform nextFragment;
    public static int nextFragmentIndex;
    public static int nextFragmentFind;
    public static int numberOfFragments;
    public static float nbHearts = 9;

    private float ZSpeedMultiplier = 0; // The current Z speed multiplier
    private float XSpeedMultiplier = 0; // The current X speed multiplier
    private float speedMultiplierBoost = 1; // The current Z speed multiplier

    private float currentVelocity; // The current velocity of the player

    private Vector3 forceToApply;



    [Tooltip("The current life of the player")]
    [SerializeField]
    private int life;

    private static int currentLife;

    public static UIBoostControl uiBoostController;
    private float timeSinceLastBoost = 0.0f;
    private bool powerboost = false;
    private static Player _player = null;

    public static GameObject Player
    {
        get
        {
            if (playerRigidbody != null)
            {
                return playerRigidbody.gameObject;
            }
            return null;
        }
    }

    public static List<Fragment> GetCollectedFragments()
    {
        return memoryFragments;
    }

    public static int GetPlayerCurrentLife()
    {
        return currentLife;
    }

    public static void SFXBoost()
    {
        _player.audioController.PlayAudio(AudioController.soundType.useBoost, volume: 0.2f);
    }

    public static void SFXReverseStream()
    {
        _player.audioController.PlayAudio(AudioController.soundType.reverseStream, volume: 0.2f);
    }

    public static IEnumerator ActivateSwitchFX(EnumStreamColor streamColor)
    {
        if (switchParticlesStatic.isPlaying)
        {
            switchParticlesStatic.Stop();
        }

        Color color;
        switch (streamColor)
        {
            case EnumStreamColor.BLUE:
                color = Color.blue;
                break;
            case EnumStreamColor.GREEN:
                color = Color.green;
                break;
            case EnumStreamColor.RED:
                color = Color.red;
                break;
            default:
                color = Color.yellow;
                break;
        }
        switchParticlesStatic.startColor = color;
        switchParticlesStatic.Play();

        yield return new WaitForSeconds(1);

        switchParticlesStatic.Stop();
    }

    public static void SetPlayerCurrentLife(int val)
    {
        int maxLife = (memoryFragments.Count + 1) * 30;

        currentLife = Mathf.Clamp(val, 0, maxLife);
    }

    public void AddLife(int val)
    {
        currentLife = Mathf.Clamp(currentLife + val, 0, maxLife);
    }

    public override void ReceiveInputEvent(InputEvent inputEvent)
    {
        if (inputEvent.InputAxis == EnumAxis.LeftJoystickX)
        {
            XSpeedMultiplier = inputEvent.Value;
            if (Mathf.Abs(XSpeedMultiplier) < 0.2)
            {
                XSpeedMultiplier = 0;
            }
        }

        if (inputEvent.InputAxis == EnumAxis.RightTrigger || inputEvent.InputAxis == EnumAxis.LeftTrigger)
        {
            boostPower();
        }

        if (inputEvent.InputAxis == EnumAxis.LeftJoystickY)
        {
            ZSpeedMultiplier = inputEvent.Value;
            if (Mathf.Abs(ZSpeedMultiplier) < 0.2)
            {
                ZSpeedMultiplier = 0;
            }
        }
    }

    public void AddForce(Vector3 force, Stream stream)
    {
        if (PlayerCanBeMoved)
        {
            forceToApply += force;
            if (force == Vector3.zero)
            {
                isPlayerOnstream = false;
                streamPlayer = null;
            }
            else {
                isPlayerOnstream = true;
                streamPlayer = stream;
            }
        }
    }

    private void boostPower()
    {
        if (!powerboost /*&& (uiBoostController.timeLeft < 10.0f)*/)
        {
            powerboost = true;
            timeSinceLastBoost = Time.time;
            speedMultiplierBoost = 5f;
            SFXBoost();

        }
    }

    private void unBoostPower()
    {
        if (powerboost)
        {
            powerboost = false;
            timeSinceLastBoost = Time.time;
            speedMultiplierBoost = 1f;
        }
    }

    public static void AddFragment(Fragment fragment)
    {
        memoryFragments.Add(fragment);

        RestoreAllLife();
        nextFragmentIndex++;
        //powerController.SetCooldownMultipliers(maxFill);
        Fragment fragmenttemp;
        for (int i = 0; i < memoryFragments.Count - 1; i++)
        {
            if (memoryFragments[i].index > memoryFragments[i + 1].index)
            {
                fragmenttemp = memoryFragments[i + 1];
                memoryFragments[i + 1] = memoryFragments[i];
                memoryFragments[i] = fragmenttemp;
            }
        }

        memoryFragments.ForEach((fragment1) =>
        {

            if (fragment1.index == nextFragmentFind)
            {
                nextFragmentFind++;
            }
        });


        if (memoryFragments.Count >= fragmentsList.Count && !HasWon)
        {
            HasWon = true;
            GameManager.SaveCheckpoint(new Checkpoint("Won"));
            loader.LoadEndLevel("win");
        }
        else {
            GameManager.SaveCheckpoint(new Checkpoint(fragment.fragmentName));
        }
    }

    public void ClearFragments()
    {
        memoryFragments.Clear();
        nextFragmentIndex = 0;
    }

    public static void RegisterFragment(Fragment fragment)
    {
        for (int i = fragmentsList.Count; i <= fragment.index; i++)
        {
            fragmentsList.Add(null);
        }
        fragmentsList[fragment.index] = fragment.GetComponent<Transform>();
        numberOfFragments++;
    }


    public void DamagePlayer(int damage)
    {
        currentLife -= damage;
        lifeBarFillStatic.fillAmount = maxFill * ((float)currentLife / (float)maxLife);

        if (currentLife <= 0)
        {
            loader.LoadEndLevel("gameOver");
        }
    }

    public List<Fragment> GetFragments()
    {
        return memoryFragments;
    }

    private void Awake()
    {
        playerRigidbody = GameObject.Find("Player").GetComponent<Rigidbody>();
        _player = Player.GetComponent<Player>();

        loader = GameObject.Find("SceneTransitionManager").GetComponent<LevelLoading>();

        GameObject musicControllerObject = GameObject.Find("AudioManager/MusicController");
        if (musicControllerObject != null)
        {
            Music = musicControllerObject.GetComponent<MusicController>();
        }

        fragmentsList = new List<Transform>(20);
        numberOfFragments = fragmentsList.Count;
        nextFragmentIndex = 0;
        nextFragmentFind = 0;

        memoryFragments = new List<Fragment>();
        forceToApply = new Vector3(0, 0, 0);
        CurrentZone = EnumZone.OPEN_WORLD;
        PlayerCanBeMoved = true;

        lifeBarFill.color = Color.red;
        lifeBarFillStatic = lifeBarFill;
        lifeBarRimStatic = lifeBarRim;

        RestoreAllLife();

        switchParticlesStatic = switchParticles;

        if (playerRigidbody == null)
        {
            Debug.LogError("No player is registered to the PlayerController");
        }
        else {
            playerRigidbody.GetComponent<Player>().PlayerController = this;
        }
    }


    private void FixedUpdate()
    {
        life = currentLife;

        if (PlayerCanBeMoved)
        {
            MovePlayer();
        }
        else {
            playerRigidbody.velocity = Vector3.zero;
        }
    }

    private void MovePlayer()
    {
        //var cam = Camera.main;

        Vector3 baseMovement = new Vector3(movementForce * XSpeedMultiplier * speedMultiplierBoost, 0, movementForce * ZSpeedMultiplier * speedMultiplierBoost);
        Vector3 movement = /*Quaternion.Euler(0, cam.transform.eulerAngles.y, 0) **/ baseMovement + forceToApply; //Adjust the movement direction depending on camera before applying external forces

        if (!(Mathf.Approximately(movement.x, 0F) && Mathf.Approximately(movement.y, 0F) && Mathf.Approximately(movement.z, 0F)))
        {

            playerRigidbody.AddForce(movement, ForceMode.Acceleration);

            Vector3 lastForward = playerRigidbody.transform.forward;
            lastForward.y = 0;

            // Check in what direction the boat should rotate
            float rotation = Vector3.Angle(lastForward, movement);
            if (Vector3.Dot(Vector3.up, Vector3.Cross(lastForward, movement)) < 0)
            {
                rotation = -rotation;
            }

            rotation = Mathf.SmoothDampAngle(0, rotation, ref currentVelocity, rotationSpeed);
            playerRigidbody.transform.Rotate(0, rotation, 0, Space.World);
        }

        if (Vector3.Magnitude(playerRigidbody.velocity) > maximumVelocity)
        {
            playerRigidbody.velocity = Vector3.Normalize(playerRigidbody.velocity) * maximumVelocity;
        }

        forceToApply = new Vector3(0, 0, 0);
        if (Time.time - timeSinceLastBoost > 1.5f && powerboost)
        {
            unBoostPower();
        }
        else
        {
          //  uiBoostController.timeLeft = (Time.time - timeSinceLastBoost);
        }
    }

    private static void RestoreAllLife()
    {
        maxFill = (memoryFragments.Count + 1) * 227f / 2047f;
        maxLife = (memoryFragments.Count + 1) * 30;
        currentLife = maxLife;

        lifeBarRimStatic.fillAmount = maxFill;
        lifeBarFillStatic.fillAmount = maxFill;
    }
}
