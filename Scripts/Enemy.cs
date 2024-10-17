using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] ParticleSystem hitEffect;

    public int enemyHealth = 50;
    public int score = 50;

    AudioPlayer audioPlayer;
    ScoreKeeper scoreKeeper;
    PlayerController player;

    private void Awake()
    {
        audioPlayer = FindObjectOfType<AudioPlayer>();
        scoreKeeper = FindObjectOfType<ScoreKeeper>();
        player = FindObjectOfType<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        DamageDealer damageDealer = other.GetComponent<DamageDealer>();

        if (damageDealer != null && other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            EnemyTakeDamage(damageDealer.GetDamage());
            PlayHitEffect(other.transform.position);
            audioPlayer.PlayDamageClip();
            damageDealer.Hit();
        }
    }

    void EnemyTakeDamage(int damage)
    {
        enemyHealth -= damage;

        if (enemyHealth <= 0)
        {
            EnemyDie();
        }
    }

    void EnemyDie()
    {
        scoreKeeper.ModifyScore(score);
        Destroy(gameObject);
    }

    public void PlayHitEffect(Vector2 position)
    {
        if (hitEffect != null)
        {
            ParticleSystem instance = Instantiate(hitEffect, transform.position, Quaternion.identity);
            Destroy(instance.gameObject, instance.main.duration + instance.main.startLifetime.constantMax);
        }
    }
}
