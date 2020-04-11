using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class AIController : Base_Controller
{
    private NavMeshAgent nma;
    private Animator anim;
    private Rigidbody rb;
    private Transform ptf;
    private Transform tf;
    private AIPawn aiPawn;
    // Start is called before the first frame update
    void Awake()
    {
        nma = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        ptf = GameManager.instance.activePlayers[0].GetComponent<Transform>();
        tf = GetComponent<Transform>();
        aiPawn = GetComponent<AIPawn>();
    }
    // Update is called once per frame
    private void Update()
    {
        if (ptf)
        {
            nma.SetDestination(ptf.position);
            Vector3 inputMovement = Vector3.MoveTowards(rb.velocity, nma.desiredVelocity, nma.acceleration * Time.deltaTime);
            inputMovement = transform.InverseTransformDirection(inputMovement);

            anim.SetFloat("Horizontal", inputMovement.x);
            anim.SetFloat("Vertical", inputMovement.z);
            AttackPlayer();
        }
        else
        {
            ptf = GameManager.instance.activePlayers[0].GetComponent<Transform>();
        }
    }

    private void OnAnimatorMove()
    {
        nma.velocity = anim.velocity;
    }

    public void AttackPlayer()
    {
        // Find the vector from current object to target
        Vector3 vectorToTarget = ptf.position - tf.position;

        // Find the distance between the two vectors in float to compare with effectiveRange
        float targetDistance = Vector3.Distance(ptf.position, tf.position);

        // Find the angle between the direction our agent is facing (forward in local space) and the vector to the target.
        float angleToTarget = Vector3.Angle(vectorToTarget, tf.forward);

        // If that angle is less than our field of view return true, else return false
        if (angleToTarget < aiPawn.weaponComponent.GetAttackAngle()  && targetDistance < aiPawn.weaponComponent.GetEffectiveRange())
        {
            aiPawn.Attack();
        }
    }
}
