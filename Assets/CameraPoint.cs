using System.Net.Http.Headers;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class CameraPoint : MonoBehaviour
{
    public float Distance = 10;
    public float Offset;
    public Transform Player;

    private Transform _front, _top;
    private Vector3 _oldPlayerDirection;

    public Vector3 FrontPosition => _front.position;
    public bool ReversedTopCamera { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        _front = transform.Find("Front");
        _top = transform.Find("Top");
        _oldPlayerDirection = Player.forward;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Player.forward == Vector3.left ? 
            new Vector3(Player.position.x - Offset, transform.position.y, transform.position.z) : 
            new Vector3(Player.position.x + Offset, transform.position.y, transform.position.z);

        bool checkPlayerAllowedDirection = Helper.CheckPlayerAllowedDirection(Player.forward);
        if(checkPlayerAllowedDirection)
            _front.localPosition = Player.forward * -Distance;

        if (checkPlayerAllowedDirection && Player.forward != _oldPlayerDirection)
        {
            _top.Rotate(new Vector3(0, 0, 180));
            _oldPlayerDirection = Player.forward;
            ReversedTopCamera = Player.forward == Vector3.left;
        }
    }
}
