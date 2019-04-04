using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fox : MonoBehaviour
{
    enum States {Neutral,Hungry,Attack,Sleep}
    [Header("Stats")]
    public int hungry;
    public int sleepy;
    public int confidence;
    States currentState = States.Neutral;

    [Header("Movement")]
    public float normalSpeed;
    public float runSpeed;
    GameObject target;

    [Header("Neutral")]
    public float detectionRadius;
    float timeForChangeDir;
    float neutralTimer;

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 1, 0.3f);
        Gizmos.DrawSphere(transform.position, detectionRadius);
    }
    void Start()
    {
        CalculateRandomStats();
        SetNewRandTarget();
    }
    void Update()
    {
        #region Neutral
        if (currentState == States.Neutral)
        {
            if (hungry > 30 && sleepy > 30)
            {
                if (hungry > sleepy) currentState = States.Hungry;
                else currentState = States.Sleep;
            }
        }
        #endregion
        #region Move
        
        #endregion
    }
    #region ChangeStates
    #endregion
    #region Stats
    public void CalculateRandomStats()
    {
        hungry = Random.Range(0, 100);
        sleepy = Random.Range(0, 100);
        confidence = Random.Range(0, 100);
    }
    #endregion
    #region Movement methods
    void SetNewRandTarget()
    {
        GameObject newTarget = new GameObject("CurrentTarget");
        newTarget.transform.position = new Vector3(transform.position.x + Random.Range(0, 10), transform.position.y, Random.Range(0, 10));
        if (target != null)
            Destroy(target);
        target = newTarget;
    }
    #endregion
}
