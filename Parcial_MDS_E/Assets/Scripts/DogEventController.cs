using System.Collections;
using UnityEngine;

public class DogEventController : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject roberto;
    public Animator dogAnimator;

    [Header("Puntos del recorrido")]
    public Transform dogSpawnZone;
    public Transform dogSitZone;
    public Transform dogExitZone;

    [Header("Movimiento")]
    public float moveSpeed = 1.2f;
    public float rotationSpeed = 5f;

    [Header("Tiempos de espera")]
    public float idleDuration = 4f;
    public float sittingDuration = 8f;

    [Header("Estados del Animator")]
    public string walkState = "Armature|Walk";
    public string idleState = "Armature|Idle";
    public string sitDownState = "Armature|Sit_Down";
    public string sitState = "Armature|Sit";
    public string standUpState = "Armature|Stand_up";

    private bool eventStarted = false;
    private Renderer[] dogRenderers;

    private void Start()
    {
        dogRenderers = roberto.GetComponentsInChildren<Renderer>(true);

        roberto.transform.position = dogSpawnZone.position;
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (eventStarted || !other.CompareTag("Player"))
            return;

        eventStarted = true;
        StartCoroutine(DogSequence());
    }

    private IEnumerator DogSequence()
    {
        // Roberto aparece y comienza a caminar.
        Debug.Log("¡EL EVENTO DEL PERRO COMENZÓ!");
        
        
        dogAnimator.Play(walkState, 0, 0f);

        yield return MoveTo(dogSitZone.position);

        // Se queda parado un momento.
        dogAnimator.Play(idleState, 0, 0f);
        yield return new WaitForSeconds(idleDuration);

        // Se sienta.
        dogAnimator.Play(sitDownState, 0, 0f);
        yield return WaitForAnimation(sitDownState);

        // Permanece sentado.
        dogAnimator.Play(sitState, 0, 0f);
        yield return new WaitForSeconds(sittingDuration);

        // Se levanta.
        dogAnimator.Play(standUpState, 0, 0f);
        yield return WaitForAnimation(standUpState);

        // Se marcha hacia la niebla.
        dogAnimator.Play(walkState, 0, 0f);
        yield return MoveTo(dogExitZone.position);

        // Desaparece definitivamente.
        SetDogVisible(false);
    }

    private IEnumerator MoveTo(Vector3 destination)
    {
        while (Vector3.Distance(roberto.transform.position, destination) > 0.05f)
        {
            Vector3 direction = destination - roberto.transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);

                roberto.transform.rotation = Quaternion.Slerp(
                    roberto.transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );
            }

            roberto.transform.position = Vector3.MoveTowards(
                roberto.transform.position,
                destination,
                moveSpeed * Time.deltaTime
            );

            yield return null;
        }

        roberto.transform.position = destination;
    }

    private IEnumerator WaitForAnimation(string stateName)
    {
        yield return null;

        while (dogAnimator.GetCurrentAnimatorStateInfo(0).IsName(stateName) &&
               dogAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            yield return null;
        }
    }

    private void SetDogVisible(bool visible)
    {
        foreach (Renderer rend in dogRenderers)
        {
            rend.enabled = visible;
        }
    }
}