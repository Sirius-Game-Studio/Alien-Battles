using System.Collections;
using UnityEngine;

public class EarthBreaker : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Vector2 abilityTime = new Vector2(3.75f, 4.25f);
    [Tooltip("The Y position this enemy stops at.")] [SerializeField] private float yPosition = 4;
    [Tooltip("The music to play after this enemy spawns.")] [SerializeField] private AudioClip music = null;

    [Header("Conical Ray")]
    [SerializeField] private float conicalRaySpread = 20;
    [SerializeField] private float conicalRayFireRate = 0.125f;
    [SerializeField] private int conicalRayShots = 35;

    [Header("Plunging Rocks")]
    [SerializeField] private float plungingRocksFireRate = 1.1f;
    [SerializeField] private int plungingRocksShots = 3;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip conicalRayFireSound = null;
    [SerializeField] private AudioClip plungingRocksFireSound = null;
    [SerializeField] private AudioClip hollowedBombFireSound = null;

    [Header("Setup")]
    [SerializeField] private GameObject laser = null;
    [SerializeField] private GameObject orb = null;
    [SerializeField] private GameObject massiveOrb = null;
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
                    if (random <= 0.33f)
                    {
                        hollowedBomb();
                    } else if (random <= 0.66f)
                    {
                        StartCoroutine(plungingRocks());
                    } else
                    {
                        StartCoroutine(conicalRay());
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
    IEnumerator conicalRay()
    {
        usingAbility = true;
        int bulletSpawn;
        for (int i = 0; i < conicalRayShots; i++)
        {
            if (Random.value <= 0.5f)
            {
                bulletSpawn = 0;
            } else
            {
                bulletSpawn = 1;
            }
            spawnProjectile(laser, bulletSpawns[bulletSpawn].position, 180, conicalRaySpread, laser.GetComponent<Mover>().speed * 0.75f, false);
            if (audioSource)
            {
                if (conicalRayFireSound)
                {
                    audioSource.PlayOneShot(conicalRayFireSound);
                } else
                {
                    audioSource.Play();
                }
            }
            yield return new WaitForSeconds(conicalRayFireRate);
        }
        usingAbility = false;
    }

    IEnumerator plungingRocks()
    {
        usingAbility = true;
        float angle = 0;
        int bulletSpawn;
        for (int i = 0; i < plungingRocksShots; i++)
        {
            if (Random.value <= 0.5f)
            {
                bulletSpawn = 2;
            } else
            {
                bulletSpawn = 3;
            }
            spawnProjectile(massiveOrb, bulletSpawns[bulletSpawn].position, 180, 0, massiveOrb.GetComponent<Mover>().speed, false);
            for (int s = 0; s < 12; s++)
            {
                spawnProjectile(orb, bulletSpawns[bulletSpawn].position, angle, 0, orb.GetComponent<Mover>().speed, false);
                angle += 30;
            }
            if (audioSource)
            {
                if (plungingRocksFireSound)
                {
                    audioSource.PlayOneShot(plungingRocksFireSound);
                } else
                {
                    audioSource.Play();
                }
            }
            angle = 0;
            yield return new WaitForSeconds(plungingRocksFireRate);
        }
        usingAbility = false;
    }

    void hollowedBomb()
    {
        float angle = 0;
        int bulletSpawn;
        if (Random.value <= 0.5f)
        {
            bulletSpawn = 0;
        } else
        {
            bulletSpawn = 1;
        }
        for (int i = 0; i < 18; i++)
        {
            spawnProjectile(orb, bulletSpawns[bulletSpawn].position, angle, 0, orb.GetComponent<Mover>().speed * 0.75f, false);
            angle += 20;
        }
        if (audioSource)
        {
            if (hollowedBombFireSound)
            {
                audioSource.PlayOneShot(hollowedBombFireSound);
            } else
            {
                audioSource.Play();
            }
        }
    }
    #endregion
}