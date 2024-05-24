//Responsible for this script: Tony Meis
using System.Collections.Generic;
//using System.Drawing;
using UnityEditor;
using UnityEngine;

public class SeashellBehaviour : MonoBehaviour
{
    [Header("Adjustments for Seashells")]
    [Range(1,25)]
    public int NumberOfSeashells = 5;
    [Range (0.01f,0.5f)]
    public float MinimumSeashellSize = 0.1f;
    [Range (0.51f,1.0f)]
    public float MaximumSeashellSize = 2.0f;
    [Range(0.1f,10f)]
    public float RadiusOfArea = 1.0f;
    public bool RandomizeSeashells = true;

    [SerializeField] GameObject seashell;
    [SerializeField] GameObject EMPTY;

    public List<GameObject> _seashells;

    void Start()
    {
        if (RandomizeSeashells) 
        { 
            ResetSeashells();
        }
    }

    void ResetSeashells()
    {
        seashell.SetActive(true);
        ClearSeashellList();

        for (int i = 0; i < NumberOfSeashells; i++)
        {
            GameObject newShell = Instantiate(EMPTY);
            RandomizeSeashellTransforms(newShell);
            _seashells.Add(newShell);
        }
        RandomizeSeashells = false;
        seashell.SetActive(false);
    }

    void ClearSeashellList()
    {
        if ( _seashells.Count >0 )
        {
            foreach (GameObject obj in _seashells)
            {
                if (obj != null)
                {
                    DestroyImmediate(obj);
                }
            }
            _seashells.Clear();
        }
    }

    void RandomizeSeashellTransforms(GameObject obj)
    {
        Vector3 randPos = GenerateRandomPosition();
        float randomAngle = Random.Range(0f, 360f);
        Quaternion randomRotation = Quaternion.Euler(0f, randomAngle, 0f);
        float randomScale = Random.Range(MinimumSeashellSize,MaximumSeashellSize);

        obj.transform.position = randPos;
        obj.transform.rotation = randomRotation;
        obj.transform.localScale = new Vector3(1.0f,1.0f,1.0f) * randomScale;

        int loopCount = 0;
        while (loopCount < 25 && !IsPositionValid(obj))
        {
            obj.transform.position = GenerateRandomPosition();
            loopCount++;
        }
    }

    Vector3 GenerateRandomPosition()
    {
        float randomAngle = Random.Range(0f, Mathf.PI * 2f);
        float randomRadius = Mathf.Sqrt(Random.value) * RadiusOfArea;

        Vector3 randPos = new Vector3(transform.position.x + randomRadius * Mathf.Cos(randomAngle), transform.position.y, transform.position.z + randomRadius * Mathf.Sin(randomAngle)); ;

        return randPos;
    }

    bool IsPositionValid(GameObject check) 
    { 
        foreach(GameObject obj in _seashells)
        {
            float dist = Vector3.Distance(check.transform.position, obj.transform.position);
            if ((check.transform.localScale.x + obj.transform.localScale.x) > dist  )
            {
                return false;
            }
        }
        return true;
    }

    void OnDrawGizmos()
    {
        Handles.color = Color.red;
        Handles.DrawWireDisc(transform.position, transform.up.normalized, RadiusOfArea);

        if (RandomizeSeashells)
        {
            ResetSeashells();

        }

        if (_seashells.Count > 0)
        {
            foreach(var v in _seashells)
            {
                if (v)
                {
                    Gizmos.color = Color.white;
                    Matrix4x4 cubeTransform = Matrix4x4.TRS(v.transform.position, v.transform.rotation, v.transform.localScale);
                    Gizmos.matrix = cubeTransform;
                    Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
                }
            }
        }
    }
}