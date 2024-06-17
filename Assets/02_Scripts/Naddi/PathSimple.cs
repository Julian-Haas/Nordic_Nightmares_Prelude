using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;

public class PathSimple : MonoBehaviour
{
    [SerializeField] NaddiSimple _naddi;
    [SerializeField] MoveAlongSpline _moveSpline;
    [SerializeField] SplineContainer _spline;

    void ActivatePath()
    {
        GameObject.Find("SimpleNaddiManager").GetComponent<NaddiSimpleActive>().SetActiveNaddi(this, _naddi);
        _naddi.gameObject.SetActive(true);
        _moveSpline.moveSpeed = _naddi.PathWalkSpeed;
    }

    public void DeactivatePath()
    {
        _naddi.gameObject.SetActive(false);
        _moveSpline.moveSpeed = 0.0f;
    }

    public void StartFurthestPoint(Vector3 _playerPos)
    {
        Vector3 furthestKnotPos = Vector3.zero;
        float furthestDistance = 0.0f;
        foreach(BezierKnot knot in _spline.Spline)
        {
            if(Vector3.Distance((knot.Position + (float3)_spline.transform.position), _playerPos) > furthestDistance)
            {
                furthestKnotPos = knot.Position;
                furthestDistance = Vector3.Distance((knot.Position + (float3)_spline.transform.position), _playerPos);
            }
            //Debug.Log("Knot " + knot.Position);
        }
        SplineUtility.GetNearestPoint<Spline>(_spline.Spline, (float3)furthestKnotPos, out float3 furthestKnot, out float percentageDistOnSpline);
        //Debug.Log("FurthestKnot " + furthestKnot);
        //Debug.Log( "SplineContainer Transform Position"+_spline.transform.position);
        furthestKnot = _spline.transform.position + _spline.transform.rotation * furthestKnot;
        //Debug.Log("FurthestKnot rotated " + furthestKnot);
        _moveSpline.ForceCurrentPosition(percentageDistOnSpline);
        _naddi.transform.position = new Vector3(furthestKnot.x, _naddi.transform.position.y, furthestKnot.z);
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player")
        {
            ActivatePath();
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.tag == "Player")
        {
            //DeactivatePath();
            _naddi.TrackBackToPath();
        }
    }
}
