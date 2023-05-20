using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : MonoBehaviour
{
    public Movement Movement
    {
        get => GenericNotImplementedError<Movement>.TryGet(movement, parentName);
        private set => movement = value;
    }
    public CollisionSenses CollisionSenses
    {
        get => GenericNotImplementedError<CollisionSenses>.TryGet(collisionSenses, parentName);
        private set => collisionSenses = value;
    }

    private Movement movement;
    private CollisionSenses collisionSenses;
    private string parentName;

    private void Awake()
    {
        Movement = GetComponentInChildren<Movement>();
        CollisionSenses = GetComponentInChildren<CollisionSenses>();
        parentName = transform.parent.name;
    }

    public void LogicUpdate()
    {
        Movement.LogicUpdate();
    }
}