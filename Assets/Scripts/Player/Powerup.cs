using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField] private GameObject sound = null;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController)
            {
                if (CompareTag("WeaponPower"))
                {
                    ++playerController.weaponPower;
                    if (sound)
                    {
                        GameObject newSound = Instantiate(sound, transform.position, transform.rotation);
                        if (newSound.GetComponent<AudioSource>())
                        {
                            if (playerController.weaponPower == 2)
                            {
                                newSound.GetComponent<AudioSource>().pitch = 1.05f;
                            } else if (playerController.weaponPower == 3)
                            {
                                newSound.GetComponent<AudioSource>().pitch = 1.1f;
                            } else if (playerController.weaponPower == 4)
                            {
                                newSound.GetComponent<AudioSource>().pitch = 1.15f;
                            } else if (playerController.weaponPower >= 5)
                            {
                                newSound.GetComponent<AudioSource>().pitch = 1.2f;
                            }
                        }
                    }
                } else
                {
                    Debug.LogError("Powerup tag " + tag + " is invalid.");
                }
                Destroy(gameObject);
            } else
            {
                Debug.LogError("Could not find PlayerController!");
            }
        }
    }
}
