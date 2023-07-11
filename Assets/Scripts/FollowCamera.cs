using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target; // 카메라가 따라갈 대상 오브젝트
    public Transform pivotPoint; // 회전 기준이 될 점
    public float rotationSpeed = 1.0f; // 회전 속도
    public Vector3 offset;


    void Update()
    {
        // 대상 오브젝트의 위치에 따라 카메라를 이동시킴
        //transform.position = target.position + offset;

        //// pivotPoint를 기준으로 카메라 회전
        //Vector3 directionToPivot = pivotPoint.position - transform.position;
        //Quaternion targetRotation = Quaternion.LookRotation(directionToPivot);
        //Vector3 rot = new Vector3(transform.rotation.x, target.rotation.y, transform.rotation.z);
        //transform.rotation = Quaternion.Euler(rot);

        Vector3 directionToPivot = transform.position - pivotPoint.position;
        Quaternion rotation = Quaternion.AngleAxis(rotationSpeed * Time.deltaTime, directionToPivot);
        transform.position = pivotPoint.position + rotation * (transform.position - pivotPoint.position);
    }

}
