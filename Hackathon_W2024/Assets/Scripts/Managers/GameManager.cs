using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; } = null;

    public static LevelManager LevelManager { get; private set; } = null;

    public static UIManager UIManager { get; private set; } = null;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // Collect all the components attached to this game object
        LevelManager = GetComponent<LevelManager>();
        UIManager = GetComponent<UIManager>();
    }

    public void SavePrefs()
    {
        // Auto save your level progress
        PlayerPrefs.SetInt("LevelProgress", LevelManager.GetCurrentLevel());

        // Save audio volume settings

        // Save control scheme

        PlayerPrefs.Save();
    }

    public void LoadPrefs()
    {
        // Auto save your level progress
        LevelManager.SetCurrentLevel(PlayerPrefs.GetInt("LevelProgress", 0));
        PlayerPrefs.Save();
    }

    public void ExitGame()
    {
        // Auto save
        SavePrefs();

        Application.Quit(0);
    }
}
