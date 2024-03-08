using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputReader : MonoBehaviour
{
    private InputSource m_inputSource;

    private void Awake()
    {
        m_inputSource = new InputSource();
    }

    private void OnEnable()
    {
        m_inputSource.Enable();
    }

    private void OnDisable()
    {
        m_inputSource.Disable();
    }

    private void OnDestroy()
    {
        m_inputSource.Dispose();
    }

    private void Update()
    {
        PlayerStates.Instance.MoveDir = m_inputSource.Player.Move.ReadValue<Vector2>();
    }
}
