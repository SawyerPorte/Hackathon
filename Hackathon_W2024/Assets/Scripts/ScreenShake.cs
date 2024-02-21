using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    [SerializeField] float shakeDuration = 0.5f;
    [SerializeField] Player player;
    WinCon win;

    private Vector3 originalPosition;
    private float shakeTimeRemaining = 0f;

    void Start()
    {
        originalPosition = transform.localPosition;
        win = player.GetComponent<WinCon>();
    }

    void Update()
    {
        if (shakeTimeRemaining > 0)
        {
            transform.localPosition = originalPosition + Random.insideUnitSphere * shakeTimeRemaining;

            shakeTimeRemaining -= Time.deltaTime;
        }
        else if(win.zoom == false)
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
