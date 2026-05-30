using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Musica")]
    [SerializeField] private AudioSource musicSource;

    [Header("SFX")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource growingSource; // loop enquanto ponte cresce

    [Header("Clips")]
    [SerializeField] private AudioClip musicBackground;
    [SerializeField] private AudioClip sfxGrowing;
    [SerializeField] private AudioClip sfxFall;
    [SerializeField] private AudioClip sfxSuccess;
    [SerializeField] private AudioClip sfxPerfect;
    [SerializeField] private AudioClip sfxFail;
    [SerializeField] private AudioClip sfxCombo;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject); // persiste entre cenas (menu e jogo)
    }

    private void Start()
    {
        if (musicBackground != null)
        {
            musicSource.clip   = musicBackground;
            musicSource.loop   = true;
            musicSource.volume = 0.4f;
            musicSource.Play();
        }
    }

    // ── Ponte ──────────────────────────────────────────
    public void PlayGrowingStart()
    {
        if (sfxGrowing == null) return;
        growingSource.clip   = sfxGrowing;
        growingSource.loop   = true;
        growingSource.volume = 0.5f;
        growingSource.Play();
    }

    public void PlayGrowingStop() => growingSource.Stop();

    public void PlayFall()    => Play(sfxFall,    0.8f);

    // ── Resultado ──────────────────────────────────────
    public void PlaySuccess() => Play(sfxSuccess, 1.0f);
    public void PlayPerfect() => Play(sfxPerfect, 1.0f);
    public void PlayFail()    => Play(sfxFail,    1.0f);
    public void PlayCombo()   => Play(sfxCombo,   0.9f);

    // ── Helpers ────────────────────────────────────────
    private void Play(AudioClip clip, float volume)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, volume);
    }
}
