using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedCollectible : MonoBehaviour
{
    public GameObject speedParticlePrefab;
    public AudioClip speedBoostSound;

    void OnTriggerEnter2D(Collider2D other)
    {
        RubyController controller = other.GetComponent<RubyController>();

        if (controller != null)
        {
            controller.SpeedBoost(1);
            GameObject speedParticleObject= Instantiate(speedParticlePrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);

            controller.PlaySound(speedBoostSound);
            
        }
    }
}
