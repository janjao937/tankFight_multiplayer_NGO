using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class ProjectileLauncher : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private CoinWallet coinWallet;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private GameObject serverProjectilePrefab;
    [SerializeField] private GameObject clientProjectilePrefab;
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private Collider2D playerCollider;
    [Header("Setting")]
    [SerializeField] private float projectileSpeed = 30;
    [SerializeField] private float fireRate = 0.75f;
    [SerializeField] private float muzzleFlashDuration = 0.075f;
    [SerializeField] private int costToFire = 0;
    private bool isFire;
    private float muzzleFlashTimer;
    private float timer;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        inputReader.PrimaryFireEvent += HandlePrimaryFire;

    }
    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;
        inputReader.PrimaryFireEvent -= HandlePrimaryFire;
    }
    private void HandlePrimaryFire(bool isFire)
    {
        this.isFire = isFire;
    }
    private void Update()
    {
        if (muzzleFlashTimer > 0)
        {
            muzzleFlashTimer -= Time.deltaTime;
            if (muzzleFlashTimer <= 0)
            {
                muzzleFlash.SetActive(false);
            }
        }
        if (!IsOwner) return;
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        if (!isFire) { return; }
        if (timer > 0) { return; }
        if(coinWallet.TotalCoins.Value<costToFire)return;
        PrimaryFireServerRpc(projectileSpawnPoint.position, projectileSpawnPoint.up);
        SpawnDummyProjectile(projectileSpawnPoint.position, projectileSpawnPoint.up);

        timer = 1 / fireRate;

    }

    [ServerRpc]
    private void PrimaryFireServerRpc(Vector2 spawnPos, Vector2 dir)
    {
        if(coinWallet.TotalCoins.Value<costToFire)return;
        coinWallet.SpendCoins(costToFire);
        
        GameObject projectileInstance = Instantiate(serverProjectilePrefab, spawnPos, Quaternion.identity);
        projectileInstance.transform.up = dir;
        Physics2D.IgnoreCollision(playerCollider, projectileInstance.GetComponent<Collider2D>());
        if (projectileInstance.TryGetComponent<DealDamageOnContact>(out DealDamageOnContact dealDamage))
        {

            dealDamage.SetOwner(this.OwnerClientId);
        }
        if (projectileInstance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.velocity = rb.transform.up * projectileSpeed;
        }

        SpawnDummyProjectileClientRpc(spawnPos, dir);
    }
    [ClientRpc]
    private void SpawnDummyProjectileClientRpc(Vector2 spawnPos, Vector2 dir)
    {
        if (IsOwner) return;
        SpawnDummyProjectile(spawnPos, dir);
    }
    private void SpawnDummyProjectile(Vector2 spawnPos, Vector2 dir)
    {
        muzzleFlash.SetActive(true);
        muzzleFlashTimer = muzzleFlashDuration;
        GameObject projectileInstance = Instantiate(clientProjectilePrefab, spawnPos, Quaternion.identity);
        projectileInstance.transform.up = dir;
        Physics2D.IgnoreCollision(playerCollider, projectileInstance.GetComponent<Collider2D>());

        if (projectileInstance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.velocity = rb.transform.up * projectileSpeed;
        }
    }
}
