// using UnityEngine;

// public class ZombieAnimationState : MonoBehaviour
// {
//     Animator animator;
//     Animator controller;
//     int attackHash, damageHash, isDeadHash, speedHash;

//     private float speed = 0.0f;
//     public float acceleration = 0.6f;
//     public float deceleration = 1.0f;

//     void start()
//     {
//         animator = GetComponent<Animator>();
//         attackHash = Animator.StringToHash("Attack");
//         damageHash = Animator.StringToHash("Damage");
//         isDeadHash = Animator.StringToHash("IsDead");
//         speedHash = Animator.StringToHash("Speed");
//     }

//     void Awake()
//     {
//         controller = GetComponentInParent<Animator>();

//     }

//     void Update()
//     {
//         controller.SetFloat("speed", Mathf.abs(nav.velocity.x) + Mathf.abs(nav.velocity.y));
//     }

//     private void DeathAnimation()
//     {
//         animator.SetTrigger("isDead");
//     }
// }