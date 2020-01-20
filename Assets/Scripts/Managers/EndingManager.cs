using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class EndingManager : MonoBehaviour
{
    [Header("Credits Settings")]
    [Tooltip("The Y position credits start at.")] [SerializeField] private float creditsY = 870;
    [SerializeField] private float scrollSpeed = 0.5f;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip buttonClick = null;

    [Header("Setup")]
    [SerializeField] private Canvas endingMenu = null;
    [SerializeField] private Canvas creditsMenu = null;
    [SerializeField] private RectTransform credits = null;
    [SerializeField] private GameObject loadingScreen = null;
    [SerializeField] private AudioMixer audioMixer = null;
    
    private AudioSource audioSource;
    private Controls input;
    private float fastScrollSpeed = 1;
    private bool loading = false;

    void Awake()
    {
        #if !UNITY_EDITOR
        Application.targetFrameRate = 60;
        #endif
        audioSource = GetComponent<AudioSource>();
        input = new Controls();
        if (audioSource) audioSource.ignoreListenerPause = true;
        fastScrollSpeed = scrollSpeed * 2;
        loading = false;
        Time.timeScale = 1;
        AudioListener.pause = false;
        if (!PlayerPrefs.HasKey("SoundVolume"))
        {
            PlayerPrefs.SetFloat("SoundVolume", 1);
        } else
        {
            audioMixer.SetFloat("SoundVolume", Mathf.Log10(PlayerPrefs.GetFloat("SoundVolume")) * 20);
        }
        if (!PlayerPrefs.HasKey("MusicVolume"))
        {
            PlayerPrefs.SetFloat("MusicVolume", 1);
        } else
        {
            audioMixer.SetFloat("MusicVolume", Mathf.Log10(PlayerPrefs.GetFloat("MusicVolume")) * 20);
        }
        PlayerPrefs.DeleteKey("IngameLevel");
        PlayerPrefs.Save();
        endingMenu.enabled = true;
        creditsMenu.enabled = false;
    }

    void OnEnable()
    {
        input.Enable();
        input.Gameplay.Press.performed += context => speedUpCredits(true);
        input.Gameplay.Press.canceled += context => speedUpCredits(false);
    }

    void OnDisable()
    {
        input.Disable();
        input.Gameplay.Press.performed -= context => speedUpCredits(true);
        input.Gameplay.Press.canceled -= context => speedUpCredits(false);
    }

    void Update()
    {
        audioMixer.SetFloat("SoundVolume", Mathf.Log10(PlayerPrefs.GetFloat("SoundVolume")) * 20);
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(PlayerPrefs.GetFloat("MusicVolume")) * 20);
        if (Input.GetKeyDown(KeyCode.Escape) && creditsMenu.enabled)
        {
            creditsMenu.enabled = false;
            endingMenu.enabled = true;
            StopCoroutine(scrollCredits());
        }
        if (!creditsMenu.enabled) credits.anchoredPosition = new Vector2(0, creditsY);
        if (!loading)
        {
            loadingScreen.SetActive(false);
        } else
        {
            loadingScreen.SetActive(true);
        }
        if (long.Parse(PlayerPrefs.GetString("Money")) > 99999999)
        {
            PlayerPrefs.SetString("Money", "99999999");
            PlayerPrefs.Save();
        }
        if (long.Parse(PlayerPrefs.GetString("HighScore")) < 0)
        {
            PlayerPrefs.SetString("HighScore", "0");
            PlayerPrefs.Save();
        }
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.DeleteKey("IngameLevel");
    }

    #region Input Functions
    void speedUpCredits(bool state)
    {
        if (state)
        {
            scrollSpeed = fastScrollSpeed;
        } else
        {
            scrollSpeed = fastScrollSpeed * 0.5f;
        }
    }
    #endregion

    #region Main Functions
    IEnumerator scrollCredits()
    {
        while (creditsMenu.enabled)
        {
            yield return new WaitForEndOfFrame();
            if (creditsMenu.enabled) credits.anchoredPosition -= new Vector2(0, scrollSpeed);
            if (credits.anchoredPosition.y <= -creditsY)
            {
                endingMenu.enabled = true;
                creditsMenu.enabled = false;
                yield break;
            }
        }
    }

    IEnumerator loadScene(string scene)
    {
        if (!loading)
        {
            loading = true;
            AsyncOperation load = SceneManager.LoadSceneAsync(scene);
            if (Camera.main.GetComponent<AudioSource>()) Camera.main.GetComponent<AudioSource>().Stop();
            while (!load.isDone)
            {
                Time.timeScale = 0;
                AudioListener.pause = true;
                endingMenu.enabled = false;
                creditsMenu.enabled = false;
                yield return null;
            }
        }
    }
    #endregion

    #region Menu Functions
    public void clickCredits()
    {
        if (audioSource)
        {
            if (buttonClick)
            {
                audioSource.PlayOneShot(buttonClick);
            } else
            {
                audioSource.Play();
            }
        }
        if (!creditsMenu.enabled)
        {
            creditsMenu.enabled = true;
            endingMenu.enabled = false;
            StartCoroutine(scrollCredits());
        } else
        {
            creditsMenu.enabled = false;
            endingMenu.enabled = true;
            StopCoroutine(scrollCredits());
        }
    }

    public void exitToMainMenu()
    {
        if (audioSource)
        {
            if (buttonClick)
            {
                audioSource.PlayOneShot(buttonClick);
            } else
            {
                audioSource.Play();
            }
        }
        StartCoroutine(loadScene("Main Menu"));
    }
    #endregion
}