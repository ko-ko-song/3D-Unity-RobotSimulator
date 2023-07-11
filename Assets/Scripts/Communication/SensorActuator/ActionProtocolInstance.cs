using Defective.JSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class ActionProtocolInstance 
{
    public ActionProtocol actionProtocol;
    protected Dictionary<string, List<string>> variables;
    public Action actionInstance;
    
    public ActionProtocolInstance(ActionProtocol actionProtocol) 
    {
        this.actionProtocol = actionProtocol;
        this.variables = new Dictionary<string, List<string>>();
        this.actionInstance = new Action();
    }

    public abstract void bindVariables();

    public abstract void bindAction(string targetObjectId);

    public abstract JSONObject getResponseMessage();
    
    public abstract JSONObject getResultMessage(int result);

    public abstract JSONObject getResultMessage(int robotID, int result);

    public abstract string getProtocolType();
    
}
