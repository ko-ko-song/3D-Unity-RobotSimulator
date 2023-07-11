//using Defective.JSON;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class ActionProtocolPacketInstance : ActionProtocolInstance
//{
//    public byte[] receivedMessage;
//    public ActionProtocolPacketInstance(ActionProtocol actionProtocol, byte[] receivedMessage) : base(actionProtocol)
//    {
//        this.actionProtocol = actionProtocol;
//        if(receivedMessage != null)
//        {
//            this.receivedMessage = receivedMessage;
//            this.variables = new Dictionary<string, List<string>>();
//            this.actionInstance = new Action();
//            try
//            {
//                this.bindVariables();
//            }
//            catch(Exception e)
//            {
//                Debug.Log(e);
//            }
//        }
//    }
    
//    public override void bindVariables()
//    {
//        ByteBuffer byteBuffer = new ByteBuffer();
//        byteBuffer.wrap(receivedMessage);

//        List<JSONObject> packetData = actionProtocol.requestMessageTemplate.GetField("packetData").list;

//        foreach(JSONObject obj in packetData)
//        {
//            string sizeString = obj.GetField("size").getStringValue();
//            int size = 0 ;
//            if (!sizeString.Contains("$"))
//            {
//                size = int.Parse(sizeString);
//            }
//            else
//            {
//                string[] arr = sizeString.Split('*');
//                int mul = 1;
//                foreach(string st in arr)
//                {
//                    string stTrim = st.Trim();
//                    if (stTrim.Contains("$"))
//                    {
//                        List<string> value = this.variables[stTrim];
//                        mul = mul * int.Parse(value[0]);
//                    }
//                    else
//                    {
//                        mul = mul * int.Parse(stTrim);
//                    }
//                }
//                size = mul;
//            }
//            string type = obj.GetField("type").getStringValue();
//            string classification = obj.GetField("classification").getStringValue();

//            if (type.Equals("int") || type.Equals("float"))
//            {
//                if (classification.Equals("var"))
//                {
//                    string variable = obj.GetField("value").getStringValue();
//                    List<string> value = new List<string>();
//                    for (int i = 0; i < (size / 4); i++)
//                    {
//                        value.Add(byteBuffer.getInt().ToString());
//                    }
//                    this.variables.Add(variable, value);
//                }
//                else
//                {
//                    for (int i = 0; i < (size / 4); i++)
//                    {
//                        byteBuffer.getInt();
//                    }
//                }
//            }
//            else if (type.Equals("bool"))
//            {
//                if (classification.Equals("var"))
//                {
//                    string variable = obj.GetField("value").getStringValue();
//                    List<string> value = new List<string>();
//                    for (int i = 0; i < size ; i++)
//                    {
//                        value.Add(byteBuffer.get().ToString());
//                    }
//                    this.variables.Add(variable, value);
//                }
//                else
//                {
//                    for (int i = 0; i < size; i++)
//                    {
//                        byteBuffer.get();
//                    }
//                }
//            }
//        }
//    }



//    public override void bindAction(string targetObjectId)
//    {
//        if (actionProtocol.action == null)
//            return;
//        string name = actionProtocol.action.actionName;
//        List<string> args = new List<string>();
//        foreach (string arg in actionProtocol.action.actionArgs)
//        {
//            if (arg.Contains("$"))
//            {
//                foreach (string value in this.variables[arg])
//                {
//                    args.Add(value);
//                }
//            }
//            else
//            {
//                args.Add(arg);
//            }
//        }

//        List<JSONObject> objs = this.actionProtocol.requestMessageTemplate.GetField("packetData").list;

//        for(int i=0; i<objs.Count; i++)
//        {
//            if (objs[i].GetField("name").getStringValue().Equals("robotID"))
//            {
//                break;
//            }
            
//            if(i == (objs.Count - 1))
//            {
//                this.actionInstance.actionArgs.Insert(0, targetObjectId);
//            }
//        }
            
//        this.actionInstance.actionName = name;
//        this.actionInstance.actionArgs = args;

//    }

    

//    public override string getProtocolType()
//    {
//        return this.actionProtocol.protocolType;
//    }

//    public override JSONObject getResponseMessage()
//    {
//        string temp = actionProtocol.responseMessageTemplate.ToString();
//        JSONObject tempMessage = new JSONObject(temp);
//        List<JSONObject> header = tempMessage.GetField("packetHeader").list;

//        List<JSONObject> packetData = tempMessage.GetField("packetData").list;

//        JSONObject responseMessage = new JSONObject();
//        JSONObject headerMessage = JSONObject.emptyArray;
//        JSONObject packetDataMessage = JSONObject.emptyArray;

//        foreach (JSONObject obj in header)
//        {
//            List<string> variableValues = new List<string>();
//            string classifiaction = obj.GetField("classification").getStringValue();

//            string v = obj.GetField("value").getStringValue();
//            if (classifiaction.Equals("var"))
//            {
//                foreach (string variableValue in this.variables[v])
//                {
//                    variableValues.Add(variableValue);
//                }
//            }
//            else
//            {
//                variableValues.Add(v);
//            }

//            foreach (string val in variableValues)
//            {
//                JSONObject tempObj = new JSONObject();
                
//                tempObj.AddField("name", obj.GetField("name"));
//                tempObj.AddField("type", obj.GetField("type"));
//                tempObj.AddField("classification", obj.GetField("classification"));
//                switch (obj.GetField("type").getStringValue())
//                {
//                    case "int":
//                        tempObj.AddField("value", (int)Convert.ToUInt32(val));
//                        tempObj.AddField("size", 4);
//                        break;
//                    case "float":
//                        tempObj.AddField("value", float.Parse(val));
//                        tempObj.AddField("size", 4);
//                        break;
//                    case "bool":
                            
//                        tempObj.AddField("value", bool.Parse(val));
//                        tempObj.AddField("size", 1);
//                        break;
//                }
//                headerMessage.Add(tempObj);
//            }
//        }

//        foreach (JSONObject obj in packetData)
//        {
//            List<string> variableValues = new List<string>();

//            string classifiaction = obj.GetField("classification").getStringValue();

//            string v = obj.GetField("value").getStringValue();
//            if (classifiaction.Equals("var"))
//            {
//                foreach (string variableValue in this.variables[v])
//                {
//                    variableValues.Add(variableValue);
//                }
//            }
//            else
//            {
//                if (v.Contains("$"))
//                {
//                    Debug.Log("classifiaction error  : " + v);
//                }
//                variableValues.Add(v);
//            }
//            foreach (string val in variableValues)
//            {
//                JSONObject tempObj = new JSONObject();
//                tempObj.AddField("name", obj.GetField("name"));
//                tempObj.AddField("type", obj.GetField("type"));
//                tempObj.AddField("classification", obj.GetField("classification"));
//                switch (obj.GetField("type").getStringValue())
//                {
//                    case "int":
//                        tempObj.AddField("value", (int)Convert.ToUInt32(val));
//                        tempObj.AddField("size", 4);
//                        break;
//                    case "float":
//                        tempObj.AddField("value", float.Parse(val));
//                        tempObj.AddField("size", 4);
//                        break;
//                    case "bool":
//                        tempObj.AddField("value", bool.Parse(val));
//                        tempObj.AddField("size", 1);
//                        break;
//                }
//                packetDataMessage.Add(tempObj);
//            }
//        }

//        responseMessage.AddField("packetHeader", headerMessage);
//        responseMessage.AddField("packetData", packetDataMessage);

//        //Debug.Log("responseMessage : \n" + responseMessage.ToString(true));
//        return responseMessage;
//    }

//    public override JSONObject getResultMessage(int result)
//    {
//        string temp = actionProtocol.resultMessageTemplate.ToString();

//        JSONObject tempMessage = new JSONObject(temp);
//        List<JSONObject> header = tempMessage.GetField("packetHeader").list;
//        List<JSONObject> packetData = tempMessage.GetField("packetData").list;

//        JSONObject resultMessage = new JSONObject();

//        JSONObject headerMessage = JSONObject.emptyArray;
//        JSONObject packetDataMessage = JSONObject.emptyArray;

//        foreach (JSONObject obj in header)
//        {
//            List<string> variableValues = new List<string>();
//            string classifiaction = obj.GetField("classification").getStringValue();
//            string v = obj.GetField("value").getStringValue();
//            if (classifiaction.Equals("var"))
//            {
//                if (!this.variables.ContainsKey(v))
//                {
//                    continue;
//                }

//                foreach (string variableValue in this.variables[v])
//                {
//                    variableValues.Add(variableValue);
//                }
//            }
//            else
//            {
//                variableValues.Add(v);
//            }
//            foreach (string val in variableValues)
//            {
//                JSONObject tempObj = new JSONObject();
//                tempObj.AddField("name", obj.GetField("name"));
//                tempObj.AddField("type", obj.GetField("type"));
//                tempObj.AddField("classification", obj.GetField("classification"));

//                switch (obj.GetField("type").getStringValue())
//                {
//                    case "int":
//                        tempObj.AddField("value", (int)Convert.ToUInt32(val));
//                        tempObj.AddField("size", 4);
//                        break;
//                    case "float":
//                        tempObj.AddField("value", float.Parse(val));
//                        tempObj.AddField("size", 4);
//                        break;
//                    case "bool":

//                        tempObj.AddField("value", bool.Parse(val));
//                        tempObj.AddField("size", 1);
//                        break;
//                }

//                headerMessage.Add(tempObj);
//            }
//        }

//        foreach (JSONObject obj in packetData)
//        {
//            List<string> variableValues = new List<string>();
//            string classifiaction = obj.GetField("classification").getStringValue();
//            string v = obj.GetField("value").getStringValue();
//            if (classifiaction.Equals("var"))
//            {
//                if (!this.variables.ContainsKey(v))
//                {
//                    continue;
//                }
//                foreach (string variableValue in this.variables[v])
//                {
//                    variableValues.Add(variableValue);
//                }
//            }
//            else
//            {
//                variableValues.Add(v);
//            }
//            foreach (string val in variableValues)
//            {
//                JSONObject tempObj = new JSONObject();
//                tempObj.AddField("name", obj.GetField("name"));
//                tempObj.AddField("type", obj.GetField("type"));
//                tempObj.AddField("classification", obj.GetField("classification"));

//                switch (obj.GetField("type").getStringValue())
//                {
//                    case "int":
//                        tempObj.AddField("value", (int)Convert.ToUInt32(val));
//                        tempObj.AddField("size", 4);
//                        break;
//                    case "float":
//                        tempObj.AddField("value", float.Parse(val));
//                        tempObj.AddField("size", 4);
//                        break;
//                    case "bool":

//                        tempObj.AddField("value", bool.Parse(val));
//                        tempObj.AddField("size", 1);
//                        break;
//                }

//                packetDataMessage.Add(tempObj);
//            }
//        }


//        foreach (JSONObject obj in packetData)
//        {
//            if (obj.GetField("name").getStringValue().Equals("result"))
//            {
//                JSONObject tempObj = new JSONObject();
//                tempObj.AddField("name", obj.GetField("name"));
//                tempObj.AddField("type", obj.GetField("type"));
//                tempObj.AddField("size", obj.GetField("size"));
//                tempObj.AddField("value", result);
//                tempObj.AddField("classification", obj.GetField("classification"));
//                packetDataMessage.Add(tempObj);
//                break;
//            }
//            else if (obj.GetField("name").getStringValue().Equals("nodeID"))
//            {
//                JSONObject tempObj = new JSONObject();
//                tempObj.AddField("name", obj.GetField("name"));
//                tempObj.AddField("type", obj.GetField("type"));
//                tempObj.AddField("size", obj.GetField("size"));
//                tempObj.AddField("value", result);
//                tempObj.AddField("classification", obj.GetField("classification"));
//                packetDataMessage.Add(tempObj);
//                break;
//            }
//        }
        
//        resultMessage.AddField("packetHeader", headerMessage);
//        resultMessage.AddField("packetData", packetDataMessage);


//        //Debug.Log("resultMessage : \n" + resultMessage.ToString(true));
//        return resultMessage;



//    }

//    public override JSONObject getResultMessage(int robotID, int result)
//    {
//        string temp = actionProtocol.resultMessageTemplate.ToString();

//        JSONObject tempMessage = new JSONObject(temp);
//        List<JSONObject> header = tempMessage.GetField("packetHeader").list;
//        List<JSONObject> packetData = tempMessage.GetField("packetData").list;

//        JSONObject resultMessage = new JSONObject();

//        JSONObject headerMessage = JSONObject.emptyArray;
//        JSONObject packetDataMessage = JSONObject.emptyArray;

//        foreach (JSONObject obj in header)
//        {
//            List<string> variableValues = new List<string>();
//            string classifiaction = obj.GetField("classification").getStringValue();
//            string v = obj.GetField("value").getStringValue();
//            if (classifiaction.Equals("var"))
//            {
//                if (!this.variables.ContainsKey(v))
//                {
//                    continue;
//                }

//                foreach (string variableValue in this.variables[v])
//                {
//                    variableValues.Add(variableValue);
//                }
//            }
//            else
//            {
//                variableValues.Add(v);
//            }
//            foreach (string val in variableValues)
//            {
//                JSONObject tempObj = new JSONObject();
//                tempObj.AddField("name", obj.GetField("name"));
//                tempObj.AddField("type", obj.GetField("type"));
//                tempObj.AddField("classification", obj.GetField("classification"));

//                switch (obj.GetField("type").getStringValue())
//                {
//                    case "int":
//                        tempObj.AddField("value", (int)Convert.ToUInt32(val));
//                        tempObj.AddField("size", 4);
//                        break;
//                    case "float":
//                        tempObj.AddField("value", float.Parse(val));
//                        tempObj.AddField("size", 4);
//                        break;
//                    case "bool":

//                        tempObj.AddField("value", bool.Parse(val));
//                        tempObj.AddField("size", 1);
//                        break;
//                }

//                headerMessage.Add(tempObj);
//            }
//        }


//        foreach (JSONObject obj in packetData)
//        {
//            if (obj.GetField("name").getStringValue().Equals("robotID"))
//            {
//                JSONObject tempObj = new JSONObject();
//                tempObj.AddField("name", obj.GetField("name"));
//                tempObj.AddField("type", obj.GetField("type"));
//                tempObj.AddField("size", obj.GetField("size"));
//                tempObj.AddField("value", robotID);
//                tempObj.AddField("classification", obj.GetField("classification"));
//                packetDataMessage.Add(tempObj);
                
//            }
//            else if (obj.GetField("name").getStringValue().Equals("result"))
//            {
//                JSONObject tempObj = new JSONObject();
//                tempObj.AddField("name", obj.GetField("name"));
//                tempObj.AddField("type", obj.GetField("type"));
//                tempObj.AddField("size", obj.GetField("size"));
//                tempObj.AddField("value", result);
//                tempObj.AddField("classification", obj.GetField("classification"));
//                packetDataMessage.Add(tempObj);
//            }

//        }

//        resultMessage.AddField("packetHeader", headerMessage);
//        resultMessage.AddField("packetData", packetDataMessage);

//        //Debug.Log("resultMessage : \n" + resultMessage.ToString(true));
//        return resultMessage;



//    }


//}
