using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public int CutoffFrequencyStart = 2000;
    
    private AudioSource _source;
    private AudioHighPassFilter _highPass;
    private bool _highPassFilterEnabled;
    private bool _reverseCutoffFrequencySpeed;
    private float _transitionSpeed;

    private const float FrequencyDefault = 5000;

    // Start is called before the first frame update
    void Start()
    {
        _source = GetComponent<AudioSource>();

        _highPass = GetComponent<AudioHighPassFilter>();
        _highPass.highpassResonanceQ = 5;
        _highPass.enabled = false;

        _highPassFilterEnabled = false;
        _reverseCutoffFrequencySpeed = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (_highPassFilterEnabled)
        {
            float cutoffSpeed = _transitionSpeed;
            if (!_reverseCutoffFrequencySpeed && _highPass.cutoffFrequency <= 1000)
                _reverseCutoffFrequencySpeed = true;
            if (_reverseCutoffFrequencySpeed && _highPass.cutoffFrequency > FrequencyDefault)
                _reverseCutoffFrequencySpeed = false;
            if (_reverseCutoffFrequencySpeed)
                cutoffSpeed *= -1;
            _highPass.cutoffFrequency -= cutoffSpeed;
        }
    }

    public void PlaySideBgm()
    {

    }
    public void PlayFrontBgm()
    {

    }
    public void PlayTopBgm()
    {

    }

    public void StartHighPassFilter(float transitionSpeed)
    {
        _transitionSpeed = (FrequencyDefault / transitionSpeed) / 100;
        _highPassFilterEnabled = _highPass.enabled = true;
        _highPass.cutoffFrequency = CutoffFrequencyStart;
        _reverseCutoffFrequencySpeed = false;
    }

    public void StopHighPassFilter()
    {
        _highPassFilterEnabled = _highPass.enabled = false;
        _highPass.cutoffFrequency = CutoffFrequencyStart;
    }
}
