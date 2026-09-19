using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneLoadManager : MonoBehaviour
{
    [SerializeField] private SceneField _defaultSceneToLoad;

    [Tooltip("Referencia a la pantalla negra. Admite prefab o instancia")]
    [SerializeField] private GameObject _blackScreenReference;

    private bool _isLoading;

    public void LoadDefaultScene()
    {
        if (_isLoading) return;

        StartCoroutine(LoadSceneRoutine(_defaultSceneToLoad));
    }

    public void LoadScene(SceneField sceneToLoad)
    {
        if (_isLoading) return;
        StartCoroutine(LoadSceneRoutine(sceneToLoad));
    }

    private IEnumerator LoadSceneRoutine(SceneField sceneToLoad)
    {
        _isLoading = true;

        OverlayMatController screenInstance = GetOrCreateBlackScreen();

        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneToLoad);
        loadOperation.allowSceneActivation = false;

        bool fadeFinished = false;
        screenInstance.gameObject.SetActive(true);
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

    private OverlayMatController GetOrCreateBlackScreen()
    {
        bool isInstanced =
            _blackScreenReference.scene.IsValid() &&
            _blackScreenReference.scene.isLoaded;

        if (isInstanced) return _blackScreenReference.GetComponent<OverlayMatController>();

        return Instantiate(_blackScreenReference).GetComponent<OverlayMatController>();
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
