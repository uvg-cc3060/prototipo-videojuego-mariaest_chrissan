using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{

    private Vector3 direction = Vector3.zero;
    private Vector3 initialPosition = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += direction * 0.4f;
        if((transform.position - initialPosition).magnitude > 100)
        {
            Destroy(gameObject);
        }
    }


    public void SetDirection(Vector3 direction)
    {
        this.direction = direction;
    }
}
