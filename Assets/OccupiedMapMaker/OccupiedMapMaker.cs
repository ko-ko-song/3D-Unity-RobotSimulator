using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Enumeration;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
//using static UnityEngine.Rendering.DebugUI;
using static UnityEngine.UI.Image;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class OccupiedMapMaker : MonoBehaviour
{
    
    private float height; //m
    private float width ; //m
    private Vector3 origin; //m
    public float resolution = 0.05f;

    private float offsetX;
    private float offsetY;

    //absolute path also OK
    public string path = "Assets/Maps/";
    public string filename = "testMap";
   
    private bool[,] occupiedMap;
    RaycastHit hitInfo;

    private float MatchToNearest(float value)
    {
        //Matching to the nearest value in units of resolution
        //ex) if resolution = 0.05, then value 0.06 -> 0.05,  0.08 -> 0.10
        float roundedValue = Mathf.Round(value / resolution) * resolution;
        return roundedValue;
    }
    
    private void InitVariableSetting()
    {
        height = transform.localScale.z;
        width = transform.localScale.x;
        origin = transform.position;
        int heightPixelCount = Mathf.CeilToInt(height / resolution);
        int widthPixelCount = Mathf.CeilToInt(width / resolution);
        occupiedMap = new bool[heightPixelCount, widthPixelCount];
        offsetX = width / 2 - origin.x;
        offsetY = height / 2 - origin.z;
    }
    
    public void MakeOccupiedMap()
    {
        InitVariableSetting();

        for(float row = 0; row< height; row += resolution)
        {
            for (float column = 0; column < width; column += resolution)
            {
                for (int i = 1; i < 6; i++)
                {
                    Vector3 pointToCheck = new Vector3(origin.x - width / 2 + column + resolution / i , origin.y, origin.z - height / 2 + row + resolution / i );
                    DetectionCollisionPoint(pointToCheck);
                }
            }
        }
        
        // SaveArrayToTxtFile(occupiedMap);
        SaveAsP5Binary(occupiedMap);
        CreateYamlFile();
    }
    
    private void DetectionCollisionPoint(Vector3 pointToCheck)
    {
        float rayMaxDistance = resolution;
        
        DetectionCollisionPointByRay(pointToCheck, Vector3.forward, rayMaxDistance);
        DetectionCollisionPointByRay(pointToCheck, Vector3.left, rayMaxDistance);
        DetectionCollisionPointByRay(pointToCheck, Vector3.right, rayMaxDistance);
        DetectionCollisionPointByRay(pointToCheck, Vector3.back, rayMaxDistance);
    }

    private void DetectionCollisionPointByRay(Vector3 rayStartPosition, Vector3 direction,  float maxDistance)
    {
        if (Physics.Raycast(rayStartPosition, direction, out hitInfo, maxDistance))
        {
            Vector3 collisionPoint = hitInfo.point;
            try
            {
                occupiedMap[Mathf.RoundToInt((MatchToNearest(collisionPoint.z) + offsetY) / resolution ), Mathf.RoundToInt((MatchToNearest(collisionPoint.x) + offsetX) / resolution )] = true;
            }
            catch
            {
                //The border part of the Occupied map image data is captured
                //I think it's not critial. so just catch 
                //If you modify the minimum maximum value of the "for" statement in MakeOccupiedMap function, you do not need to use the catch statement
                //Debug.LogError("index of out ranges");
                //Debug.LogError(collisionPoint.x + ",  " + collisionPoint.z);
                //Debug.LogError("z : " + Mathf.RoundToInt((MatchToNearest(collisionPoint.z) + offsetY) / resolution) + ", x: " + Mathf.RoundToInt((MatchToNearest(collisionPoint.x) + offsetX) / resolution));
                //Debug.LogError(occupiedMap.GetLength(0) + " , " + occupiedMap.GetLength(1));
                    
            }
        }
    }
    void SaveArrayToTxtFile(bool[,] array)
    {
        string txtfileName = filename + ".txt";
        if (Directory.Exists(path))
        {
            using (StreamWriter writer = new StreamWriter(path + txtfileName))
            {
                for (int i = array.GetLength(0) - 1; i >= 0; i--)
                {
                    for (int j = 0; j < array.GetLength(1); j++)
                    {
                        writer.Write(array[i, j] ? "1" : "0");
                        writer.Write(" ");
                    }
                    writer.WriteLine();
                }
            }
            Debug.Log("saved to: " + path + txtfileName);
        }
        else
        {
            Debug.LogWarning("does not find directory : " + path);
        }
        
    }
   
    public void SaveAsP5Binary(bool[,] data)
    {
        if (Directory.Exists(path))
        {
            // P5 format header
            string header = $"P5\n{data.GetLength(1)} {data.GetLength(0)}\n255\n";

            // image data create by byte
            MemoryStream imageData = new MemoryStream();
            for (int y = data.GetLength(0) - 1; y >= 0; y--)
            {
                for (int x = 0; x < data.GetLength(1); x++)
                {
                    //반대아닌가?.. 
                    byte pixelValue = (byte)(data[y, x] == true ? 0 : 254);
                    imageData.WriteByte(pixelValue);
                }
            }

            // write header, image data to file
            using (FileStream fileStream = new FileStream(path + filename + ".pgm", FileMode.Create))
            {
                byte[] headerBytes = System.Text.Encoding.ASCII.GetBytes(header);
                fileStream.Write(headerBytes, 0, headerBytes.Length);
                imageData.WriteTo(fileStream);
            }
            Debug.Log("saved to: " + path + filename + ".pgm");

        }
        else
        {
            Debug.LogWarning("does not find directory : " + path);
        }
        
    }
    
    public void CreateYamlFile() 
    {
        if (Directory.Exists(path))
        {
            using (StreamWriter writer = new StreamWriter(path + filename + ".yaml"))
            {
                writer.WriteLine("image: "  + filename + ".pgm");
                writer.WriteLine("resolution: " + resolution);
                writer.WriteLine("origin: " + $"[{-width / 2}, {-height / 2}, 0 ]");
                writer.WriteLine("negate: " + 0);

                //thresh default value,  maybe it's OK to use occupied_thresh = 0.99, free_tresh =  0.01
                writer.WriteLine("occupied_thresh: " + 0.65f);
                writer.WriteLine("free_thresh: " + 0.25f);
            }
            Debug.Log("saved to: " + path + filename + ".yaml");

        }
        else
        {
            Debug.LogWarning("does not find directory : " + path);

        }
    }

}
