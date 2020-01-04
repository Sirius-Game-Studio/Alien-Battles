using System.Collections;
using UnityEngine;

public class SpriteAnimation : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool loop = false;

    [Header("Setup")]
    [SerializeField] private Sprite[] frames = new Sprite[0];
    [SerializeField] private float[] nextFrame = new float[0];

    private SpriteRenderer spriteRenderer;
    private int frame = 0;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(animateSprite());
    }

    IEnumerator animateSprite()
    {
        spriteRenderer.sprite = frames[0];
        if (!loop)
        {
            while (frame < frames.Length - 1)
            {
                yield return new WaitForSeconds(nextFrame[frame]);
                ++frame;
                spriteRenderer.sprite = frames[frame];
            }
            yield return new WaitForSeconds(nextFrame[frames.Length - 1]);
            spriteRenderer.sprite = null;
        } else
        {
            while (true)
            {
                yield return new WaitForSeconds(nextFrame[frame]);
                if (frame < frames.Length - 1)
                {
                    ++frame;
                    spriteRenderer.sprite = frames[frame];
                } else
                {
                    frame = 0;
                    spriteRenderer.sprite = frames[0];
                }
            }
        }
    }
}
