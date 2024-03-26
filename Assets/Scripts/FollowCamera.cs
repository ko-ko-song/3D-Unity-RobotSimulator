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
        Vector3 directionToPivot = transform.position - pivotPoint.position;
        Quaternion rotation = Quaternion.AngleAxis(rotationSpeed * Time.deltaTime, directionToPivot);
        transform.position = pivotPoint.position + rotation * (transform.position - pivotPoint.position);
    }

}
