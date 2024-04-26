using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class VisualizeViewCone : MonoBehaviour
{
    [SerializeField]
    private NaddiViewField _viewField;
    private Mesh _visionConeMesh;
    private MeshFilter _meshFiler;
    [SerializeField]
    private Material _visionConeMaterial;
    private float _viewDistance;
    private float _coneAngle;
    [SerializeField]
    private LayerMask VisionObstructingLayer;
    [SerializeField]
    private int _visionConeResolution = 120;

    void Start()
    {
        transform.GetComponent<MeshRenderer>().material = _visionConeMaterial;
        _meshFiler = GetComponent<MeshFilter>(); 
        _visionConeMesh = new Mesh();
        _coneAngle = _viewField.HalfAngleDegree * 2;
        _coneAngle *= Mathf.Deg2Rad;
        _viewDistance = _viewField.ConeRadius;
    } 


    void Update()
    {
        DrawVisionCone();
    }

    void DrawVisionCone()
    {
        int[] triangles = new int[(_visionConeResolution - 1) * 3];
        Vector3[] Vertices = new Vector3[_visionConeResolution + 1];
        Vertices[0] = Vector3.zero;
        float Currentangle = -_coneAngle / 2;
        float angleIcrement = _coneAngle / (_visionConeResolution - 1);
        float Sine;
        float Cosine;

        for (int i = 0; i < _visionConeResolution; i++)
        {
            Sine = Mathf.Sin(Currentangle);
            Cosine = Mathf.Cos(Currentangle);
            Vector3 RaycastDirection = (transform.forward * Cosine) + (transform.right * Sine);
            Vector3 VertForward = (Vector3.forward * Cosine) + (Vector3.right * Sine);

            //check if there is an Obstacle in range
            if (Physics.Raycast(transform.position, RaycastDirection, out RaycastHit hit, _viewDistance, VisionObstructingLayer))
            {
                //if thats the case, vertex lenght is the distance from origin to the hit point of the raycast 
                Vertices[i + 1] = VertForward * hit.distance;
            }
            else
            {
                //if thats not the case, vertex lenght is the distance from origin times the view distance 
                Vertices[i + 1] = VertForward * _viewDistance;
            }
            Currentangle += angleIcrement;
        }
        for (int i = 0, j = 0; i < triangles.Length; i += 3, j++)
        {
            triangles[i] = 0;
            triangles[i + 1] = j + 1;
            triangles[i + 2] = j + 2;
        }
        _visionConeMesh.Clear(); // clears old mesh data
        //"instantiate" mesh with new triangle and vertice data. 
        _visionConeMesh.vertices = Vertices;
        _visionConeMesh.triangles = triangles;
        _meshFiler.mesh = _visionConeMesh;
    }

}
