using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using TMPro;
using Defective.JSON;
using System.IO;

public class EnvironmentManager : MonoBehaviour
{
    public static EnvironmentManager instance = null;
    public bool isEditor;

    private void Awake()
    {
        Application.runInBackground = true;
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {

        string path = "";
        if (!isEditor)
        {
            path = "./Models/";
        }
        else if (isEditor)
        {
            path = "./Assets/Resources/";
        }

        LoadActuatorModel(path);
    }
    

    private void LoadActuatorModel(string path)
    {
        string[] actuatorModels = Directory.GetFiles(path + "/actuatorModels", "*.json", SearchOption.AllDirectories);
        foreach (var file in actuatorModels)
        {
            StreamReader sr = new StreamReader(file);
            string data = sr.ReadToEnd();

            LoadActuatorModel(new JSONObject(data));
        }
    }

    public void LoadActuatorModel(JSONObject actuatorModuleModels)
    {
        List<JSONObject> actuatorModuleModelList = actuatorModuleModels[0].list;
        BuildActuatorModels(actuatorModuleModelList);
    }


    private void BuildActuatorModels(List<JSONObject> ActuatorModelList)
    {
        foreach (JSONObject actuatorModel in ActuatorModelList)
        {
            BuildAcutatorModel(actuatorModel);
        }
    }

    private void BuildAcutatorModel(JSONObject actuatorModel)
    {
        GameObject gobj = new GameObject();
        
        SensorActuatorModule actuatorModule = gobj.AddComponent<SensorActuatorModule>();
        
        actuatorModule.id             = actuatorModel.GetField("id").getStringValue();
        actuatorModule.name           = actuatorModel.GetField("name").getStringValue();
        actuatorModule.type           = actuatorModel.GetField("type").getStringValue();
        actuatorModule.targetObjectId = actuatorModel.GetField("targetObjectId").getStringValue();
        actuatorModule.messageFormat  = actuatorModel.GetField("messageFormat").getStringValue();
        actuatorModule.ip             = actuatorModel.GetField("ip").getStringValue();
        actuatorModule.port           = actuatorModel.GetField("port").getStringValue();

        gobj.name = actuatorModule.id;
        
        List<JSONObject> actionProtocolModels = actuatorModel.GetField("actionProtocols").list;

        if (actionProtocolModels != null)
            buildActionProtocols(actuatorModule, actionProtocolModels, actuatorModule.messageFormat);

        actuatorModule.openServer();
        actuatorModule.print();
    }
    
    private void buildActionProtocols(SensorActuatorModule actuatorModule, List<JSONObject> actionProtocolModels, string messageFormat)
    {
        foreach (JSONObject actionProtocolModel in actionProtocolModels)
        {
            ActionProtocol actionProtocol = builidActionProtocol(actionProtocolModel, messageFormat);

            //Debug.Log(actionProtocol.protocolId);
            actuatorModule.addActionProtocol(actionProtocol.protocolId, actionProtocol);
        }
    }
    
    
    private ActionProtocol builidActionProtocol(JSONObject actionProtocolModel, string messageFormat)
    {
        ActionProtocol actionProtocol = new ActionProtocol();
        actionProtocol.protocolId = actionProtocolModel.GetField("protocolId").getStringValue();
        actionProtocol.protocolType = actionProtocolModel.GetField("protocolType").getStringValue();
        actionProtocol.messageFormat = messageFormat;
        if(actionProtocol.protocolType =="result")
        {
            actionProtocol.requestMessageTemplate = actionProtocolModel.GetField("requestMessageTemplate");
            actionProtocol.responseMessageTemplate = actionProtocolModel.GetField("responseMessageTemplate");
            actionProtocol.resultMessageTemplate = actionProtocolModel.GetField("resultMessageTemplete");
            List<JSONObject> actionModels = actionProtocolModel.GetField("actionList").list;
            foreach (JSONObject actionModel in actionModels)
            {
                Action action = buildAction(actionModel);
                actionProtocol.action = action;
            }
        }
        
        if(actionProtocol.protocolType == "send")
        {
            actionProtocol.resultMessageTemplate = actionProtocolModel.GetField("resultMessageTemplete");
        }
        
        if(actionProtocol.protocolType == "request")
        {
            actionProtocol.requestMessageTemplate = actionProtocolModel.GetField("requestMessageTemplate");
            actionProtocol.responseMessageTemplate = actionProtocolModel.GetField("responseMessageTemplate");
            if(actionProtocolModel.GetField("actionList") != null)
            {
                List<JSONObject> actionModels = actionProtocolModel.GetField("actionList").list;
                foreach (JSONObject actionModel in actionModels)
                {
                    Action action = buildAction(actionModel);
                    actionProtocol.action = action;
                }
            }
        }

        if (actionProtocolModel.GetField("headerSize") != null)
        {
            actionProtocol.HEADER_SIZE = actionProtocolModel.GetField("headerSize").intValue;
        }

        return actionProtocol;
    }
    
    private Action buildAction(JSONObject actionModel)
    {
        string actionString = actionModel.getStringValue();
        int index1 = actionString.IndexOf("(");
        int index2 = actionString.IndexOf(")");
        string actionName = actionString.Substring(0, index1);
        string argsString = actionString.Substring(index1 + 1, index2 - index1 - 1);

        string[] args = argsString.Split(',');
        for (int i = 0; i < args.Length; i++)
        {
            args[i] = args[i].Replace(" ", String.Empty);
        }

        List<string> argList = new List<string>(args);
        
        Action action = new Action();

        action.ActionName = actionName;
        action.actionArgs = argList;
        return action;
    }

}
