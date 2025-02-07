using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource sfxSource;           // For one-shot effects
    public AudioSource footstepsSource;     // Dedicated source for footsteps

    [Header("Sound Effects")]
    public AudioClip[] enemyHitSounds;
    public AudioClip[] playerAttackSounds;
    public AudioClip[] playerFootstepSounds;
    public AudioClip playerJumpSound;

    private static AudioManager instance;
    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<AudioManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("AudioManager");
                    instance = go.AddComponent<AudioManager>();
                }
            }
            return instance;
        }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // Create audio sources if not assigned
            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.playOnAwake = false;
            }

            if (footstepsSource == null)
            {
                footstepsSource = gameObject.AddComponent<AudioSource>();
                footstepsSource.playOnAwake = false;
                footstepsSource.volume = 0.7f;  // Lower volume for footsteps
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayEnemyHitSound()
    {
        if (enemyHitSounds != null && enemyHitSounds.Length > 0)
        {
            AudioClip hitSound = enemyHitSounds[Random.Range(0, enemyHitSounds.Length)];
            sfxSource.PlayOneShot(hitSound);
        }
    }

    public void PlayPlayerAttackSound(bool isPowerAttack = false)
    {
        if (playerAttackSounds != null && playerAttackSounds.Length > 0)
        {
            int soundIndex = isPowerAttack ? 1 : 0;
            soundIndex = Mathf.Min(soundIndex, playerAttackSounds.Length - 1);
            sfxSource.PlayOneShot(playerAttackSounds[soundIndex]);
        }
    }

    public void PlayPlayerJumpSound()
    {
        if (playerJumpSound != null)
        {
            sfxSource.PlayOneShot(playerJumpSound);
        }
    }

    public void PlayPlayerFootstepSound()
    {
        if (playerFootstepSounds != null && playerFootstepSounds.Length > 0 && !footstepsSource.isPlaying)
        {
            AudioClip footstepSound = playerFootstepSounds[Random.Range(0, playerFootstepSounds.Length)];
            footstepsSource.clip = footstepSound;
            footstepsSource.Play();
        }
    }

    // Helper method to stop footsteps immediately if needed
    public void StopFootsteps()
    {
        if (footstepsSource != null)
        {
            footstepsSource.Stop();
        }
    }
}