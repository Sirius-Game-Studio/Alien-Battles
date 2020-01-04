using UnityEngine;

public class DestroyByBoundary : MonoBehaviour
{
    void OnTriggerExit2D(Collider2D other)
    {
        if (CompareTag("Boundary") && !other.CompareTag("Player")) Destroy(other.gameObject);
    }
}