using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float followSpeed = 5f;
    [SerializeField] private float rotateSpeed = 5f;
    [SerializeField] private bool trackForward;

    private void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, target.transform.position, followSpeed * Time.fixedDeltaTime);
        
        if (!trackForward) { return; }
        
        transform.rotation = Quaternion.Lerp(transform.rotation, target.transform.rotation, rotateSpeed * Time.fixedDeltaTime);
    }
}
