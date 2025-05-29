using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Prevents destruction when loading new scenes
            GetComponent<AudioSource>().Play(); // Starts playing music
        }
        else
        {
            Destroy(gameObject); // Ensures only one AudioManager exists
        }
    }
}
