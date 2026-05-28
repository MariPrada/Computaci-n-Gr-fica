using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Gestiona 3 efectos de partículas + posprocesado + animación de personaje.
/// Cada efecto se activa con su botón correspondiente (toggle).
/// </summary>
public class EffectManager : MonoBehaviour
{
    // -------------------------------------------------------------------------
    // Datos de cada efecto
    // -------------------------------------------------------------------------
    [System.Serializable]
    public class EffectSlot
    {
        [Tooltip("Nombre descriptivo (solo para el Inspector)")]
        public string name;

        [Header("Partículas")]
        [Tooltip("ParticleSystem raíz del efecto")]
        public ParticleSystem particles;

        [Header("Post-procesado")]
        [Tooltip("El componente FullScreenEffectToggler asignado a este efecto")]
        public FullScreenEffectToggler postProcess;

        [Header("Animación")]
        [Tooltip("Nombre exacto del Estado en el Animator Controller (ej: 'Attack', 'Dance')")]
        public string animationStateName;

        [Header("UI")]
        public Button button;
        public Color activeColor   = new Color(1f, 0.75f, 0.1f);
        public Color inactiveColor = new Color(0.75f, 0.75f, 0.75f);
    }

    // -------------------------------------------------------------------------
    // Inspector
    // -------------------------------------------------------------------------
    [Header("Efectos")]
    public EffectSlot[] effects = new EffectSlot[3];

    [Header("Personaje")]
    public Animator characterAnimator;

    [Tooltip("Nombre exacto del estado Idle en el Animator Controller")]
    public string idleStateName = "Idle";

    [Tooltip("Índice del layer del Animator donde están los estados (normalmente 0)")]
    public int animatorLayer = 0;

    [Header("Transición")]
    [Range(0f, 0.5f)]
    [Tooltip("Pausa en segundos entre apagar el efecto anterior y encender el nuevo")]
    public float switchDelay = 0.1f;

    // -------------------------------------------------------------------------
    // Estado interno
    // -------------------------------------------------------------------------
    private int  _activeIndex = -1;
    private bool _isSwitching = false;

    // -------------------------------------------------------------------------
    // Inicio
    // -------------------------------------------------------------------------
    void Start()
    {
        for (int i = 0; i < effects.Length; i++)
        {
            int captured = i;
            effects[i].button?.onClick.AddListener(() => OnButtonPressed(captured));
        }

        DeactivateAll();
        RefreshButtonColors();
    }

    // -------------------------------------------------------------------------
    // Callback de botón
    // -------------------------------------------------------------------------
    public void OnButtonPressed(int index)
    {
        if (_isSwitching) return;

        // Mismo botón = toggle OFF; botón distinto = switch
        int target = (_activeIndex == index) ? -1 : index;
        StartCoroutine(SwitchTo(target));
    }

    // -------------------------------------------------------------------------
    // Coroutine de transición
    // -------------------------------------------------------------------------
    private IEnumerator SwitchTo(int newIndex)
    {
        _isSwitching = true;

        // 1. Apagar efecto actual
        if (_activeIndex >= 0)
            DeactivateSlot(_activeIndex);

        // 2. Pausa de transición
        if (switchDelay > 0f)
            yield return new WaitForSeconds(switchDelay);

        // 3. Activar nuevo efecto (o volver a idle)
        _activeIndex = newIndex;

        if (newIndex >= 0)
            ActivateSlot(newIndex);
        else
            PlayState(idleStateName);

        // 4. Refrescar UI
        RefreshButtonColors();

        _isSwitching = false;
    }

    // -------------------------------------------------------------------------
    // Activar / Desactivar un slot
    // -------------------------------------------------------------------------
    private void ActivateSlot(int i)
    {
        var slot = effects[i];

        if (slot.particles != null)
        {
            slot.particles.gameObject.SetActive(true);
            slot.particles.Play(withChildren: true);
        }

        slot.postProcess?.Activate();

        PlayState(slot.animationStateName);
    }

    private void DeactivateSlot(int i)
    {
        var slot = effects[i];

        if (slot.particles != null)
            slot.particles.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmitting);

        slot.postProcess?.Deactivate();
    }

    private void DeactivateAll()
    {
        for (int i = 0; i < effects.Length; i++)
            DeactivateSlot(i);

        PlayState(idleStateName);
        _activeIndex = -1;
    }

    // -------------------------------------------------------------------------
    // Animación: Play por nombre de estado (sin triggers necesarios)
    // -------------------------------------------------------------------------
    private void PlayState(string stateName)
    {
        if (characterAnimator == null || string.IsNullOrEmpty(stateName)) return;
        characterAnimator.Play(stateName, animatorLayer, 0f);
    }

    // -------------------------------------------------------------------------
    // UI: feedback visual en botones
    // -------------------------------------------------------------------------
    private void RefreshButtonColors()
    {
        for (int i = 0; i < effects.Length; i++)
        {
            var btn = effects[i].button;
            if (btn == null) continue;

            var colors        = btn.colors;
            colors.normalColor   = (i == _activeIndex) ? effects[i].activeColor : effects[i].inactiveColor;
            colors.selectedColor = colors.normalColor;
            btn.colors        = colors;
        }
    }

    // -------------------------------------------------------------------------
    // Editor helpers
    // -------------------------------------------------------------------------
#if UNITY_EDITOR
    void OnValidate()
    {
        if (effects != null && effects.Length != 3)
            System.Array.Resize(ref effects, 3);
    }
#endif
}