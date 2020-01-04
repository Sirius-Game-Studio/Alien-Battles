using System.Collections;
using UnityEngine;

public class BioSaucer : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Vector2 abilityTime = new Vector2(4, 4.25f);
    [Tooltip("The Y position this enemy stops at.")] [SerializeField] private float yPosition = 5;
    [Tooltip("The music to play after this enemy spawns.")] [SerializeField] private AudioClip music = null;

    [Header("Energetic Wave")]
    [SerializeField] private float energeticWaveFireRate = 0.14f;
    [SerializeField] private int energeticWaveShots = 15;

    [Header("C-Lamper")]
    [SerializeField] private float CLamperFireRate = 0.13f;
    [SerializeField] private float CLamperShots = 30;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip energeticWaveFireSound = null;
    [SerializeField] private AudioClip CLamperFireSound = null;
    [SerializeField] private AudioClip seekingSphereFireSound = null;

    [Header("Setup")]
    [SerializeField] private GameObject wave = null;
    [SerializeField] private GameObject homingOrb = null;
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
                    if (random <= 0.25f) //Seeking Sphere (25% chance)
                    {
                        seekingSphere();
                    } else if (random <= 0.6f) //C-Lamper (35% chance)
                    {
                        StartCoroutine(CLamper());
                    } else //Energetic Wave (40% chance)
                    {
                        StartCoroutine(energeticWave());
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
    IEnumerator energeticWave()
    {
        usingAbility = true;
        for (int i = 0; i < energeticWaveShots; i++)
        {
            spawnProjectile(wave, bulletSpawns[2].position, 180, 0, wave.GetComponent<Mover>().speed, false);
            spawnProjectile(wave, bulletSpawns[2].position, 160, 0, wave.GetComponent<Mover>().speed, false);
            spawnProjectile(wave, bulletSpawns[2].position, -160, 0, wave.GetComponent<Mover>().speed, false);
            if (audioSource)
            {
                if (energeticWaveFireSound)
                {
                    audioSource.PlayOneShot(energeticWaveFireSound);
                } else
                {
                    audioSource.Play();
                }
            }
            yield return new WaitForSeconds(energeticWaveFireRate);
        }
        usingAbility = false;
    }

    IEnumerator CLamper()
    {
        usingAbility = true;
        for (int i = 0; i < CLamperShots; i++)
        {
            spawnProjectile(wave, bulletSpawns[0].position, 180, 0, wave.GetComponent<Mover>().speed, false);
            spawnProjectile(wave, bulletSpawns[1].position, 180, 0, wave.GetComponent<Mover>().speed, false);
            if (audioSource)
            {
                if (CLamperFireSound)
                {
                    audioSource.PlayOneShot(CLamperFireSound);
                } else
                {
                    audioSource.Play();
                }
            }
            yield return new WaitForSeconds(CLamperFireRate);
        }
        usingAbility = false;
    }

    void seekingSphere()
    {
        int bulletSpawn;
        if (Random.value <= 0.5f)
        {
            bulletSpawn = 0;
        } else
        {
            bulletSpawn = 1;
        }
        spawnProjectile(homingOrb, bulletSpawns[bulletSpawn].position, 180, 0, 5, true);
        if (audioSource)
        {
            if (seekingSphereFireSound)
            {
                audioSource.PlayOneShot(seekingSphereFireSound);
            } else
            {
                audioSource.Play();
            }
        }
    }
    #endregion
}