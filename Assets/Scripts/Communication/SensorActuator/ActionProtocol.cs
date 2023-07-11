using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Defective.JSON;
using System.Text;

public class ActionProtocol 
{
    public string protocolId;
    public string protocolType;
    public string messageFormat;
    public int HEADER_SIZE;
    public JSONObject requestMessageTemplate;
    public JSONObject responseMessageTemplate;
    public JSONObject resultMessageTemplate;
    public Action action;
    
    public ActionProtocolInstance getInstance(JSONObject receivedMessage)
    {
        ActionProtocolInstance actionProtocolInstance = new ActionProtocolJSONInstance(this, receivedMessage);
        return actionProtocolInstance;
   
    }
    
    public ActionProtocolInstance getInstance(byte[] receivedMessage)
    {

        //ActionProtocolInstance actionProtocolInstance;
        //if (receivedMessage.Length==0)
        //    actionProtocolInstance = new ActionProtocolPacketInstance(this, null);
        //else
        //    actionProtocolInstance = new ActionProtocolPacketInstance(this, receivedMessage);
        //return actionProtocolInstance;
        return null;
    }
    

    public string print()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("protocolID : \t" + this.protocolId + "\n");
        sb.Append("protocolType : \t" + this.protocolType + "\n");
        sb.Append("messageFormat : \t" + this.messageFormat + "\n");
        if(this.requestMessageTemplate!= null)
            sb.Append("requestMessageTemplate : \n" + this.requestMessageTemplate.Print(true) + "\n");
        if(this.responseMessageTemplate!= null)
            sb.Append("responseMessageTemplate : \n" + this.responseMessageTemplate.Print(true) + "\n");
        if(this.resultMessageTemplate != null)
            sb.Append("resultMessageTemplate : \n" + this.resultMessageTemplate.Print(true) + "\n");
        //sb.Append("actionName : \t" + this.action.actionName + "\n");
        //sb.Append("actionArgs : \t");
        //foreach (string arg in this.action.actionArgs)
        //{
        //    sb.Append(arg + "\t");
        //}
            
        
        sb.Append("\n");
        return sb.ToString();
    }
}
