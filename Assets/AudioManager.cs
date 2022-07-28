using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public int CutoffFrequencyStart = 2000;
    public float SongChangeSpeed = 5;

    /// <summary>
    /// 0: Side, 1: Front, 2: Top
    /// </summary>
    private List<AudioSource> _sources;
    private AudioHighPassFilter _highPass;
    private bool _highPassFilterEnabled;
    private bool _reverseCutoffFrequencySpeed;
    private float _transitionSpeed;
    private int _currentSong;

    private const float FrequencyDefault = 5000;

    // Start is called before the first frame update
    void Start()
    {
        _sources = new List<AudioSource>();
        foreach (AudioSource source in transform.GetComponents<AudioSource>())
        {
            if(_sources.Contains(source))
                continue;
            _sources.Add(source);
        }

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

        if (_sources[_currentSong].volume < 1)
        {
            for (int i = 0; i < _sources.Count; i++)
            {
                if(i == _currentSong)
                    _sources[i].volume += SongChangeSpeed * Time.deltaTime;
                else
                    _sources[i].volume -= SongChangeSpeed * Time.deltaTime;
            }
        }
    }

    public void PlaySideBgm()
    {
        _currentSong = 0;
    }
    public void PlayFrontBgm()
    {
        _currentSong = 1;
    }
    public void PlayTopBgm()
    {
        _currentSong = 2;
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
