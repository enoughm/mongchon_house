using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputManager
{
    public Action<Define.KeyEvent> KeyboardAction = null;
    public Action<Define.MouseEvent> MouseAction = null;
    private bool _pressed = false;
    
    public void OnUpdate()
    {
        //뉴 인풋시스템 쓸지 올드 인풋시스템 쓸지 체크하세요.
        OnUpdate_OldInputSystem();
        //OnUpdate_NewInputSystem();
    }

    private void OnUpdate_OldInputSystem()
    {
        if(Input.anyKey)
            KeyboardAction?.Invoke(Define.KeyEvent.Any);
        if(Input.GetKeyUp(KeyCode.Escape))
            KeyboardAction?.Invoke(Define.KeyEvent.Escape);

        if (!EventSystem.current.IsPointerOverGameObject() || MouseAction == null)
            return;

        if (Input.GetMouseButton(0))
        {
            MouseAction?.Invoke(Define.MouseEvent.Press);
            _pressed = true;
        }
        else
        {
            if (_pressed)
                MouseAction?.Invoke(Define.MouseEvent.Click);
            _pressed = false;
        }
            
    }

    private void OnUpdate_NewInputSystem()
    {
        if (Keyboard.current.anyKey.isPressed)
            KeyboardAction?.Invoke(Define.KeyEvent.Any);
        //Keyboard입력
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            KeyboardAction?.Invoke(Define.KeyEvent.Escape);
        //Mouse입력
        if (!EventSystem.current.IsPointerOverGameObject())
            return;
        if (Mouse.current.leftButton.wasPressedThisFrame)
            MouseAction?.Invoke(Define.MouseEvent.Down);
        else if (Mouse.current.leftButton.isPressed)
            MouseAction?.Invoke(Define.MouseEvent.Press);
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
            MouseAction?.Invoke(Define.MouseEvent.Click);
    }

    public void Clear()
    {
        KeyboardAction = null;
        MouseAction = null;
    }
}