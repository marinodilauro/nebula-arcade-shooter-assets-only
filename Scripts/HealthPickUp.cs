using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickUp : MonoBehaviour
{
    public int healAmount = 25;
    
    PlayerController playerHealth;
    AudioPlayer audioPlayer;

    private void Awake()
    {
        playerHealth = FindObjectOfType<PlayerController>();
        audioPlayer = FindObjectOfType<AudioPlayer>();
    }

    public int GetHealAmount()
    {
        return healAmount;
    }

    public bool Heal(int healAmount)
    {
        if (playerHealth.IsAlive && playerHealth.PlayerCurrentHealth < playerHealth.PlayerMaxHealth)
        {
            int maxHeal = Mathf.Max(playerHealth.PlayerMaxHealth - playerHealth.PlayerCurrentHealth, 0);
            int actualHeal = Mathf.Min(maxHeal, healAmount);
            playerHealth.PlayerCurrentHealth += actualHeal;

            return true;
        }

        return false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Heal(healAmount);
            audioPlayer.PlayShieldOnClip();
            Destroy(gameObject);
        }
    }
}
