using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


// This class handles the global and generic settings that can be reused in each level and dont need to be reinstanced
public class GameManager : MonoBehaviour
{
    [HideInInspector] public static GameManager Instance { get; private set; }
    [HideInInspector] public CameraManager mainCamera = default; 
    [HideInInspector] public AudioSource mainAudioSource = default; // This audio source handles all the SFX of the game
    [HideInInspector] public LevelManager currentLevel; // The current level we are playing

    [Header("Audio")]
    public AudioSource mainSong = default; // This audio source plays the game music only

    [Header("Timer")] // When it comes to 0, game over
    public float timer = default; 
    public Text _timerText = default;

    // Unity Methods
    public void Awake()
    {       
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

        DontDestroyOnLoad(this);
    }
    public void Start() 
    {
        StartCoroutine(TimerUpdate());

        timer = currentLevel.timer;
        _timerText = currentLevel.timerText;
    }

  
    private IEnumerator TimerUpdate()   // When timer comes to 0 the game restarts. Its made on fixed time so framerate doesnt affect time scale
    {
        while (timer > 0)
        {
            timer -= Time.fixedDeltaTime;
            _timerText.text = timer.ToString();
            yield return new WaitForFixedUpdate();
        }
            _timerText.text = "YOU DIED";
            Restart();        
    }


    public async void Restart()     // Call this to restart level
    {
        Time.timeScale = 0;
        await Task.Delay(2000);
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public async void NextLevel()   // Call this to go to next level
    {
        Time.timeScale = 0;
        await Task.Delay(2000);
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
