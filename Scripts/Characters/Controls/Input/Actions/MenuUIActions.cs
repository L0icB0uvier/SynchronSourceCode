using System.Collections;
using System.Collections.Generic;
using InControl;
using UnityEngine;

public class MenuUIActions : PlayerActionSet
{
    public PlayerAction Pause;

    public MenuUIActions()
    {
        Pause = CreatePlayerAction("Pause");
    }
}
