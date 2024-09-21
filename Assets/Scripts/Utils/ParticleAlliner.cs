using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleAlliner : MonoBehaviour
{
    private ParticleSystem.MainModule particleSystem;

    void Start()
    {
        this.particleSystem = GetComponent<ParticleSystem>().main;
    }


    void Update()
    {
        particleSystem.startRotation = -transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
    }
}
