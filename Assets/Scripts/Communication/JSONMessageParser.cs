using Defective.JSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JSONMessageParser
{
    private Dictionary<string, ActionProtocol>.ValueCollection actionProtocols;
    private ActionProtocolInstance actionProtocolInstance;

    public JSONMessageParser(Dictionary<string, ActionProtocol>.ValueCollection actionProtocols)
    {
        this.actionProtocols = actionProtocols;
    }

    public bool parse(string jsonString)
    {
        JSONObject receivedMessageJSON = new JSONObject(jsonString);
        if (receivedMessageJSON.keys == null)
            return false;
        

        ActionProtocol actionProtocol = this.getMatchedProtocol(receivedMessageJSON);
        if (actionProtocol == null)
            return false;
        else
            this.actionProtocolInstance = actionProtocol.getInstance(receivedMessageJSON);

        return true;
    }

    public ActionProtocolInstance getActionProtocolInstance()
    {
        return this.actionProtocolInstance;
    }
    
    private ActionProtocol getMatchedProtocol(JSONObject receivedMessageJSON)
    {
        foreach (ActionProtocol actionProtocol in this.actionProtocols)
        {
            bool isMatched = isMatch(receivedMessageJSON, actionProtocol.requestMessageTemplate);
            if (isMatched)
            {
                return actionProtocol;
            }
        }
        return null;
    }

    private bool isMatch(JSONObject receivedMessageJSON, JSONObject requestMessageTemplate)
    {

        if (requestMessageTemplate.keys.Count != receivedMessageJSON.keys.Count)
            return false;
        foreach (string key in requestMessageTemplate.keys)
        {
            if (!receivedMessageJSON.keys.Contains(key))
                return false;
        }
        foreach (string key in receivedMessageJSON.keys)
        {
            if (!requestMessageTemplate.keys.Contains(key))
                return false;
        }
        foreach (string key in requestMessageTemplate.keys)
        {
            if (requestMessageTemplate.GetField(key).isArray)
            {
                if (requestMessageTemplate.GetField(key).list[0].getStringValue().Contains("$"))
                {
                    continue;
                }
            }
            else if (requestMessageTemplate.GetField(key).getStringValue().Contains("$"))
            {
                continue;
            }
            else if (!requestMessageTemplate.GetField(key).getStringValue().Equals(receivedMessageJSON.GetField(key).getStringValue()))
            {

                return false;
            }
        }
        return true;
    }

        
}
