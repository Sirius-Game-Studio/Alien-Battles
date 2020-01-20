using UnityEngine;
using UnityEngine.UI;

public class ScoreCount : MonoBehaviour
{
    [SerializeField] private Text count = null;

    void Update()
    {
        if (GameController.instance.gamemode == GameController.Gamemodes.Campaign)
        {
            gameObject.SetActive(false);
        } else if (GameController.instance.gamemode == GameController.Gamemodes.Endless)
        {
            gameObject.SetActive(true);
            count.text = GameController.instance.score.ToString();
        }
    }
}
