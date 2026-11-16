using UnityEngine;

public class DogIdleBark : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip barkSound;
    [SerializeField] private Transform sitZone;

    [Header("Settings")]
    [SerializeField] private float barkInterval = 5f;
    [SerializeField] private float sitZoneRadius = 0.5f;

    private float barkTimer;

    private void Update()
    {
        if (animator == null || audioSource == null ||
            barkSound == null || sitZone == null)
            return;

        // Comprobar si Roberto está reproduciendo Idle
        AnimatorStateInfo state =
            animator.GetCurrentAnimatorStateInfo(0);

        bool isIdle = state.IsName("Armature|Idle");

        // Comprobar si está en el SitZone
        float distance = Vector3.Distance(
            transform.position,
            sitZone.position
        );

        bool isInSitZone = distance <= sitZoneRadius;

        if (isIdle && isInSitZone)
        {
            barkTimer += Time.deltaTime;

            if (barkTimer >= barkInterval)
            {
                audioSource.PlayOneShot(barkSound);
                barkTimer = 0f;
            }
        }
        else
        {
            barkTimer = 0f;
        }
    }
}
