using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class FadeOutAndLoadScene : MonoBehaviour
{
    [Header("Fade")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float beforeFadeDuration = 2f;
    [SerializeField] private float fadeInDuration = 3f;
    [SerializeField] private float stayDuration = 2f;
    [SerializeField] private float fadeOutDuration = 3f;
    [SerializeField] private string sceneToLoad;

    [Header("Audio")]
    [SerializeField] private AudioClip roarClip;
    [SerializeField] private float audioStartDelay = 0.5f;

    [Header("Logo Image Rotations")]
    [SerializeField] private Transform imageToRotate;

    [Header("Roar 1 Settings")]
    [SerializeField] private float roar1StartDelay = 0.5f;
    [SerializeField] private float roar1RotationZ = 15f;
    [SerializeField] private float roar1RotationDuration = 0.2f;

    [Header("Roar 2 Settings")]
    [SerializeField] private float roar2StartDelay = 1.5f;
    [SerializeField] private float roar2RotationZ = -20f;
    [SerializeField] private float roar2RotationDuration = 0.25f;

    private Quaternion _initialImageRotation;
    private AudioSource _audioSource;

    private void Awake()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;

        _audioSource = GetComponent<AudioSource>();

        if (imageToRotate != null)
            _initialImageRotation = imageToRotate.localRotation;
    }

    private void Start()
    {
        // Start all sequences
        StartCoroutine(Sequence());
        StartCoroutine(PlaySoundWithDelay());

        if (imageToRotate != null)
        {
            StartCoroutine(RoarRotationSequence());
        }
    }

    private IEnumerator PlaySoundWithDelay()
    {
        if (roarClip == null || _audioSource == null)
            yield break;

        yield return new WaitForSecondsRealtime(audioStartDelay);
        _audioSource.PlayOneShot(roarClip);
    }

    private IEnumerator Sequence()
    {
        canvasGroup.blocksRaycasts = true;

        // Wait before fading in
        yield return new WaitForSeconds(beforeFadeDuration);

        // Fade IN (using the uncommented code's approach)
        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsed / fadeInDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;

        // Stay visible
        yield return new WaitForSecondsRealtime(stayDuration);

        // Fade OUT
        yield return StartCoroutine(Fade(1f, 0f, fadeOutDuration));

        SceneManager.LoadScene(sceneToLoad);
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        float elapsed = 0f;
        canvasGroup.alpha = from;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            canvasGroup.alpha = Mathf.Lerp(from, to, t);
            yield return null;
        }

        canvasGroup.alpha = to;
    }

    private IEnumerator RoarRotationSequence()
    {
        // --- Roar 1 ---
        yield return new WaitForSecondsRealtime(roar1StartDelay);
        yield return StartCoroutine(RotateOutAndBack(imageToRotate, roar1RotationZ, roar1RotationDuration));

        // --- Roar 2 ---
        yield return new WaitForSecondsRealtime(roar2StartDelay - roar1StartDelay);
        yield return StartCoroutine(RotateOutAndBack(imageToRotate, roar2RotationZ, roar2RotationDuration));
    }

    private IEnumerator RotateOutAndBack(Transform target, float angleZ, float duration)
    {
        Quaternion startRot = target.localRotation;
        Quaternion targetRot = _initialImageRotation * Quaternion.Euler(0f, 0f, angleZ);

        // Rotate out
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            target.localRotation = Quaternion.Lerp(startRot, targetRot, t);
            yield return null;
        }

        target.localRotation = targetRot;

        // Rotate back
        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            target.localRotation = Quaternion.Lerp(targetRot, _initialImageRotation, t);
            yield return null;
        }

        target.localRotation = _initialImageRotation;
    }
}