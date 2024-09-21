using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleAlliner : MonoBehaviour
{
    private ParticleSystem.MainModule particle;

    void Start()
    {
        this.particle = GetComponent<ParticleSystem>().main;
    }


    void Update()
    {
        particle.startRotation = -transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
    }
}
