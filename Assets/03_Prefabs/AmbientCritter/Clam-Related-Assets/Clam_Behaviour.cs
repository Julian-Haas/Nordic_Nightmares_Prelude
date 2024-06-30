//Responsible for this script: Tony Meis
using System.Collections.Generic;
using System.Collections;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class Clam_Behaviour : MonoBehaviour
{
    [Header("Adjustments for Clams")]
    [Range(1, 25)]
    public int NumberOfClams = 5;
    [Range(0.01f, 0.5f)]
    public float MinimumClamSize = 0.1f;
    [Range(0.51f, 1.0f)]
    public float MaximumClamSize = 2.0f;
    [Range(0.1f, 10f)]
    public float RadiusOfArea = 1.0f;
    [Range(0f, 10f)]
    public float ReactionArea = 2.0f;

    [SerializeField] private bool RandomizeClams = false;
    [SerializeField] private List<GameObject> _Clams;

    private GameObject Clam;
    private SphereCollider _reactionArea;

    void Start()
    {
        _reactionArea = transform.GetComponent<SphereCollider>();
        _reactionArea.radius = RadiusOfArea + ReactionArea;
        Clam = transform.GetChild(1).gameObject;

        _Clams.Clear();
        int shellcount = transform.GetChild(0).childCount;
        for(int i = 0; i < shellcount; i++)
        {
            _Clams.Add(transform.GetChild(0).GetChild(i).gameObject);
        }
    }

    void ResetClams()
    {
#if UNITY_EDITOR
        Clam.SetActive(true);
        ClearClamList();
        if (!Clam)
        {
            Clam = transform.GetChild(1).gameObject;
        }
        for (int i = 0; i < NumberOfClams; i++)
        {
            GameObject newShell = Instantiate(Clam, transform.GetChild(0));
            RandomizeClamTransforms(newShell);
            _Clams.Add(newShell);
        }
        Clam.SetActive(false);
#endif
        RandomizeClams = false;
    }

    void ClearClamList()
    {
        _Clams.RemoveAll(shell => shell == null);

        while (_Clams.Count > 0)
        {
            DestroyImmediate(_Clams[0]);
            _Clams.RemoveAt(0);
        }
        _Clams.Clear();

        int shellcount = transform.GetChild(0).childCount;
        for (int i = 0; i < shellcount; i++)
        {
            DestroyImmediate(transform.GetChild(0).GetChild(0));
        }
    }

    void RandomizeClamTransforms(GameObject obj)
    {
        Vector3 randPos = GenerateRandomPosition();
        float randomAngle = Random.Range(0f, 360f);
        Quaternion randomRotation = Quaternion.Euler(0f, randomAngle, 0f);
        float randomScale = Random.Range(MinimumClamSize, MaximumClamSize);

        obj.transform.position = randPos;
        obj.transform.rotation = randomRotation;
        obj.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f) * randomScale * 0.5f;

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
        foreach (GameObject obj in _Clams)
        {
            float dist = Vector3.Distance(check.transform.position, obj.transform.position);
            if ((check.transform.localScale.x + obj.transform.localScale.x) > dist)
            {
                return false;
            }
        }
        return true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Application.isPlaying && other.CompareTag("Player"))
        {
            foreach (var v in _Clams)
            {
                StartCoroutine(DelayAnimation(v, "Armature|Close_Mouth"));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (Application.isPlaying && other.CompareTag("Player"))
        {
            foreach (var v in _Clams)
            {
                StartCoroutine(DelayAnimation(v, "Armature|Open_Mouth"));
            }
        }
    }

    IEnumerator DelayAnimation(GameObject shell, string animation)
    {
        float tmp = Random.Range(0f, 1f);

        yield return new WaitForSeconds(tmp);

        shell.GetComponent<Animator>().Play(animation);
    }

    void OnDrawGizmos()
    {
      //  Handles.color = Color.red;
      //  Handles.DrawWireDisc(transform.position, transform.up.normalized, RadiusOfArea);
      //  Handles.color = Color.green;
      //  Handles.DrawWireDisc(transform.position, transform.up.normalized, RadiusOfArea + ReactionArea);
      //
        if (RandomizeClams)
        {
            if (!Application.isPlaying)
            {
                ResetClams();
            }
            RandomizeClams = false;
        }
    }
}