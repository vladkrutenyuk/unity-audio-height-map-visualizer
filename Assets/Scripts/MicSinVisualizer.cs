using UnityEngine;
using UnityEngine.Audio;

public class MicSinVisualizer : MonoBehaviour
{
    [SerializeField] 
    private LineRenderer _lineRenderer;
    [SerializeField] 
    private AudioSource _audioSource;
    [SerializeField] 
    private AudioMixerGroup _micAudioMixGroup;
    
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
    [Range(0f, 0.1f)] 
    private float _sinWaveShiftSpeed;
    [SerializeField]
    private float _sinWaveFreq;
    [SerializeField] 
    private float _sinWaveAmplitude;
    
    private Vector3 _samplePointPosition = new Vector3();
    private float[] _samples;
    private float _samplesSize;

    private float _currentSinWaveTime;

    private void Awake()
    {
        if (Microphone.devices.Length > 0)
        {
            foreach (var mic in Microphone.devices)
            {
                Debug.Log(mic);
            }
            
            var micDevice = Microphone.devices[0];
            _audioSource.clip = Microphone.Start(micDevice, true, 100, 44100);
            _audioSource.outputAudioMixerGroup = _micAudioMixGroup;
        }
        else
        {
            Debug.LogWarning("Microphone was not found!");
        }
    }

    private void Start()
    {
        _samplesSize = Mathf.Pow(2, _samplesSizePower);
        _samples = new float[(int)_samplesSize];

        _lineRenderer.positionCount = Mathf.Clamp(_samples.Length, 0, _length);

        _audioSource.Play();
    }
    
    private void Update()
    {
        _audioSource.GetSpectrumData(_samples, 0, FFTWindow.BlackmanHarris);

        float currentAudioAmplitude = GetCurrentAudioAmplitude(_samples) * _power;
        
        for(int i = 0; i < _length; i++)
        {
            var z = _scale / (_samples.Length - 1) * i * 0.1f;
            float amplitude = Mathf.Lerp(
                _sinWaveAmplitude + currentAudioAmplitude,
                _sinWaveAmplitude,
                Mathf.Abs((float)_length / 2 - i) / ((float)_length / 2)
            );

            var y = SinWave(
                i, _sinWaveShiftSpeed * Time.deltaTime,
                _sinWaveFreq,
                amplitude
            );

            _samplePointPosition.Set(0, y, z);
            _samplePointPosition += transform.position;
  
            _lineRenderer.SetPosition(i, _samplePointPosition);  
        }
    }

    private float GetCurrentAudioAmplitude(float[] samplesData)
    {
        float result = 0;

        for (int i = 0; i < samplesData.Length; i++)
        {
            result += samplesData[i];
        }

        return result;
    }

    private float SinWave(float time, float shift, float freq, float amplitude)
    {
        _currentSinWaveTime += shift;
        return Mathf.Sin(freq * time + _currentSinWaveTime) * amplitude;
    }
}
