using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Action _activatation = default;
    public void Start()
    {
        _activatation += GameManager.Instance.NextLevel;
    }
    public void OnInteract()
    {
        _activatation?.Invoke();
    }
}
