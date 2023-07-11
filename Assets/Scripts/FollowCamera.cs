using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target; // ī�޶� ���� ��� ������Ʈ
    public Transform pivotPoint; // ȸ�� ������ �� ��
    public float rotationSpeed = 1.0f; // ȸ�� �ӵ�
    public Vector3 offset;


    void Update()
    {
        // ��� ������Ʈ�� ��ġ�� ���� ī�޶� �̵���Ŵ
        //transform.position = target.position + offset;

        //// pivotPoint�� �������� ī�޶� ȸ��
        //Vector3 directionToPivot = pivotPoint.position - transform.position;
        //Quaternion targetRotation = Quaternion.LookRotation(directionToPivot);
        //Vector3 rot = new Vector3(transform.rotation.x, target.rotation.y, transform.rotation.z);
        //transform.rotation = Quaternion.Euler(rot);

        Vector3 directionToPivot = transform.position - pivotPoint.position;
        Quaternion rotation = Quaternion.AngleAxis(rotationSpeed * Time.deltaTime, directionToPivot);
        transform.position = pivotPoint.position + rotation * (transform.position - pivotPoint.position);
    }

}
