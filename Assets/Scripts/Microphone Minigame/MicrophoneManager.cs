using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Android;

public class MicrophoneManager : MonoBehaviour
{
    public bool IsRecording { get; private set; }
    public float CurrentAvgVolume { get; private set; }
    public float FinalAvgVolume { get; private set; }

    [SerializeField] private float _volumeMult;

    private AudioClip _recordedClip;
    private string _micName;
    private const int SAMPLE_RATE = 44100;

    private void Start()
    {
#if UNITY_IOS
        StartCoroutine(AskForPermissionIfRequired(UserAuthorization.Microphone, InitMicrophone));
        return;
#elif UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            AskMicPermission();
            return;
        }
#endif
        InitMicrophone();
    }

    private void InitMicrophone()
    {
        if (Microphone.devices.Length == 0)
            throw new Exception("Microphone not found");

        _micName = Microphone.devices[0];
    }

    private void Update()
    {
        if (!IsRecording) return;
        CurrentAvgVolume = GetAverageVolume();
        Debug.Log($"Volume: {CurrentAvgVolume:F2}");
    }


    public void StartRecording()
    {
        if (IsRecording) return;
        if (string.IsNullOrEmpty(_micName)) return;

        IsRecording = true;
        _recordedClip = Microphone.Start(_micName, true, 10, SAMPLE_RATE); // Graba un bucle de 10 segundos
    }

    public void StopRecording()
    {
        if (!IsRecording) return;

        IsRecording = false;
        Microphone.End(_micName);
        FinalAvgVolume = GetAverageVolume();
    }

    private float GetAverageVolume()
    {
        if (_recordedClip == null) return 0f;

        float[] samples = new float[_recordedClip.samples];
        _recordedClip.GetData(samples, 0);

        // Calcula la amplitud promedio
        float sum = 0f;
        foreach (var sample in samples)
        {
            sum += Mathf.Abs(sample);
        }

        return Mathf.Clamp01(sum * _volumeMult / samples.Length);
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
        StartCoroutine(DelayedMicInitialization());
    }

    private IEnumerator DelayedMicInitialization()
    {
        yield return null;
    }

    private void PermissionCallbacksPermissionDenied(string permissionName)
    {
        Debug.LogWarning($"Permission {permissionName} Denied");
    }

    private void AskMicPermission()
    {
        var callbacks = new PermissionCallbacks();
        callbacks.PermissionDenied += PermissionCallbacksPermissionDenied;
        callbacks.PermissionGranted += PermissionCallbacksPermissionGranted;
        Permission.RequestUserPermission(Permission.Microphone, callbacks);
    }
#endif
}
