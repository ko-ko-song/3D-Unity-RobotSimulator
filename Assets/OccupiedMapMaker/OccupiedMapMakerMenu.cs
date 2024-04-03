using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class OccupiedMapMakerMenu : MonoBehaviour
{
    [MenuItem("OccupiedMapMaker/MakeOccupiedMap")]
    static void MakeOccupiedMap()
    {
        OccupiedMapMaker occupiedMapMaker = Selection.activeTransform.gameObject.GetComponent<OccupiedMapMaker>();
        occupiedMapMaker.MakeOccupiedMap();
    }
    [MenuItem("OccupiedMapMaker/MakeOccupiedMap", isValidateFunction: true)]
    static bool ValidateSelectedGameObjectHasOccupiedMapMaker()
    {
        OccupiedMapMaker occupiedMapMaker = Selection.activeTransform?.gameObject.GetComponent<OccupiedMapMaker>();
        return occupiedMapMaker != null;
    }

}
