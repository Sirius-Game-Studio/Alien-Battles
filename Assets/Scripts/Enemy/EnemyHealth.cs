using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Settings")]
    public long health = 20;
    [Tooltip("Values below 1 reduce damage and values above 1 increase damage.")] public float defense = 1;
    [Tooltip("Powerup drop chance.")] [Range(0, 1)] [SerializeField] private float powerupChance = 0.07f;
    [Tooltip("Coin drop chance (Campaign only).")] [Range(0, 1)] [SerializeField] private float coinChance = 0.35f;
    [Tooltip("Amount of score given on death (Endless only).")] [SerializeField] private long score = 5;

    [Header("Setup")]
    [SerializeField] private GameObject[] powerups = new GameObject[0];
    [SerializeField] private GameObject coin = null;
    [SerializeField] private GameObject explosion = null;

    [HideInInspector] public long maxHealth = 0;
    [HideInInspector] public bool invulnerable = false;
    private bool dropped = false;
    private long mh = 0; //Stores the max health value

    void Start()
    {
        if (GameController.instance.gamemode == GameController.Gamemodes.Campaign)
        {
            if (PlayerPrefs.GetInt("IngameLevel") > 1 && gameObject.layer != 8) for (int i = 0; i < PlayerPrefs.GetInt("IngameLevel") - 1; i++) health += 2;
        } else if (GameController.instance.gamemode == GameController.Gamemodes.Endless)
        {
            if (GameController.instance.wave > 1)
            {
                float multiplier = 1;
                for (long i = 0; i < GameController.instance.wave - 1; i++) multiplier += 0.2f;
                if (multiplier > 5) multiplier = 5;
                health = (long)(health * multiplier);
            }
            powerupChance *= 0.75f;
            coinChance = 0;
        }
        maxHealth = health;
        mh = maxHealth;
    }

    void Update()
    {
        if (maxHealth != mh) maxHealth = mh; //Checks if max health is different than the stored value
        if (health > maxHealth)
        {
            health = maxHealth;
        } else if (health <= 0)
        {
            if (powerups.Length > 0 && Random.value <= powerupChance && !dropped)
            {
                Instantiate(powerups[Random.Range(0, powerups.Length)], transform.position, Quaternion.Euler(0, 0, 0));
                dropped = true;
            }
            if (GameController.instance.gamemode == GameController.Gamemodes.Campaign && coin && Random.value <= coinChance && !dropped)
            {
                Instantiate(coin, transform.position, Quaternion.Euler(0, 0, 0));
                dropped = true;
            }
            if (GameController.instance.gamemode == GameController.Gamemodes.Endless && score > 0) GameController.instance.score += score;
            if (explosion) Instantiate(explosion, transform.position, Quaternion.Euler(0, 0, 0));
            Destroy(gameObject);
        }
    }

    public void takeDamage(long damage)
    {
        if (!invulnerable)
        {
            if (damage > 0)
            {
                health -= damage;
            } else
            {
                --health;
            }
        }
    }
}