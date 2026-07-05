using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LoreScreenController : MonoBehaviour
{
    [Header("Referencias Default UI")]
    [SerializeField] private Image _screenImage;
    [SerializeField] private TextMeshProUGUI _screenText;

    [Header("Contenedor para Prefabs")]
    [SerializeField] private RectTransform _prefabContainer;

    [Header("Ajustes")]
    [SerializeField] private float _typingSpeed = 0.05f;

    private bool _isTypingDefault;
    private GameObject _instantiatedPrefab;
    private LoreCustomScreen _currentCustomScreen;

    public bool IsFinished
    {
        get
        {
            if (_currentCustomScreen != null)
            {
                return _currentCustomScreen.IsFinished;
            }
            return !_isTypingDefault;
        }
    }

    public void Initialize(ScreenDisplayData data)
    {
        ClearCurrentScreen();

        if (data.DisplayMode == ScreenDisplayMode.DefaultUI)
        {
            SetupDefaultUI(data);
        }
        else if (data.DisplayMode == ScreenDisplayMode.CustomPrefab)
        {
            SetupPrefabUI(data);
        }
    }

    private void SetupDefaultUI(ScreenDisplayData data)
    {
        if (_screenImage != null)
        {
            _screenImage.gameObject.SetActive(data.DisplayImage != null);
            _screenImage.sprite = data.DisplayImage;
        }

        if (_screenText != null)
        {
            _screenText.gameObject.SetActive(!string.IsNullOrEmpty(data.DisplayText));

            if (!string.IsNullOrEmpty(data.DisplayText))
            {
                StartCoroutine(TypeTextDefault(data.DisplayText));
            }
            else
            {
                _isTypingDefault = false;
            }
        }
    }

    private void SetupPrefabUI(ScreenDisplayData data)
    {
        if (_screenImage != null) _screenImage.gameObject.SetActive(false);
        if (_screenText != null) _screenText.gameObject.SetActive(false);

        if (data.ScreenPrefab != null && _prefabContainer != null)
        {
            _instantiatedPrefab = Instantiate(data.ScreenPrefab, _prefabContainer);
            _currentCustomScreen = _instantiatedPrefab.GetComponent<LoreCustomScreen>();

            if (_currentCustomScreen != null)
            {
                _currentCustomScreen.Initialize();
            }
            else
            {
                Debug.LogWarning("<color=orange>[LoreScreenController]</color> El prefab no tiene el componente LoreCustomScreen.");
            }
        }
        else
        {
            Debug.LogWarning("<color=orange>[LoreScreenController]</color> Falta asignar el prefab o el RectTransform del contenedor.");
        }

        _isTypingDefault = false;
    }

    private IEnumerator TypeTextDefault(string textToType)
    {
        _isTypingDefault = true;
        _screenText.text = string.Empty;

        for (int i = 0; i < textToType.Length; i++)
        {
            _screenText.text += textToType[i];
            yield return new WaitForSeconds(_typingSpeed);
        }

        _isTypingDefault = false;
    }

    public void SkipTyping(string fullText)
    {
        if (_currentCustomScreen != null)
        {
            _currentCustomScreen.SkipTyping();
        }
        else
        {
            StopAllCoroutines();
            if (_screenText != null && _screenText.gameObject.activeSelf)
            {
                _screenText.text = fullText;
            }
            _isTypingDefault = false;
        }
    }

    public void ClearCurrentScreen()
    {
        StopAllCoroutines();
        _isTypingDefault = false;

        if (_instantiatedPrefab != null)
        {
            Destroy(_instantiatedPrefab);
            _instantiatedPrefab = null;
        }

        _currentCustomScreen = null;

        if (_screenImage != null)
        {
            _screenImage.gameObject.SetActive(false);
            _screenImage.sprite = null;
        }

        if (_screenText != null)
        {
            _screenText.gameObject.SetActive(false);
            _screenText.text = string.Empty;
        }
    }
}