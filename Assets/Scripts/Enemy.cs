using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum State { Patrol,Alert,Attack};
    public Transform playerTransform;
    public float speed;
    public float speedRot;
    Transform target;

    [Header("Patrol")]
    public Color patrolColor;
    public Transform[] patrolPoints;
    public float detectionDistance = 0;
    public float minDistanceToPatrolPoint = 0.5f;
    float lastRotDir;
    int currentIndex = 0;

    [Header("Alert")]
    public Color alertColor;
    public float fieldOfView;
    public float alertTime;
    float alertTimer = 0;
    public float rotRange;
    public float speedAlertRot;

    [Header("Attack")]
    public Color attackColor;
    public float minAttackDistance = 0;
    bool move = true;
    bool lookAtPlayer = false;

    State currentState;
    Rigidbody body;
    Renderer rend;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionDistance);
    }
    void Start()
    {
        target = FindClosestPatrolPoint();
        body = GetComponent<Rigidbody>();
        rend = GetComponent<Renderer>();
        ChangeToPatrol();
    }
    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        Vector3 dir = playerTransform.position - transform.position;
        #region Patrol algorithm
        if (currentState.Equals(State.Patrol))
        {
            if (distanceToPlayer <= detectionDistance)
            {
                ChangeToAlert();
            }
        }
        #endregion
        #region Alert algorithm
        if (currentState.Equals(State.Alert))
        {
            alertTimer += Time.deltaTime;
            //Rotate to look for player
            Quaternion newRotation = Quaternion.EulerAngles(0,lastRotDir + Mathf.Sin(speedAlertRot * Time.time)*rotRange, 0);
            transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, speedRot);
            //Look for player
            float angle = Vector3.Angle(dir, transform.forward);
            if(angle < fieldOfView * 0.5f)
            {
                RaycastHit hit;
                if(Physics.Raycast(transform.position + transform.up,dir.normalized,out hit, detectionDistance))
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        ChangeToAttack();
                    }
                }
            }
            //Alert time exceeded
            if (alertTimer > alertTime)
            {
                if (currentState == State.Alert)
                {
                    ChangeToPatrol();
                }
            }
        }
        #endregion
        #region Attack algorithm
        if (currentState == State.Attack) {
            if (distanceToPlayer <= minAttackDistance)
            {
                move = false;
            }
            float angle = Vector3.Angle(dir, transform.forward);
            if (angle < fieldOfView * 0.5f)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position + transform.up, dir.normalized, out hit, detectionDistance))
                {
                    if (!hit.collider.CompareTag("Player"))
                    {
                        ChangeToPatrol();
                    }
                }
            }
        }
        #endregion
        #region Movement
        if (move)
        {
            Vector3 dirToMove = target.position - transform.position;
            if (dirToMove.magnitude <= minDistanceToPatrolPoint && currentState == State.Patrol)
                LookNextPatrolPoint();
            dirToMove = dirToMove.normalized;
            body.velocity = dirToMove * speed;
            if(currentState != State.Alert)
                transform.LookAt(target);
        }
        #endregion
    }
    
    #region Change State
    public void ChangeToPatrol()
    {
        rend.material.SetColor("_Color",patrolColor);
        alertTimer = 0;
        currentState = State.Patrol;
        move = true;
        target = FindClosestPatrolPoint();
    }
    public void ChangeToAlert()
    {
        rend.material.SetColor("_Color", alertColor);
        currentState = State.Alert;
        move = false;
        lastRotDir = transform.rotation.y;
        body.velocity = Vector3.zero;
    }
    public void ChangeToAttack()
    {
        rend.material.SetColor("_Color", attackColor);
        currentState = State.Attack;
        lookAtPlayer = true;
        move = true;
        target = playerTransform;
    }
    #endregion

    #region PatrolPoint
    Transform FindClosestPatrolPoint()
    {
        float distance = 0;
        for (int i = 0; i < patrolPoints.Length; i++)
        {
            if (Vector3.Distance(transform.position, patrolPoints[i].position) > distance)
            {
                distance = Vector3.Distance(transform.position, patrolPoints[i].position);
                currentIndex = i;
            }
        }
        return patrolPoints[currentIndex];
    }
    void LookNextPatrolPoint()
    {
        currentIndex++;
        if (currentIndex >= patrolPoints.Length)
            currentIndex = 0;
        target = patrolPoints[currentIndex];
    }
    #endregion
}
