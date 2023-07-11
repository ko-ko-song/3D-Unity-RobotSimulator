using System.Collections;
using System.Collections.Generic;
using UnityEngine;



class VariableValue
{
    public string type;
    public int intVar;
    public string stringVar;
    public bool boolVar;
    public List<string> listVar;

    public VariableValue(string type)
    {
        this.type = type;    
    }


}
