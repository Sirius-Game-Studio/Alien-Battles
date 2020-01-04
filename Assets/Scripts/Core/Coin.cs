using UnityEngine;
using TMPro;

public class Coin : MonoBehaviour
{
    [SerializeField] private long money = 1;
    [SerializeField] private GameObject sound = null;
    [SerializeField] private GameObject textPopup = null;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.GetComponent<PlayerController>())
        {
            if (PlayerPrefs.HasKey("Money"))
            {
                long cash = long.Parse(PlayerPrefs.GetString("Money"));
                cash += money;
                PlayerPrefs.SetString("Money", cash.ToString());
            } else
            {
                PlayerPrefs.SetString("Money", money.ToString());
            }
            if (sound) Instantiate(sound, transform.position, transform.rotation);
            if (textPopup)
            {
                if (textPopup.GetComponent<TextMeshPro>())
                {
                    GameObject popup = Instantiate(textPopup, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.Euler(0, 0, 0));
                    popup.GetComponent<TextMeshPro>().text = "+$" + money;
                    popup.GetComponent<TextMeshPro>().color = new Color32(255, 215, 0, 255);
                    popup.GetComponent<TextMeshPro>().outlineColor = new Color32(127, 107, 0, 255);
                } else
                {
                    Debug.LogError("TextPopup object does not have a TextMeshPro component!");
                }
            }
            Destroy(gameObject);
        }
    }
}