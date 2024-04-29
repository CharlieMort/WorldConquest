using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowPulseScript : MonoBehaviour
{
    private Transform model;
    public float pulseSpeed = 5f;
    public Transform target;

    private void Start()
    {
        model = transform.GetChild(0);
        if (target != null)
        {
            SetTarget(target);
        }
    }

    public void SetTarget(Transform t)
    {
        target = t;
        transform.localRotation = Quaternion.LookRotation(target.position - transform.position);
        transform.localEulerAngles = transform.localRotation.eulerAngles + Vector3.up * 180;
    }

    private void Update()
    {
        if (target != null)
        {

        }
    }
}
