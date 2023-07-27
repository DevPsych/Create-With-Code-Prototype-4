using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerX : MonoBehaviour
{
    private Rigidbody playerRb;
    public float speed = 500;
    public float boostSpeed = 700;
    private GameObject focalPoint;
    public ParticleSystem smokeParticle;

    public bool hasPowerup;
    public bool hasSpeedBoost;
    public GameObject powerupIndicator;
    public GameObject smokeParticleIndicator;
    public int powerUpDuration = 5;
    public int speedBoostDuration = 3;

    private float normalStrength = 10; // how hard to hit enemy without powerup
    private float powerupStrength = 25; // how hard to hit enemy with powerup
    
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        focalPoint = GameObject.Find("Focal Point");
    }

    void Update()
    {
        // Add force to player in direction of the focal point (and camera)
        float verticalInput = Input.GetAxis("Vertical");
        playerRb.AddForce(focalPoint.transform.forward * verticalInput * speed * Time.deltaTime); 

        // Set powerup indicator position to beneath player
        powerupIndicator.transform.position = transform.position + new Vector3(0, -0.6f, 0);
        smokeParticleIndicator.transform.position = transform.position + new Vector3(0, -0.75f, 0);

        SpeedBoostAbility(verticalInput);
    }

    // If Player collides with powerup, activate powerup
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Powerup"))
        {
            Destroy(other.gameObject);
            hasPowerup = true;
            powerupIndicator.SetActive(true);
            StartCoroutine(PowerupCooldown());
        }
    }

    void SpeedBoostAbility(float playerInput)
    {
        if (Input.GetKeyDown(KeyCode.Space) && !hasSpeedBoost)
        {
            hasSpeedBoost = true;
            smokeParticle.Play();
            playerRb.AddForce(focalPoint.transform.forward * playerInput * boostSpeed * Time.deltaTime, ForceMode.Impulse);
            StartCoroutine(SpeedBoostCooldown());
        }
    }

    // Coroutine to count down powerup duration
    IEnumerator PowerupCooldown()
    {
        yield return new WaitForSeconds(powerUpDuration);
        hasPowerup = false;
        powerupIndicator.SetActive(false);
    }

    IEnumerator SpeedBoostCooldown()
    {
        yield return new WaitForSeconds(speedBoostDuration);
        hasSpeedBoost = false;
        smokeParticle.Stop();
    }

    // If Player collides with enemy
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Rigidbody enemyRigidbody = other.gameObject.GetComponent<Rigidbody>();
            Vector3 awayFromPlayer =  other.gameObject.transform.position - transform.position; 
           
            if (hasPowerup) // if have powerup hit enemy with powerup force
            {
                enemyRigidbody.AddForce(awayFromPlayer * powerupStrength, ForceMode.Impulse);
            }
            else // if no powerup, hit enemy with normal strength 
            {
                enemyRigidbody.AddForce(awayFromPlayer * normalStrength, ForceMode.Impulse);
            }


        }
    }
 }
