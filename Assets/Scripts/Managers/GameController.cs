using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    
    [Header("Settings")]
    [Tooltip("Amount of money earned after completing this level.")] [SerializeField] private long moneyReward = 40;
    [SerializeField] private Gamemodes gamemode = Gamemodes.Campaign;

    [Header("UI")]
    [SerializeField] private Canvas gameHUD = null;
    [SerializeField] private Canvas gamePausedMenu = null;
    [SerializeField] private Canvas gameOverMenu = null;
    [SerializeField] private Canvas levelCompletedMenu = null;
    [SerializeField] private Canvas settingsMenu = null;
    [SerializeField] private Canvas quitGameMenu = null;
    [SerializeField] private Canvas restartPrompt = null;
    [SerializeField] private Text waveText = null;
    [SerializeField] private Text bossName = null;
    [SerializeField] private Slider bossHealthBar = null;
    [SerializeField] private Text bossHealthText = null;
    public Image pauseButton = null;
    [SerializeField] private GameObject loadingScreen = null;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip loseJingle = null;
    [SerializeField] private AudioClip winJingle = null;
    [SerializeField] private AudioClip buttonClick = null;

    [Header("Setup")]
    [SerializeField] private GameObject[] enemyGroups = new GameObject[0];
    [SerializeField] private GameObject[] backgrounds = new GameObject[0];
    [SerializeField] private AudioClip[] randomMusic = new AudioClip[0];
    [SerializeField] private AudioMixer audioMixer = null;

    private AudioSource audioSource;
    private enum Gamemodes {Campaign, Endless};
    private enum ClickSources {GamePaused, GameOver, LevelCompleted};
    [HideInInspector] public bool gameOver = false;
    [HideInInspector] public bool won = false;
    [HideInInspector] public bool paused = false;
    private long wave = 1;
    [HideInInspector] public GameObject currentBoss;
    private long bossMaxHealth = 0;
    private bool completed = false;
    private bool playedLoseSound = false, playedWinSound = false;
    private ClickSources clickSource = ClickSources.GamePaused;
    private bool loading = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else if (instance != this)
        {
            Destroy(gameObject);
        }
        #if !UNITY_EDITOR
        Application.targetFrameRate = 60;
        #endif
        audioSource = GetComponent<AudioSource>();
        if (audioSource) audioSource.ignoreListenerPause = true;
        gameOver = false;
        won = false;
        paused = false;
        currentBoss = null;
        loading = false;
        Time.timeScale = 1;
        AudioListener.pause = false;

        //Destroys all enemy groups in the scene
        foreach (GameObject enemyGroup in GameObject.FindGameObjectsWithTag("EnemyGroup")) Destroy(enemyGroup);

        //Destroys all enemies in the scene
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy")) Destroy(enemy);

        //Destroys all objects with Coin script in the scene
        foreach (Coin coin in FindObjectsOfType<Coin>()) Destroy(coin.gameObject);

        //Destroys all backgrounds in the scene and instantiates a new background (only if backgrounds array's length is 1 or up)
        if (backgrounds.Length > 0)
        {
            foreach (GameObject background in GameObject.FindGameObjectsWithTag("Background")) Destroy(background);
            Instantiate(backgrounds[Random.Range(0, backgrounds.Length)], Vector3.zero, Quaternion.Euler(0, 0, 0));
        }

        //Randomizes music (only if randomMusic array's length is 1 or up)
        if (Camera.main.GetComponent<AudioSource>() && randomMusic.Length > 0)
        {
            Camera.main.GetComponent<AudioSource>().clip = randomMusic[Random.Range(0, randomMusic.Length)];
            Camera.main.GetComponent<AudioSource>().loop = true;
            Camera.main.GetComponent<AudioSource>().Stop();
            Camera.main.GetComponent<AudioSource>().Play();
        }

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
        gameHUD.enabled = true;
        gamePausedMenu.enabled = false;
        gameOverMenu.enabled = false;
        levelCompletedMenu.enabled = false;
        settingsMenu.enabled = false;
        quitGameMenu.enabled = false;
        restartPrompt.enabled = false;
        waveText.enabled = false;
        StartCoroutine(firstWaveSpawn());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && paused)
        {
            if (settingsMenu.enabled)
            {
                settingsMenu.enabled = false;
                if (clickSource == ClickSources.GamePaused)
                {
                    gamePausedMenu.enabled = true;
                } else if (clickSource == ClickSources.GameOver)
                {
                    gameOverMenu.enabled = true;
                } else if (clickSource == ClickSources.LevelCompleted)
                {
                    levelCompletedMenu.enabled = true;
                }
            } else if (quitGameMenu.enabled)
            {
                quitGameMenu.enabled = false;
                if (clickSource == ClickSources.GamePaused)
                {
                    gamePausedMenu.enabled = true;
                } else if (clickSource == ClickSources.GameOver)
                {
                    gameOverMenu.enabled = true;
                } else if (clickSource == ClickSources.LevelCompleted)
                {
                    levelCompletedMenu.enabled = true;
                }
            } else if (restartPrompt.enabled)
            {
                restartPrompt.enabled = false;
                if (clickSource == ClickSources.GamePaused)
                {
                    gamePausedMenu.enabled = true;
                } else if (clickSource == ClickSources.GameOver)
                {
                    gameOverMenu.enabled = true;
                } else if (clickSource == ClickSources.LevelCompleted)
                {
                    levelCompletedMenu.enabled = true;
                }
            }
        }
        if (gameOver)
        {
            clickSource = ClickSources.GameOver;
            if (!loading && !quitGameMenu.enabled) gameOverMenu.enabled = true;
            if (audioSource && loseJingle && !playedLoseSound)
            {
                playedLoseSound = true;
                audioSource.PlayOneShot(loseJingle);
            }
            if (Camera.main.GetComponent<AudioSource>()) Camera.main.GetComponent<AudioSource>().Stop();
            pauseButton.gameObject.SetActive(false);
        }
        if (gamemode == Gamemodes.Campaign && !gameOver && won)
        {
            clickSource = ClickSources.LevelCompleted;
            if (!loading && !quitGameMenu.enabled) levelCompletedMenu.enabled = true;
            if (!completed)
            {
                completed = true;
                int currentLevel = PlayerPrefs.GetInt("IngameLevel");
                if (PlayerPrefs.GetInt("Level") < PlayerPrefs.GetInt("MaxLevels") && PlayerPrefs.GetInt("Level") == currentLevel)
                {
                    PlayerPrefs.SetInt("Level", currentLevel + 1);
                    PlayerPrefs.Save();
                }
                long money = long.Parse(PlayerPrefs.GetString("Money"));
                if (!PlayerPrefs.HasKey("Completed" + currentLevel))
                {
                    money += moneyReward;
                    PlayerPrefs.SetString("Money", money.ToString());
                    PlayerPrefs.SetInt("Completed" + currentLevel, 1);
                } else
                {
                    money += (long)(moneyReward * 0.2);
                    PlayerPrefs.SetString("Money", money.ToString());
                }
                PlayerPrefs.Save();
            }
            if (audioSource && winJingle && !playedWinSound)
            {
                playedWinSound = true;
                audioSource.PlayOneShot(winJingle);
            }
            if (Camera.main.GetComponent<AudioSource>()) Camera.main.GetComponent<AudioSource>().Stop();
            pauseButton.gameObject.SetActive(false);
        }
        if (gamemode == Gamemodes.Campaign)
        {
            waveText.text = "WAVE " + wave + "/" + enemyGroups.LongLength;
        } else if (gamemode == Gamemodes.Endless)
        {
            waveText.text = "WAVE " + wave;
        }
        if (!currentBoss)
        {
            currentBoss = null;
            bossMaxHealth = 0;
            bossName.gameObject.SetActive(false);
        } else
        {
            if (bossMaxHealth <= 0) bossMaxHealth = currentBoss.GetComponent<EnemyHealth>().health;
            bossName.gameObject.SetActive(true);
            bossName.text = currentBoss.name;
            bossHealthBar.value = currentBoss.GetComponent<EnemyHealth>().health;
            bossHealthBar.maxValue = bossMaxHealth;
            bossHealthText.text = bossHealthBar.value + " / " + bossHealthBar.maxValue;
        }
        if (!loading)
        {
            loadingScreen.SetActive(false);
        } else
        {
            loadingScreen.SetActive(true);
        }
        if (gamemode == Gamemodes.Campaign)
        {
            if (wave < 1)
            {
                wave = 1;
            } else if (wave > enemyGroups.LongLength)
            {
                wave = enemyGroups.LongLength;
            }
        } else if (gamemode == Gamemodes.Endless)
        {
            if (wave < 1)
            {
                wave = 1;
            } else if (wave > 9223372036854775806)
            {
                wave = 9223372036854775806;
            }
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
    IEnumerator spawnWaves()
    {
        while (true)
        {
            if (GameObject.FindGameObjectsWithTag("Enemy").Length > 0)
            {
                yield return null;
            } else
            {
                if (gamemode == Gamemodes.Campaign)
                {
                    if (wave < enemyGroups.LongLength)
                    {
                        yield return new WaitForSeconds(2);
                        ++wave;
                        waveText.enabled = true;
                        waveText.text = "WAVE " + wave + "/" + enemyGroups.LongLength;
                        yield return new WaitForSeconds(1);
                        GameObject enemyGroup = enemyGroups[wave - 1];
                        if (enemyGroup.layer != 8)
                        {
                            if (enemyGroup.GetComponent<EnemyMover>())
                            {
                                Instantiate(enemyGroup, new Vector3(0, enemyGroup.GetComponent<EnemyMover>().initialY, 0), Quaternion.Euler(0, 0, 0));
                            } else if (enemyGroup.GetComponent<AsteroidSpawner>())
                            {
                                Instantiate(enemyGroup, new Vector3(0, 13, 0), Quaternion.Euler(0, 0, 0));
                            }
                        } else
                        {
                            currentBoss = Instantiate(enemyGroup, new Vector3(0, enemyGroup.GetComponent<HorizontalOnlyMover>().initialY, 0), Quaternion.Euler(0, 0, 0));
                            currentBoss.name = enemyGroup.name;
                        }
                        waveText.enabled = false;
                    } else
                    {
                        PlayerController playerController = FindObjectOfType<PlayerController>();
                        if (playerController)
                        {
                            playerController.startWinAnimation();
                            yield return null;
                        } else
                        {
                            won = true;
                            yield break;
                        }
                    }
                } else if (gamemode == Gamemodes.Endless)
                {
                    yield return new WaitForSeconds(2);
                    ++wave;
                    waveText.enabled = true;
                    waveText.text = "WAVE " + wave;
                    yield return new WaitForSeconds(1);
                    GameObject enemyGroup = enemyGroups[Random.Range(0, enemyGroups.Length)];
                    if (enemyGroup.layer != 8)
                    {
                        if (enemyGroup.GetComponent<EnemyMover>())
                        {
                            Instantiate(enemyGroup, new Vector3(0, enemyGroup.GetComponent<EnemyMover>().initialY, 0), Quaternion.Euler(0, 0, 0));
                        } else if (enemyGroup.GetComponent<AsteroidSpawner>())
                        {
                            Instantiate(enemyGroup, new Vector3(0, 13, 0), Quaternion.Euler(0, 0, 0));
                        }
                    } else
                    {
                        currentBoss = Instantiate(enemyGroup, new Vector3(0, enemyGroup.GetComponent<HorizontalOnlyMover>().initialY, 0), Quaternion.Euler(0, 0, 0));
                        currentBoss.name = enemyGroup.name;
                    }
                    waveText.enabled = false;
                }
            }
        }
    }

    IEnumerator firstWaveSpawn()
    {
        if (gamemode == Gamemodes.Campaign)
        {
            yield return new WaitForSeconds(2);
            wave = 1;
            waveText.enabled = true;
            waveText.text = "WAVE " + wave + "/" + enemyGroups.LongLength;
            yield return new WaitForSeconds(1);
            GameObject enemyGroup = enemyGroups[0];
            if (enemyGroup.layer != 8)
            {
                if (enemyGroup.GetComponent<EnemyMover>())
                {
                    Instantiate(enemyGroup, new Vector3(0, enemyGroup.GetComponent<EnemyMover>().initialY, 0), Quaternion.Euler(0, 0, 0));
                } else if (enemyGroup.GetComponent<AsteroidSpawner>())
                {
                    Instantiate(enemyGroup, new Vector3(0, 13, 0), Quaternion.Euler(0, 0, 0));
                }
            } else
            {
                currentBoss = Instantiate(enemyGroup, new Vector3(0, enemyGroup.GetComponent<HorizontalOnlyMover>().initialY, 0), Quaternion.Euler(0, 0, 0));
                currentBoss.name = enemyGroup.name;
            }
            StartCoroutine(spawnWaves());
            waveText.enabled = false;
        } else if (gamemode == Gamemodes.Endless)
        {
            yield return new WaitForSeconds(2);
            wave = 1;
            waveText.enabled = true;
            waveText.text = "WAVE " + wave;
            yield return new WaitForSeconds(1);
            GameObject enemyGroup = enemyGroups[Random.Range(0, enemyGroups.Length)];
            if (enemyGroup.layer != 8)
            {
                if (enemyGroup.GetComponent<EnemyMover>())
                {
                    Instantiate(enemyGroup, new Vector3(0, enemyGroup.GetComponent<EnemyMover>().initialY, 0), Quaternion.Euler(0, 0, 0));
                } else if (enemyGroup.GetComponent<AsteroidSpawner>())
                {
                    Instantiate(enemyGroup, new Vector3(0, 13, 0), Quaternion.Euler(0, 0, 0));
                }
            } else
            {
                currentBoss = Instantiate(enemyGroup, new Vector3(0, enemyGroup.GetComponent<HorizontalOnlyMover>().initialY, 0), Quaternion.Euler(0, 0, 0));
                currentBoss.name = enemyGroup.name;
            }
            StartCoroutine(spawnWaves());
            waveText.enabled = false;
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
                gameHUD.enabled = false;
                gamePausedMenu.enabled = false;
                gameOverMenu.enabled = false;
                levelCompletedMenu.enabled = false;
                settingsMenu.enabled = false;
                quitGameMenu.enabled = false;
                restartPrompt.enabled = false;
                yield return null;
            }
        }
    }
    #endregion

    #region Menu Functions
    public void pause()
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
        if (!gameOver && !won && !gameOverMenu.enabled)
        {
            if (!paused)
            {
                clickSource = ClickSources.GamePaused;
                paused = true;
                Time.timeScale = 0;
                AudioListener.pause = true;
                gamePausedMenu.enabled = true;
                pauseButton.gameObject.SetActive(false);
            } else
            {
                if (!settingsMenu.enabled && !quitGameMenu.enabled && !restartPrompt.enabled)
                {
                    paused = false;
                    Time.timeScale = 1;
                    AudioListener.pause = false;
                    gamePausedMenu.enabled = false;
                    pauseButton.gameObject.SetActive(true);
                }
            }
        }
    }

    public void restart()
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
        PlayerPrefs.SetInt("Restarted", 1);
        PlayerPrefs.Save();
        StartCoroutine(loadScene(SceneManager.GetActiveScene().name));
    }

    public void toNextLevel()
    {
        if (gamemode == Gamemodes.Campaign && won && levelCompletedMenu.enabled)
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
            int newLevel = PlayerPrefs.GetInt("IngameLevel") + 1;
            if (PlayerPrefs.GetInt("IngameLevel") < PlayerPrefs.GetInt("MaxLevels"))
            {
                StartCoroutine(loadScene("Level " + newLevel));
            } else
            {
                StartCoroutine(loadScene("Ending"));
            }
        }
    }

    public void exitGame()
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
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
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

    public void openCanvasFromClickSource(Canvas canvas)
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
        if (!canvas.enabled)
        {
            canvas.enabled = true;
            if (clickSource == ClickSources.GamePaused)
            {
                gamePausedMenu.enabled = false;
            } else if (clickSource == ClickSources.GameOver)
            {
                gameOverMenu.enabled = false;
            } else if (clickSource == ClickSources.LevelCompleted)
            {
                levelCompletedMenu.enabled = false;
            }
        } else
        {
            canvas.enabled = false;
            if (clickSource == ClickSources.GamePaused)
            {
                gamePausedMenu.enabled = true;
            } else if (clickSource == ClickSources.GameOver)
            {
                gameOverMenu.enabled = true;
            } else if (clickSource == ClickSources.LevelCompleted)
            {
                levelCompletedMenu.enabled = true;
            }
        }
    }
    #endregion
}