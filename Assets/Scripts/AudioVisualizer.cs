using UnityEngine;
  
public class AudioVisualizer : MonoBehaviour  
{   
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private LineRenderer lineRenderer; 
    [SerializeField] private GameObject trailPrefab;
    [SerializeField] [Range(6, 13)] private int samplesSizePower = 4;
    [SerializeField] [Range(0f, 100f)] private float power = 1f;
    [SerializeField] private float scale = 100f;
    [SerializeField] private int length = 100;
    [SerializeField] [Range(1, 100)]  private int trailsDivider = 50;
    
    private GameObject[] audioTrailObjects;
    private Vector3 _samplePointPosition;
    private float[] _samples;
    private float _samplesSize;
  
    private void Start()
    {
        _samplesSize = Mathf.Pow(2, samplesSizePower);
        _samples = new float[(int)_samplesSize];

        lineRenderer.positionCount = Mathf.Clamp(_samples.Length, 0, length); 

        audioSource.Play();

        audioTrailObjects = new GameObject[length / trailsDivider];
        SpawnTrails();
    }
  
    private void Update()
    { 
        //lineRenderer.positionCount = Mathf.Clamp(_samples.Length, 0, length); 
        audioSource.GetSpectrumData(_samples, 0, FFTWindow.BlackmanHarris);  

        for(int i = 0; i < length; i++)  
        {  
            _samplePointPosition.Set
            (
                0, 
                Mathf.Clamp(_samples[i]  * power, 0, 50), 
                scale / (_samples.Length - 1) * i
            ); 
  
            lineRenderer.SetPosition(i, _samplePointPosition);  
        }

        SetTrailsPosition();
    }

    private void SpawnTrails()
    {
        for (int i = 0; i < audioTrailObjects.Length; i++)
        {
            audioTrailObjects[i] = Instantiate(
                trailPrefab, 
                new Vector3(0, 0, scale / (_samples.Length - 1) * i * trailsDivider),
                Quaternion.identity
                );
        }
    }

    private void SetTrailsPosition()
    {
        for (int i = 0; i < audioTrailObjects.Length; i++)
        {
            float height = Mathf.Clamp(_samples[i * trailsDivider]  * power, 0, 50); 
            audioTrailObjects[i].transform.position = new Vector3(
                -audioSource.time, height, audioTrailObjects[i].transform.position.z);
        }
    }
}
