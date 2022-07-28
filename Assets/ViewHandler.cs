using UnityEngine;

public class ViewHandler : MonoBehaviour
{
    public enum Views
    {
        Side, Front, Top
    }
    public Views CurrentView { get; private set; }

    private GameManager _gameManager;

    // Start is called before the first frame update
    void Start()
    {
        CurrentView = Views.Side;
        _gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(_gameManager.GameState != GameManager.State.Active)
            return;

        if (Input.GetKey(KeyCode.Alpha1))
            CurrentView = Views.Side;
        else if(Input.GetKey(KeyCode.Alpha2))
            CurrentView = Views.Front;
        else if (Input.GetKey(KeyCode.Alpha3))
            CurrentView = Views.Top;
    }
}
