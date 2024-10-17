using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.U2D;

public class PlayerController : MonoBehaviour
{
    [Header("Moving constraints")]
    [SerializeField] float paddingLeft;
    [SerializeField] float paddingRight;
    [SerializeField] float paddingTop;
    [SerializeField] float paddingBottom;

    [Header("Health")]
    [SerializeField] bool isAlive = true;
    [SerializeField] int playerMaxHealth = 100;
    [SerializeField] int playerCurrentHealth = 100;

    [Header("Speed")]
    public float moveSpeed = 10f;

    [Header("Reference")]
    [SerializeField] GameObject shield;
    [SerializeField] GameObject engine0;
    [SerializeField] GameObject engine1;
    [SerializeField] ParticleSystem hitEffect;
    [SerializeField] Transform parent;

    [Header("PowerUp")]
    public bool hasPowerUP = false;
    public bool hasShield = false;

    private bool pauseButtonPressed;

    public bool IsMoving { get; private set; }

    float powerUPCooldown = 15f;
    float shieldCooldown = 10f;

    Vector2 moveInput;
    Vector2 minBounds;
    Vector2 maxBounds;

    Shooter shooter;
    Enemy enemyHealth;
    Rigidbody2D playerRB;
    PickUpController pickUp;
    HealthPickUp healthPickUp;
    AudioPlayer audioPlayer;
    LevelManager levelManager;
    CameraShake cameraShake;
    GameManager gameManager;

    public int PlayerMaxHealth
    {
        get
        {
            return playerMaxHealth;
        }
        private set
        {
            playerMaxHealth = value;
        }
    }

    public int PlayerCurrentHealth
    {
        get
        {
            return playerCurrentHealth;
        }
        set
        {
            playerCurrentHealth = value;

            // If health drops below 0 the character is no longer alive
            if (playerCurrentHealth <= 0)
            {
                PlayerDie();
            }
        }
    }

    public bool IsAlive
    {
        get
        {
            return isAlive;
        }
        private set
        {
            isAlive = value;
        }
    }

    void Awake()
    {
        playerRB = GetComponent<Rigidbody2D>();
        shooter = GetComponent<Shooter>();
        enemyHealth = FindObjectOfType<Enemy>();
        pickUp = FindObjectOfType<PickUpController>();
        healthPickUp = FindObjectOfType<HealthPickUp>();
        audioPlayer = FindObjectOfType<AudioPlayer>();
        levelManager = FindObjectOfType<LevelManager>();
        cameraShake = Camera.main.GetComponent<CameraShake>();
        gameManager = FindObjectOfType<GameManager>();
    }

    void Start()
    {
        InitBound();
    }

    void FixedUpdate()
    {
        Move();
    }

    void InitBound()
    {
        Camera mainCamera = Camera.main;
        minBounds = mainCamera.ViewportToWorldPoint(new Vector2(0, 0));
        maxBounds = mainCamera.ViewportToWorldPoint(new Vector2(1, 1));
    }

    void Move()
    {
        Vector2 movement = moveSpeed * Time.deltaTime * moveInput;
        Vector2 newPos = new()
        {
            x = Mathf.Clamp(transform.position.x + movement.x, minBounds.x + paddingLeft, maxBounds.x - paddingRight),
            y = Mathf.Clamp(transform.position.y + movement.y, minBounds.y + paddingBottom, maxBounds.y - paddingTop)
        };

        transform.position = newPos;
    }

    public void OnMove(InputAction.CallbackContext value)
    {
        moveInput = value.ReadValue<Vector2>();

        IsMoving = moveInput != Vector2.zero;
    }


    /*     public void OnFire(InputAction.CallbackContext value)
        {
            if (shooter != null)
            {
                shooter.isFiring = value.performed;
            }
        } */


    public void OnPauseGame(InputAction.CallbackContext value)
    {
        if (value.performed && !gameManager.pressingPause)
        {
            gameManager.pressingPause = true;

            if (gameManager != null)
            {
                Debug.Log("Pause");
                gameManager.PausingGame();
            }
        }
        else if (value.canceled && gameManager.pressingPause)
        {
            gameManager.pressingPause = false;
        }
    }

    public int GetHealth()
    {
        return PlayerCurrentHealth;
    }

    public void PlayerTakeDamage(int damage)
    {
        PlayerCurrentHealth -= damage;
    }

    public void PlayHitEffect(Vector2 position)
    {
        if (hitEffect != null)
        {
            ParticleSystem instance = Instantiate(hitEffect, transform.position, Quaternion.identity);
            Destroy(instance.gameObject, instance.main.duration + instance.main.startLifetime.constantMax);
        }
    }

    public void ShakeCamera()
    {
        if (cameraShake != null)
        {
            cameraShake.Play();
        }
    }

    void PlayerDie()
    {
        levelManager.LoadGameOver();
        Destroy(gameObject);
    }

    // Collisions with PickUps
    void OnTriggerEnter2D(Collider2D collider)
    {
        DamageDealer damageDealer = collider.GetComponent<DamageDealer>();

        if (damageDealer != null && !hasShield && collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            PlayerTakeDamage(damageDealer.GetDamage());
            PlayHitEffect(collider.transform.position);
            audioPlayer.PlayDamageClip();
            ShakeCamera();
            damageDealer.Hit();
        }

        //Collision with shield PickUp
        else if (collider.gameObject.CompareTag("ShieldPickUp"))
        {
            StopCoroutine("ShieldCooldown");
            Debug.Log("ShieldPickUp");
            hasShield = true;
            audioPlayer.PlayShieldOnClip();
            shield.SetActive(true);
            Destroy(collider.gameObject);
            gameObject.GetComponent<CircleCollider2D>().enabled = false;
            StartCoroutine("ShieldCooldown");
        }

        // Collision with speed PickUp
        else if (collider.gameObject.CompareTag("SpeedPickUp"))
        {
            StopCoroutine("SpeedPowerUPCooldown");
            Debug.Log("SpeedPickUp");
            hasPowerUP = true;
            audioPlayer.PlayShieldOnClip();
            engine0.SetActive(false);
            engine1.SetActive(true);
            moveSpeed = 15f;
            Destroy(collider.gameObject);
            StartCoroutine("SpeedPowerUPCooldown");
        }
    }

    // Shield cooldown
    IEnumerator ShieldCooldown()
    {
        yield return new WaitForSeconds(shieldCooldown);

        SpriteRenderer sprite = shield.GetComponent<SpriteRenderer>();
        float time = 0;
        float counter = 2f;
        float blinkTime = 0.25f;

        while (time < counter)
        {
            sprite.enabled = !sprite.enabled;
            yield return new WaitForSeconds(blinkTime);
            time += blinkTime;
        }
        sprite.enabled = true;
        hasShield = false;
        shield.SetActive(false);
        audioPlayer.PlayShieldOffClip();
        gameObject.GetComponent<CircleCollider2D>().enabled = true;
    }

    // Speed cooldown
    public IEnumerator SpeedPowerUPCooldown()
    {
        yield return new WaitForSeconds(powerUPCooldown);
        hasPowerUP = false;
        engine0.SetActive(true);
        engine1.SetActive(false);
        moveSpeed = 10f;
    }

    //Health cooldown
    public IEnumerator HealthPowerUPCooldown()
    {
        yield return new WaitForSeconds(powerUPCooldown);
        hasPowerUP = false;
    }
}
