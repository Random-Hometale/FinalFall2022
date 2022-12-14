using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed;
    public bool vertical;
    public float changeTime = 3.0f;


    Rigidbody2D rigidbody2D;
    float timer;
    int direction = 1;
    
    //broken
    bool broken = true;

    Animator animator;

    //smoke
    public ParticleSystem smokeEffect;

    AudioSource audioSource;
    public AudioClip fixedSound;
    public AudioClip brokenSound;

    private RubyController rubyController;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        timer = changeTime;
        
        animator = GetComponent<Animator>();

        smokeEffect.Play();

        audioSource = GetComponent<AudioSource>();
        audioSource.clip = brokenSound;
        audioSource.loop = true;
        audioSource.Play();

        GameObject rubyControllerObject = GameObject.FindWithTag("Player");
        rubyController = rubyControllerObject.GetComponent<RubyController>();

    }

     void Update()
    {
        
        //remember ! inverse the test, so if broken is true !broken will be false and return won’t be executed.
        if(!broken)
        {
            return;
        }

        timer -= Time.deltaTime;

        if (timer < 0)
        {
            direction = -direction;
            timer = changeTime;
        }

    }

    
    void FixedUpdate()
    {        
        
        Vector2 position = rigidbody2D.position;
        
        if (vertical)
        {
            position.y = position.y + Time.deltaTime * speed * direction;
            animator.SetFloat("Move X", 0);
            animator.SetFloat("Move Y", direction);
        }

        else
        {
            position.x = position.x + Time.deltaTime * speed * direction;
            animator.SetFloat("Move X", direction);
            animator.SetFloat("Move Y", 0);
        }

        if(!broken)
        {
            return;
        }
        
        rigidbody2D.MovePosition(position);

    }


    void OnCollisionEnter2D(Collision2D other)
    {
        RubyController player = other.gameObject.GetComponent<RubyController>();

        if (player != null)
        {
            player.ChangeHealth(-1);
        }
    }


    //Public because we want to call it from elsewhere like the projectile script
    public void Fix()
    {
        broken = false;
        rigidbody2D.simulated = false;

        animator.SetTrigger("Fixed");

        smokeEffect.Stop();

        audioSource.clip = fixedSound;
        audioSource.loop = false;
        audioSource.Play();

        if (rubyController != null)
        {
            rubyController.FixedRobots(1);
        }

    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }


}
