using UnityEngine;

public class SphereFloat : MonoBehaviour
{
    public float floatHeight = 0.1f; // Height of the floating motion
    public float floatSpeed = 1.0f; // Speed of the floating motion

    private Vector3 startPosition;

    void Start() {
        // Store the initial position of the sphere
        startPosition = transform.position;
    }

    void Update() {
        // Calculate the vertical offset using a sine wave
        float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatHeight;

        // Update the position of the sphere with the vertical offset
        transform.position = startPosition + new Vector3(0f,yOffset,0f);
    }
}


//using UnityEngine;

//public class SphereFloat : MonoBehaviour
//{
//    public float floatHeight = 1.0f; // Height of the floating motion
//    public float floatSpeed = 1.0f; // Speed of the floating motion

//    private Vector3 startPosition;
//    private Vector3 targetPosition;

//    void Start() {
//        // Store the initial position of the sphere
//        startPosition = transform.position;

//        // Calculate the target position based on the float height
//        targetPosition = startPosition + Vector3.up * floatHeight;
//    }

//    void Update() {
//        // Calculate the new position using linear interpolation
//        float t = Mathf.PingPong(Time.time * floatSpeed,1.0f);
//        transform.position = Vector3.Lerp(startPosition,targetPosition,t);
//    }
//}
