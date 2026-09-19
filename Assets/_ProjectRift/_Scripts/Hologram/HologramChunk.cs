using UnityEngine;
using UnityEngine.SceneManagement;

public class HologramChunk : MonoBehaviour
{
    private Scene _originalScene;

    private void Start()
    {
        _originalScene = gameObject.scene;

        HologramSync holoSync = HologramSync.Instance;

        if (holoSync != null)
        {
            Transform container = holoSync.HologramOrigin;

            if (container != null)
            {
                transform.SetParent(container);

                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
                transform.localScale = Vector3.one;
            }

            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }
    }

    private void OnSceneUnloaded(Scene scene)
    {
        if (scene == _originalScene)
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy() => SceneManager.sceneUnloaded -= OnSceneUnloaded;
}