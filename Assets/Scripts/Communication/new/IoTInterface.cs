using System.Collections;
using System.Collections.Generic;
using System;
using System.Net.Sockets;
using System.Net;
using UnityEngine;
using System.Threading;
using System.IO;
using Defective.JSON;

public class IoTInterface : MonoBehaviour
{
    private NetworkStream ns;
    private StreamWriter sw;
    private StreamReader sr;
    private TcpListener tcpListener;
    private List<TcpClient> clients;


    void Start()
    {        
        clients = new List<TcpClient>();
           
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void connect(string ip, string port){
        try
        {
            tcpListener = new TcpListener(IPAddress.Parse(ip), Int32.Parse(port));
            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(OnAcceptClient, null);
            Debug.Log("IoTInterface Open!   " + "   " + ip + ":" + port);
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
        Debug.Log("IoTInterface : " + clients.Count + "번째 Client 접속 성공");

        tcpListener.BeginAcceptTcpClient(OnAcceptClient, null);
    }


    private void send(string JSONString)
    {
        foreach (TcpClient n in clients.ToArray())
        {
            try{
                ns = n.GetStream();
                sw = new StreamWriter(ns);
                //Debug.Log(JSONString);
                sw.WriteLine(JSONString);
                sw.Flush();
            }catch{
                clients.Remove(n);
                n.Close();
                continue;
            }
        }
    }
    
    public void sendJSONMessage(JSONObject JSONObject)
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
    
    private void parseMessage(string receivedMessage){
        try{
            JSONObject receivedMessageJSON = new JSONObject(receivedMessage);
        }
        catch{
            Debug.LogWarning("parse message error : " + receivedMessage);
        }
        

    }

}
