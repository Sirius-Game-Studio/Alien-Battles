using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct Perk
{
    [Tooltip("Perk data key.")] public string key;
    [Tooltip("Perk price.")] public int price;
}

[System.Serializable]
public struct Upgrade
{
    [Tooltip("Upgrade name.")] public string name;
    [Tooltip("Maximum upgrade level.")] public int maxLevel;
    [Tooltip("Upgrade price.")] public int price;
    [Tooltip("Upgrade price multiplier.")] public float priceMultiplier;
    [Tooltip("Amount added to the upgrade multiplier.")] public float multiplierIncrease;
}

public class ShopManager : MonoBehaviour
{
    public static ShopManager instance;

    [SerializeField] private Perk[] perks = new Perk[0];
    [SerializeField] private Upgrade[] upgrades = new Upgrade[0];

    [Header("Upgrade Shop")]
    [SerializeField] private GameObject damageButton = null;
    [SerializeField] private GameObject speedButton = null;
    [SerializeField] private Text damagePrice = null;
    [SerializeField] private Text speedPrice = null;
    [SerializeField] private Slider damageSlider = null;
    [SerializeField] private Slider speedSlider = null;

    [Header("Perk Shop")]
    [SerializeField] private GameObject SLBuyText = null;
    [SerializeField] private Text SLUseText = null;
    [SerializeField] private Text SLPriceText = null;
    [SerializeField] private GameObject TTBuyText = null;
    [SerializeField] private Text TTUseText = null;
    [SerializeField] private Text TTPriceText = null;
    [SerializeField] private GameObject TSBuyText = null;
    [SerializeField] private Text TSUseText = null;
    [SerializeField] private Text TSPriceText = null;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip buttonClick = null;
    [SerializeField] private AudioClip purchaseItem = null;
    [SerializeField] private AudioClip cannotAfford = null;

    private AudioSource audioSource;
    [HideInInspector] public bool open = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else if (instance != this)
        {
            Destroy(gameObject);
        }
        audioSource = GetComponent<AudioSource>();
        open = false;
        for (int i = 0; i < upgrades.Length; i++)
        {
            if (!PlayerPrefs.HasKey(upgrades[i].name + "Multiplier")) PlayerPrefs.SetFloat(upgrades[i].name + "Multiplier", 1);
            if (!PlayerPrefs.HasKey(upgrades[i].name + "Price")) PlayerPrefs.SetInt(upgrades[i].name + "Price", upgrades[i].price);
        }
        PlayerPrefs.Save();
    }

    void Update()
    {
        //Sets perk text states
        perkState(SLBuyText, SLUseText, SLPriceText, perks[0].key, perks[0].price);
        perkState(TTBuyText, TTUseText, TTPriceText, perks[1].key, perks[1].price);
        perkState(TSBuyText, TSUseText, TSPriceText, perks[2].key, perks[2].price);

        //Sets upgrade text states
        upgradeState(damageButton, damagePrice, damageSlider, upgrades[0].name + "Level", upgrades[0].name + "Price", upgrades[0].maxLevel);
        upgradeState(speedButton, speedPrice, speedSlider, upgrades[1].name + "Level", upgrades[1].name + "Price", upgrades[1].maxLevel);
    }

    #region Main Functions
    void perkState(GameObject buyText, Text useText, Text priceText, string perkKey, int price)
    {
        if (perkKey != "")
        {
            if (!PlayerPrefs.HasKey("Has" + perkKey))
            {
                buyText.SetActive(true);
                useText.enabled = false;
            } else
            {
                buyText.SetActive(false);
                useText.enabled = true;
                if (PlayerPrefs.GetString("Perk") == perkKey)
                {
                    useText.text = "Using";
                } else
                {
                    useText.text = "Use";
                }
            }
            priceText.text = "$" + price;
        }
    }

    void upgradeState(GameObject button, Text priceText, Slider slider, string levelKey, string priceKey, int maxLevel)
    {
        if (levelKey != "")
        {
            if (PlayerPrefs.GetInt(levelKey) < maxLevel)
            {
                button.SetActive(true);
                priceText.enabled = true;
                if (PlayerPrefs.GetInt(priceKey) > 0)
                {
                    priceText.text = "$" + PlayerPrefs.GetInt(priceKey);
                } else
                {
                    priceText.text = "Free";
                }
            } else
            {
                button.SetActive(false);
                priceText.enabled = false;
            }
            slider.maxValue = maxLevel;
            slider.value = PlayerPrefs.GetInt(levelKey);
        }
    }
    #endregion

    #region Purchase Functions
    public void buyPerk(int index)
    {
        if (open)
        {
            if (PlayerPrefs.GetInt("Has" + perks[index].key) <= 0)
            {
                if (perks[index].price > 0)
                {
                    long money = long.Parse(PlayerPrefs.GetString("Money"));
                    if (money >= perks[index].price)
                    {
                        if (audioSource)
                        {
                            if (purchaseItem)
                            {
                                audioSource.PlayOneShot(purchaseItem);
                            } else
                            {
                                audioSource.Play();
                            }
                        }
                        money -= perks[index].price;
                        PlayerPrefs.SetString("Money", money.ToString());
                        PlayerPrefs.SetInt("Has" + perks[index].key, 1);
                        PlayerPrefs.SetString("Perk", perks[index].key);
                    } else
                    {
                        if (audioSource)
                        {
                            if (cannotAfford)
                            {
                                audioSource.PlayOneShot(cannotAfford);
                            } else
                            {
                                audioSource.Play();
                            }
                        }
                    }
                } else
                {
                    if (audioSource)
                    {
                        if (purchaseItem)
                        {
                            audioSource.PlayOneShot(purchaseItem);
                        } else
                        {
                            audioSource.Play();
                        }
                    }
                    PlayerPrefs.SetInt("Has" + perks[index].key, 1);
                }
                PlayerPrefs.Save();
            } else
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
                PlayerPrefs.SetString("Perk", perks[index].key);
                PlayerPrefs.Save();
            }
        }
    }

    public void buyUpgrade(int index)
    {
        if (open)
        {
            float maxLevel = upgrades[index].maxLevel;
            int c = (int)(PlayerPrefs.GetInt(upgrades[index].name + "Price") * upgrades[index].priceMultiplier);
            float m = PlayerPrefs.GetFloat(upgrades[index].name + "Multiplier") + upgrades[index].multiplierIncrease;
            if (PlayerPrefs.GetInt(upgrades[index].name + "Level") < maxLevel)
            {
                if (upgrades[index].price > 0)
                {
                    long money = long.Parse(PlayerPrefs.GetString("Money"));
                    if (money >= PlayerPrefs.GetInt(upgrades[index].name + "Price"))
                    {
                        if (audioSource)
                        {
                            if (purchaseItem)
                            {
                                audioSource.PlayOneShot(purchaseItem);
                            } else
                            {
                                audioSource.Play();
                            }
                        }
                        money -= PlayerPrefs.GetInt(upgrades[index].name + "Price");
                        PlayerPrefs.SetString("Money", money.ToString());
                        PlayerPrefs.SetInt(upgrades[index].name + "Price", c);
                        PlayerPrefs.SetFloat(upgrades[index].name + "Multiplier", m);
                        if (PlayerPrefs.HasKey(upgrades[index].name + "Level"))
                        {
                            PlayerPrefs.SetInt(upgrades[index].name + "Level", PlayerPrefs.GetInt(upgrades[index].name + "Level") + 1);
                        } else
                        {
                            PlayerPrefs.SetInt(upgrades[index].name + "Level", 1);
                        }
                    } else
                    {
                        if (audioSource)
                        {
                            if (cannotAfford)
                            {
                                audioSource.PlayOneShot(cannotAfford);
                            } else
                            {
                                audioSource.Play();
                            }
                        }
                    }
                } else
                {
                    if (audioSource)
                    {
                        if (purchaseItem)
                        {
                            audioSource.PlayOneShot(purchaseItem);
                        } else
                        {
                            audioSource.Play();
                        }
                    }
                    PlayerPrefs.SetInt(upgrades[index].name + "Price", c);
                    PlayerPrefs.SetFloat(upgrades[index].name + "Multiplier", m);
                }
                PlayerPrefs.Save();
            }
        }
    }
    #endregion
}