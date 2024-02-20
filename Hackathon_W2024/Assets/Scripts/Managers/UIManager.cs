using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public Canvas currentCanvas;
    public TMP_Text levelText;

    public void Awake()
    {
        //currentCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        //levelText = currentCanvas.GetComponent<TMP_Text>();
    }

    public void Update()
    {
        //levelText.text = GameManager.LevelManager.GetCurrentLevel().ToString();
    }
}
