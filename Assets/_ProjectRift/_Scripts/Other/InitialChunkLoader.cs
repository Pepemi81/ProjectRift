using UnityEngine;
using UnityEngine.SceneManagement;

public class InitialChunkLoader : MonoBehaviour
{
    [SerializeField] private bool _loadAllAtOnce = true;

    [SerializeField] private SceneField[] _scenesToLoad;

    public void LoadInitialChunks()
    {
        if (_loadAllAtOnce)
        {
            foreach (var scene in _scenesToLoad)
            {
                SceneManager.LoadSceneAsync(scene.SceneName, LoadSceneMode.Additive);
            }
        }
        else
        {
            SceneManager.LoadSceneAsync(_scenesToLoad[0].SceneName, LoadSceneMode.Additive);
        }
    }
}
