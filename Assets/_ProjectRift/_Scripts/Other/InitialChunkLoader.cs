using UnityEngine;
using UnityEngine.SceneManagement;

public class InitialChunkLoader : MonoBehaviour
{
    [SerializeField] private SceneField[] _scenesToLoad;

    public void LoadInitialChunks()
    {
        foreach (var scene in _scenesToLoad)
        {
            SceneManager.LoadSceneAsync(scene.SceneName, LoadSceneMode.Additive);
        }
    }
}
