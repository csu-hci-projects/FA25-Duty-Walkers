// using UnityEngine;

// public class AnimationState : MonoBehaviour
// {
//     Animator animator;
//     int isWalkingHash, isRunningHash, isBackwardsHash, isStrafingHash, velocityHash;

//     private float velocity = 0.0f;
//     public float acceleration = 0.6f;
//     public float deceleration = 1.0f;
//     // Start is called once before the first execution of Update after the MonoBehaviour is created
//     void Start()
//     {
//         animator = GetComponent<Animator>();
//         isWalkingHash = Animator.StringToHash("isWalking");
//         isRunningHash = Animator.StringToHash("isRunning");
//         isBackwardsHash = Animator.StringToHash("isBackwards");
//         isStrafingHash = Animator.StringToHash("isStrafing");
//         velocityHash = Animator.StringToHash("Velocity");
//     }

//     // Update is called once per frame
//     void Update()
//     {
//         WalkRun();
//         WalkBackwards();
//         Strafe();
//     }

//     private void WalkRun()
//     {
//         bool isWalking = animator.GetBool(isWalkingHash);
//         bool isRunning = animator.GetBool(isRunningHash);
//         bool forwardPressed = Input.GetKey("w");
//         bool runningPressed = Input.GetKey("left shift");

//         if ((forwardPressed || runningPressed) && velocity < 1.0) velocity += Time.deltaTime * acceleration;
//         if (!runningPressed && velocity > 0.0) velocity -= Time.deltaTime * deceleration;
//         if ((!forwardPressed && !runningPressed) && velocity < 0.0) velocity = 0.0f;

//         animator.SetFloat(velocityHash, velocity);

//         if (!isWalking && forwardPressed) animator.SetBool(isWalkingHash, true);
//         if (isWalking && !forwardPressed) animator.SetBool(isWalkingHash, false);
//         if (!isRunning && (forwardPressed && runningPressed)) animator.SetBool(isRunningHash, true);
//         if (isRunning && (!forwardPressed || !runningPressed)) animator.SetBool(isRunningHash, false);
//     }

//     private void WalkBackwards()
//     {
//         bool isBackwards = animator.GetBool(isBackwardsHash);
//         bool backwardPressed = Input.GetKey("s");

//         if (!isBackwards && backwardPressed) animator.SetBool(isBackwardsHash, true);
//         if (isBackwards && !backwardPressed) animator.SetBool(isBackwardsHash, false);
//     }

//     private void Strafe()
//     {
//         bool isStrafing = animator.GetBool(isStrafingHash);
//         bool strafePressed = Input.GetKey("a") || Input.GetKey("d");

//         if (!isStrafing && strafePressed) animator.SetBool(isStrafingHash, true);
//         if (isStrafing && !strafePressed) animator.SetBool(isStrafingHash, false);
//     }
// }


// using UnityEngine;

// public class AnimationState : MonoBehaviour
// {
//     Animator animator;

//     void Start()
//     {
//         animator = GetComponent<Animator>();
//     }

//     void Update()
//     {
//         // Build a target based on input
//         float target = 0f;

//         bool forward = Input.GetKey(KeyCode.W);
//         bool backward = Input.GetKey(KeyCode.S);
//         bool strafe = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D);
//         bool sprint = Input.GetKey(KeyCode.LeftShift);

//         float dampTime = sprint ? 0.8f : 0.3f;  // build up/break down velocity

//         if (forward) target = sprint ? 1f : 0.5f;    // run or walk
//         else if (backward) target = -0.5f;

//         animator.SetFloat("Velocity", target, dampTime, Time.deltaTime);
//     }
// }

using UnityEngine;

public class AnimationState : MonoBehaviour
{
    Animator animator;

    public float runAccel = 0.35f;
    public float runDecel = 0.25f;
    // private float velocityX;
    // private float velocityZ;
    private float sprintFactor = 0.5f;
    private float sprintVelocity;

    void Start() => animator = GetComponent<Animator>();

    void Update()
    {
        // WASD as a vector
        float x = (Input.GetKey(KeyCode.D) ? 1f : 0f) - (Input.GetKey(KeyCode.A) ? 1f : 0f);
        float z = (Input.GetKey(KeyCode.W) ? 1f : 0f) - (Input.GetKey(KeyCode.S) ? 1f : 0f);

        Vector2 move = new Vector2(x, z);
        if (move.sqrMagnitude > 1f) move.Normalize(); // no diagonal boost

        bool forward = move.y > 0f;
        bool sprint = forward && Input.GetKey(KeyCode.LeftShift);

        float targetFactor = sprint ? 1f : 0.5f;
        float smoothTime = sprint ? runAccel : runDecel;
        //float rate = (targetFactor > sprintFactor) ? runAccel : runDecel;
        sprintFactor = Mathf.SmoothDamp(sprintFactor, targetFactor, ref sprintVelocity, smoothTime);

        // Map to your tree: forward walk = 0.5, forward run = 1.0, back/strafe = 0.5
        float targetZ = move.y * sprintFactor;
        float targetX = move.x * 0.5f;

        const float damp = 0.08f;
        animator.SetFloat("VelocityZ", targetZ, damp, Time.deltaTime);
        animator.SetFloat("VelocityX", targetX, damp, Time.deltaTime);

        // Optional Speed (for Idle<->Locomotion transitions)
        float speed = new Vector2(targetX, targetZ).magnitude;

        // Damped for smooth blends (0.08–0.15 feels good)
        animator.SetFloat("Speed", speed, 0.12f, Time.deltaTime);
    }
}
