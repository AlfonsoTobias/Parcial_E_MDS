using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ProximityAudio : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private XRGrabInteractable grabInteractable;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip soundClip;

    private Transform mainCamera;

    [Header("Detection")]
    [SerializeField] private float triggerDistance = 0.25f;
    [SerializeField] private float resetMargin = 0.15f;

    [Header("Optional - Bottle")]
    [SerializeField] private bool requireMouthFacingCamera;
    [SerializeField] private Transform mouthPoint;
    [SerializeField] private float maxFacingAngle = 45f;

    private bool hasPlayed;

    private void Start()
    {
        Camera cam = Camera.main;

        if (cam != null)
        {
            mainCamera = cam.transform;
        }
        else
        {
            Debug.LogError("No se encontró una cámara con el tag MainCamera.");
        }
    }

    private void Update()
    {
        if (grabInteractable == null || mainCamera == null || audioSource == null || soundClip == null)
            return;

        if (!grabInteractable.isSelected)
        {
            hasPlayed = false;
            return;
        }

        Transform detectionPoint = mouthPoint != null ? mouthPoint : transform;

        float distance = Vector3.Distance(
            detectionPoint.position,
            mainCamera.position
        );

        if (distance > triggerDistance + resetMargin)
        {
            hasPlayed = false;
            return;
        }

        if (hasPlayed || distance > triggerDistance)
            return;

        if (requireMouthFacingCamera)
        {
            if (mouthPoint == null)
                return;

            Vector3 directionToCamera =
                (mainCamera.position - mouthPoint.position).normalized;

            float angle = Vector3.Angle(
                mouthPoint.forward,
                directionToCamera
            );

            if (angle > maxFacingAngle)
                return;
        }

        audioSource.PlayOneShot(soundClip);
        hasPlayed = true;
    }
}