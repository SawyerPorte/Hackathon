using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public Canvas currentCanvas;
    public TMP_Text levelText;

    public GameObject creditsText;
    private bool _isShown = false;

    public void Awake()
    {
        //currentCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        //levelText = currentCanvas.GetComponent<TMP_Text>();

        creditsText.SetActive(false);
    }

    public void Update()
    {
        //levelText.text = GameManager.LevelManager.GetCurrentLevel().ToString();
    }

    public void ToggleCredits()
    {
        if (_isShown)
        {
            creditsText.SetActive(false);
            _isShown = false;
        }
        else
        {
            creditsText.SetActive(true);
            _isShown = true;
        }
    }
}
