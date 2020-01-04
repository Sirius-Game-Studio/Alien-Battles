using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private Text loadingText = null;
    [SerializeField] private Image spaceship = null;

    void OnEnable()
    {
        if (loadingText)
        {
            loadingText.text = "LOADING";
            StartCoroutine(updateLoadingText());
        }
        if (spaceship)
        {
            spaceship.rectTransform.rotation = Quaternion.Euler(0, 0, 0);
            StartCoroutine(spinSpaceship());
        }
    }

    void OnDisable()
    {
        if (loadingText) loadingText.text = "LOADING";
        if (spaceship) spaceship.rectTransform.rotation = Quaternion.Euler(0, 0, 0);
        StopAllCoroutines();
    }

    IEnumerator updateLoadingText()
    {
        while (enabled)
        {
            loadingText.text = "LOADING";
            yield return new WaitForSecondsRealtime(1);
            loadingText.text = "LOADING.";
            yield return new WaitForSecondsRealtime(1);
            loadingText.text = "LOADING..";
            yield return new WaitForSecondsRealtime(1);
            loadingText.text = "LOADING...";
            yield return new WaitForSecondsRealtime(1);
        }
    }

    IEnumerator spinSpaceship()
    {
        while (enabled)
        {
            spaceship.rectTransform.Rotate(0, 0, 90 * Time.unscaledDeltaTime);
            yield return new WaitForEndOfFrame();
        }
    }
}
