using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerBoxSceneLoader : MonoBehaviour
{
    [Header("Escenas")]
    [SerializeField] private SceneField[] _scenesToLoad;
    [SerializeField] private SceneField[] _scenesToUnload;

    [Header("Gizmos")]
    [SerializeField] private bool _showGizmos = true;
    private BoxCollider _boxCollider;

    private void OnTriggerEnter(Collider other)
    {
        LoadScenes();
        UnloadScenes();
    }

    private void LoadScenes()
    {
        for(int i = 0; i < _scenesToLoad.Length; i++)
        {
            bool isSceneLoaded = false;

            for(int j = 0; j < SceneManager.sceneCount; j++)
            {
                Scene loadedScene = SceneManager.GetSceneAt(j);
                if (loadedScene.name == _scenesToLoad[i].SceneName)
                {
                    isSceneLoaded = true;
                    break;
                }
            }

            if(!isSceneLoaded) SceneManager.LoadSceneAsync(_scenesToLoad[i], LoadSceneMode.Additive);
        }
    }

    private void UnloadScenes()
    {
        for(int i = 0; i < _scenesToUnload.Length; i++)
        {
            for(int j = 0; j < SceneManager.sceneCount; j++)
            {
                Scene loadedScene = SceneManager.GetSceneAt(j);
                if (loadedScene.name == _scenesToUnload[i].SceneName)
                {
                    SceneManager.UnloadSceneAsync(_scenesToUnload[i]);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (_boxCollider == null) _boxCollider = GetComponent<BoxCollider>();
        if (_boxCollider == null || !_showGizmos) return;

        Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        Gizmos.matrix = rotationMatrix;

        Gizmos.color = new Color(0f, 1f, 0f, 0.6f);
        Gizmos.DrawWireCube(_boxCollider.center, _boxCollider.size);

        Gizmos.color = new Color(0f, 1f, 0f, 0.15f);
        Gizmos.DrawCube(_boxCollider.center, _boxCollider.size);
    }
}