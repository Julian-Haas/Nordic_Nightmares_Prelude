//The person responsible for this code is Nils Oskar Henningsen 
using UnityEngine;


public class VisualizeViewCone : MonoBehaviour
{
    
    [SerializeField, Header("Remove this Script in the final build. This is just for Debugging!")]
    private NaddagilViewingSensor _viewField;
    [SerializeField]
    private Material _visionConeMaterial;
    [SerializeField]
    private int _visionConeResolution = 120;
    [SerializeField]
    private LayerMask VisionObstructingLayer;

    private float _viewDistance;
    private Mesh _visionConeMesh;
    private MeshFilter _meshFilter;
    private float _coneAngle;

    void Start()
    {
        transform.GetComponent<MeshRenderer>().material = _visionConeMaterial;
        _meshFilter = GetComponent<MeshFilter>(); 
        _visionConeMesh = new Mesh();
        _coneAngle = _viewField.HalfAngleDegree * 2;
        _coneAngle *= Mathf.Deg2Rad;
        _viewDistance = _viewField.ConeRadius;
    } 


    void Update()
    {
#if UNITY_EDITOR
        DrawVisionCone();
#endif
    }

    void DrawVisionCone()
    {
        int[] triangles = new int[(_visionConeResolution - 1) * 3];
        Vector3[] vertices = new Vector3[_visionConeResolution + 1];
        vertices[0] = Vector3.zero;
        float currentAngle = -_coneAngle / 2;
        float angleIcrement = _coneAngle / (_visionConeResolution - 1);
        float sinus;
        float cosinus;

        for (int i = 0; i < _visionConeResolution; i++)
        {
            sinus = Mathf.Sin(currentAngle);
            cosinus = Mathf.Cos(currentAngle);
            Vector3 raycastDirection = (transform.forward * cosinus) + (transform.right * sinus);
            Vector3 vertForward = (Vector3.forward * cosinus) + (Vector3.right * sinus);

            //check if there is an Obstacle in range
            if (Physics.Raycast(transform.position, raycastDirection, out RaycastHit hit, _viewDistance, VisionObstructingLayer))
            {
                //if thats the case, vertex lenght is the distance from origin to the hit point of the raycast 
                vertices[i + 1] = vertForward * hit.distance;
            }
            else
            {
                //if thats not the case, vertex lenght is the distance from origin times the view distance 
                vertices[i + 1] = vertForward * _viewDistance;
            }
            currentAngle += angleIcrement;
        }
        for (int i = 0, j = 0; i < triangles.Length; i += 3, j++)
        {
            triangles[i] = 0;
            triangles[i + 1] = j + 1;
            triangles[i + 2] = j + 2;
        }
        _visionConeMesh.Clear(); // clears old mesh data
        //"instantiate" mesh with new triangle and vertice data. 
        _visionConeMesh.vertices = vertices;
        _visionConeMesh.triangles = triangles;
        _meshFilter.mesh = _visionConeMesh;
    }

}
