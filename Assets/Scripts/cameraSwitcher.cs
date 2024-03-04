using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraSwitcher : MonoBehaviour
{
    public Camera camera1;
    public Camera camera2;
    public Camera camera3;
    public Camera camera4;
 
    void Start(){
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            camera1.enabled = true;
            camera2.enabled = false;
            camera3.enabled = false;
            camera4.enabled = false;
            camera1.rect = new Rect(0f, 0f, 1f, 1f);
            camera2.rect = new Rect(0f, 0f, 0f, 0f);
            camera3.rect = new Rect(0f, 0f, 0f, 0f);
            camera4.rect = new Rect(0f, 0f, 0f, 0f);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            camera1.enabled = false;
            camera2.enabled = true;
            camera3.enabled = false;
            camera4.enabled = false;
            camera1.rect = new Rect(0f, 0f, 0f, 0f);
            camera2.rect = new Rect(0f, 0f, 1f, 1f);
            camera3.rect = new Rect(0f, 0f, 0f, 0f);
            camera4.rect = new Rect(0f, 0f, 0f, 0f);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            camera1.enabled = false;
            camera2.enabled = false;
            camera3.enabled = true;
            camera4.enabled = false;
            camera1.rect = new Rect(0f, 0f, 0f, 0f);
            camera2.rect = new Rect(0f, 0f, 0f, 0f);
            camera3.rect = new Rect(0f, 0f, 1f, 1f);
            camera4.rect = new Rect(0f, 0f, 0f, 0f);
            

        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            camera1.enabled = false;
            camera2.enabled = false;
            camera3.enabled = false;
            camera4.enabled = true;
            camera1.rect = new Rect(0f, 0f, 0f, 0f);
            camera2.rect = new Rect(0f, 0f, 0f, 0f);
            camera3.rect = new Rect(0f, 0f, 0f, 0f);
            camera4.rect = new Rect(0f, 0f, 1f, 1f);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            camera1.enabled = true;
            camera2.enabled = true;
            camera3.enabled = true;
            camera4.enabled = true;

            // 첫 번째 카메라의 Viewport 설정 (화면의 왼쪽 절반)
            camera1.rect = new Rect(0f, 0.5f, 0.5f, 0.5f);
            camera2.rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
            // 두 번째 카메라의 Viewport 설정 (화면의 오른쪽 절반)
            camera3.rect = new Rect(0f, 0f, 0.5f, 0.5f);
            camera4.rect = new Rect(0.5f, 0f, 0.5f, 0.5f);
        }
        
        
    }
    	

}
