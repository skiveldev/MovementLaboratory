using UnityEngine;

// Central audio hub: looping background music plus one-shot action effects.
// Self-bootstraps on scene load, so it needs no scene wiring: clips resolve
// by name from any Resources folder and missing clips only warn.
public sealed class AudioManager : MonoBehaviour
{
    private const string MusicPath = "POL-king-of-coins-short";
    private const string CoinPath = "Coin1";
    private const string ExplosionPath = "BarrelSound";
    private const string GoalPath = "confirmation_003";

    private static AudioManager instance;

    private AudioSource musicSource;
    private AudioSource sfxSource;

    private AudioClip coinClip;
    private AudioClip explosionClip;
    private AudioClip goalClip;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        Ensure();
        EnsureListener();
        instance.PlayMusic();
    }

    // Unity renders no audio at all without an active AudioListener, and this
    // scene ships without one, so attach it to the main camera at runtime.
    private static void EnsureListener()
    {
        if (FindFirstObjectByType<AudioListener>() != null)
        {
            return;
        }

        var cam = Camera.main;
        var host = cam != null ? cam.gameObject : instance.gameObject;
        host.AddComponent<AudioListener>();
    }

    private static void Ensure()
    {
        if (instance != null)
        {
            return;
        }

        var go = new GameObject("AudioManager");
        DontDestroyOnLoad(go);
        instance = go.AddComponent<AudioManager>();
    }

    private void Awake()
    {
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.spatialBlend = 0f;
        musicSource.volume = 0.35f;
        musicSource.clip = Resources.Load<AudioClip>(MusicPath);

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.loop = false;
        sfxSource.playOnAwake = false;
        sfxSource.spatialBlend = 0f;
        sfxSource.volume = 1f;

        coinClip = Resources.Load<AudioClip>(CoinPath);
        explosionClip = Resources.Load<AudioClip>(ExplosionPath);
        goalClip = Resources.Load<AudioClip>(GoalPath);
    }

    private void PlayMusic()
    {
        if (musicSource.clip == null)
        {
            Debug.LogWarning("AudioManager: music clip missing at Resources/" + MusicPath, this);
            return;
        }

        if (!musicSource.isPlaying)
        {
            musicSource.Play();
        }
    }

    public static void PlayCoin()
    {
        Ensure();
        instance.PlayOneShot(instance.coinClip, 0.4f);
    }

    public static void PlayExplosion()
    {
        Ensure();
        instance.PlayOneShot(instance.explosionClip, 1f);
    }

    public static void PlayGoal()
    {
        Ensure();
        instance.PlayOneShot(instance.goalClip, 0.8f);
    }

    private void PlayOneShot(AudioClip clip, float volumeScale)
    {
        if (clip == null)
        {
            return;
        }

        sfxSource.PlayOneShot(clip, volumeScale);
    }
}
