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
    [SerializeField] private Image _monitorImage;
    [SerializeField] private TextMeshProUGUI _monitorText;
    [SerializeField] private GameObject _decorations;

    [Header("Contenedor para Prefabs")]
    [SerializeField] private RectTransform _prefabContainer;

    [Header("Ajustes")]
    [SerializeField] private float _typingSpeed = 0.05f;

    private bool _isTypingDefault;
    private GameObject _instantiatedPrefab;
    private CustomSlideAnimator _currentCustomSlide;

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
        _decorations.SetActive(true);

        _monitorImage.gameObject.SetActive(data.DisplayImage != null);
        _monitorImage.sprite = data.DisplayImage;

        _monitorText.gameObject.SetActive(!string.IsNullOrEmpty(data.DisplayText));

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
        _decorations.SetActive(false);
        _monitorImage.gameObject.SetActive(false);
        _monitorText.gameObject.SetActive(false);

        if (data.SlidePrefab != null && _prefabContainer != null)
        {
            _instantiatedPrefab = Instantiate(data.SlidePrefab, _prefabContainer);
            _currentCustomSlide = _instantiatedPrefab.GetComponent<CustomSlideAnimator>();

            if (_currentCustomSlide != null)
            {
                _currentCustomSlide.Initialize();
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
        _monitorText.text = string.Empty;

        for (int i = 0; i < textToType.Length; i++)
        {
            _monitorText.text += textToType[i];
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
            if (_monitorText != null && _monitorText.gameObject.activeSelf)
            {
                _monitorText.text = fullText;
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

        _decorations.SetActive(false);

        _monitorImage.gameObject.SetActive(false);
        _monitorImage.sprite = null;

        _monitorText.gameObject.SetActive(false);
        _monitorText.text = string.Empty;
    }
}