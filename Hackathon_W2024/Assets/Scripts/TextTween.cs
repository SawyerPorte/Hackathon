using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextTween : MonoBehaviour
{
    [SerializeField] float DurationSeconds = 1;
    [SerializeField] float StartDelaySeconds = 0;
    [SerializeField] float StartAlpha = 0;
    [SerializeField] float EndAlpha = 1;
    [SerializeField] float StartX = 0;
    [SerializeField] float EndX = 0;
    [SerializeField] float StartY = -80;
    [SerializeField] float EndY = -100;
    TextMeshProUGUI text;
    RectTransform pos;
    float t;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        text.alpha = StartAlpha;
        pos = GetComponent<RectTransform>();
        pos.anchoredPosition = new Vector2(StartX, StartY);
        t = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(StartDelaySeconds > 0)
        {
            StartDelaySeconds -= Time.deltaTime;
        }
        else
        {
            t += Time.deltaTime;
            text.alpha = EaseIn(StartAlpha, EndAlpha, t / DurationSeconds);
            pos.anchoredPosition = new Vector2(EaseIn(StartX, EndX, t / DurationSeconds), EaseIn(StartY, EndY, t / DurationSeconds));
        }
        
    }

    float EaseIn(float start, float end, float percentage)
    {
        if(percentage <= 0) return start;
        if(percentage >= 1) return end;

        return start + (end - start) * percentage * percentage * percentage;
    }

    float EaseOut(float start, float end, float percentage)
    {
        if (percentage <= 0) return start;
        if (percentage >= 1) return end;

        return start + (end - start) * (1 - Mathf.Pow(1 - percentage, 3));
    }

}
