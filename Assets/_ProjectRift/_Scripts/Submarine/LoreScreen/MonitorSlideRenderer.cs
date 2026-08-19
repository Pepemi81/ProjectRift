using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Determina si la diapositiva utiliza el diseño por defecto del Canvas o un Prefab modular.
/// </summary>
public class MonitorSlideRenderer : MonoBehaviour
{
    [Header("Referencias Default UI")]
    [SerializeField] private GameObject _defaultUI;
    [SerializeField] private Image defaultImage;
    [SerializeField] private TextMeshProUGUI _defaultText;

    [Header("Contenedor para Prefabs")]
    [SerializeField] private RectTransform _prefabContainer;

    [Header("Ajustes")]
    [SerializeField] private float _typingSpeed = 0.05f;

    private bool _isTypingDefault;
    private GameObject _instantiatedPrefab;
    private CustomSlideTextAnimator _currentCustomSlide;

    public bool IsFinished
    {
        get
        {
            if (_currentCustomSlide != null)
            {
                return _currentCustomSlide.IsFinished;
            }
            return !_isTypingDefault;
        }
    }

    private void Start()
    {
        ClearCurrentScreen();
    }

    public void Initialize(MonitorSlideData data)
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

    private void SetupDefaultUI(MonitorSlideData data)
    {
        _defaultUI.SetActive(true);

        defaultImage.gameObject.SetActive(data.DisplayImage != null);
        defaultImage.sprite = data.DisplayImage;

        _defaultText.gameObject.SetActive(!string.IsNullOrEmpty(data.DisplayText));

        if (!string.IsNullOrEmpty(data.DisplayText))
        {
            StartCoroutine(TypeTextDefault(data.DisplayText));
        }
        else
        {
            _isTypingDefault = false;
        }
    }

    private void SetupPrefabUI(MonitorSlideData data)
    {
        _defaultUI.SetActive(false);
        defaultImage.gameObject.SetActive(false);
        _defaultText.gameObject.SetActive(false);

        if (data.SlidePrefab != null && _prefabContainer != null)
        {
            _instantiatedPrefab = Instantiate(data.SlidePrefab, _prefabContainer);
            _currentCustomSlide = _instantiatedPrefab.GetComponent<CustomSlideTextAnimator>();

            if (_currentCustomSlide != null)
            {
                _currentCustomSlide.Initialize();
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
        _defaultText.text = string.Empty;

        for (int i = 0; i < textToType.Length; i++)
        {
            _defaultText.text += textToType[i];
            yield return new WaitForSeconds(_typingSpeed);
        }

        _isTypingDefault = false;
    }

    public void SkipTyping(string fullText)
    {
        if (_currentCustomSlide != null)
        {
            _currentCustomSlide.SkipTyping();
        }
        else
        {
            StopAllCoroutines();
            if (_defaultText != null && _defaultText.gameObject.activeSelf)
            {
                _defaultText.text = fullText;
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

        _currentCustomSlide = null;

        _defaultUI.SetActive(false);
        defaultImage.sprite = null;
        _defaultText.text = string.Empty;
    }
}