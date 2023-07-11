using Defective.JSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class SensorActuatorModule : MonoBehaviour
{
    public string id;
    public string name;
    public string type;
    public string targetObjectId;
    public string messageFormat;
    public string ip;
    public string port;

    public Dictionary<string, ActionProtocol> actionProtocols= new Dictionary<string, ActionProtocol>();
    public Dictionary<string, SensingProtocol> sensingProtocols = new Dictionary<string, SensingProtocol>();
    public ArbiInterfaceJSON arbiInterfaceJSON;

    private void Awake()
    {
    }
    void Start()
    {
        ip = GetLocalIPAddress();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void openServer()
    {
        if (messageFormat.Equals("JSON"))
        {
            arbiInterfaceJSON = new ArbiInterfaceJSON(this);
            arbiInterfaceJSON.connect(GetLocalIPAddress(), port);
            StartCoroutine("CheckReceivedJSONMessage");

        }
        else if (messageFormat.Equals("packet"))
        {
            //arbiInterfacePacket = new ArbiInterfacePacket(this);
            //arbiInterfacePacket.connect(GetLocalIPAddress(), port);
            //StartCoroutine("CheckReceivedPacketMessage");
        }
        
        sensingStart();

    }

    public static string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        
        foreach (var ip in host.AddressList)
        {
            if (ip.ToString().StartsWith("192") || ip.ToString().StartsWith("172"))
            {
                return ip.ToString();
            }
        }
        throw new Exception("No network adapters with an IPv4 address in the system!");
    }

    public void addActionProtocol(string key, ActionProtocol actionProtocol)
    {

        this.actionProtocols.Add(key, actionProtocol);
    }
    public void removeActionProtocol(string key)
    {
        this.actionProtocols.Remove(key);
    }

    public ActionProtocol getActionProtocol(string id)
    {
        return this.actionProtocols[id];

    }

    public void addSensingProtocol(string key, SensingProtocol sensingProtocol)
    {
        this.sensingProtocols.Add(key, sensingProtocol);
    }

    public void removeSensingProtocol(string key)
    {
        this.sensingProtocols.Remove(key);
    }


    public void sendMessgae(JSONObject message)
    {
        if(message == null)
        {
            return;
        }
        else if (messageFormat.Equals("JSON"))
        {
            this.arbiInterfaceJSON.sendMessage(message);
        }
        else if (messageFormat.Equals("packet"))
        {
            //Debug.Log("sendMessage \n" + message.ToString(true));
            //this.arbiInterfacePacket.sendMessage(message);
        }
    }
    
    public void executeAction(ActionProtocolInstance actionProtocolInstance)
    {
        SensingActions sensingActions = GameObject.Find("EnvironmentManager").GetComponent<SensingActions>();
        sensingActions.executeAction(this, actionProtocolInstance);
        
    }

    private IEnumerator CheckReceivedJSONMessage()
    {
        while (true)
        {
            arbiInterfaceJSON.checkMessage();
            yield return null;
        }
    }
    private IEnumerator CheckReceivedPacketMessage()
    {
        while (true)
        {
            //arbiInterfacePacket.checkMessage();
            yield return null;
        }
    }

    public void print()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("id :   " + id + "\n");
        sb.Append("name :   " + name + "\n");
        sb.Append("targetObjectId :   " + targetObjectId + "\n");
        sb.Append("messageFormat :   " + messageFormat + "\n");
        sb.Append("ip :   " + ip + "\n");
        sb.Append("port :   " + port + "\n");

        //foreach (ActionProtocol ap in actionProtocols.Values)
        //{
        //    sb.Append(ap.print());
        //}
        //foreach (SensingProtocol sp in sensingProtocols.Values)
        //{
        //    sb.Append(sp.print());
        //}
        Debug.Log(sb.ToString());
    }
    
    public void receiveMessage(ActionProtocolInstance actionProtocolInstance)
    {
        if (!actionProtocolInstance.getProtocolType().Equals("request"))
        {
            JSONObject responseMessage = actionProtocolInstance.getResponseMessage();
            if(responseMessage != null)
                sendMessgae(responseMessage);
        }
        //Debug.Log("received : " + actionProtocolInstance.actionProtocol.protocolId);
        actionProtocolInstance.bindAction(targetObjectId);
        this.executeAction(actionProtocolInstance);

        //if (actionProtocolInstance.getProtocolType().Equals("result"))
        //{
        //    string resultMessage = actionProtocolInstance.bindResultMessage(result);
        //    sendMessgae(resultMessage);
        //}

    }

    private void sensingStart()
    {
        foreach (SensingProtocol s in sensingProtocols.Values)
        {
            IEnumerator coroutine = sensing(s);
            StartCoroutine(coroutine);
        }
        
    }

    private IEnumerator sensing(SensingProtocol s)
    {
        yield return new WaitForSecondsRealtime(1);

        string id = this.id;
        SensingProtocolInstance sensingProtocolInstance = s.getInstance();
        if (this.messageFormat.Equals("JSON"))
        {
            if (s.sensingMessageTemplate.keys.Contains("robotID"))
            {
                id = s.sensingMessageTemplate.GetField("robotID").getStringValue();
            }
        }
        else if (this.messageFormat.Equals("packet"))
        {
            foreach(JSONObject obj in s.sensingMessageTemplate.GetField("packetData").list)
            {
                if (obj.GetField("name").getStringValue().Contains("robotID"))
                {
                    id = obj.GetField("value").getStringValue();
                }
            }
        }
        
        
        while (true)
        {
            JSONObject sensingMessage = sensingProtocolInstance.getSensingMessage(id);
            this.sendMessgae(sensingMessage);
            yield return new WaitForSecondsRealtime(s.period);

        }
        
    }

    
}
