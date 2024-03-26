using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility : MonoBehaviour
{

    public IEnumerator WaitForCoroutineToEnd(IEnumerator coroutine, System.Action callback)
    {
        yield return StartCoroutine(coroutine);
        callback(); 
    }
}
