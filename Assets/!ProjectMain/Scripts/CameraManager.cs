using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    [Header("Shake")]
    [SerializeField] private float _shakeIntensity;
    [SerializeField] private float _shakeDuration;

    private Vector3 _initialPosition;
    private bool _isShaking;


    // Unity methods
    private void Start()
    {
        _initialPosition = transform.position;
        GameManager.Instance.mainCamera = this;
    }

    #region methods

    // When the slug collides the camera shakes
    public void ShakeCamera()
    {
        if (!_isShaking)
        { StartCoroutine(CameraShakeCorroutine()); }
    }
    private IEnumerator CameraShakeCorroutine()
    {
        _isShaking = true;
        float timer = 0;

        while (timer < _shakeDuration)
        {
            timer += Time.deltaTime;
            transform.position = _initialPosition + Random.insideUnitSphere * _shakeIntensity;
            yield return null;
        }

        transform.position = _initialPosition;
        _isShaking = false;
    } 
    #endregion
}
