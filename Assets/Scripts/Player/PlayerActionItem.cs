using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActionItem
{
    public enum InputAction { Move, Jump, Dash, DashDirection };
    public InputAction Action;
    public float Timestamp;

    public static float TimeBeforeActionsExpire = 2f;

    //Constructor
    public PlayerActionItem(InputAction playerAction, float stamp)
    {
        Action = playerAction;
        Timestamp = stamp;
    }

    //returns true if this action hasn't expired due to the timestamp
    public bool CheckIfValid()
    {
        bool returnValue = false;
        if (Timestamp + TimeBeforeActionsExpire >= Time.time)
        {
            returnValue = true;
        }
        return returnValue;
    }
}
