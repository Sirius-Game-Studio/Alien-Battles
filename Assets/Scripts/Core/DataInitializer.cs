using UnityEngine;
using UnityEngine.SceneManagement;

public class DataInitializer : MonoBehaviour
{
    [SerializeField] private int level = 1;
    [SerializeField] private int maxLevels = 10;
    [SerializeField] private bool setLevel = false;

    void Awake()
    {
        if (setLevel)
        {
            string sceneName = SceneManager.GetActiveScene().name.ToLower();
            if (!PlayerPrefs.HasKey("Level"))
            {
                if (sceneName.Contains("level"))
                {
                    PlayerPrefs.SetInt("Level", level);
                    PlayerPrefs.SetInt("IngameLevel", level);
                } else
                {
                    PlayerPrefs.SetInt("Level", 1);
                }
            } else
            {
                if (sceneName.Contains("level")) PlayerPrefs.SetInt("IngameLevel", level);
            }
        }
        if (maxLevels > 0)
        {
            PlayerPrefs.SetInt("MaxLevels", maxLevels);
        } else
        {
            PlayerPrefs.SetInt("MaxLevels", 1);
        }
        if (!PlayerPrefs.HasKey("Money")) PlayerPrefs.SetString("Money", "0");
        PlayerPrefs.Save();
        Destroy(gameObject);
    }
}