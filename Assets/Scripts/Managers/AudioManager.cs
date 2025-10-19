using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [Header("Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource soundEffectsSource;

    [Header("Clips")]
    [SerializeField] private AudioClip gameMusic;
    [SerializeField] private AudioClip segmentDestroyedSound;
    [SerializeField] private AudioClip laserGunSound;
    [SerializeField] private AudioClip centipedeDiveSound;
    [SerializeField] private AudioClip lifeLostSound;
    [SerializeField] private AudioClip ladybugFliesSound;
    [SerializeField] private AudioClip mushroomPoisonedSound;
    [SerializeField] private AudioClip scorpionDestroyedSound;
    [SerializeField] private AudioClip spiderDestroyedSound;
    [SerializeField] private AudioClip ladybugDestroyedSound;

    [Header("Volumes")]
    [SerializeField] private float musicVolume = 0.35f;
    [SerializeField] private float SoundEffectsVolume = 0.8f;

    private void OnEnable()
    {
        GameplayEvents.BulletFired += OnBulletFired;
        GameplayEvents.SegmentDestroyed += OnSegmentDestroyed;
        GameplayEvents.CentipedeDiveStarted += OnCentipedeDive;
        GameplayEvents.LifeLost += OnLifeLost;
        GameplayEvents.GameResetRequested += OnGameReset;
        GameplayEvents.LadybugSpawned += OnLadybugSpawned;
        GameplayEvents.MushroomPoisoned += OnMushroomPoisoned;
        GameplayEvents.ScorpionDestroyed += OnScorpionDestroyed;
        GameplayEvents.SpiderDestroyed += OnSpiderDestroyed;
        GameplayEvents.LadybugDestroyed += OnLadybugDestroyed;
    }

    private void OnDisable()
    {
        GameplayEvents.BulletFired -= OnBulletFired;
        GameplayEvents.SegmentDestroyed -= OnSegmentDestroyed;
        GameplayEvents.CentipedeDiveStarted -= OnCentipedeDive;
        GameplayEvents.LifeLost -= OnLifeLost;
        GameplayEvents.GameResetRequested -= OnGameReset;
        GameplayEvents.LadybugSpawned -= OnLadybugSpawned;
        GameplayEvents.MushroomPoisoned -= OnMushroomPoisoned;
        GameplayEvents.ScorpionDestroyed -= OnScorpionDestroyed;
        GameplayEvents.SpiderDestroyed -= OnSpiderDestroyed;
        GameplayEvents.LadybugDestroyed -= OnLadybugDestroyed;
    }

    private void Start()
    {
        PlayMusic();
    }

    private void OnGameReset()
    {
        PlayMusic();
    }

    private void PlayMusic()
    {
        musicSource.clip = gameMusic;
        musicSource.loop = true;
        musicSource.volume = musicVolume;

        if (!musicSource.isPlaying)
        {
            musicSource.Play();
        }
    }

    private void OnBulletFired()
    {
        PlaySoundEffect(laserGunSound);
    }

    private void OnSegmentDestroyed()
    {
        PlaySoundEffect(segmentDestroyedSound);
    }

    private void OnCentipedeDive()
    {
        PlaySoundEffect(centipedeDiveSound);
    }

    private void OnLifeLost()
    {
        PlaySoundEffect(lifeLostSound);
    }

    private void OnLadybugSpawned()
    {
        PlaySoundEffect(ladybugFliesSound);
    }

    private void OnMushroomPoisoned()
    {
        PlaySoundEffect(mushroomPoisonedSound);
    }

    private void OnScorpionDestroyed()
    {
        PlaySoundEffect(scorpionDestroyedSound);
    }
    private void OnSpiderDestroyed()
    {
        PlaySoundEffect(spiderDestroyedSound);
    }

    private void OnLadybugDestroyed()
    {
        PlaySoundEffect(ladybugDestroyedSound);
    }

    private void PlaySoundEffect(AudioClip clip)
    {
        soundEffectsSource.PlayOneShot(clip, SoundEffectsVolume);
    }
}