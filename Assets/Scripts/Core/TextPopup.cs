using System.Collections;
using UnityEngine;
using TMPro;

public class TextPopup : MonoBehaviour
{
    [SerializeField] private float alphaIncrease = 5;
    [SerializeField] private float lifetime = 1;

    private TextMeshPro textMesh;

    void Start()
    {
        textMesh = GetComponent<TextMeshPro>();
        StartCoroutine(animateTMP());
    }

    IEnumerator animateTMP()
    {
        textMesh.alpha = 0;
        while (textMesh.alpha < 1)
        {
            textMesh.alpha += alphaIncrease * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }
}