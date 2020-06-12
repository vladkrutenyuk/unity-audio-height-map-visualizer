using UnityEngine;
  
public class AudioVisualizer : MonoBehaviour  
{   
    [SerializeField] 
    private AudioSource _audioSource;
    [SerializeField] 
    private LineRenderer _lineRenderer; 
    [SerializeField] 
    private GameObject _trailPrefab;
    
    [SerializeField] 
    [Range(6, 13)] 
    private int samplesSizePower = 4;
    [SerializeField] 
    [Range(0f, 100f)] 
    private float power = 1f;
    [SerializeField] 
    private float scale = 100f;
    [SerializeField] 
    private int length = 100;
    [SerializeField] 
    [Range(1, 100)]  
    private int trailsDivider = 50;
    
    private GameObject[] _audioTrailObjects;
    private Vector3 _samplePointPosition = new Vector3();
    private float[] _samples;
    private float _samplesSize;
  
    private void Start()
    {
        _samplesSize = Mathf.Pow(2, samplesSizePower);
        _samples = new float[(int)_samplesSize];

        _lineRenderer.positionCount = Mathf.Clamp(_samples.Length, 0, length); 

        _audioSource.Play();

        _audioTrailObjects = new GameObject[length / trailsDivider];
        SpawnTrails();
    }
  
    private void Update()
    {
        _audioSource.GetSpectrumData(_samples, 0, FFTWindow.BlackmanHarris);  

        for(int i = 0; i < length; i++)  
        {
            _samplePointPosition.Set
            (
                0, 
                Mathf.Clamp(_samples[i]  * power, 0, 50), 
                scale / (_samples.Length - 1) * i
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
                new Vector3(0, 0, scale / (_samples.Length - 1) * i * trailsDivider),
                Quaternion.identity
                );
        }
    }

    private void SetTrailsPosition()
    {
        for (int i = 0; i < _audioTrailObjects.Length; i++)
        {
            float height = Mathf.Clamp(_samples[i * trailsDivider] * power, 0, 50); 
            _audioTrailObjects[i].transform.position = new Vector3(
                -_audioSource.time, height, _audioTrailObjects[i].transform.position.z);
        }
    }
}
