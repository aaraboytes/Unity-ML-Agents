using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    Rigidbody body;
    Vector3 move;

    private void Start()
    {
        body = GetComponent<Rigidbody>();
        move = Vector3.zero;
    }
    private void Update()
    {
        move = Input.GetAxis("Horizontal")*Vector3.right + Input.GetAxis("Vertical")*Vector3.forward;
        move *= speed;
        body.velocity = move;
    }
}
