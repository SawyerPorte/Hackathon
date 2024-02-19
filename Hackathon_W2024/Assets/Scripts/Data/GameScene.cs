using UnityEngine;

/// <summary>
/// Reference:
/// https://blog.unity.com/engine-platform/achieve-better-scene-workflow-with-scriptableobjects
/// </summary>
public class GameScene : ScriptableObject
{
    [Header("Information")]
    public string sceneName;
    public string shortDescription;

    [Header("Sounds")]
    public AudioClip music;
    [Range(0.0f, 1.0f)]
    public float musicVolume;

    // Add any more properties between levels and menus
}
