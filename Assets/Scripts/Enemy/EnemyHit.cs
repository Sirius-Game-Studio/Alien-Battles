using UnityEngine;

public class EnemyHit : MonoBehaviour
{
    [SerializeField] private GameObject explosion = null;

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.GetComponent<PlayerController>() && !other.GetComponent<PlayerController>().invulnerable)
        {
            other.GetComponent<PlayerController>().die();
            if (explosion) Instantiate(explosion, transform.position, Quaternion.Euler(0, 0, 0));
            Destroy(gameObject);
        }
    }
}
