using UnityEngine;

public class AudioController : MonoBehaviour
{
    public static AudioController Instance;

    [SerializeField] private AudioSource sfxSource;

    [Header("Building")]
    [SerializeField] private AudioClip placeTowerSound;
    [SerializeField, Range(0f, 1f)] private float placeTowerVolume = 1f;

    [SerializeField] private AudioClip insertCrystalSound;
    [SerializeField, Range(0f, 1f)] private float insertCrystalVolume = 1f;


    [Header("Crystals")]
    [SerializeField] private AudioClip crystalSelectSound;
    [SerializeField, Range(0f, 1f)] private float crystalSelectVolume = 1f;


    [Header("Combat")]
    [SerializeField] private AudioClip[] shootSound;
    [SerializeField, Range(0f, 1f)] private float shootVolume = 1f;

    [SerializeField] private AudioClip fireAoeSound;
    [SerializeField, Range(0f, 1f)] private float fireAoeVolume = 1f;

    [SerializeField] private AudioClip rootSound;
    [SerializeField, Range(0f, 1f)] private float rootVolume = 1f;

    [SerializeField] private AudioClip critShootSound;
    [SerializeField, Range(0f, 1f)] private float critShootVolume = 1f;

    [SerializeField] private AudioClip enemyDeathSound;
    [SerializeField, Range(0f, 1f)] private float enemyDeathVolume = 1f;

    [SerializeField] private AudioClip loseLifeSound;
    [SerializeField, Range(0f, 1f)] private float loseLifeVolume = 1f;


    [Header("UI / Waves")]
    [SerializeField] private AudioClip rewardSound;
    [SerializeField, Range(0f, 1f)] private float rewardVolume = 1f;

    [SerializeField] private AudioClip waveStartSound;
    [SerializeField, Range(0f, 1f)] private float waveStartVolume = 1f;

    private void Awake()
    {
        Instance = this;
    }

    private void Play(AudioClip clip, float volume)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip, volume);
        }
    }

    public void PlayTowerPlace()
    {
        Play(placeTowerSound, placeTowerVolume);
    }

    public void PlayCrystalInsert()
    {
        Play(insertCrystalSound, insertCrystalVolume);
    }

    public void PlayCrystalSelect()
    {
        Play(crystalSelectSound, crystalSelectVolume);
    }

    public void PlayShoot(bool isCrit)
    {
        if (isCrit)
        {
            Play(critShootSound, critShootVolume);
            return;
        }

        if (shootSound.Length == 0)
            return;

        int index = Random.Range(0, shootSound.Length);

        Play(shootSound[index], shootVolume);
    }

    public void PlayFireAoe()
    {
        Play(fireAoeSound, fireAoeVolume);
    }

    public void PlayRoot()
    {
        Play(rootSound, rootVolume);
    }

    public void PlayEnemyDeath()
    {
        Play(enemyDeathSound, enemyDeathVolume);
    }

    public void PlayLoseLife()
    {
        Play(loseLifeSound, loseLifeVolume);
    }

    public void PlayReward()
    {
        Play(rewardSound, rewardVolume);
    }

    public void PlayWaveStart()
    {
        Play(waveStartSound, waveStartVolume);
    }
}