using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class COMSetter : MonoBehaviour
{
    [SerializeField] private Transform com;
    [SerializeField] private Rigidbody rb;
    private void Awake()
    {
        rb.centerOfMass = transform.InverseTransformPoint(com.position);
    }
}
