using Defective.JSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SensingProtocolInstance 
{
    public SensingProtocol sensingProtocol;
    public SensingProtocolInstance(SensingProtocol s)
    {
        this.sensingProtocol = s;
    }
    public abstract JSONObject getSensingMessage(string id);

    protected abstract Variable getPreDefinedKeywordValue(string id, string key);

}
