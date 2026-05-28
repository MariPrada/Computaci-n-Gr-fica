using UnityEngine;

public class weso : MonoBehaviour
{
    [Header("Referencias asignadas desde el editor")]
    [SerializeField] private GameObject cameraObject;
    [SerializeField] private GameObject objectToActivate;
    [SerializeField] private GameObject objectToDeactivate;

    void Start()
    {
        if (cameraObject == null)
            Debug.LogWarning("weso: No se ha asignado la cámara en el editor.", this);
        if (objectToActivate == null)
            Debug.LogWarning("weso: No se ha asignado el objeto a activar en el editor.", this);
        if (objectToDeactivate == null)
            Debug.LogWarning("weso: No se ha asignado el objeto a desactivar en el editor.", this);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == cameraObject)
        {
            if (objectToActivate != null)
                objectToActivate.SetActive(true);
            if (objectToDeactivate != null)
                objectToDeactivate.SetActive(false);
        }
    }
}
