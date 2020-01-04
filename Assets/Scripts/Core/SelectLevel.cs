using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectLevel : MonoBehaviour
{
    [SerializeField] private int level = 1;
    [SerializeField] private Text levelText = null;
    [SerializeField] private Image lockImage = null;
    
    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
    }

    void Update()
    {
        if (PlayerPrefs.GetInt("Level") >= level)
        {
            levelText.enabled = true;
            lockImage.enabled = false;
            button.interactable = true;
        } else
        {
            levelText.enabled = false;
            lockImage.enabled = true;
            button.interactable = false;
        }
    }
}
