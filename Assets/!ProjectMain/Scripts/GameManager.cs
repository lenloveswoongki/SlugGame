using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; } = default;
    public CameraManager mainCamera = default;
    public AudioSource mainAudioSource = default;
    public AudioSource mainSong = default;

    [Header("Timer")]
    public float timer = default;
    public Text _timerText = default;
    public void Awake()
    {
       

        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

        DontDestroyOnLoad(this);
    }

    public void OnLevelStarts()
    {
        StartCoroutine(TimerUpdate());
    }
    private IEnumerator TimerUpdate()
    {
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            _timerText.text = timer.ToString();
            yield return null;
        }
            _timerText.text = "YOU DIED";
            Restart();        
    }
    public async void Restart()
    {
        Time.timeScale = 0;
        await Task.Delay(2000);
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public async void NextLevel()
    {
        Time.timeScale = 0;
        await Task.Delay(2000);
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
public interface IInteractible
{
    public void OnInteract();
}
