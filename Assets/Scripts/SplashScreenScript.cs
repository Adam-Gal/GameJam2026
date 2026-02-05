using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class FadeOutAndLoadScene : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float beforeFadeDuration = 2f;
    [SerializeField] private float fadeDuration = 3f;
    [SerializeField] private string sceneToLoad;

    private void Awake()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
    }

    public void Start()
    {
        StartCoroutine(FadeAndLoad());
    }

    private IEnumerator FadeAndLoad()
    {
        canvasGroup.blocksRaycasts = true;
        
        yield return new WaitForSeconds(beforeFadeDuration);

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsed / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f;

        SceneManager.LoadScene(sceneToLoad);
    }
}