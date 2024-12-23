using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    [Header("General")]
    [SerializeField] float projectileSpeed;
    [SerializeField] float projectileLifetime;
    [HideInInspector] public GameObject missile;
    [HideInInspector] public GameObject greenProjectile;
    public GameObject projectile;
    public float baseFiringRate = 0.3f;

    [Header("AI")]
    [SerializeField] bool useAI;
    [SerializeField] float firingRateVariance = 0f;
    [SerializeField] float minimumFiringRate = 0.1f;

    [HideInInspector] public bool isFiring;

    Coroutine firingCoroutine;
    AudioPlayer audioPlayer;

    private void Awake()
    {
        audioPlayer = FindObjectOfType<AudioPlayer>();
        missile = FindObjectOfType<GameObject>();
        greenProjectile = FindObjectOfType<GameObject>();
    }

    void Start()
    {

        isFiring = true;

    }

    void FixedUpdate()
    {
        Fire();
    }

    void Fire()
    {
        if (isFiring && firingCoroutine == null)
        {
            firingCoroutine = StartCoroutine(FireContinuously());
        }
        else if (!isFiring && firingCoroutine != null)
        {
            StopCoroutine(firingCoroutine);
            firingCoroutine = null;
        }
    }

    IEnumerator FireContinuously()
    {
        while (true)
        {
            GameObject instance = Instantiate(projectile, transform.position, Quaternion.identity);
            Rigidbody2D projectileRB = instance.GetComponent<Rigidbody2D>();

            if (projectileRB != null)
            {
                projectileRB.velocity = transform.up * projectileSpeed;
            }

            Destroy(instance, projectileLifetime);

            float timeToNextProjectile = Random.Range(baseFiringRate - firingRateVariance, baseFiringRate + firingRateVariance);
            timeToNextProjectile = Mathf.Clamp(timeToNextProjectile, minimumFiringRate, float.MaxValue);

            if (useAI)
            {
                audioPlayer.PlayEnemyShootingClip();
            }
            else
            {
                audioPlayer.PlayPlayerShootingClip();
            }

            yield return new WaitForSeconds(timeToNextProjectile);
        }
    }
}
