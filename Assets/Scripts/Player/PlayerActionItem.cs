using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActionItem
{
    public enum InputAction { Move, Jump, Dash, DashDirection };
    public InputAction action;
    public float timeStamp;

    public static float timeBeforeActionsExpire = 2f;

    //Constructor
    public PlayerActionItem(InputAction playerAction, float stamp)
    {
        action = playerAction;
        timeStamp = stamp;
    }

    //returns true if this action hasn't expired due to the timestamp
    public bool CheckIfValid()
    {
        bool returnValue = false;
        if (timeStamp + timeBeforeActionsExpire >= Time.time)
        {
            returnValue = true;
        }
        return returnValue;
    }
}
