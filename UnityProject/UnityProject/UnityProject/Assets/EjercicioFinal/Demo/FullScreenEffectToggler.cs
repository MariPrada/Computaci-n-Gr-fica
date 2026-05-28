using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Coloca este componente en un GameObject vacío por cada efecto.
/// Asigna el RendererData asset y escribe el nombre exacto de la Feature.
/// </summary>
public class FullScreenEffectToggler : MonoBehaviour
{
    [Tooltip("El asset UniversalRendererData de tu proyecto (en Project > Settings o tu carpeta de renderer)")]
    public UniversalRendererData rendererData;

    [Tooltip("Nombre exacto de la FullScreenPassRendererFeature tal como aparece en el Renderer")]
    public string featureName;

    private ScriptableRendererFeature _feature;

    void Awake()
    {
        if (rendererData == null)
        {
            Debug.LogError($"[FullScreenEffectToggler] '{gameObject.name}': falta asignar el RendererData.", this);
            return;
        }

        _feature = rendererData.rendererFeatures.Find(f => f.name == featureName);

        if (_feature == null)
            Debug.LogError($"[FullScreenEffectToggler] No se encontró ninguna feature llamada '{featureName}' en {rendererData.name}.", this);

        // Empieza desactivada
        _feature?.SetActive(false);
    }

    public void Activate()   => _feature?.SetActive(true);
    public void Deactivate() => _feature?.SetActive(false);
}