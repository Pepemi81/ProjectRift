using System.Collections;
using UnityEngine;

public class LoreSequenceManager : MonoBehaviour
{
    [SerializeField] private LoreScreenController _screenController;

    [Header("Testing")]
    [SerializeField] private ScreenDisplayData _testData;
    [SerializeField] private ScreenSequenceData _testSequence;

    private Coroutine _sequenceCoroutine;
    private bool _interactPressed;

    public void PlaySequence(ScreenSequenceData sequenceData)
    {
        if (sequenceData == null || sequenceData.Screens.Count == 0)
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

    public void PlaySingleData(ScreenDisplayData data)
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

    private IEnumerator ProcessSequence(ScreenSequenceData sequenceData)
    {
        foreach (ScreenDisplayData screenData in sequenceData.Screens)
        {
            yield return StartCoroutine(ProcessScreenData(screenData));
        }

        _screenController.ClearCurrentScreen();
        Debug.LogWarning("<color=cyan>[LoreSequenceManager]</color> Secuencia finalizada. Pantalla restaurada.");
        _sequenceCoroutine = null;
    }

    private IEnumerator ProcessSingleData(ScreenDisplayData data)
    {
        yield return StartCoroutine(ProcessScreenData(data));

        _screenController.ClearCurrentScreen();
        Debug.LogWarning("<color=cyan>[LoreSequenceManager]</color> Data individual finalizado. Pantalla restaurada.");
        _sequenceCoroutine = null;
    }

    private IEnumerator ProcessScreenData(ScreenDisplayData screenData)
    {
        _screenController.Initialize(screenData);
        _interactPressed = false;

        yield return new WaitUntil(() => _screenController.IsFinished || _interactPressed);

        if (_interactPressed && !_screenController.IsFinished)
        {
            _screenController.SkipTyping(screenData.DisplayText);
            _interactPressed = false;

            yield return new WaitForSeconds(0.2f);
        }

        if (screenData.AutoAdvanceDelay > 0f)
        {
            float timer = 0f;
            while (timer < screenData.AutoAdvanceDelay && !_interactPressed)
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

    public void TestSingleData()
    {
        PlaySingleData(_testData);
    }
}