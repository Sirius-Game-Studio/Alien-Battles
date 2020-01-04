using System.Collections;
using UnityEngine;

public class Toxicitation : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Vector2 abilityTime = new Vector2(3.75f, 4.25f);
    [Tooltip("The Y position this enemy stops at.")] [SerializeField] private float yPosition = 3.5f;
    [Tooltip("The music to play after this enemy spawns.")] [SerializeField] private AudioClip music = null;

    [Header("Spiralic Gestation")]
    [SerializeField] private float spiralicGestationFireRate = 0.08f;

    [Header("Toxic Blast")]
    [SerializeField] private float toxicBlastFireRate = 0.5f;
    [SerializeField] private float toxicBlastShots = 3;

    [Header("X-Scissors")]
    [SerializeField] private float xScissorsFireRate = 0.09f;
    [SerializeField] private float xScissorsShots = 40;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip spiralicGestationFireSound = null;
    [SerializeField] private AudioClip toxicBlastFireSound = null;
    [SerializeField] private AudioClip xScissorsFireSound = null;

    [Header("Setup")]
    [SerializeField] private GameObject orb = null;
    [SerializeField] private GameObject doubleOrb = null;
    [SerializeField] private Transform[] bulletSpawns = new Transform[0];

    private AudioSource audioSource;
    private bool usingAbility = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (Camera.main.GetComponent<AudioSource>() && music)
        {
            Camera.main.GetComponent<AudioSource>().clip = music;
            Camera.main.GetComponent<AudioSource>().loop = true;
            Camera.main.GetComponent<AudioSource>().Stop();
            Camera.main.GetComponent<AudioSource>().Play();
        }
        StartCoroutine(main());
    }

    #region Main Functions
    IEnumerator main()
    {
        while (transform.position.y > yPosition)
        {
            GetComponent<EnemyHealth>().invulnerable = true;
            GetComponent<Mover>().enabled = true;
            if (GetComponent<HorizontalOnlyMover>()) GetComponent<HorizontalOnlyMover>().enabled = false;
            yield return new WaitForEndOfFrame();
        }
        GetComponent<EnemyHealth>().invulnerable = false;
        GetComponent<Mover>().enabled = false;
        if (GetComponent<HorizontalOnlyMover>()) GetComponent<HorizontalOnlyMover>().enabled = true;
        while (true)
        {
            if (!GameController.instance.gameOver && !GameController.instance.won && !usingAbility)
            {
                yield return new WaitForSeconds(Random.Range(abilityTime.x, abilityTime.y));
                if (!GameController.instance.gameOver && !GameController.instance.won && !GameController.instance.paused && !usingAbility)
                {
                    float random = Random.value;
                    if (random <= 0.2f) //Toxic Blast (20% chance)
                    {
                        StartCoroutine(toxicBlast());
                    } else if (random <= 0.55f) //X-Scissors (35% chance)
                    {
                        StartCoroutine(xScissors());
                    } else //Spiralic Gestation (45% chance)
                    {
                        StartCoroutine(spiralicGestation());
                    }
                }
            } else
            {
                yield return null;
            }
        }
    }

    GameObject spawnProjectile(GameObject projectile, Vector3 spawnPosition, float spawnRotation, float spreadDegree, float speed, bool turnToPlayer)
    {
        GameObject bullet = Instantiate(projectile, spawnPosition, Quaternion.Euler(0, 0, spawnRotation));
        if (turnToPlayer)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player)
            {
                Vector3 direction = player.transform.position - projectile.transform.position;
                projectile.transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg, Vector3.forward);
            }
        }
        if (spreadDegree != 0) bullet.transform.Rotate(0, 0, Random.Range(-spreadDegree, spreadDegree));
        if (bullet.GetComponent<Mover>()) bullet.GetComponent<Mover>().speed = speed;
        return bullet;
    }
    #endregion

    #region Ability Functions
    IEnumerator spiralicGestation()
    {
        float angle = 0;
        usingAbility = true;
        if (audioSource)
        {
            if (spiralicGestationFireSound)
            {
                audioSource.PlayOneShot(spiralicGestationFireSound);
            } else
            {
                audioSource.Play();
            }
        }
        for (int i = 0; i < 19; i++)
        {
            spawnProjectile(orb, bulletSpawns[3].position, angle, 0, orb.GetComponent<Mover>().speed, false);
            spawnProjectile(orb, bulletSpawns[3].position, angle + 180, 0, orb.GetComponent<Mover>().speed, false);
            spawnProjectile(orb, bulletSpawns[3].position, angle + 45, 0, orb.GetComponent<Mover>().speed, false);
            spawnProjectile(orb, bulletSpawns[3].position, angle - 45, 0, orb.GetComponent<Mover>().speed, false);
            spawnProjectile(orb, bulletSpawns[3].position, angle + 90, 0, orb.GetComponent<Mover>().speed, false);
            spawnProjectile(orb, bulletSpawns[3].position, angle - 90, 0, orb.GetComponent<Mover>().speed, false);
            spawnProjectile(orb, bulletSpawns[3].position, angle + 135, 0, orb.GetComponent<Mover>().speed, false);
            spawnProjectile(orb, bulletSpawns[3].position, angle - 135, 0, orb.GetComponent<Mover>().speed, false);
            angle += 2.5f;
            yield return new WaitForSeconds(spiralicGestationFireRate);
        }
        usingAbility = false;
    }

    IEnumerator toxicBlast()
    {
        float angle = Random.Range(-180, 180);
        usingAbility = true;
        for (int i = 0; i < toxicBlastShots; i++)
        {
            spawnProjectile(doubleOrb, bulletSpawns[2].position, angle, 0, doubleOrb.GetComponent<Mover>().speed, false);
            spawnProjectile(doubleOrb, bulletSpawns[2].position, angle + 180, 0, doubleOrb.GetComponent<Mover>().speed, false);
            spawnProjectile(doubleOrb, bulletSpawns[2].position, angle + 45, 0, doubleOrb.GetComponent<Mover>().speed, false);
            spawnProjectile(doubleOrb, bulletSpawns[2].position, angle - 45, 0, doubleOrb.GetComponent<Mover>().speed, false);
            spawnProjectile(doubleOrb, bulletSpawns[2].position, angle + 90, 0, doubleOrb.GetComponent<Mover>().speed, false);
            spawnProjectile(doubleOrb, bulletSpawns[2].position, angle - 90, 0, doubleOrb.GetComponent<Mover>().speed, false);
            spawnProjectile(doubleOrb, bulletSpawns[2].position, angle + 135, 0, doubleOrb.GetComponent<Mover>().speed, false);
            spawnProjectile(doubleOrb, bulletSpawns[2].position, angle - 135, 0, doubleOrb.GetComponent<Mover>().speed, false);
            angle = Random.Range(-180, 180);
            if (audioSource)
            {
                if (toxicBlastFireSound)
                {
                    audioSource.PlayOneShot(toxicBlastFireSound);
                } else
                {
                    audioSource.Play();
                }
            }
            yield return new WaitForSeconds(toxicBlastFireRate);
        }
        usingAbility = false;
    }
    
    IEnumerator xScissors()
    {
        usingAbility = true;
        for (int i = 0; i < xScissorsShots; i ++)
        {
            spawnProjectile(orb, bulletSpawns[0].position, -135, 0, orb.GetComponent<Mover>().speed * 1.25f, false);
            spawnProjectile(orb, bulletSpawns[1].position, 135, 0, orb.GetComponent<Mover>().speed * 1.25f, false);
            spawnProjectile(orb, bulletSpawns[2].position, 180, 0, orb.GetComponent<Mover>().speed * 1.25f, false);
            if (audioSource)
            {
                if (xScissorsFireSound)
                {
                    audioSource.PlayOneShot(xScissorsFireSound);
                } else
                {
                    audioSource.Play();
                }
            }
            yield return new WaitForSeconds(xScissorsFireRate);
        }
        usingAbility = false;
    }
    #endregion
}