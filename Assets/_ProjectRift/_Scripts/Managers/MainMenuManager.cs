using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private SceneField _sceneToLoad;
    [SerializeField] private GameObject _blackScreenPrefab;

    private bool _isLoading;

    public void StartGame()
    {
        if (_isLoading) return;

        StartCoroutine(StartGameRoutine());
    }

    private IEnumerator StartGameRoutine()
    {
        _isLoading = true;

        OverlayMatController screenInstance = Instantiate(_blackScreenPrefab).GetComponent<OverlayMatController>();

        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(_sceneToLoad);
        loadOperation.allowSceneActivation = false;

        bool fadeFinished = false;
        screenInstance.Show(() => fadeFinished = true);

        while (!fadeFinished)
        {
            yield return null;
        }

        while (loadOperation.progress < 0.9f)
        {
            yield return null;
        }

        loadOperation.allowSceneActivation = true;
    }
}
