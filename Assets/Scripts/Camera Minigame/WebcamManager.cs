using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif

public class WebcamManager : MonoBehaviour
{
    public bool IsReady { get; private set; }
    public WebCamTexture WebcamTexture { get; private set; }

    [SerializeField] private RawImage _cameraDisplay;
    [SerializeField] private AspectRatioFitter _aspectRatioFitter;

    private WebCamDevice? _webcamDeviceBack;
    private WebCamDevice? _webcamDeviceFront;
    private WebCamDevice? _activeWebcamDevice;

    private void Start()
    {
#if UNITY_IOS
        StartCoroutine(AskForPermissionIfRequired(UserAuthorization.WebCam,
            () => StartCoroutine(InitializeCamera())));
        return;
#elif UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            AskCameraPermission();
            return;
        }
#endif
        StartCoroutine(InitializeCamera());
    }

    private void OnDisable()
    {
        WebcamTexture.Stop();
    }


    public void SwitchCamera()
    {
        if (!IsReady) return;
        if (_webcamDeviceBack is null || _webcamDeviceFront is null) return;

        _activeWebcamDevice = _activeWebcamDevice.Value.Equals(_webcamDeviceBack.Value)
            ? _webcamDeviceFront
            : _webcamDeviceBack;
        StartCoroutine(SetCameraTexture());

        ServiceLocator.Get<IMusicService>().PlaySoundPitch("camara");
    }


    private IEnumerator InitializeCamera()
    {
        IsReady = false;
        yield return new WaitForSecondsRealtime(.5f);
        InitCameraDevices();
        yield return SetCameraTexture();
    }
    
    private void InitCameraDevices()
    {
        foreach (var device in WebCamTexture.devices)
        {
            if (device.isFrontFacing)
            {
                _webcamDeviceFront ??= device;
            }
            else if (!device.isFrontFacing)
            {
                _webcamDeviceBack ??= device;
            }
        }

        _activeWebcamDevice = _webcamDeviceBack ?? _webcamDeviceFront;
    }

    private IEnumerator SetCameraTexture()
    {
        IsReady = false;
        WebcamTexture?.Stop();
        WebcamTexture = new WebCamTexture(_activeWebcamDevice.Value.name);
        _cameraDisplay.texture = WebcamTexture;
        WebcamTexture.Play();

        yield return new WaitUntil(() => WebcamTexture.isPlaying && WebcamTexture.width > 100);

        var rotation = new Vector3(0f, 0f, -WebcamTexture.videoRotationAngle);
        _cameraDisplay.rectTransform.localEulerAngles = rotation;
        float videoRatio = (float)WebcamTexture.width / WebcamTexture.height;
        _aspectRatioFitter.aspectRatio = videoRatio;
        var scale = _cameraDisplay.rectTransform.localScale;
        if (_activeWebcamDevice.Value.isFrontFacing)
            scale.x = -Mathf.Abs(scale.x);
        else
            scale.x = Mathf.Abs(scale.x);
        _cameraDisplay.rectTransform.localScale = scale;

        IsReady = true;
    }





#if UNITY_IOS
    private bool CheckPermissionAndRaiseCallbackIfGranted(UserAuthorization authenticationType,
        Action authenticationGrantedAction)
    {
        if (Application.HasUserAuthorization(authenticationType))
        {
            if (authenticationGrantedAction != null)
                authenticationGrantedAction();

            return true;
        }

        return false;
    }

    private IEnumerator AskForPermissionIfRequired(UserAuthorization authenticationType,
        Action authenticationGrantedAction)
    {
        if (!CheckPermissionAndRaiseCallbackIfGranted(authenticationType, authenticationGrantedAction))
        {
            yield return Application.RequestUserAuthorization(authenticationType);
            if (!CheckPermissionAndRaiseCallbackIfGranted(authenticationType, authenticationGrantedAction))
                Debug.LogWarning($"Permission {authenticationType} Denied");
        }
    }

#elif UNITY_ANDROID
    private void PermissionCallbacksPermissionGranted(string permissionName)
    {
        StartCoroutine(DelayedCameraInitialization());
    }

    private IEnumerator DelayedCameraInitialization()
    {
        yield return null;
        yield return InitializeCamera();
    }

    private void PermissionCallbacksPermissionDenied(string permissionName)
    {
        Debug.LogWarning($"Permission {permissionName} Denied");
    }

    private void AskCameraPermission()
    {
        var callbacks = new PermissionCallbacks();
        callbacks.PermissionDenied += PermissionCallbacksPermissionDenied;
        callbacks.PermissionGranted += PermissionCallbacksPermissionGranted;
        Permission.RequestUserPermission(Permission.Camera, callbacks);
    }
#endif
}