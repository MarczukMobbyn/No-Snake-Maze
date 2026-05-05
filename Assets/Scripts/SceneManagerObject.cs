using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerObject : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;

    bool playBlackScreen = false;
    [SerializeField] CanvasGroup blackScreenCanvasGroup;
    [SerializeField] float blackScreenFadeSpeed = .5f;

    private void Start()
    {
        blackScreenCanvasGroup.alpha = 1f;
        playBlackScreen = false;
    }
    public void ReloadScene(float waitSeconds)
    {
        audioSource.Play();
        StartCoroutine(RealoadSceneRoutine(waitSeconds));
    }

    IEnumerator RealoadSceneRoutine(float waitSeconds)
    {
        yield return new WaitForSeconds(waitSeconds);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void LoadNextScene(float waitSeconds)
    {
        audioSource.Play();
        playBlackScreen = true;
        StartCoroutine(LoadNextSceneRoutine(waitSeconds));
    }

    IEnumerator LoadNextSceneRoutine(float waitSeconds)
    {
        yield return new WaitForSeconds(waitSeconds);

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.LogWarning("No next scene available to load.");
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void Update()
    {
        if (playBlackScreen)
        {
            //change black screen alpha
            blackScreenCanvasGroup.gameObject.SetActive(true);
            blackScreenCanvasGroup.alpha = Mathf.MoveTowards(blackScreenCanvasGroup.alpha, 1f, Time.deltaTime * blackScreenFadeSpeed);
        }
        else
        {
            blackScreenCanvasGroup.alpha = Mathf.MoveTowards(blackScreenCanvasGroup.alpha, 0f, Time.deltaTime * blackScreenFadeSpeed);
            if (blackScreenCanvasGroup.alpha <= 0f)
            {
                blackScreenCanvasGroup.gameObject.SetActive(false);
            }
        }
    }

    public void LoadMainMenu(float waitSeconds)
    {
        audioSource.Play();
        blackScreenCanvasGroup.gameObject.SetActive(true);
        playBlackScreen = true;
        StartCoroutine(LoadMainMenuRoutine(waitSeconds));
    }

    IEnumerator LoadMainMenuRoutine(float waitSeconds)
    {
        yield return new WaitForSeconds(waitSeconds); // Wait for the audio to finish playing
        SceneManager.LoadScene(0);
    }

    public void LoadVictoryScene(float waitSeconds)
    {
        audioSource.Play();
        blackScreenCanvasGroup.gameObject.SetActive(true);
        playBlackScreen = true;
        StartCoroutine(LoadVictorySceneRoutine(waitSeconds));
    }

    IEnumerator LoadVictorySceneRoutine(float waitSeconds)
    {
        yield return new WaitForSeconds(waitSeconds); // Wait for the audio to finish playing
        SceneManager.LoadScene("You Won Scene");
    }
}
