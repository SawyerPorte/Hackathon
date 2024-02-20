using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance => GameManager.LevelManager;

    // Current level the players are on
    private int _level;
    public TMP_Text levelText;

    [SerializeField] private GameObject _mainCanvas;
    [SerializeField] private GameObject _loaderCanvas;
    [SerializeField] private Image _progressBar;
    private float _target;

    private void Start()
    {
        _level = PlayerPrefs.GetInt("LevelProgress", 0);
        //levelText.text = _level.ToString();
    }

    private void Update()
    {
        _progressBar.fillAmount = Mathf.MoveTowards(_progressBar.fillAmount, _target, 3 * Time.deltaTime);
    }

    public async void LoadScene(string sceneName)
    {
        print("loading scene");

        _target = 0;
        _progressBar.fillAmount = 0;

        var scene = SceneManager.LoadSceneAsync(sceneName);
        scene.allowSceneActivation = false;

        _mainCanvas.SetActive(false);
        _loaderCanvas.SetActive(true);

        do
        {
            await Task.Delay(100);
            _target = scene.progress;
        } while (scene.progress < 0.9f);

        await Task.Delay(1000);

        scene.allowSceneActivation = true;
        _loaderCanvas.SetActive(false);
    }

    public int GetCurrentLevel()
    {
        return _level;
    }

    public void SetCurrentLevel(int level)
    {
        _level = level;
    }
}
