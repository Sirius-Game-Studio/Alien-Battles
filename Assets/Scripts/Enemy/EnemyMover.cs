using UnityEngine;

public class EnemyMover : MonoBehaviour
{
    [SerializeField] private float speed = 0.5f;
    public float initialY = 14;

    private bool hit = false;

    void Update()
    {
        Vector3 screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        float[] widths = new float[transform.childCount];
        foreach (Transform enemy in transform)
        {
            if (enemy.CompareTag("Enemy") && enemy.GetComponent<Collider2D>())
            {
                for (int i = 0; i < transform.childCount; i++) widths[i] = enemy.GetComponent<Collider2D>().bounds.extents.x * 0;
            }
        }
        if (transform.position.y <= 6)
        {
            transform.position += new Vector3(-speed, 0, 0) * Time.deltaTime;
            if (!hit)
            {
                foreach (Transform enemy in transform)
                {
                    for (int i = 0; i < widths.Length; i++)
                    {
                        if (enemy.position.x <= screenBounds.x * -1 + widths[i] || enemy.position.x >= screenBounds.x - widths[i]) hit = true;
                    }
                }
                if (hit)
                {
                    speed = -speed;
                    if (speed < 0)
                    {
                        transform.position += new Vector3(0.005f, 0, 0);
                    } else if (speed > 0)
                    {
                        transform.position -= new Vector3(0.005f, 0, 0);
                    }
                    hit = false;
                }
            }
        } else
        {
            transform.position -= new Vector3(0, 7, 0) * Time.deltaTime;
        }
    }
}