using UnityEngine;

public class HorizontalOnlyMover : MonoBehaviour
{
    public float speed = 1;
    public float initialY = 11;

    private bool left = true;

    void Update()
    {
        Vector3 screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        float width = GetComponent<Collider2D>().bounds.extents.x;
        Vector3 movement;
        if (left)
        {
            movement = -Vector3.right.normalized * speed * Time.deltaTime;
        } else
        {
            movement = Vector3.right.normalized * speed * Time.deltaTime;
        }
        transform.position += movement;
        if (transform.position.x <= screenBounds.x * -1 + width)
        {
            left = false;
            transform.position += new Vector3(0.005f, 0, 0);
        } else if (transform.position.x >= screenBounds.x - width)
        {
            left = true;
            transform.position -= new Vector3(0.005f, 0, 0);
        }
    }
}