using Defective.JSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensingProtocolJSONInstance : SensingProtocolInstance
{
    public SensingProtocolJSONInstance(SensingProtocol s) : base(s)
    {

    }

    public override JSONObject getSensingMessage(string id)
    {
        JSONObject sensingMessage = new JSONObject();

        foreach (string key in this.sensingProtocol.sensingMessageTemplate.keys)
        {
            if (this.sensingProtocol.sensingMessageTemplate.GetField(key).getStringValue().Contains("$"))
            {
                Variable v = getPreDefinedKeywordValue(id, this.sensingProtocol.sensingMessageTemplate.GetField(key).getStringValue());

                switch (v.type)
                {
                    case "string":
                        sensingMessage.AddField(key, v.getStringValue());
                        break;

                    case "int":
                        sensingMessage.AddField(key, v.getIntValue());
                        break;

                    case "float":
                        sensingMessage.AddField(key, v.getFloatValue());
                        break;

                    case "bool":
                        sensingMessage.AddField(key, v.getBoolValue());
                        break;

                    default:
                        Debug.Log("method : getSensingMessage, variable  type : " + v.type);
                        break;
                }

            }
            else
            {
                sensingMessage.AddField(key, this.sensingProtocol.sensingMessageTemplate.GetField(key));
            }
        }

        //Debug.Log("sensingMessage : \n" + sensingMessage.ToString(true));
        return sensingMessage;
    }

    protected override Variable getPreDefinedKeywordValue(string id, string key)
    {
        Variable v = new Variable();

        GameObject gobj = GameObject.Find(id);

        //    Robot robot = gobj.GetComponent<Robot>();
        //    EnvironmentObject eobj = gobj.GetComponent<EnvironmentObject>();


        //    switch (key)
        //    {
        //        case "$robotID":
        //            v.type = "int";
        //            v.value = eobj.id;
        //            break;

        //        case "$status":
        //            v.type = "int";
        //            v.value = ((int)robot.robotStatus).ToString();

        //            break;

        //        case "$positionX":
        //            v.type = "float";
        //            v.value = robot.transform.position.x.ToString();

        //            break;
        //        case "$positionY":
        //            v.type = "float";
        //            if(robot.GetComponent<EnvironmentObject>().position.y >= 2)
        //            {
        //                v.value = (robot.transform.position.y - EnvironmentManager.zPlus).ToString();
        //            }
        //            else
        //            {
        //                v.value = robot.transform.position.y.ToString();
        //            }
        //            break;
        //        case "$theta":
        //            v.type = "float";
        //            v.value = robot.transform.eulerAngles.z.ToString();

        //            break;
        //        case "$speed":
        //            v.type = "float";
        //            v.value = robot.speed.ToString();

        //            break;
        //        case "$battery":
        //            v.type = "int";
        //            v.value = robot.remainingBattery.ToString();

        //            break;
        //        case "$loading":
        //            v.type = "bool";
        //            v.value = robot.loading.ToString();

        //            break;
        //        default:
        //            Debug.Log("get PreDefinedKeywordValue : " + key);
        //            break;
        //    }

        //    return v;
        return v;
    }



}
