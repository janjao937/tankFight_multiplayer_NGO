using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;



public class PlayerMovement : NetworkBehaviour
{
    [Header("Reference")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform bodyTransform;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private ParticleSystem dustCloud;

    [Header("Setting")]
    [SerializeField] private float movementSpeed = 4f;
    [SerializeField] private float turningRate = 270f;
    [SerializeField] private float particleEmmisionValue = 10;

    private ParticleSystem.EmissionModule emissionModule = default;
    private Vector2 previousMovementInput;
    private Vector3 previousPos;
    private const float _particleStopThreshold = 0.005f;
    private void Awake()
    {
        emissionModule = dustCloud.emission;
    }
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        inputReader.MoveEvent += HandleMove;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;
        inputReader.MoveEvent -= HandleMove;
    }

    private void Update()
    {
        if (!IsOwner) return;
        float zRotate = previousMovementInput.x * (-turningRate) * Time.deltaTime;
        bodyTransform.Rotate(0f, 0f, zRotate);
    }
    private void FixedUpdate()
    {
        if ((transform.position - previousPos).sqrMagnitude > _particleStopThreshold)
        {
            emissionModule.rateOverTime = particleEmmisionValue;
        }
        else
        {
            emissionModule.rateOverTime = 0;
        }
        previousPos = transform.position;
        if (!IsOwner) return;
        rb.velocity = (Vector2)bodyTransform.up * previousMovementInput.y * movementSpeed;
    }
    private void HandleMove(Vector2 movementInput)
    {
        previousMovementInput = movementInput;
    }


}
