using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float angle = 0;

    void Update()
    {
        transform.Rotate(new Vector3(0, 0, angle) * Time.deltaTime);
    }
}
