using Defective.JSON;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class SensingProtocol
{
    public string protocolId;
    public string protocolType;
    public string messageFormat;
    public float period;
    public JSONObject sensingMessageTemplate;
    
    public SensingProtocolInstance getInstance()
    {
        if (messageFormat.Equals("JSON"))
        {
            SensingProtocolInstance si = new SensingProtocolJSONInstance(this);
            return si;
        }
        else if (messageFormat.Equals("packet"))
        {
            //SensingProtocolInstance si = new SensingProtocolPacketInstance(this);
            //return si;
            return null;
        }
        else
        {
            Debug.Log("undefined message format");
            return null;
        }
        
    }

    public string print()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("protocolID : \t" + this.protocolId + "\n");
        sb.Append("protocolType : \t" + this.protocolType + "\n");
        sb.Append("messageFormat : \t" + this.messageFormat + "\n");

        sb.Append(this.sensingMessageTemplate.ToString() + "\n");
        return sb.ToString();
    }
}
