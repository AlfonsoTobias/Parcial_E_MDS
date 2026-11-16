using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class CigaretteDispenser : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private XRBaseInteractable boxInteractable;
    [SerializeField] private XRInteractionManager interactionManager;
    [SerializeField] private GameObject cigarettePrefab;

    private void Awake()
    {
        if (boxInteractable == null)
            boxInteractable = GetComponent<XRBaseInteractable>();
    }

    private void OnEnable()
    {
        boxInteractable.selectEntered.AddListener(SpawnCigarette);
    }

    private void OnDisable()
    {
        boxInteractable.selectEntered.RemoveListener(SpawnCigarette);
    }

    private void SpawnCigarette(SelectEnterEventArgs args)
    {
        if (cigarettePrefab == null || interactionManager == null)
            return;

        // Identificar la mano que interactuó
        var interactor = args.interactorObject;

        // Obtener la posición de agarre de esa mano
        Transform hand = interactor.GetAttachTransform(
            args.interactableObject
        );

        // Instanciar el cigarrillo en la mano
        GameObject cigarette = Instantiate(
            cigarettePrefab,
            hand.position,
            hand.rotation
        );

        XRGrabInteractable grabInteractable =
            cigarette.GetComponent<XRGrabInteractable>();

        if (grabInteractable == null)
        {
            Debug.LogError("El prefab necesita XR Grab Interactable.");
            Destroy(cigarette);
            return;
        }

        // Liberar la caja
        interactionManager.SelectExit(
            interactor,
            args.interactableObject
        );

        // Transferir el agarre al cigarrillo
        interactionManager.SelectEnter(
            interactor,
            grabInteractable
        );
    }
}