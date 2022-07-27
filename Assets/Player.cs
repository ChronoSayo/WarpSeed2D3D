using System;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Vector3 = UnityEngine.Vector3;

public class Player : MonoBehaviour
{
    public Transform Bullet;
    public Transform Views;
    public Transform CameraPoint;
    public Camera Camera;

    private ViewHandler.Views _currentView;
    private ViewHandler _viewHandler;
    private int _speed;
    private bool _reversedTopControls;

    public Quaternion LastRotation { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        _speed = 20;
        _viewHandler = Views.GetComponent<ViewHandler>();
        _currentView = _viewHandler.CurrentView;
        transform.forward = Vector3.right;
        LastRotation = transform.rotation;
        _reversedTopControls = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(_currentView != _viewHandler.CurrentView)
            ResetPosition();

        switch (_viewHandler.CurrentView)
        {
            case ViewHandler.Views.Side:
                SideControls();
                break;
            case ViewHandler.Views.Front:
                FrontControls();
                break;
            case ViewHandler.Views.Top:
                TopControls();
                if (Helper.CheckPlayerAllowedDirection(transform.forward))
                    LastRotation = transform.rotation;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        Shoot();

        Restart();
        Quit();
    }

    private void Restart()
    {
        if (Input.GetKeyDown(KeyCode.Return))
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    private void Quit()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }

    private void Shoot()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Bullet bullet = Bullet.GetComponent<Bullet>();
            bullet.Forward = transform.forward;
            Instantiate(bullet, transform.position + transform.forward * (transform.localScale.x * 0.5f),
                Quaternion.identity);
        }
    }

    private void SideControls()
    {
        Vector3 upLeftLimit = ScreenLimit(true);
        Vector3 downRightLimit = ScreenLimit(false);

        UpDownControls(upLeftLimit.y, downRightLimit.y);

        if (Input.GetKey(KeyCode.A))
            MoveTowardDirection(Vector3.left, true);
        else if (Input.GetKey(KeyCode.D))
            MoveTowardDirection(Vector3.right, true);
    }

    private void FrontControls()
    {
        Vector3 upLeftLimit = ScreenLimit(true);
        Vector3 downRightLimit = ScreenLimit(false);

        UpDownControls(upLeftLimit.y, downRightLimit.y);
        
        if (transform.forward == Vector3.right)
        {
            LeftRightControls(upLeftLimit.z, downRightLimit.z);

            if (Input.GetKey(KeyCode.LeftShift))
                MoveTowardDirection(Vector3.right);
            else if (Input.GetKey(KeyCode.LeftControl))
                MoveTowardDirection(Vector3.left);
        }
        else
        {
            if (Input.GetKey(KeyCode.A) && transform.position.z > upLeftLimit.z)
                MoveTowardDirection(Vector3.back);
            else if (Input.GetKey(KeyCode.D) && transform.position.z < downRightLimit.z)
                MoveTowardDirection(Vector3.forward);

            if (Input.GetKey(KeyCode.LeftShift))
                MoveTowardDirection(Vector3.left);
            else if (Input.GetKey(KeyCode.LeftControl))
                MoveTowardDirection(Vector3.right);
        }
    }

    private void TopControls()
    {
        Vector3 upLeftLimit = ScreenLimit(true);
        Vector3 downRightLimit = ScreenLimit(false);

        if (_reversedTopControls)
        {
            if (Input.GetKey(KeyCode.A) && transform.position.z > upLeftLimit.z)
                MoveTowardDirection(Vector3.back, true);
            else if (Input.GetKey(KeyCode.D) && transform.position.z < downRightLimit.z)
                MoveTowardDirection(Vector3.forward, true);

            if (Input.GetKey(KeyCode.W))
                MoveTowardDirection(Vector3.left, true);
            else if (Input.GetKey(KeyCode.S))
                MoveTowardDirection(Vector3.right, true);
        }
        else
        {
            if (Input.GetKey(KeyCode.A) && transform.position.z < upLeftLimit.z)
                MoveTowardDirection(Vector3.forward, true);
            else if (Input.GetKey(KeyCode.D) && transform.position.z > downRightLimit.z)
                MoveTowardDirection(Vector3.back, true);

            if (Input.GetKey(KeyCode.W))
                MoveTowardDirection(Vector3.right, true);
            else if (Input.GetKey(KeyCode.S))
                MoveTowardDirection(Vector3.left, true);
        }

    }

    private void ResetPosition()
    {
        _currentView = _viewHandler.CurrentView;
        Vector3 targetPos;
        CameraFollow cameraFollow = Camera.GetComponent<CameraFollow>();
        Quaternion targetRot = LastRotation;
        switch (_currentView)
        {
            case ViewHandler.Views.Side:
                targetPos = new Vector3(transform.position.x, transform.position.y, CameraPoint.position.z);
                break;
            case ViewHandler.Views.Front:
                targetPos = transform.position;
                break;
            case ViewHandler.Views.Top:
                targetPos = CameraPoint.position;
                targetRot = transform.rotation;
                _reversedTopControls = CameraPoint.GetComponent<CameraPoint>().ReversedTopCamera;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        StartCoroutine(Helper.LerpToPosition(transform, targetPos, targetRot, cameraFollow.ChangeViewSpeed));
    }

    private Vector3 ScreenLimit(bool upLeft)
    {
        float difference;
        switch (_currentView)
        {
            case ViewHandler.Views.Side:
                difference = transform.position.z - Camera.transform.position.z;
                break;
            case ViewHandler.Views.Front:
                difference = transform.position.x - Camera.transform.position.x;
                break;
            case ViewHandler.Views.Top:
                difference = transform.position.y - Camera.transform.position.y;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        var distFromCamera = Math.Abs(difference);
        if (upLeft)
            return Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, distFromCamera));

        return Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, distFromCamera));
    }

    private void UpDownControls(float upLimit, float downLimit)
    {
        if (Input.GetKey(KeyCode.W) && transform.position.y < upLimit)
            MoveTowardDirection(Vector3.up);
        else if (Input.GetKey(KeyCode.S) && transform.position.y > downLimit)
            MoveTowardDirection(Vector3.down);
    }

    private void LeftRightControls(float upLimit, float downLimit)
    {
        if (Input.GetKey(KeyCode.A) && transform.position.z < upLimit)
            MoveTowardDirection(Vector3.forward);
        else if (Input.GetKey(KeyCode.D) && transform.position.z > downLimit)
            MoveTowardDirection(Vector3.back);
    }

    private void MoveTowardDirection(Vector3 dir, bool turnTowardDir = false)
    {
        transform.position += dir * _speed * Time.deltaTime;
        if (turnTowardDir && transform.forward != dir)
            transform.forward = dir;
    }
}
