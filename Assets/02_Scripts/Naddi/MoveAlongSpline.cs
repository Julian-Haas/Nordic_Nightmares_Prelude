using UnityEngine;
using UnityEngine.Splines;

public class MoveAlongSpline : MonoBehaviour
{
    public SplineContainer spline;
    public float moveSpeed = 1f;
    public float rotationSpeed = 5f;

    private float currentDistance = 0f;

    public void ForceCurrentPosition(float percentageSplineDistance)
    { 
        currentDistance = percentageSplineDistance;
        transform.position = spline.EvaluatePosition(currentDistance);
    }

    public void ManualUpdate()
    {
        //calculate target position on spline
        Vector3 targetPosition = spline.EvaluatePosition(currentDistance);

        //move objects towards target position on spline
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        //calculate target rotation on spline
        Vector3 targetDirection = spline.EvaluateTangent(currentDistance);

        //rotate object towards target rotation on spline
        if (targetDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection, transform.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        //if reached end of spline, loop back to beginning
        if (currentDistance >= 1f)
        {
            currentDistance = 0f;
        }
        else
        {
            //adjust movement based on length of spline
            float splineLength = spline.CalculateLength();
            float movement = moveSpeed * Time.deltaTime / splineLength;
            currentDistance += movement;
        }
    }


}