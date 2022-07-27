using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float Speed;
    public float LifeTime;

    [HideInInspector]
    public Vector3 Forward;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LifeTimer());
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Forward * Speed * Time.deltaTime;
    }

    private IEnumerator LifeTimer()
    {
        yield return new WaitForSeconds(LifeTime);
        Destroy(gameObject);
    }
}
