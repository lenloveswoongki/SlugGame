using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public float timer = default;
    public Text timerText = default;
    public void Start()
    {
        GameManager.Instance.timer = timer;
        GameManager.Instance._timerText = timerText;
        GameManager.Instance.OnLevelStarts();
    }
}
