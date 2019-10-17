using UnityEngine;
using System.Collections;

public class EnemyAi : MonoBehaviour {
     Transform target; //the enemy's target

     [SerializeField] private Animator m_animator;
     [SerializeField] private int moveSpeed = 1; //move speed

     int currentMoveSpeed;
     int rotationSpeed = 3; //speed of turning
     double attackThreshold = 1.5; // distance within which to attack
     int chaseThreshold = 10; // distance within which to start chasing
     int giveUpThreshold = 20; // distance beyond which AI gives up
     int attackRepeatTime = 1; // delay between attacks when within range
     double Damage = 0.001;
     private bool chasing = false;
     public float attackTime = 10;
     Transform myTransform; //current transform data of this enemy
      
     void Awake(){
        myTransform = transform; //cache transform data for easy access/preformance
     }
     
     void Start(){
        if (target == null && GameObject.FindWithTag("Player"))
           target = GameObject.FindWithTag("Player").transform;
     }
      
     void Update () {
        // check distance to target every frame:
        m_animator.SetBool("Grounded", true);
        float distance = (target.position - myTransform.position).magnitude;
      
        if (chasing) {
           m_animator.SetFloat("MoveSpeed", currentMoveSpeed);
           //rotate to look at the player
           myTransform.rotation = Quaternion.Slerp(myTransform.rotation, Quaternion.LookRotation(target.position - myTransform.position), rotationSpeed*Time.deltaTime);
           //move towards the player
           myTransform.position += myTransform.forward * currentMoveSpeed * Time.deltaTime;
           // give up, if too far away from target:
           if (distance > giveUpThreshold) {
              chasing = false;
           }
           // attack, if close enough, and if time is OK:
           if (distance < attackThreshold && Time.time > attackRepeatTime) {
              target.SendMessageUpwards("ApplyDamage" , Damage , SendMessageOptions.DontRequireReceiver);
           // Attack! (call whatever attack function you like here)
           }
           if (distance < attackThreshold) {
            currentMoveSpeed = 0;
           } else {
              currentMoveSpeed = moveSpeed;
           }
           attackTime = Time.time + attackRepeatTime;
        }
        else {
           // not currently chasing.
           // start chasing if target comes close enough
           if (distance < chaseThreshold) {
              chasing = true;
              
           }
        }
     }
}