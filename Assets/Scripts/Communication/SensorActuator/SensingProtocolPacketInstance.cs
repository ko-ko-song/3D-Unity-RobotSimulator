//using Defective.JSON;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class SensingProtocolPacketInstance : SensingProtocolInstance
//{
//    public SensingProtocolPacketInstance(SensingProtocol s) : base(s)
//    {

//    }

//    public override JSONObject getSensingMessage(string id)
//    {
//        string tempString = sensingProtocol.sensingMessageTemplate.ToString();
//        JSONObject sensingMessage = new JSONObject(tempString);
        
//        List<JSONObject> packetData = sensingMessage.GetField("packetData").list;
//        foreach (JSONObject obj in packetData)
//        {
//            if (obj.GetField("classification").getStringValue().Equals("var"))
//            {
//                Variable v = getPreDefinedKeywordValue(id, obj.GetField("value").getStringValue());
                
//                switch (v.type)
//                {
//                    case "string":
//                        obj.SetField("value", v.getStringValue());
//                        break;
//                    case "int":
//                        obj.SetField("value", v.getIntValue());
//                        break;

//                    case "float":
//                        obj.SetField("value", v.getFloatValue());
//                        break;

//                    case "bool":
//                        obj.SetField("value", v.getBoolValue());
//                        break;

//                    default:
//                        Debug.Log("method : getSensingMessage, variable  type : " + v.type);
//                        break;
//                }
//            }
//            else
//            {
//                string type = obj.GetField("type").getStringValue();
//                if (obj.GetField("value").isString)
//                {
//                    if (type.Equals("int"))
//                    {
//                        string value = obj.GetField("value").getStringValue();
//                        obj.SetField("value", int.Parse(value));
//                    }
//                    else if (type.Equals("float"))
//                    {
//                        string value = obj.GetField("value").getStringValue();
//                        obj.SetField("value", float.Parse(value));
//                    }
//                    else if (type.Equals("bool"))
//                    {
//                        string value = obj.GetField("value").getStringValue();
//                        obj.SetField("value", bool.Parse(value));
//                    }
//                }
//            }
//        }
        


//        //Debug.Log("sensingMessage : \n" + sensingMessage.ToString(true));
//        return sensingMessage;
//    }

//    protected override Variable getPreDefinedKeywordValue(string id, string key)
//    {
//        Variable v = new Variable();
//        GameObject gobj = GameObject.Find(id);

//        if (gobj == null)
//        {
//            Debug.LogError("[error]  | id로 game object를 찾을 수 없음");
//            return v;
//        }

//        Robot robot = gobj.GetComponent<Robot>();
//        if (robot == null)
//        {
//            Debug.LogError("[error]   | 로봇 id에 해당하는 Robot class를 찾을 수 없음");
//            return v;
//        }
            

//        switch (key)
//        {
//            case "$robotID":
//                v.type = "int";
//                v.value = robot.id;
//                break;

//            case "$status":
//                v.type = "int";
//                v.value = ((int)robot.robotStatus).ToString();

//                break;

//            case "$positionX":
//                v.type = "int";
//                v.value = ((int)(robot.transform.position.x*1000)).ToString();

//                break;
//            case "$positionY":
//                v.type = "int";
//                if (robot.GetComponent<EnvironmentObject>().position.y >= 2)
//                {
//                    v.value = ((int)(robot.transform.position.y - EnvironmentManager.zPlus)*1000).ToString();
//                }
//                else
//                {
//                    v.value = ((int)(robot.transform.position.y * 1000)).ToString();
//                }
                
//                break;
//            case "$theta":
//                v.type = "float";
//                v.value = robot.transform.eulerAngles.z.ToString();

//                break;
//            case "$speed":
//                v.type = "float";
//                v.value = robot.speed.ToString();

//                break;
//            case "$battery":
//                v.type = "int";
//                v.value = robot.remainingBattery.ToString();

//                break;
//            case "$loading":
//                v.type = "bool";
//                v.value = robot.loading.ToString();

//                break;
//            default:
//                Debug.Log("get PreDefinedKeywordValue : " + key);
//                break;
//        }

//        return v;
//    }

//}
