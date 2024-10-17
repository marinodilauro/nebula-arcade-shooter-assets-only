using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class ShieldController : MonoBehaviour
{
    [SerializeField] ParticleSystem hitEffect;
    [SerializeField]  PlayerController player;

    ScoreKeeper scoreKeeper;
    Enemy enemyHealth;
    AudioPlayer audioPlayer;

    private void Awake()
    {
        scoreKeeper = FindObjectOfType<ScoreKeeper>();
        enemyHealth = FindObjectOfType<Enemy>();
        audioPlayer = FindObjectOfType<AudioPlayer>();
    }

    void PlayHitEffect(Vector2 position)
    {
        if (hitEffect != null)
        {
            ParticleSystem instance = Instantiate(hitEffect, transform.position, Quaternion.identity);
            Destroy(instance.gameObject, instance.main.duration + instance.main.startLifetime.constantMax);
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        Destroy(collider.gameObject);
        Debug.Log("Shield Collision");

        if (collider.CompareTag("Enemy"))
        {
            scoreKeeper.ModifyScore(enemyHealth.score);
            PlayHitEffect(collider.gameObject.transform.position);
        }
        else if (collider.CompareTag("SpeedPickUp"))
        {
            audioPlayer.PlayShieldOnClip();

            player.hasPowerUP = true;
            player.moveSpeed = 15f;
            StartCoroutine(player.SpeedPowerUPCooldown());
        }
        else if (collider.gameObject.CompareTag("HealthPickUp"))
        {
            HealthPickUp healthPickUp = FindObjectOfType<HealthPickUp>();

            audioPlayer.PlayShieldOnClip();
            healthPickUp.Heal(healthPickUp.GetHealAmount());
            Destroy(collider.gameObject);
        }
    }
}
