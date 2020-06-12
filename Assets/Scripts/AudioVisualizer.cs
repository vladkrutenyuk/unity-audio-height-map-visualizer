using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioVisualizer : MonoBehaviour
{
    [SerializeField] 
    private bool _useMicrophone;
    
    [Header("Refs")]
    [Space(10)]
    
    [SerializeField] 
    private AudioSource _audioSource;
    [SerializeField] 
    private LineRenderer _lineRenderer;

    [SerializeField] 
    private GameObject _musicTrailPrefab;
    [SerializeField] 
    private GameObject _micTrailPrefab;

    [SerializeField] 
    private AudioMixerGroup _musicMixer;
    [SerializeField] 
    private AudioMixerGroup _micMixer;
    
    [Header("Settings")]
    
    [SerializeField] 
    [Range(6, 13)] 
    private int _samplesSizePower = 13;
    [SerializeField] 
    [Range(0f, 100f)] 
    private float _power = 55;
    [SerializeField] 
    private float _scale = 800;
    [SerializeField] 
    private int _length = 500;
    [SerializeField] 
    [Range(1, 100)]  
    private int _trailsDivider = 1;
    
    // -------------------------------------
    
    private GameObject _trailPrefab;
    private GameObject[] _audioTrailObjects;
    private Vector3 _samplePointPosition = new Vector3();
    private float[] _samples;
    private float _samplesSize;
    
    private string _mic;

    private void Awake()
    {
        if (Microphone.devices.Length == 0)
        {
            _useMicrophone = false;
            Debug.LogWarning("Microphone was not found!");
        }
        
        if (_useMicrophone)
        {
            _trailPrefab = _micTrailPrefab;

            _mic = Microphone.devices[0];
            _audioSource.clip = Microphone.Start(_mic, true, 100, 44100);
            _audioSource.outputAudioMixerGroup = _micMixer; 
        }
        else
        {
            _trailPrefab = _musicTrailPrefab;
            
            _audioSource.outputAudioMixerGroup = _musicMixer;
        }
    }

    private void Start()
    {
        _samplesSize = Mathf.Pow(2, _samplesSizePower);
        _samples = new float[(int)_samplesSize];

        _lineRenderer.positionCount = Mathf.Clamp(_samples.Length, 0, _length); 

        _audioSource.Play();

        _audioTrailObjects = new GameObject[_length / _trailsDivider];
        SpawnTrails();
    }
  
    private void Update()
    {
        _audioSource.GetSpectrumData(_samples, 0, FFTWindow.BlackmanHarris);  

        for(int i = 0; i < _length; i++)  
        {
            _samplePointPosition.Set
            (
                0, 
                Mathf.Clamp(_samples[i]  * _power, 0, 50), 
                _scale / (_samples.Length - 1) * i
            );
  
            _lineRenderer.SetPosition(i, _samplePointPosition);  
        }

        SetTrailsPosition();
    }

    private void SpawnTrails()
    {
        for (int i = 0; i < _audioTrailObjects.Length; i++)
        {
            _audioTrailObjects[i] = Instantiate(
                _trailPrefab, 
                new Vector3(0, 0, _scale / (_samples.Length - 1) * i * _trailsDivider),
                Quaternion.identity
                );
        }
    }

    private void SetTrailsPosition()
    {
        for (int i = 0; i < _audioTrailObjects.Length; i++)
        {
            float height = Mathf.Clamp(_samples[i * _trailsDivider] * _power, 0, 50); 
            _audioTrailObjects[i].transform.position = new Vector3(
                -_audioSource.time, height, _audioTrailObjects[i].transform.position.z);
        }
    }
}
