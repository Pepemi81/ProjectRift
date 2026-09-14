using System.Collections;
using UnityEngine;

public enum MonitorSequenceTestMode
{
    SingleSlide,
    Sequence
}
/// <summary>
/// ScriptableObject que almacena una lista ordenada de diapositivas (MonitorSlideData) 
/// para reproducirlas de forma consecutiva en el monitor.
/// </summary>
[RequireComponent(typeof(MonitorSlideManager))]
public class MonitorSequenceDirector : MonoBehaviour
{
    [SerializeField] private MonitorSequenceTestMode _testMode = MonitorSequenceTestMode.SingleSlide;
    [SerializeField] private MonitorSlideData _testSlide;
    [SerializeField] private MonitorSequenceData _testSequence;

    [SerializeField, ReadOnly] private MonitorSequenceData _currentSequence;
    [SerializeField, ReadOnly] private MonitorSlideData _currentSlide;
    [SerializeField, ReadOnly] private int _currentSlideNumber;
    [SerializeField, ReadOnly] private int _totalSlides;
    [SerializeField, ReadOnly] private int _remainingSlides;
    [SerializeField, ReadOnly] private bool _interactPressed;
    [SerializeField, ReadOnly] private bool _isPlaying;

    private MonitorSlideManager _monitorManager;
    private Coroutine _sequenceCoroutine;

    public MonitorSequenceTestMode TestMode => _testMode;
    public bool IsPlaying => _isPlaying;
    public int CurrentSlideNumber => _currentSlideNumber;
    public int RemainingSlides => _remainingSlides;

    private void Awake()
    {
        _monitorManager = GetComponent<MonitorSlideManager>();
    }

    public void PlaySequence(MonitorSequenceData sequenceData)
    {
        if (sequenceData == null || sequenceData.Slides.Count == 0)
        {
            Debug.LogWarning("<color=red>[LoreSequenceManager]</color> La secuencia est� vac�a o es nula.");
            return;
        }

        if (_sequenceCoroutine != null)
        {
            StopCoroutine(_sequenceCoroutine);
            ClearRuntimeState();
        }

        _sequenceCoroutine = StartCoroutine(ProcessSequence(sequenceData));
    }

    public void PlaySingleSlide(MonitorSlideData data)
    {
        if (data == null)
        {
            Debug.LogWarning("<color=red>[LoreSequenceManager]</color> El data de prueba es nulo.");
            return;
        }

        if (_sequenceCoroutine != null)
        {
            StopCoroutine(_sequenceCoroutine);
            ClearRuntimeState();
        }

        _sequenceCoroutine = StartCoroutine(ProcessSlide(data));
    }

    private IEnumerator ProcessSequence(MonitorSequenceData sequenceData)
    {
        _currentSequence = sequenceData;
        _totalSlides = sequenceData.Slides.Count;
        _isPlaying = true;

        for (int i = 0; i < sequenceData.Slides.Count; i++)
        {
            _currentSlideNumber = i + 1;
            _currentSlide = sequenceData.Slides[i];
            _remainingSlides = _totalSlides - i - 1;

            yield return StartCoroutine(ProcessScreenData(_currentSlide));
        }

        _monitorManager.ClearCurrentScreen();
        Debug.LogWarning("<color=cyan>[LoreSequenceManager]</color> Secuencia finalizada. Pantalla restaurada.");
        _sequenceCoroutine = null;
        ClearRuntimeState();

        sequenceData.EventOnComplete?.Invoke();
    }

    private IEnumerator ProcessSlide(MonitorSlideData data)
    {
        _currentSequence = null;
        _currentSlide = data;
        _currentSlideNumber = 1;
        _totalSlides = 1;
        _remainingSlides = 0;
        _isPlaying = true;

        yield return StartCoroutine(ProcessScreenData(data));

        _monitorManager.ClearCurrentScreen();
        Debug.LogWarning("<color=cyan>[LoreSequenceManager]</color> Slide individual finalizado. Pantalla restaurada.");
        _sequenceCoroutine = null;
        ClearRuntimeState();
    }

    private IEnumerator ProcessScreenData(MonitorSlideData slideData)
    {
        _monitorManager.Initialize(slideData);
        _interactPressed = false;

        yield return new WaitUntil(() => _monitorManager.IsFinished || _interactPressed);

        if (_interactPressed && !_monitorManager.IsFinished)
        {
            _monitorManager.SkipTyping(slideData.DisplayText);
            _interactPressed = false;

            yield return new WaitForSeconds(0.2f);
        }

        if (slideData.AutoAdvanceDelay > 0f)
        {
            float timer = 0f;
            while (timer < slideData.AutoAdvanceDelay && !_interactPressed)
            {
                timer += Time.deltaTime;
                yield return null;
            }
        }
        else
        {
            yield return new WaitUntil(() => _interactPressed);
        }
    }

    public void OnPlayerInteract()
    {
        _interactPressed = true;
    }

    public void TestSequence()
    {
        PlaySequence(_testSequence);
    }

    public void TestSingleSlide()
    {
        PlaySingleSlide(_testSlide);
    }

    private void ClearRuntimeState()
    {
        _currentSequence = null;
        _currentSlide = null;
        _currentSlideNumber = 0;
        _totalSlides = 0;
        _remainingSlides = 0;
        _interactPressed = false;
        _isPlaying = false;
    }
}