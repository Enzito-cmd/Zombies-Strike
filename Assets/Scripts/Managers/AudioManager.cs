using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager _instance;

    [Header("Gun sounds")]
    public AudioSource _sfxSource; 
    public AudioClip _pistolShot;
    public AudioClip _pistolReload;

    //[Header("Zombies sound")]
    //public AudioClip _zombieDeath;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            _sfxSource.PlayOneShot(clip);
        }
    }
}
