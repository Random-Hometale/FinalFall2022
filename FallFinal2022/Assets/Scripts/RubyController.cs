using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class RubyController : MonoBehaviour
{
    public float speed = 4.0f;

    //health
    public int maxHealth = 5;
    public int health { get { return currentHealth; }}
    int currentHealth;

    public GameObject projectilePrefab;
    public int ammo { get { return currentAmmo; }}
    public int currentAmmo;

    //text
    public TextMeshProUGUI ammoText;

    public TextMeshProUGUI fixedText;
    private int scoreFixed = 0;

    public GameObject WinTextObject;
    public GameObject LoseTextObject;
    private bool gameOver = false;
    
    //invincibility
    public float timeInvincible = 2.0f;
    bool isInvincible;
    float invincibleTimer;

    Rigidbody2D rigidbody2d;
    float horizontal; 
    float vertical;

    //speedboost
    public float timeBoosting = 4.0f;
    float speedBoostTimer;
    bool isBoosting;

    public float timeSlowing = 0.5f;
    float speedSlowTimer;
    bool isSlowing;

    //animation
    Animator animator;
    Vector2 lookDirection = new Vector2(1,0);

    //audio
    AudioSource audioSource;
    public AudioSource backgroundManager;
    public AudioClip WinSound;
    public AudioClip LoseSound;
    public AudioClip throwSound;
    public AudioClip hitSound;

    //effects
    public ParticleSystem hitEffect;

    //level
    public static int level = 1;
    public GameObject newScene;


    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();

        //health
        currentHealth = maxHealth;
        
        //animator
        animator = GetComponent<Animator>();

        audioSource = GetComponent<AudioSource>();

        // win/lose text
        WinTextObject.SetActive(false);
        LoseTextObject.SetActive(false);

        newScene.SetActive(false);


        //ammo at start
        rigidbody2d = GetComponent<Rigidbody2D>();
        currentAmmo = 0;
        AmmoText();

        //fixed robot count
        fixedText.text = "Fixed Robots: " + scoreFixed.ToString() + "/4";

    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    public void StopSound(AudioClip clip)
    {
        audioSource.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        // animation
        Vector2 move = new Vector2(horizontal, vertical);
        
        if(!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }
        
        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);


        // invincibility
        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }

        // Speed Boost Timer
        if (isBoosting == true)
        {
            speedBoostTimer -= Time.deltaTime;
            speed = 8;
        
            if (speedBoostTimer < 0)
            {
                isBoosting = false;
                speed = 4; 
            }
        }

        //speed slow timer
        if (isSlowing == true)
        {
            speedSlowTimer -= Time.deltaTime;
            speed = 2;

            if (speedSlowTimer < 0)
            {
                isSlowing = false;
                speed = 4;
            }
        }

        // cog bullet
        if(Input.GetKeyDown(KeyCode.C))
        {
            Launch();

            if (currentAmmo > 0)
            {
                ChangeAmmo(-1);
                AmmoText();
            }

        }

        // talking to the frog
        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {

                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null)
                {
                    if (scoreFixed == 4)
                    {
                        SceneManager.LoadScene("MainScene2");
                        level = 2;
                    }
                    
                    else
                    {
                        character.DisplayDialog();
                    }
                }
            }
        }

        if (Input.GetKey(KeyCode.R))
        {
            if (gameOver == true)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                SceneManager.LoadScene("MainScene");
                level = 1;
            }
        }

    }
    
    
    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;

        rigidbody2d.MovePosition(position);
    }

    
    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible)
                return;
            
            isInvincible = true;
            invincibleTimer = timeInvincible;

            hitEffect = Instantiate(hitEffect, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

            animator.SetTrigger("Hit");

            PlaySound(hitSound);


        }

        if (currentHealth <= 0)
        {
            LoseTextObject.SetActive(true);
            gameOver = true;

            Destroy(gameObject.GetComponent<SpriteRenderer>());

            speed = 0;

            backgroundManager.Stop();
            audioSource.clip = LoseSound;
            audioSource.Play();
        }

        if (amount < 0 && gameOver == true)
        {
            speed = 0;
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        
        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);

    }

    public void SpeedBoost(int amount)
    {
        if (amount > 0)
        {
            speedBoostTimer = timeBoosting;
            isBoosting = true;
        }
    }

    public void SpeedSlow(int amount)
    {
        if (amount < 0)
        {
            speedSlowTimer = timeSlowing;
            isSlowing = true;
        }
    }



    //ammo
    public void ChangeAmmo(int amount)
    {
        //math code
        currentAmmo = Mathf.Abs(currentAmmo + amount);
        
    }

    public void AmmoText()
    {
        ammoText.text = "Ammo: " + currentAmmo.ToString();
    }

    // launch projectile
    void Launch()
    {
        if (currentAmmo > 0)
        {
            GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

            Projectile projectile = projectileObject.GetComponent<Projectile>();
            projectile.Launch(lookDirection, 300);

            animator.SetTrigger("Launch");

            PlaySound(throwSound);

        }

    }

    public void FixedRobots(int amount)
    {
        scoreFixed += amount;
        fixedText.text = "Fixed Robots: " + scoreFixed.ToString() + "/4";

        if (scoreFixed == 4 && level == 1)
        {
            WinTextObject.SetActive(true);
            newScene.SetActive(true);

        }

        else if (scoreFixed == 4 && level == 2)
        {
            WinTextObject.SetActive(true);
            gameOver = true;
            gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            Destroy(gameObject.GetComponent<BoxCollider2D>());

            backgroundManager.Stop();
            PlaySound(WinSound);


        }
    }

} 


