using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    // Level data
    public float timer; // Time to pass the challenge
    public Text timerText;
    public Vector3Int startPos; // Start position of the slug

    
    public Slug player;
    public TilemapsManager _tiles; // The game tilemaps
    public EventManager eventManager; // level events

    public event Action OnReady; // Suscribe to this event to execute when the level is all set


    #region unity methods
    public void Awake()
    {
        eventManager = new EventManager();
        GameManager gm = GameManager.Instance;
        gm.currentLevel = this;
    }

    private void Start()
    {
        StartCoroutine(GetReady());
    } 
    #endregion


    // When is all set, the game starts
    private IEnumerator GetReady()
    {
        yield return new WaitUntil(IsReady);
        OnReady?.Invoke();
        OnReady = null;
        player.BeginPlay(startPos);
    }


    // Add to this method all the conditions needed to make sure the level is all set
    private bool IsReady()
    {
        if (player != null)
        {
            return true;
        }
        else
            return false;
    }
   
}

