using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private SceneField _sceneToLoad;
    [SerializeField] private GameObject _blackScreenPrefab;

    public void StartGame()
    {
        OverlayMatController screenInstance = Instantiate(_blackScreenPrefab).GetComponent<OverlayMatController>();

        if (screenInstance.IsAnimating) return;

        screenInstance.Show(()=>
        {
            SceneManager.LoadSceneAsync(_sceneToLoad);
        });
    }
}
