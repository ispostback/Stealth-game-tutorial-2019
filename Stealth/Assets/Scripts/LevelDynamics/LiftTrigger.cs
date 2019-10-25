﻿using UnityEngine;

public class LiftTrigger : MonoBehaviour
{

    [SerializeField] float timeToDoorsClose = 2f;       // Time since the player entered the lift before the doors close.
    [SerializeField] float timeToLiftStart = 3f;        // Time since the player entered the lift before it starts to move.
    [SerializeField] float timeToEndLevel = 6f;         // Time since the player entered the lift before the level ends.
    [SerializeField] float liftSpeed = 3f;              // The speed at which the lift moves.

    GameObject player = null;                           // Reference to the player.
    Animator playerAnim = null;                         // Reference to the players animator component.
    HashIDs hash = null;                                // Reference to the HashIDs script.
    CameraMovement camMovement = null;                  // Reference to the camera movement script.
    SceneFadeInOut sceneFadeInOut = null;               // Reference to the SceneFadeInOut script.
    LiftDoorsTracking liftDoorsTracking = null;         // Reference to LiftDoorsTracking script.
    bool playerInLift = false;                          // Whether the player is in the lift or not.
    float timer = 0f;                                   // Timer to determine when the lift moves and when the level ends.

    AudioSource audioSource;

    void Awake ()
    {
        // Setting up references.
        audioSource = GetComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag(Tags.player);
        playerAnim = player.GetComponent<Animator>();
        hash = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<HashIDs>();
        camMovement = Camera.main.gameObject.GetComponent<CameraMovement>();
        sceneFadeInOut = GameObject.FindGameObjectWithTag(Tags.fader).GetComponent<SceneFadeInOut>();
        liftDoorsTracking = GetComponent<LiftDoorsTracking>();
    }

    void OnTriggerEnter(Collider other)
    {
        // If the colliding gameobject is the player...
        if (other.gameObject == player)
        {
            // ... the player is in the lift.
            playerInLift = true;
        }
    }

    void OnTriggerExit (Collider other)
    {
        // If the player leaves the trigger area...
        if (other.gameObject == player)
        {
            // ... reset the timer, the player is no longer in the lift and unparent the player from the lift.
            playerInLift = false;
            timer = 0;
        }
    }

    // Update is called once per frame
    void Update ()
    {
        // If the player is in the lift...
        if (playerInLift)
        {
            // ... activate the lift.
            LiftActivation();
        }

        // If the timer is less than the time before the doors close...
        if (timer < timeToDoorsClose)
        {
            // ... the inner doors should follow the outer doors.
            liftDoorsTracking.DoorFollowing();
        }
        else
        {
            // Otherwise the doors should close.
            liftDoorsTracking.CloseDoors();
        }
    }

    void LiftActivation ()
    {
        // Increment the timer by the amount of time since the last frame.
        timer += Time.deltaTime;

        // If the timer is greater than the amount of time before the lift should start...
        if (timer >= timeToLiftStart)
        {
            // ... stop the player and the camera moving and parent the player to the lift.
            playerAnim.SetFloat(hash.speedFloat, 0f);
            camMovement.enabled = false;
            player.transform.parent = transform;

            // Move the lift upwards.
            transform.Translate(Vector3.up * liftSpeed * Time.deltaTime);

            // If the audio clip isn't playing...
            if (!audioSource.isPlaying)
            {
                // ... play the clip.
                audioSource.Play();
            }

            // If the timer is greater than the amount of time before the level should end...
            if (timer >= timeToEndLevel)
            {
                // ... call the EndScene function.
                sceneFadeInOut.EndScene();
            }
        }
    }
}
