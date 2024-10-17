using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpController : MonoBehaviour
{
    [SerializeField] float speed = 4f;
    [SerializeField] float destroyPosition = -10.0f;

    void Update()
    {
        transform.Translate(Vector3.down * Time.deltaTime * speed);

        if (transform.position.y < destroyPosition)
        {
            Destroy(gameObject);
        }
    }
}
