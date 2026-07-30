using System.Collections;
using UnityEngine;

/// <summary>
/// ScriptableObject que almacena una lista ordenada de diapositivas (MonitorSlideData) 
/// para reproducirlas de forma consecutiva en el monitor.
/// </summary>
[RequireComponent(typeof(MonitorSlideRenderer))]
public class MonitorSequenceDirector : MonoBehaviour
{
    [SerializeField] private MonitorSlideData _testSlide;
    [SerializeField] private MonitorSequenceData _testSequence;

    private MonitorSlideRenderer _monitorRenderer;
    private Coroutine _sequenceCoroutine;
    private bool _interactPressed;

    private void Awake()
    {
        _monitorRenderer = GetComponent<MonitorSlideRenderer>();
    }

    public void PlaySequence(MonitorSequenceData sequenceData)
    {
        if (sequenceData == null || sequenceData.Slides.Count == 0)
        {
            Debug.LogWarning("<color=red>[LoreSequenceManager]</color> La secuencia está vacía o es nula.");
            return;
        }

        if (_sequenceCoroutine != null)
        {
            StopCoroutine(_sequenceCoroutine);
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
        }

        _sequenceCoroutine = StartCoroutine(ProcessSingleData(data));
    }

    private IEnumerator ProcessSequence(MonitorSequenceData sequenceData)
    {
        foreach (MonitorSlideData screenData in sequenceData.Slides)
        {
            yield return StartCoroutine(ProcessScreenData(screenData));
        }

        _monitorRenderer.ClearCurrentScreen();
        Debug.LogWarning("<color=cyan>[LoreSequenceManager]</color> Secuencia finalizada. Pantalla restaurada.");
        _sequenceCoroutine = null;

        sequenceData.EventOnComplete?.Invoke();
    }

    private IEnumerator ProcessSingleData(MonitorSlideData data)
    {
        yield return StartCoroutine(ProcessScreenData(data));

        _monitorRenderer.ClearCurrentScreen();
        Debug.LogWarning("<color=cyan>[LoreSequenceManager]</color> Data individual finalizado. Pantalla restaurada.");
        _sequenceCoroutine = null;
    }

    private IEnumerator ProcessScreenData(MonitorSlideData slideData)
    {
        _monitorRenderer.Initialize(slideData);
        _interactPressed = false;

        yield return new WaitUntil(() => _monitorRenderer.IsFinished || _interactPressed);

        if (_interactPressed && !_monitorRenderer.IsFinished)
        {
            _monitorRenderer.SkipTyping(slideData.DisplayText);
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
}