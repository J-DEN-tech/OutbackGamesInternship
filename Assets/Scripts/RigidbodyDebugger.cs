using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyDebugger : MonoBehaviour
{
    public Rigidbody2D rigidBody;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(rigidBody.velocity.y);
    }
}
