using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager current;

    private float horizontalValue;
    private float verticalValue;
    
    public bool useAxisValues = true;

    public float HorizontalValue 
    {
        get { return horizontalValue; }
    }

    public float VerticalValue 
    {
        get { return verticalValue; }
    }

    private Dictionary<InputMapping, List<KeyCode>> controlMappings = new Dictionary<InputMapping, List<KeyCode>>();

    private Dictionary<InputMapping, bool> controlKey = new Dictionary<InputMapping, bool>();

    private void Awake()
    {
        if (current != null) Debug.LogWarning("Oops! it looks like there might already be a GameManager in this scene!");
        current = this;

        //Assign all key inputs, later on we can 
        controlMappings.Add(InputMapping.Left, new List<KeyCode> { KeyCode.A, KeyCode.LeftArrow });
        controlMappings.Add(InputMapping.Right, new List<KeyCode> { KeyCode.D, KeyCode.RightArrow });
        controlMappings.Add(InputMapping.Up, new List<KeyCode> { KeyCode.W, KeyCode.UpArrow });
        controlMappings.Add(InputMapping.Down, new List<KeyCode> { KeyCode.S, KeyCode.DownArrow });
        controlMappings.Add(InputMapping.Cancel, new List<KeyCode> { KeyCode.Escape });
        controlMappings.Add(InputMapping.Jump, new List<KeyCode> { KeyCode.Space });
        controlMappings.Add(InputMapping.Interact, new List<KeyCode> { KeyCode.E });

        foreach (InputMapping input in (InputMapping[]) Enum.GetValues(typeof(InputMapping))) 
        {
            controlKey.Add(input, false);
        }
    }

    private void Update()
    {
        if (useAxisValues)
        {
            horizontalValue = Math.Abs((float)Input.GetAxisRaw("Horizontal"));
            verticalValue = Math.Abs((float)Input.GetAxisRaw("Vertical"));
        }
        else 
        {
            horizontalValue = 0f;
            verticalValue = 0f;
        }

        bool keyDownRegistered;
        bool keyRegistered;

        foreach (KeyValuePair<InputMapping, List<KeyCode>> control in controlMappings) 
        {
            keyDownRegistered = false;
            keyRegistered = false;

            controlKey[control.Key] = false;

            foreach (KeyCode key in control.Value) 
            {
                if (!keyDownRegistered && Input.GetKeyDown(key))
                {
                    AnyKeyDown();
                    InputMappingKeyDown(control.Key);
                    keyDownRegistered = true;
                }

                if (!keyRegistered && Input.GetKey(key)) 
                {
                    controlKey[control.Key] = true;
                    keyRegistered = true;
                }

                if (keyRegistered && keyDownRegistered) break; //I don't think I need this?
            }
        }
    }

    private void FixedUpdate()
    {
        if (GameManager.current != null && GameManager.current.Player != null) 
        {
            Player platformerPlayer = GameManager.current.Player;
            VerticalPlatform verticalPlatform = GameManager.current.verticalPlatform;

            if (UIManager.current.UIState == UIState.None)
            {
                //Move values
                float currentHorizontalValue = controlKey[InputMapping.Left] && controlKey[InputMapping.Right]
                    ? 0f
                    : controlKey[InputMapping.Left]
                    ? -horizontalValue
                    : controlKey[InputMapping.Right]
                    ? horizontalValue
                    : 0f;

                float currentVerticalValue = controlKey[InputMapping.Up] && controlKey[InputMapping.Down]
                    ? 0f
                    : controlKey[InputMapping.Up]
                    ? verticalValue
                    : controlKey[InputMapping.Down]
                    ? -verticalValue
                    : 0f;

                //Move Input
                platformerPlayer.Move(new Vector2(currentHorizontalValue, currentVerticalValue));
                if (platformerPlayer.Crouch != controlKey[InputMapping.Down]) platformerPlayer.Crouch = controlKey[InputMapping.Down];
                if (platformerPlayer.JumpHold != controlKey[InputMapping.Jump]) platformerPlayer.JumpHold = controlKey[InputMapping.Jump];

                //Drop through platform input
                if (verticalPlatform != null && verticalPlatform.DropThroughPlatform != controlKey[InputMapping.Down]) verticalPlatform.DropThroughPlatform = controlKey[InputMapping.Down];

            }
            else 
            {
                platformerPlayer.Move(Vector2.zero);
            }
        }
    }

    public KeyCode[] GetKeyCodesForInputMapping(InputMapping inputMapping) 
    {
        return controlMappings[inputMapping].ToArray();
    }

    //This will need a slight rearrange when we implement other player types/genres
    private void InputMappingKeyDown(InputMapping inputMapping) 
    {
        switch (inputMapping) 
        {
            case InputMapping.Left:
                break;

            case InputMapping.Right:
                break;

            case InputMapping.Up:
                break;

            case InputMapping.Down:
                break;

            case InputMapping.Jump:
                if(UIManager.current.UIState == UIState.None 
                    && GameManager.current != null 
                    && GameManager.current.Player != null) GameManager.current.Player.Jump();

                break;

            case InputMapping.Cancel:
                PauseGame();
                break;

            case InputMapping.Interact:
                if (UIManager.current.UIState == UIState.None
                    && GameManager.current != null
                    && GameManager.current.Player != null) GameManager.current.Player.Interact();
                break;
        }
    }

    private void AnyKeyDown() 
    {
        if (DialogueManager.current != null) 
        {
            DialogueManager.current.SkipDialogueTextEffect();
        }
    }

    private void PauseGame()
    {
        //Set UI and stuff
        if (UIManager.current != null && GameManager.current != null)
        {
            GameManager.current.SetSelectedSaveOption(0);
            UIManager.current.ToggleContexts(UIContextType.PauseMenu);
            UIManager.current.SetContextsActive(true, UIContextType.PauseMain);
            UIManager.current.SetContextsActive(false, UIContextType.LoadMenu, UIContextType.SaveMenu, UIContextType.SaveSelection);
        }
    }

}

public enum InputMapping 
{
    Left,
    Right,
    Up,
    Down,
    Jump,
    Cancel,
    Interact
}

public enum GameLevelType 
{
    None,
    Platformer
}