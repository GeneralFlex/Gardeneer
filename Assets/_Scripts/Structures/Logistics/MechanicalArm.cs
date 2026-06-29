using NUnit.Framework;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class MechanicalArm : MonoBehaviour
{
    [Header("Components")]
    public Transform armBase;
    public Transform armSegment1;
    private Transform arm1End;
    public Transform armSegment2;
    private Transform arm2End;
    public Transform grabber;
    public Transform grabber1;
    public Transform grabber2;
    [Header("Stats")]
    public float minArmLength = 1f;
    public float maxArmLength = 1f;
    private float currentArmLength = 1f;
    public float minSpeed = 5f;
    public float maxSpeed = 20f;
    public float smoothDragTime = 0.1f;

    [Header("Movement")]
    private float segment1Angle;
    private float segment2Angle;

    [Header("Private")]
    private Vector2 elevatedTarget;
    private Vector2 target;
    private Vector2 pivot;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        arm1End = armSegment1.Find("end");
        arm2End = armSegment2.Find("end");
        target = new Vector2(armBase.position.x - 0.3f, armBase.position.y + 0.4f);
    }

    // Update is called once per frame
    void Update()
    {
        //target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        moveArmToPos();
    }

    public void moveArmToPos()
    {
        // Create an elevated target above target using pythagoras based on arm length and distance
        float extent = (Vector2.Distance(target,armBase.position) - minArmLength * 2f) * 0.3f;
        //currentArmLength = Mathf.Clamp(extent, minArmLength, maxArmLength);
        currentArmLength = Mathf.Lerp(minArmLength, maxArmLength, Mathf.InverseLerp(minArmLength,maxArmLength*2,Vector2.Distance(target, armBase.position)));

        if (Vector2.Distance(target, armBase.position) > 2 * currentArmLength)
        {
            target = new Vector2(armBase.position.x - 0.3f, armBase.position.y + 0.4f);
            currentArmLength = minArmLength;
        }

        elevatedTarget = target + new Vector2(0, Mathf.Sqrt(Mathf.Pow((2*currentArmLength),2)-Mathf.Pow(Vector2.Distance(target,armBase.position),2)));
        pivot = ((Vector2)armSegment1.position + elevatedTarget) * 0.5f;

        float currentSpeed = Mathf.Clamp(Vector2.Distance(target, arm2End.position) * 2, minSpeed, maxSpeed);

        segment1Angle = Mathf.MoveTowardsAngle(segment1Angle, CalculateAngle(elevatedTarget, armSegment1.position).eulerAngles.z-180, currentSpeed * Time.deltaTime * currentSpeed);
        segment2Angle = Mathf.MoveTowardsAngle(segment2Angle, CalculateAngle(target, armSegment2.position).eulerAngles.z, currentSpeed * Time.deltaTime * currentSpeed);

        armSegment1.rotation = Quaternion.Euler(0, 0, segment1Angle);
        armSegment2.rotation = Quaternion.Euler(0, 0, segment2Angle);

        //armSegment1.localScale = new Vector2(1, Mathf.MoveTowards(armSegment1.localScale.y, Vector2.Distance(armSegment1.position, pivot) / armLength, Time.deltaTime*(1+ Mathf.Abs(armSegment1.localScale.y- Vector2.Distance(armSegment1.position, pivot) / armLength))));
        //armSegment2.localScale = new Vector2(1, Mathf.MoveTowards(armSegment2.localScale.y, Vector2.Distance(target, pivot) / armLength, Time.deltaTime * (1 + Mathf.Abs(armSegment2.localScale.y - Vector2.Distance(target, pivot) / armLength))));
        
        //extend 1. arm segment
        float segment1length = Vector2.Distance(armSegment1.position, pivot);
        Transform shaft1 = armSegment1.Find("shaft");
        shaft1.localScale = new Vector3(shaft1.localScale.x, Mathf.MoveTowards(shaft1.localScale.y, segment1length*2, Time.deltaTime*currentSpeed));
        Transform gantry1_1 = armSegment1.Find("gantry 1");
        Transform gantry1_2 = armSegment1.Find("gantry 2");
        shaft1.localPosition = new Vector3(shaft1.localPosition.x, shaft1.localScale.y/4, shaft1.localPosition.z);
        gantry1_2.localPosition = new Vector3(gantry1_2.localPosition.x, shaft1.localPosition.y + shaft1.localScale.y / 4, gantry1_2.localPosition.z);
        
        arm1End.localPosition = gantry1_2.localPosition;
        Transform joint = armSegment1.Find("joint");
        joint.localPosition = new Vector3(joint.localPosition.x, gantry1_2.localPosition.y, joint.localPosition.z);

        armSegment2.position = new Vector3(arm1End.position.x, arm1End.position.y, armSegment2.position.z);

        //extend 2. arm segment
        float segment2length = Vector2.Distance(target, pivot);
        Transform shaft2 = armSegment2.Find("shaft");
        shaft2.localScale = new Vector3(shaft2.localScale.x, Mathf.MoveTowards(shaft2.localScale.y, segment2length*2, Time.deltaTime * currentSpeed));
        Transform gantry2_1 = armSegment2.Find("gantry 1");
        Transform gantry2_2 = armSegment2.Find("gantry 2");
        shaft2.localPosition = new Vector3(shaft2.localPosition.x, -shaft2.localScale.y / 4, shaft2.localPosition.z);
        gantry2_2.localPosition = new Vector3(gantry2_2.localPosition.x, shaft2.localPosition.y - shaft2.localScale.y / 4, gantry2_2.localPosition.z);

        arm2End.localPosition = gantry2_2.localPosition;

        grabber.position = new Vector3(arm2End.position.x, arm2End.position.y, grabber.position.z);
    }

    public Quaternion CalculateAngle(Vector2 target, Vector2 origin)
    {
        Vector2 direction = target - origin;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        return Quaternion.Euler(0, 0, (90+angle)).normalized;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(elevatedTarget, 0.05f);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(target, 0.1f);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(armSegment1.position, pivot);
        Gizmos.DrawLine(target, pivot);

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(pivot, 0.05f);
        Gizmos.DrawSphere(target, 0.05f);

        Gizmos.color = Color.darkMagenta;
        Gizmos.DrawWireSphere(armBase.position, 2 * currentArmLength);

        Gizmos.color = Color.purple;
        Gizmos.DrawWireSphere(armBase.position, 2 * maxArmLength);
    }
}
