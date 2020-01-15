using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;

public class MainMenuManager : MonoBehaviour
{
    [Header("Sound Effects")]
    [SerializeField] private AudioClip buttonClick = null;

    [Header("Setup")]
    [SerializeField] private Canvas mainMenu = null;
    [SerializeField] private Canvas shopMenu = null;
    [SerializeField] private Canvas settingsMenu = null;
    [SerializeField] private Canvas gamemodesMenu = null;
    [SerializeField] private Canvas levelSelectMenu = null;
    [SerializeField] private Canvas perksMenu = null;
    [SerializeField] private Canvas upgradesMenu = null;
    [SerializeField] private Canvas IAPShopMenu = null;
    [SerializeField] private GameObject loadingScreen = null;
    [SerializeField] private AudioMixer audioMixer = null;
    
    private AudioSource audioSource;
    private Canvas lastCanvas;
    private bool loading = false;

    void Awake()
    {
        #if !UNITY_EDITOR
        Application.targetFrameRate = 60;
        #endif
        audioSource = GetComponent<AudioSource>();
        if (audioSource) audioSource.ignoreListenerPause = true;
        lastCanvas = mainMenu;
        loading = false;
        Time.timeScale = 1;
        AudioListener.pause = false;
        if (!PlayerPrefs.HasKey("SoundVolume"))
        {
            PlayerPrefs.SetFloat("SoundVolume", 1);
            PlayerPrefs.Save();
        } else
        {
            audioMixer.SetFloat("SoundVolume", Mathf.Log10(PlayerPrefs.GetFloat("SoundVolume")) * 20);
        }
        if (!PlayerPrefs.HasKey("MusicVolume"))
        {
            PlayerPrefs.SetFloat("MusicVolume", 1);
            PlayerPrefs.Save();
        } else
        {
            audioMixer.SetFloat("MusicVolume", Mathf.Log10(PlayerPrefs.GetFloat("MusicVolume")) * 20);
        }
        PlayerPrefs.DeleteKey("IngameLevel");
        mainMenu.enabled = true;
        shopMenu.enabled = false;
        settingsMenu.enabled = false;
        gamemodesMenu.enabled = false;
        levelSelectMenu.enabled = false;
        perksMenu.enabled = false;
        upgradesMenu.enabled = false;
        IAPShopMenu.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (shopMenu.enabled)
            {
                shopMenu.enabled = false;
                mainMenu.enabled = true;
            } else if (settingsMenu.enabled)
            {
                settingsMenu.enabled = false;
                mainMenu.enabled = true;
            } else if (gamemodesMenu.enabled)
            {
                gamemodesMenu.enabled = false;
                mainMenu.enabled = true;
            } else if (levelSelectMenu.enabled)
            {
                levelSelectMenu.enabled = false;
                mainMenu.enabled = true;
            } else if (shopMenu.enabled)
            {
                shopMenu.enabled = false;
                mainMenu.enabled = false;
            } else if (perksMenu.enabled)
            {
                perksMenu.enabled = false;
                shopMenu.enabled = true;
            } else if (upgradesMenu.enabled)
            {
                upgradesMenu.enabled = false;
                shopMenu.enabled = true;
            } else if (IAPShopMenu.enabled)
            {
                IAPShopMenu.enabled = false;
                lastCanvas.enabled = true;
            }
        }
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
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.DeleteKey("IngameLevel");
    }

    #region Main Functions
    void playButtonClick()
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
    }

    public void grantMoney(int amount)
    {
        if (PlayerPrefs.GetString("Money") != "")
        {
            long money = long.Parse(PlayerPrefs.GetString("Money"));
            money += amount;
            PlayerPrefs.SetString("Money", money.ToString());
        } else
        {
            PlayerPrefs.SetString("Money", amount.ToString());
        }
        PlayerPrefs.Save();
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
                mainMenu.enabled = false;
                shopMenu.enabled = false;
                settingsMenu.enabled = false;
                gamemodesMenu.enabled = false;
                levelSelectMenu.enabled = false;
                perksMenu.enabled = false;
                upgradesMenu.enabled = false;
                IAPShopMenu.enabled = false;
                yield return null;
            }
        }
    }
    #endregion

    #region Menu Functions
    public void startLevel(int level)
    {
        playButtonClick();
        if (PlayerPrefs.GetInt("Level") >= level)
        {
            PlayerPrefs.SetInt("IngameLevel", level);
            PlayerPrefs.Save();
            StartCoroutine(loadScene("Level " + level));
        }
    }

    public void startEndless()
    {
        playButtonClick();
        StartCoroutine(loadScene("Endless"));
    }

    public void openCanvasFromMainMenu(Canvas canvas)
    {
        playButtonClick();
        if (!canvas.enabled)
        {
            canvas.enabled = true;
            mainMenu.enabled = false;
            lastCanvas = canvas;
        } else
        {
            canvas.enabled = false;
            mainMenu.enabled = true;
            lastCanvas = mainMenu;
        }
    }

    public void openCanvasFromShop(Canvas canvas)
    {
        playButtonClick();
        if (!canvas.enabled)
        {
            canvas.enabled = true;
            shopMenu.enabled = false;
            lastCanvas = canvas;
            ShopManager.instance.open = true;
        } else
        {
            canvas.enabled = false;
            shopMenu.enabled = true;
            lastCanvas = shopMenu;
            ShopManager.instance.open = false;
        }
    }

    public void openCanvasFromGamemodes(Canvas canvas)
    {
        playButtonClick();
        if (!canvas.enabled)
        {
            canvas.enabled = true;
            gamemodesMenu.enabled = false;
            lastCanvas = canvas;
        } else
        {
            canvas.enabled = false;
            gamemodesMenu.enabled = true;
            lastCanvas = gamemodesMenu;
        }
    }

    public void openIAPShop()
    {
        playButtonClick();
        if (!IAPShopMenu.enabled)
        {
            IAPShopMenu.enabled = true;
            mainMenu.enabled = false;
            shopMenu.enabled = false;
            settingsMenu.enabled = false;
            gamemodesMenu.enabled = false;
            levelSelectMenu.enabled = false;
            perksMenu.enabled = false;
            upgradesMenu.enabled = false;
        } else
        {
            IAPShopMenu.enabled = false;
            lastCanvas.enabled = true;
        }
    }

    public void quitGame()
    {
        playButtonClick();
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
    #endregion
}