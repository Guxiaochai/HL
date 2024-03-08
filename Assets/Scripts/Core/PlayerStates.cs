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

    public Vector2 MoveDir;
    public float Speed;
}