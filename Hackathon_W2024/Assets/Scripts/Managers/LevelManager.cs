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

    private bool _gameBGMStart;

    private void Start()
    {
        _level = PlayerPrefs.GetInt("LevelProgress", 0);
        //levelText.text = _level.ToString();
    }
    private void OnApplicationQuit()
    {
        PlayerPrefs.DeleteAll();
    }

    private void Update()
    {
        if(_progressBar!= null)
        {
            _progressBar.fillAmount = Mathf.MoveTowards(_progressBar.fillAmount, _target, 3 * Time.deltaTime);
        }
        
    }

    public async void LoadScene(string sceneName)
    {
        print("loading scene");

        _target = 0;
        if (_progressBar != null)
        {
            _progressBar.fillAmount = 0;
        }
       

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

    public void LoadNextLevel()
    {
        // Start music
        if (!_gameBGMStart)
        {
            _gameBGMStart = true;
            SoundManager.Instance.PlayMusic("GameBGM");
        }

        PlayerPrefs.SetInt("LevelProgress", ++_level);
        if(_level > 9)
        {
            SceneManager.LoadScene("EndScene");
        }
        else
        {
            string sceneName = "Level" + (PlayerPrefs.GetInt("LevelProgress")).ToString();
            //Debug.Log("loading level " + _level + " saved as " + PlayerPrefs.GetInt("LevelProgress"));

            // End game
            // if level value is last one, when we go to next level, it'll  be "EndScene" instead of scenename + #
            SceneManager.LoadScene(sceneName);
            //LoadScene(sceneName);
        }


    }

    public int GetCurrentLevel()
    {
        return _level;
    }

    public void SetCurrentLevel(int level)
    {
        _level = level;
        print(_level);
    }
}
