using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private CameraFollow _cameraFollow;
    private ViewHandler _viewHandler;
    private AudioManager _audioManager;

    private ViewHandler.Views _currentView;

    public enum State
    {
        SetupActive, Active, SetupTransition, Transition,
    }
    
    public State GameState { get; private set; }

    void Awake()
    {
        _cameraFollow = GameObject.Find("Main Camera").GetComponent<CameraFollow>();
        _viewHandler = GameObject.Find("Camera Views").GetComponent<ViewHandler>();
        _audioManager = GameObject.Find("Audio Manager").GetComponent<AudioManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        GameState = State.Active;

        _currentView = ViewHandler.Views.Side;
    }

    // Update is called once per frame
    void Update()
    {
        switch (GameState)
        {
            case State.SetupActive:
                _audioManager.StopHighPassFilter();

                Time.timeScale = 1;
                GameState = State.Active;
                break;
            case State.Active:
                if (_currentView != _viewHandler.CurrentView)
                {
                    SetToTransition();
                    _currentView = _viewHandler.CurrentView;
                }
                break;
            case State.SetupTransition:
                _audioManager.StartHighPassFilter(_cameraFollow.ChangeViewSpeed);

                switch (_currentView)
                {
                    case ViewHandler.Views.Side:
                        _audioManager.PlaySideBgm();
                        break;
                    case ViewHandler.Views.Front:
                        _audioManager.PlayFrontBgm();
                        break;
                    case ViewHandler.Views.Top:
                        _audioManager.PlayTopBgm();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                Time.timeScale = 0.1f;
                GameState = State.Transition;
                break;
            case State.Transition:
                if (_cameraFollow.CameraState == CameraFollow.State.Following)
                    SetToActive();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void SetToActive()
    {
        GameState = State.SetupActive;
    }

    public void SetToTransition()
    {
        GameState = State.SetupTransition;
    }
}
