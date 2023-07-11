using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Variable 
{
    public string type;
    public string value;

    public string getStringValue()
    {
        return value.ToString();
    }
    
    public int getIntValue()
    {
        return int.Parse(value);
    }
    
    public float getFloatValue()
    {
        return float.Parse(value);
    }

    public bool getBoolValue()
    {
        return bool.Parse(value);
    }
}
