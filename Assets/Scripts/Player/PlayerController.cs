using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    [Range(5, 50)] [SerializeField] private long damage = 10;
    [Range(200, 400)] [SerializeField] private float RPM = 300;
    [SerializeField] private float speed = 15;
    [Range(1, 10)] [SerializeField] private int maxWeaponPower = 5;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip fireSound = null;

    [Header("Setup")]
    [SerializeField] private GameObject[] weaponPower1Guns = new GameObject[0];
    [SerializeField] private GameObject[] weaponPower2Guns = new GameObject[0];
    [SerializeField] private GameObject[] weaponPower3Guns = new GameObject[0];
    [SerializeField] private GameObject[] weaponPower4Guns = new GameObject[0];
    [SerializeField] private GameObject[] weaponPower5Guns = new GameObject[0];
    [SerializeField] private GameObject bullet = null;
    [SerializeField] private GameObject explosion = null;

    private AudioSource audioSource;
    [HideInInspector] public int weaponPower = 1;
    [HideInInspector] public long lives = 3;
    [HideInInspector] public bool invulnerable = false;
    private float nextShot = 0;
    [HideInInspector] public bool animatingWinPose = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        lives = 3;
        invulnerable = false;
        if (PlayerPrefs.GetString("Perk") == "SharpLasers")
        {
            damage += 2;
            speed *= 0.85f;
        } else if (PlayerPrefs.GetString("Perk") == "TurboThrusters")
        {
            damage = (long)(damage * 0.8);
            speed *= 1.25f;
        }
        if (PlayerPrefs.HasKey("DamageMultiplier")) damage = (long)(damage * PlayerPrefs.GetFloat("DamageMultiplier"));
        if (PlayerPrefs.HasKey("SpeedMultiplier")) speed *= PlayerPrefs.GetFloat("SpeedMultiplier");
    }

    void Update()
    {
        if (!animatingWinPose)
        {
            if (!GameController.instance.gameOver && !GameController.instance.won && !GameController.instance.paused && GameController.instance.pauseButton.color != GameController.instance.pauseButton.GetComponent<ButtonHover>().hoverColor)
            {
                #if UNITY_EDITOR
                Vector3 newPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
                transform.position = Vector3.Lerp(transform.position, newPosition, speed * 0.375f * Time.deltaTime);
                #else
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
                {
                    Vector3 newPosition = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 0));
                    transform.position = Vector3.Lerp(transform.position, newPosition, speed * 0.375f * Time.deltaTime);
                }
                #endif
                if (Time.time >= nextShot) fire();
            }
            Vector3 screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
            float width = GetComponent<Collider2D>().bounds.extents.x * 0;
            float height = GetComponent<Collider2D>().bounds.extents.y * 0;
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, screenBounds.x * -1 + width, screenBounds.x - width), Mathf.Clamp(transform.position.y, screenBounds.y * -1 + height, 6), 0);
        }
        foreach (Transform bulletSpawn in transform)
        {
            if (bulletSpawn.CompareTag("BulletSpawn")) bulletSpawn.gameObject.SetActive(false);
        }
        if (weaponPower <= 1)
        {
            foreach (GameObject bulletSpawn in weaponPower1Guns)
            {
                if (bulletSpawn.CompareTag("BulletSpawn")) bulletSpawn.SetActive(true);
            }
        } else if (weaponPower == 2)
        {
            foreach (GameObject bulletSpawn in weaponPower2Guns)
            {
                if (bulletSpawn.CompareTag("BulletSpawn")) bulletSpawn.SetActive(true);
            }
        } else if (weaponPower == 3)
        {
            foreach (GameObject bulletSpawn in weaponPower3Guns)
            {
                if (bulletSpawn.CompareTag("BulletSpawn")) bulletSpawn.SetActive(true);
            }
        } else if (weaponPower == 4)
        {
            foreach (GameObject bulletSpawn in weaponPower4Guns)
            {
                if (bulletSpawn.CompareTag("BulletSpawn")) bulletSpawn.SetActive(true);
            }
        } else if (weaponPower >= maxWeaponPower)
        {
            foreach (GameObject bulletSpawn in weaponPower5Guns)
            {
                if (bulletSpawn.CompareTag("BulletSpawn")) bulletSpawn.SetActive(true);
            }
        }
        if (weaponPower < 1) //Checks if weapon power is less than 1
        {
            weaponPower = 1;
        } else if (weaponPower > maxWeaponPower) //Checks if weapon power is more than the maximum
        {
            weaponPower = maxWeaponPower;
        }
    }

    #region Main Functions
    void fire()
    {
        bool foundBulletSpawns = false;
        nextShot = Time.time + 60 / RPM;
        foreach (Transform bulletSpawn in transform)
        {
            if (bulletSpawn.CompareTag("BulletSpawn") && bulletSpawn.gameObject.activeSelf)
            {
                GameObject newBullet = Instantiate(bullet, new Vector3(bulletSpawn.position.x, bulletSpawn.position.y, 0), bulletSpawn.rotation);
                newBullet.GetComponent<Bullet>().damage = damage;
                foundBulletSpawns = true;
            }
        }
        if (!foundBulletSpawns)
        {
            GameObject newBullet = Instantiate(bullet, new Vector3(transform.position.x, transform.position.y + 1, 0), Quaternion.Euler(0, 0, 0));
            newBullet.GetComponent<Bullet>().damage = damage;
            foundBulletSpawns = true;
        }
        if (audioSource && foundBulletSpawns)
        {
            if (fireSound)
            {
                audioSource.PlayOneShot(fireSound);
            } else
            {
                audioSource.Play();
            }
        }
    }

    public void die()
    {
        if (!invulnerable)
        {
            if (!GameController.instance.won) GameController.instance.gameOver = true;
            if (explosion) Instantiate(explosion, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }

    public void startWinAnimation()
    {
        if (!animatingWinPose)
        {
            invulnerable = true;
            animatingWinPose = true;
            StartCoroutine(winAnimation());
        }
    }

    IEnumerator winAnimation()
    {
        yield return new WaitForSeconds(1);
        float y = transform.position.y - 0.75f;
        while (transform.position.y > y) //Moves the player down a little bit
        {
            transform.position -= Vector3.up * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        y = 10;
        while (transform.position.y < y) //Moves the player up until its Y position is 10 or up
        {
            transform.position += new Vector3(0, 23, 0) * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        Destroy(gameObject);
    }
    #endregion
}