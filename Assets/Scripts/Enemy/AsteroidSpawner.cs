using System.Collections;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Vector2 asteroidSpeed = new Vector2(3.5f, 6);
    [SerializeField] private Vector2 asteroidRotSpeed = new Vector2(-90, 90);
    [SerializeField] private Vector2 asteroidSpawnTime = new Vector2(0.25f, 0.4f);
    [SerializeField] private int asteroidAmount = 20;

    [Header("Setup")]
    [SerializeField] private GameObject[] asteroids = new GameObject[0];

    private int asteroidsSpawned = 0;

    void Start()
    {
        StartCoroutine(spawnAsteroids());
    }

    IEnumerator spawnAsteroids()
    {
        while (asteroidsSpawned < asteroidAmount)
        {
            Vector3 left = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));
            Vector3 right = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, Camera.main.nearClipPlane));
            GameObject newAsteroid = Instantiate(asteroids[Random.Range(0, asteroids.Length)], new Vector3(Random.Range(left.x, right.x), transform.position.y, 0), Quaternion.Euler(0, 0, Random.Range(-180, 180)));
            newAsteroid.transform.SetParent(transform);
            newAsteroid.GetComponent<Mover>().speed = Random.Range(asteroidSpeed.x, asteroidSpeed.y);
            newAsteroid.GetComponent<Rotator>().angle = Random.Range(asteroidRotSpeed.x, asteroidRotSpeed.y);
            ++asteroidsSpawned;
            yield return new WaitForSeconds(Random.Range(asteroidSpawnTime.x, asteroidSpawnTime.y));
        }
        while (transform.childCount > 0) yield return null;
        Destroy(gameObject);
    }
}
