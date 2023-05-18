using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public Vector2 teleportPosition;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        collision.gameObject.transform.position = teleportPosition;
    }
}
