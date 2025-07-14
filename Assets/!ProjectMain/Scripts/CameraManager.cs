using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    [Header("Shake")]
    [SerializeField] private float _shakeIntensity = default;
    [SerializeField] private float _shakeDuration = default;
    private Vector3 _initialPosition = default;
    private bool _isShaking = default;

    private void Start()
    {
        _initialPosition = transform.position;
        GameManager.Instance.mainCamera = this;
    }
    public void ShakeCamera()
    {
        if(!_isShaking)
        { StartCoroutine(CameraShakeCorroutine()); }
    }

    private IEnumerator CameraShakeCorroutine()
    {
        _isShaking=true;
        float timer = 0;
        while(timer < _shakeDuration)
        {
            timer += Time.deltaTime;
            transform.position = _initialPosition + Random.insideUnitSphere * _shakeIntensity;
            yield return null;
        }
        _isShaking = false;
    }
}
