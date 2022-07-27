using System;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float FollowSpeed = 5;
    public float ChangeViewSpeed = 5;

    public enum State
    {
        Following, Change, Changing
    }
    
    public Transform Player;
    public Transform Follow;
    public Transform View;
    public Transform CameraPoint;

    private List<Transform> _views;
    private ViewHandler _viewHandler;
    private ViewHandler.Views _viewInCamera;
    private Vector3 _viewPosition;
    private State _state;
    private float _distance;

    public State CameraState => _state;

    // Start is called before the first frame update
    void Start()
    {
        _views = new List<Transform>();
        foreach (Transform child in CameraPoint)
            _views.Add(child);

        _viewHandler = View.GetComponent<ViewHandler>();
        _viewInCamera = _viewHandler.CurrentView;
        _viewPosition = GetViewPosition(_viewInCamera.ToString());

        _state = State.Following;
    }

    // Update is called once per frame
    void Update()
    {
        if (_viewInCamera != _viewHandler.CurrentView)
            _state = State.Change;

        switch (_state)
        {
            case State.Following:
                FollowObject();
                break;
            case State.Change:
                _viewInCamera = _viewHandler.CurrentView;
                string currentViewName = _viewInCamera.ToString();
                _viewPosition = GetViewPosition(currentViewName);
                Quaternion viewRotation;
                switch (_viewInCamera)
                {
                    case ViewHandler.Views.Side:
                        viewRotation = GetViewRotation(currentViewName);
                        break;
                    case ViewHandler.Views.Front:
                        viewRotation = Player.GetComponent<Player>().LastRotation;
                        break;
                    case ViewHandler.Views.Top:
                        viewRotation = GetViewRotation(currentViewName);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                StartCoroutine(Helper.LerpToPosition(transform, _viewPosition, viewRotation, ChangeViewSpeed));
                _distance = CameraPoint.GetComponent<CameraPoint>().Distance;
                if (Player.forward == Vector3.right)
                    _distance = -_distance;

                _state = State.Changing;
                break;
            case State.Changing:
                if (transform.position == _viewPosition)
                    _state = State.Following;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

    }



    private void FollowObject()
    {
        switch (_viewHandler.CurrentView)
        {
            case ViewHandler.Views.Side:
            case ViewHandler.Views.Top:
                transform.position = Vector3.Lerp(transform.position, 
                    new Vector3(Follow.position.x, transform.position.y, transform.position.z),
                    FollowSpeed * Time.deltaTime);
                break;
            case ViewHandler.Views.Front:
                transform.position = Vector3.Lerp(transform.position,
                    CameraPoint.GetComponent<CameraPoint>().FrontPosition, FollowSpeed * Time.deltaTime);
                break;
        }
    }

    private Vector3 GetViewPosition(string viewName)
    {
        return GetView(viewName).position;
    }

    private Quaternion GetViewRotation(string viewName)
    {
        return GetView(viewName).rotation;
    }

    private Transform GetView(string viewName)
    {
        return _views.Find(x => x.name == viewName);
    }
}
