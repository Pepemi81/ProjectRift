using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Determina si la diapositiva utiliza el diseño por defecto del Canvas o un Prefab modular.
/// </summary>
public class MonitorSlideManager : MonoBehaviour
{
    [Header("Referencias Default UI")]
    [SerializeField] private GameObject _defaultUI;
    [FormerlySerializedAs("defaultImage")]
    [SerializeField] private Image _defaultImage;
    [SerializeField] private TextMeshProUGUI _defaultText;
    [SerializeField] private ImageBlink _dialogueEndArrow;

    [Header("Contenedor para Prefabs")]
    [SerializeField] private RectTransform _prefabContainer;

    [Header("Ajustes")]
    [SerializeField] private float _typingSpeed = 0.05f;
    [SerializeField] private MonitorSlideAudioPlayer _audioPlayer;

    private bool _isTyping;
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
            return !_isTyping;
        }
    }

    private void Awake()
    {
        if (_audioPlayer == null)
        {
            _audioPlayer = GetComponent<MonitorSlideAudioPlayer>();
        }
    }

    private void Start()
    {
        ClearCurrentScreen();
    }

    public void Initialize(MonitorSlideData data)
    {
        ClearCurrentScreen();

        _audioPlayer?.BeginSlide(data.AudioSettings);

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

        _defaultImage.gameObject.SetActive(data.DisplayImage != null);
        _defaultImage.sprite = data.DisplayImage;

        _defaultText.gameObject.SetActive(!string.IsNullOrEmpty(data.DisplayText));

        if (!string.IsNullOrEmpty(data.DisplayText))
        {
            StartCoroutine(TypeTextDefault(data));
        }
        else
        {
            _isTyping = false;
        }
    }

    private void SetupPrefabUI(MonitorSlideData data)
    {
        _defaultUI.SetActive(false);
        _defaultImage.gameObject.SetActive(false);
        _defaultText.gameObject.SetActive(false);

        if (data.SlidePrefab != null && _prefabContainer != null)
        {
            _instantiatedPrefab = Instantiate(data.SlidePrefab, _prefabContainer);
            _currentCustomSlide = _instantiatedPrefab.GetComponent<CustomSlideTextAnimator>();

            if (_currentCustomSlide != null)
            {
                _currentCustomSlide.Initialize(() => _audioPlayer?.NotifyCharacterRevealed(data.AudioSettings));
            }
        }
        else
        {
            Debug.LogWarning("<color=orange>[LoreScreenController]</color> Falta asignar el prefab o el RectTransform del contenedor.");
        }

        _isTyping = false;
    }

    private IEnumerator TypeTextDefault(MonitorSlideData data)
    {
        _isTyping = true;
        string textToType = data.DisplayText;
        _defaultText.text = string.Empty;
        _dialogueEndArrow.SetHidden();

        for (int i = 0; i < textToType.Length; i++)
        {
            _defaultText.text += textToType[i];
            _audioPlayer?.NotifyCharacterRevealed(data.AudioSettings);

            yield return new WaitForSeconds(_typingSpeed);
        }

        _isTyping = false;
        _dialogueEndArrow.Blink();
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
            _isTyping = false;
        }
    }

    public void ClearCurrentScreen()
    {
        StopAllCoroutines();
        _isTyping = false;

        if (_instantiatedPrefab != null)
        {
            Destroy(_instantiatedPrefab);
            _instantiatedPrefab = null;
        }

        _currentCustomSlide = null;
        _audioPlayer?.StopSlideAudio();

        _defaultUI.SetActive(false);
        _defaultImage.sprite = null;
        _defaultText.text = string.Empty;
    }
}