using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action 
{
    public string actionName;
    public List<string> actionArgs;

    public string ActionName { get => actionName; set => actionName = value; }

    public Action()
    {
        actionArgs = new List<string>();
    }

    
}
