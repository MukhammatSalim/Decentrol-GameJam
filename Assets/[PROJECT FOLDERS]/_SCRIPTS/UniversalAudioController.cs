using UnityEngine;

public class UniversalAudioController : MonoBehaviour
{
    public static UniversalAudioController Instance;
    [SerializeField] private AudioSource audioSourceforBG;
    [SerializeField] private AudioSource audioSourceforSC;
    [Header("Audio Clips")]
    [SerializeField] private AudioClip MainMenuBG;
    [SerializeField] private AudioClip GameplayBGMusic;
    [SerializeField] private AudioClip[] PointTaken;
    [SerializeField] private AudioClip TimesUp;
    [SerializeField] private AudioClip LevelUp;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        DontDestroyOnLoad(gameObject);
    }

    public void PlayAudioClip(AudioType type)
    {
        switch (type)
        {
            case AudioType.MainMenuBG:
            {
                audioSourceforBG.Stop();
                audioSourceforBG.clip = MainMenuBG;
                audioSourceforBG.Play();
            }
                break;
            case AudioType.GameplayBGMusic:
            {
                audioSourceforBG.Stop();
                audioSourceforBG.clip = GameplayBGMusic;
                audioSourceforBG.Play();
            }
                break;
            case AudioType.PointTaken:
            {
                audioSourceforSC.PlayOneShot(PointTaken[Random.Range(0, PointTaken.Length)]);
            }
                break;
            case AudioType.TimesUp:
            {
                audioSourceforSC.PlayOneShot(TimesUp);
            }
                break;
            case AudioType.LevelUp:
            {
                audioSourceforSC.PlayOneShot(LevelUp);
            }
                break;
        }
    }

    public void StopAllAudio()
    {
        audioSourceforBG.Stop();
        audioSourceforSC.Stop();
    }
}

public enum AudioType
{
    MainMenuBG,
    GameplayBGMusic,
    PointTaken,
    TimesUp,
    LevelUp,
    
}
