using TMPro;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class BeatManager : MonoBehaviour
{
    [Header("Rhythm")]
    [SerializeField] private float firstNote;
    [SerializeField] private float bpm;
    [SerializeField] private int beatsPerBar = 4;

    [Header("Music")]
    [SerializeField] private AudioSource introSource;
    [SerializeField] private AudioSource loopSource;
    [SerializeField] private AudioSource endingSource;

    [SerializeField] private AudioClip introClip;
    [SerializeField] private AudioClip loopClip;
    [SerializeField] private AudioClip winEnding;
    [SerializeField] private AudioClip loseEnding;

    [Header("UI")]
    [SerializeField] private RectTransform leftMarker;
    [SerializeField] private float markerStartX = -300f;

    [SerializeField] private RectTransform rightMarker;
    [SerializeField] private float markerEndX = 300f;

    [SerializeField] private TMP_Text timingFeedback;

    private int lastAttemptedBeat = -1;
    private float timeBetweenNotes;

    private double songStartDspTime;
    private double loopStartDspTime;
    private double nextBeatDspTime;

    private bool musicStarted;
    private bool beatSystemActive;
    private bool endingStarted;

    public event Action OnBeat;

    public float BeatLength => timeBetweenNotes;

    private void Start()
    {
        timeBetweenNotes = 60f / bpm;

        leftMarker.anchoredPosition = new Vector2(
            markerStartX,
            leftMarker.anchoredPosition.y
        );

        rightMarker.anchoredPosition = new Vector2(
            markerEndX,
            rightMarker.anchoredPosition.y
        );
    }

    private void Update()
    {
        if (!beatSystemActive)
            return;

        if (AudioSettings.dspTime < songStartDspTime)
            return;

        while (AudioSettings.dspTime >= nextBeatDspTime)
        {
            nextBeatDspTime += timeBetweenNotes;
            OnBeat?.Invoke();
        }

        double timeUntilBeat =
            nextBeatDspTime - AudioSettings.dspTime;

        float progress =
            1f - ((float)timeUntilBeat / timeBetweenNotes);

        progress = Mathf.Clamp01(progress);

        float leftX =
            Mathf.Lerp(markerStartX, 0f, progress);

        float rightX =
            Mathf.Lerp(markerEndX, 0f, progress);

        leftMarker.anchoredPosition = new Vector2(
            leftX,
            leftMarker.anchoredPosition.y
        );

        rightMarker.anchoredPosition = new Vector2(
            rightX,
            rightMarker.anchoredPosition.y
        );
    }

    // --------------------------------------------------
    // MUSIC
    // --------------------------------------------------

    public void StartMusic()
    {
        if (musicStarted)
            return;
        loopSource.volume = 1f;
        musicStarted = true;
        endingStarted = false;
        lastAttemptedBeat = -1;

        double startTime =
            AudioSettings.dspTime + 0.2;

        // INTRO
        introSource.clip = introClip;
        introSource.loop = false;
        introSource.PlayScheduled(startTime);

        double introDuration =
            (double)introClip.samples / introClip.frequency;

        // 16-SEKUNDEN LOOP
        loopStartDspTime =
            startTime + introDuration;

        loopSource.clip = loopClip;
        loopSource.loop = true;
        loopSource.PlayScheduled(loopStartDspTime);

        // Beat-Grid beginnt mit dem Intro
        songStartDspTime = startTime;

        nextBeatDspTime =
            songStartDspTime + firstNote;

        beatSystemActive = true;
    }

    public void PlayWinEnding()
    {
        PlayEnding(winEnding);
    }

    public void PlayLoseEnding()
    {
        PlayEnding(loseEnding);
    }

    private void PlayEnding(AudioClip endingClip)
    {
        if (endingStarted || endingClip == null)
            return;

        endingStarted = true;
        StartCoroutine(FadeOutLoop(0.5f));
        // Gameplay / Rhythmus sofort stoppen
        beatSystemActive = false;

        double now = AudioSettings.dspTime;

        // Falls das Spiel schon während des Intros endet
        if (now < loopStartDspTime)
        {
            introSource.Stop();
            loopSource.Stop();

            double earlyTransitionTime =
                now + 0.05;

            endingSource.clip = endingClip;
            endingSource.loop = false;
            endingSource.PlayScheduled(earlyTransitionTime);

            return;
        }

        // Nächste saubere Taktgrenze
        double barLength =
            timeBetweenNotes * beatsPerBar;

        double firstBeatTime =
            songStartDspTime + firstNote;

        double elapsed =
            now - firstBeatTime;

        double barsPassed =
            Math.Floor(elapsed / barLength) + 1;

        double transitionTime =
            firstBeatTime + barsPassed * barLength;

        // Loop exakt an der Taktgrenze beenden
        loopSource.SetScheduledEndTime(transitionTime);

        // Ending exakt dort starten
        endingSource.clip = endingClip;
        endingSource.loop = false;
        endingSource.PlayScheduled(transitionTime);
    }

    public void StopMusic()
    {
        beatSystemActive = false;

        introSource.Stop();
        loopSource.Stop();
        endingSource.Stop();
    }


    private IEnumerator FadeOutLoop(float duration)
    {
        float startVolume = loopSource.volume;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            loopSource.volume = Mathf.Lerp(startVolume, 0f, time / duration);
            yield return null;
        }

        loopSource.volume = 0f;
    }

    // --------------------------------------------------
    // RHYTHM / INPUT
    // --------------------------------------------------

    private double CheckTiming()
    {
        double previousBeatDspTime =
            nextBeatDspTime - timeBetweenNotes;

        double distanceToPreviousBeat =
            Math.Abs(
                AudioSettings.dspTime -
                previousBeatDspTime
            );

        double distanceToNextBeat =
            Math.Abs(
                AudioSettings.dspTime -
                nextBeatDspTime
            );

        return Math.Min(
            distanceToPreviousBeat,
            distanceToNextBeat
        );
    }

    private void TryTriggerElement(CrystalType type)
    {
        if (!beatSystemActive)
            return;

        if (AudioSettings.dspTime < songStartDspTime)
            return;

        int currentBeat = Mathf.RoundToInt(
            (float)(
                (
                    AudioSettings.dspTime -
                    songStartDspTime -
                    firstNote
                )
                / BeatLength
            )
        );

        if (currentBeat == lastAttemptedBeat)
            return;

        lastAttemptedBeat = currentBeat;

        double timing = CheckTiming();

        switch (timing)
        {
            case <= 0.05:
                timingFeedback.text =
                    $"PERFECT {timing * 1000:F0} ms";

                TriggerElement(type, true);
                break;

            case <= 0.10:
                timingFeedback.text =
                    $"HIT {timing * 1000:F0} ms";

                TriggerElement(type, false);
                break;

            default:
                timingFeedback.text =
                    $"MISS {timing * 1000:F0} ms";
                break;
        }
    }

    private void TriggerElement(
        CrystalType type,
        bool isCrit
    )
    {
        Tower[] towers =
            FindObjectsByType<Tower>(
                FindObjectsSortMode.None
            );

        foreach (Tower tower in towers)
        {
            if (
                tower.HasCrystal(type) ||
                tower.IsEmpty()
            )
            {
                tower.Attack(
                    BeatLength,
                    isCrit
                );
            }
        }
    }

    public void OnFireBeat(InputValue value)
    {
        if (value.isPressed)
        {
            TryTriggerElement(
                CrystalType.Fire
            );
        }
    }

    public void OnNatureBeat(InputValue value)
    {
        if (value.isPressed)
        {
            TryTriggerElement(
                CrystalType.Nature
            );
        }
    }

    public void OnLightningBeat(InputValue value)
    {
        if (value.isPressed)
        {
            TryTriggerElement(
                CrystalType.Lightning
            );
        }
    }

    
}
