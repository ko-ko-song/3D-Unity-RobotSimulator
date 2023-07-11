using Defective.JSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionProtocolJSONInstance : ActionProtocolInstance
{
    public JSONObject receivedMessage;

    public ActionProtocolJSONInstance(ActionProtocol actionProtocol, JSONObject receivedMessage) : base(actionProtocol)
    {
        this.actionProtocol = actionProtocol;
        this.receivedMessage = receivedMessage;
        this.variables = new Dictionary<string, List<string>>();
        this.actionInstance = new Action();
        this.bindVariables();
    }
    
    public override void bindVariables()
    {
        foreach (string key in this.actionProtocol.requestMessageTemplate.keys)
        {
            if (this.actionProtocol.requestMessageTemplate.GetField(key).isArray)
            {
                if (this.actionProtocol.requestMessageTemplate.GetField(key).list[0].getStringValue().Contains("$"))
                {
                    List<string> list = new List<string>();
                    foreach (JSONObject s in this.receivedMessage.GetField(key).list)
                    {
                        list.Add(s.getStringValue());
                    }
                    this.variables.Add(this.actionProtocol.requestMessageTemplate.GetField(key).list[0].getStringValue(), list);
                }
            }
            else if (this.actionProtocol.requestMessageTemplate.GetField(key).getStringValue().Contains("$"))
            {
                List<string> list = new List<string>();
                list.Add(this.receivedMessage.GetField(key).getStringValue());
                this.variables.Add(this.actionProtocol.requestMessageTemplate.GetField(key).getStringValue(), list);
            }
        }
    }
    public override void bindAction(string targetObjectId)
    {
        string name = actionProtocol.action.actionName;
        List<string> args = new List<string>();
        foreach (string arg in actionProtocol.action.actionArgs)
        {
            if (arg.Contains("$"))
            {
                foreach (string value in this.variables[arg])
                {
                    args.Add(value);
                }
            }
            else
            {
                args.Add(arg);
            }
        }
        if (!this.receivedMessage.keys.Contains("robotID") && !this.receivedMessage.keys.Contains("id") && !this.receivedMessage.keys.Contains("ID"))
        {
            this.actionInstance.actionArgs.Insert(0, targetObjectId);
        }

        this.actionInstance.actionName = name;
        this.actionInstance.actionArgs = args;

    }
    public override JSONObject getResponseMessage()
    {
        JSONObject responseMessage = new JSONObject();
        if (actionProtocol.responseMessageTemplate == null)
            return null;
        foreach (string key in actionProtocol.responseMessageTemplate.keys)
        {
            string value = actionProtocol.responseMessageTemplate.GetField(key).getStringValue();
            List<string> variableValues = new List<string>();
            if (value.Contains("$"))
            {
                foreach (string variableValue in this.variables[value])
                {
                    variableValues.Add(variableValue);
                }
            }
            else
            {
                variableValues.Add(value);
            }
            if (variableValues.Count > 1)
            {
                JSONObject array = JSONObject.emptyArray;
                foreach (string v in variableValues)
                {
                    array.Add(v);
                }
                responseMessage.AddField(key, array);
            }
            else
            {
                responseMessage.AddField(key, variableValues[0]);
            }
        }
        Debug.Log("responseMessage : \n" + responseMessage.ToString(true));
        return responseMessage;
    }

    public override JSONObject getResultMessage(int result)
    {
        JSONObject resultMessage = new JSONObject();
        foreach (string key in actionProtocol.resultMessageTemplate.keys)
        {
            string value = actionProtocol.resultMessageTemplate.GetField(key).getStringValue();
            List<string> variableValues = new List<string>();
            if (value.Contains("$"))
            {
                if (!this.variables.ContainsKey(value))
                {
                    continue;
                }
                foreach (string variableValue in this.variables[value])
                {
                    variableValues.Add(variableValue);
                }
            }
            else
            {
                variableValues.Add(value);
            }
            if (variableValues.Count > 1)
            {
                JSONObject array = JSONObject.emptyArray;
                foreach (string v in variableValues)
                {
                    array.Add(v);
                }
                resultMessage.AddField(key, array);
            }
            else
            {
                resultMessage.AddField(key, variableValues[0]);
            }
        }
        resultMessage.AddField("result", result);
        Debug.Log("resultMessage : \n" + resultMessage.ToString());
        return resultMessage;
    }

    public override string getProtocolType()
    {
        return this.actionProtocol.protocolType;
    }

    public override JSONObject getResultMessage(int robotID, int result)
    {
        throw new System.NotImplementedException();
    }
}
