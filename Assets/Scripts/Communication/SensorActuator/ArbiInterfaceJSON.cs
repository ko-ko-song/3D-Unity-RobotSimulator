using System.Collections;
using System.Collections.Generic;
using System;
using System.Net.Sockets;
using System.Net;
using UnityEngine;
using System.Threading;
using System.IO;
using Defective.JSON;

public class ArbiInterfaceJSON
{
    private NetworkStream ns;
    private StreamWriter sw;
    private StreamReader sr;
    private TcpListener tcpListener;
    private List<TcpClient> clients;
    private SensorActuatorModule sensorActuatorModule;

    public ArbiInterfaceJSON(SensorActuatorModule sensorActuatorModule)
    {
        this.sensorActuatorModule = sensorActuatorModule;
        clients = new List<TcpClient>();
    }
    
    public void connect(string ip, string port)
    {
        try
        {
            tcpListener = new TcpListener(IPAddress.Parse(ip), Int32.Parse(port));
            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(OnAcceptClient, null);
            Debug.Log("Arbi Server Open!   " + "   " + ip + ":" + port);
        }
        catch (SocketException e)
        {
            Debug.Log(e.ToString());
            this.connect(ip, (Int32.Parse(port) + 1).ToString());
        }
    }

    private void OnAcceptClient(IAsyncResult ar)
    {
        TcpClient client = tcpListener.EndAcceptTcpClient(ar);
        clients.Add(client);
        Debug.Log("Arbi Server : " + clients.Count + "번째 Client 접속 성공");

        tcpListener.BeginAcceptTcpClient(OnAcceptClient, null);
    }

    private void send(string JSONString)
    {
        foreach (TcpClient n in clients.ToArray())
        {
            if (!n.Connected)
                continue;
            ns = n.GetStream();
            sw = new StreamWriter(ns);
            //Debug.Log(JSONString);
            sw.WriteLine(JSONString);
            sw.Flush();
        }
    }
    
    public void sendMessage(JSONObject JSONObject)
    {
        this.send(JSONObject.ToString(false));
    }
    public void checkMessage()
    {
        foreach (TcpClient c in clients.ToArray())
        {
            if (!c.Connected)
            {
                clients.Remove(c);
                Debug.Log("client 연결 해제");
                continue;
            }
            ns = c.GetStream();
            if (ns.DataAvailable)
            {
                sr = new StreamReader(ns);
                string receivedMessage = sr.ReadLine();
                Debug.Log("receivedMessage : \n" + receivedMessage);
                this.parseMessage(receivedMessage);
            }
        }
    }
    
    private void parseMessage(string receivedMessage)
    {
        JSONObject receivedMessageJSON = new JSONObject(receivedMessage);
        if (receivedMessageJSON.keys == null)
        {
            Debug.Log("key not found");
            return;
        }
           

        ActionProtocol actionProtocol = this.getMatchedProtocol(receivedMessageJSON);
        if (actionProtocol == null)
        {
            Debug.Log("can't find matched protocol");
            return;
        }
            

        ActionProtocolInstance actionProtocolInstance = actionProtocol.getInstance(receivedMessageJSON);
        sensorActuatorModule.receiveMessage(actionProtocolInstance);
    }

    
    private ActionProtocol getMatchedProtocol(JSONObject receivedMessageJSON)
    {
        foreach (ActionProtocol actionProtocol in this.sensorActuatorModule.actionProtocols.Values)
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









    public void parseMessageForTest(string testMessage)
    {
        this.parseMessage(testMessage);
    }






    public void quit()
    {
        if (ns != null)
            ns.Close();
    }
}








