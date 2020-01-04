using UnityEngine;

public class EnemyGun : MonoBehaviour
{
    public Vector2 fireRate = new Vector2(0.25f, 4);

    private AudioSource audioSource;
    private float currentFireRate = 0;
    private float nextShot = 0;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        currentFireRate = Random.Range(fireRate.x, fireRate.y);
    }

    void Update()
    {
        if (!GameController.instance.gameOver && !GameController.instance.won)
        {
            if (nextShot >= currentFireRate)
            {
                Alien[] aliens = GetComponentsInChildren<Alien>();
                Alien alien = aliens[Random.Range(0, aliens.Length)];
                if (aliens.Length > 0)
                {
                    if (alien)
                    {
                        currentFireRate = Random.Range(fireRate.x, fireRate.y);
                        nextShot = 0;
                        alien.fire();
                    }
                } else
                {
                    nextShot = 0;
                }
            } else
            {
                nextShot += Time.deltaTime;
            }
        }
    }
}