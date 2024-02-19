using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Resources:
/// https://blog.unity.com/engine-platform/achieve-better-scene-workflow-with-scriptableobjects
/// </summary>
public class ScenesData : ScriptableObject
{
    public List<Level> levels = new List<Level>();
    public List<Menu> menus = new List<Menu>();
    public int CurrentLevelIndex = 1;

    /*
     * Levels
     */

    // Load a scene with a given index
    public void LoadLevelWithIndext(int index)
    {
        if (index <= levels.Count)
        {
            // Load Gameplay scene for the level
            //SceneManager.LoadSceneAsync("Gameplay" + index.ToString());
        }
    }
}
