using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStates
{
    private static PlayerStates m_states;

    public static PlayerStates Instance
    {
        get
        {
            if (m_states == default)
            {
                m_states = new PlayerStates();
            }

            return m_states;
        }
    }

    #region TODO:Turn to ScriptObject for Player Setting
    public Vector2 MoveDir;
    public float Speed;
    public float JumpPower = 20;
    #endregion
   
}