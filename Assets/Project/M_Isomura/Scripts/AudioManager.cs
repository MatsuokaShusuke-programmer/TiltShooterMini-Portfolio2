using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [SerializeField] private AudioSource SEAudioSource;
    [SerializeField] private AudioSource BGMAudioSource;
    [SerializeField] private AudioClip[] audioClip;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySE(int SENum)
    {
        if (audioClip.Length - 1 < SENum) return;
        SEAudioSource.PlayOneShot(audioClip[SENum]);
    }
}
