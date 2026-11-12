
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("Referencias")]
    public Transform skySphere;
    public Light sunLight;
    public Light moonLight;

    [Header("Duracion del ciclo")]
    public float cycleDuration = 600f;

    [Header("Rotacion")]
    public float rotationOffset = 0f;

    [Header("Luz solar")]
    public float sunMaxIntensity = 1.2f;
    public Color sunriseColor = new Color(1f, 0.35f, 0.15f);
    public Color middayColor = new Color(1f, 0.92f, 0.75f);
    public Color sunsetColor = new Color(1f, 0.3f, 0.12f);

    [Header("Luz lunar")]
    public float moonMaxIntensity = 0.2f;
    public Color moonColor = new Color(0.35f, 0.45f, 1f);

    [Header("Transiciones")]
    [Range(0f, 1f)]
    public float sunriseStart = 0.20f;
    [Range(0f, 1f)]
    public float sunsetStart = 0.70f;

    [Range(0.01f, 1f)]
    public float horizonFade = 0.25f;

    private float currentTime;

    private Quaternion skyInitialRotation;
    private Quaternion sunInitialRotation;
    private Quaternion moonInitialRotation;

    void Start()
    {
        if (skySphere != null)
            skyInitialRotation = skySphere.rotation;

        if (sunLight != null)
            sunInitialRotation = sunLight.transform.rotation;

        if (moonLight != null)
            moonInitialRotation = moonLight.transform.rotation;
    }

    void Update()
    {
        currentTime += Time.deltaTime / cycleDuration;
        currentTime %= 1f;

        float angle = currentTime * 360f;

        // El cielo gira en sentido contrario.
        if (skySphere != null)
            skySphere.rotation = skyInitialRotation * Quaternion.Euler(-angle, 0f, 0f);

        // Las luces giran en el sentido original.
        if (sunLight != null)
        {
            sunLight.transform.rotation =
                sunInitialRotation * Quaternion.Euler(angle, 0f, 0f);

            // La luna siempre apunta en dirección opuesta al sol.
            if (moonLight != null)
            {
                moonLight.transform.rotation =
                    sunLight.transform.rotation * Quaternion.Euler(180f, 0f, 0f);
            }
        }

        UpdateLighting();
    }

    void UpdateLighting()
    {
        if (sunLight == null || moonLight == null)
            return;

        // Comprueba cuánto apunta cada luz hacia abajo.
        float sunDown = Vector3.Dot(
            sunLight.transform.forward,
            Vector3.down
        );

        float moonDown = Vector3.Dot(
            moonLight.transform.forward,
            Vector3.down
        );

        // Convierte la orientación en una intensidad gradual.
        sunDown = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(sunDown));
        moonDown = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(moonDown));

        sunLight.intensity = sunDown * sunMaxIntensity;
        moonLight.intensity = moonDown * moonMaxIntensity;

        // Colores de las luces.
        sunLight.color = Color.Lerp(
            sunriseColor,
            middayColor,
            sunDown
        );

        moonLight.color = moonColor;
    }
}