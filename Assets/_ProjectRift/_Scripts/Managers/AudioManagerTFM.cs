using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManagerTFM : MonoBehaviour
{
    [Header("Referencias de Audio")]
    [SerializeField] private AudioSource m_AudioSource;
    [SerializeField] private AudioClip m_Clip;

    [Header("Configuracion de Trayectoria")]
    [Tooltip("Marcar si el sonido es estatico (no se traslada)")]
    [SerializeField] private bool m_IsStatic = false;
    [SerializeField] private Transform m_StartPoint;
    [SerializeField] private Transform m_EndPoint;

    [Header("Curva de Movimiento")]
    [Tooltip("Permite acelerar o desacelerar el paso del monstruo")]
    [SerializeField] private AnimationCurve m_MovementCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    private Coroutine m_MoveCoroutine;

    private void Reset()
    {
        m_AudioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Metodo público para invocar desde el evento On Press () del XRPushButton
    /// </summary>
    public void PlayEvent()
    {
        if (m_AudioSource == null || m_Clip == null) return;

        // Detener reproduccion previa si se pulsa de nuevo
        if (m_MoveCoroutine != null)
        {
            StopCoroutine(m_MoveCoroutine);
        }

        m_AudioSource.Stop();
        m_AudioSource.clip = m_Clip;

        if (m_IsStatic || m_StartPoint == null || m_EndPoint == null)
        {
            if (m_StartPoint != null) transform.position = m_StartPoint.position;
            m_AudioSource.Play();
        }
        else
        {
            m_MoveCoroutine = StartCoroutine(MoveAudioRoutine());
        }
    }

    private IEnumerator MoveAudioRoutine()
    {
        transform.position = m_StartPoint.position;
        m_AudioSource.Play();

        float duration = m_Clip.length;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float curveT = m_MovementCurve.Evaluate(t);

            transform.position = Vector3.Lerp(m_StartPoint.position, m_EndPoint.position, curveT);
            yield return null;
        }

        transform.position = m_EndPoint.position;
    }

    private void OnDrawGizmosSelected()
    {
        if (m_StartPoint != null && m_EndPoint != null && !m_IsStatic)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(m_StartPoint.position, m_EndPoint.position);
            Gizmos.DrawWireSphere(m_StartPoint.position, 0.5f);
            Gizmos.DrawWireSphere(m_EndPoint.position, 0.5f);
        }
        else if (m_StartPoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(m_StartPoint.position, 0.8f);
        }
    }
}