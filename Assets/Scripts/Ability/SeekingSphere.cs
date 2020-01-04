using UnityEngine;

public class SeekingSphere : MonoBehaviour
{
    [SerializeField] private float speed = 4.5f;
    [SerializeField] private float lifetime = 5;

    private GameObject player;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        Invoke("destroyOrb", lifetime);
    }
    
    void Update()
    {
        transform.position += transform.up * speed * Time.deltaTime;
        if (player)
        {
            Vector3 direction = player.transform.position - transform.position;
            float angle = Mathf.Atan2(-direction.x, direction.y) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    void destroyOrb()
    {
        Destroy(gameObject);
    }
}