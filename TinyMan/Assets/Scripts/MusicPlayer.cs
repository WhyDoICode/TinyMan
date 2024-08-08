using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public static MusicPlayer Instance { get; private set; }
    private AudioSource _audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            _audioSource = GetComponent<AudioSource>();
            if (_audioSource != null)
            {
                _audioSource.loop = true;
                _audioSource.Play();
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ChangeMusic(AudioClip newClip)
    {
        if (_audioSource != null && newClip != null)
        {
            _audioSource.Stop();
            _audioSource.clip = newClip;
            _audioSource.Play();
        }
    }
}