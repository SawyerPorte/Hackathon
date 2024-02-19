using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    [SerializeField] float shakeDuration = 0.5f;

    private Vector3 originalPosition;
    private float shakeTimeRemaining = 0f;

    void Start()
    {
        originalPosition = transform.localPosition;
    }

    void Update()
    {
        if (shakeTimeRemaining > 0)
        {
            transform.localPosition = originalPosition + Random.insideUnitSphere * shakeTimeRemaining;

            shakeTimeRemaining -= Time.deltaTime;
        }
        else
        {
            shakeTimeRemaining = 0f;
            transform.localPosition = originalPosition;
        }
    }

    public void ShakeCamera(float intensity)
    {
        shakeTimeRemaining = shakeDuration;
        // Adjust shake intensity based on player's falling Y velocity
        shakeTimeRemaining *= intensity;
    }
}
