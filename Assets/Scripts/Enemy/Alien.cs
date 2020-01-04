using UnityEngine;

public class Alien : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float spreadDegree = 0;
    [SerializeField] private int shots = 1;
    [SerializeField] private bool turnToPlayer = false;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip fireSound = null;

    [Header("Setup")]
    [SerializeField] private GameObject bullet = null;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void fire()
    {
        if (!GameController.instance.gameOver && !GameController.instance.won)
        {
            bool foundBulletSpawns = false;
            for (int i = 0; i < shots; i++)
            {
                foreach (Transform bulletSpawn in transform)
                {
                    if (bulletSpawn.CompareTag("BulletSpawn") && bulletSpawn.gameObject.activeSelf)
                    {
                        GameObject newBullet = Instantiate(bullet, new Vector3(bulletSpawn.position.x, bulletSpawn.position.y, 0), bulletSpawn.rotation);
                        if (turnToPlayer && GameObject.FindWithTag("Player")) newBullet.transform.LookAt(GameObject.FindWithTag("Player").transform);
                        if (spreadDegree != 0) newBullet.transform.Rotate(0, 0, Random.Range(-spreadDegree, spreadDegree));
                        foundBulletSpawns = true;
                    }
                }
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
    }
}
