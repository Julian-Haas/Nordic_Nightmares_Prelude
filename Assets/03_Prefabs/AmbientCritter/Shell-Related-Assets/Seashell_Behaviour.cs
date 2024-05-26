//Responsible for this script: Tony Meis
using System.Collections.Generic;
using System.Collections;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class Seashell_Behaviour : MonoBehaviour
{
    [Header("Adjustments for Seashells")]
    [Range(1, 25)]
    public int NumberOfSeashells = 5;
    [Range(0.01f, 0.5f)]
    public float MinimumSeashellSize = 0.1f;
    [Range(0.51f, 1.0f)]
    public float MaximumSeashellSize = 2.0f;
    [Range(0.1f, 10f)]
    public float RadiusOfArea = 1.0f;
    [Range(0f, 10f)]
    public float ReactionArea = 2.0f;

    [SerializeField] private bool RandomizeSeashells = false;
    [SerializeField] private List<GameObject> _seashells;

    private GameObject seashell;
    private SphereCollider _reactionArea;

    void Start()
    {
        _reactionArea = transform.GetComponent<SphereCollider>();
        _reactionArea.radius = RadiusOfArea + ReactionArea;
        seashell = transform.GetChild(1).gameObject;

        _seashells.Clear();
        int shellcount = transform.GetChild(0).childCount;
        for(int i = 0; i < shellcount; i++)
        {
            _seashells.Add(transform.GetChild(0).GetChild(i).gameObject);
        }
    }

    void ResetSeashells()
    {
#if UNITY_EDITOR
        seashell.SetActive(true);
        ClearSeashellList();
        if (!seashell)
        {
            seashell = transform.GetChild(1).gameObject;
        }
        for (int i = 0; i < NumberOfSeashells; i++)
        {
            GameObject newShell = Instantiate(seashell, transform.GetChild(0));
            RandomizeSeashellTransforms(newShell);
            _seashells.Add(newShell);
        }
        seashell.SetActive(false);
#endif
        RandomizeSeashells = false;
    }

    void ClearSeashellList()
    {
        _seashells.RemoveAll(shell => shell == null);

        while (_seashells.Count > 0)
        {
            DestroyImmediate(_seashells[0]);
            _seashells.RemoveAt(0);
        }
        _seashells.Clear();

        int shellcount = transform.GetChild(0).childCount;
        for (int i = 0; i < shellcount; i++)
        {
            DestroyImmediate(transform.GetChild(0).GetChild(0));
        }
    }

    void RandomizeSeashellTransforms(GameObject obj)
    {
        Vector3 randPos = GenerateRandomPosition();
        float randomAngle = Random.Range(0f, 360f);
        Quaternion randomRotation = Quaternion.Euler(0f, randomAngle, 0f);
        float randomScale = Random.Range(MinimumSeashellSize, MaximumSeashellSize);

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
        foreach (GameObject obj in _seashells)
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
            foreach (var v in _seashells)
            {
                StartCoroutine(DelayAnimation(v, "Armature|Close_Mouth"));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (Application.isPlaying && other.CompareTag("Player"))
        {
            foreach (var v in _seashells)
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
        Handles.color = Color.red;
        Handles.DrawWireDisc(transform.position, transform.up.normalized, RadiusOfArea);
        Handles.color = Color.green;
        Handles.DrawWireDisc(transform.position, transform.up.normalized, RadiusOfArea + ReactionArea);

        if (RandomizeSeashells)
        {
            if (!Application.isPlaying)
            {
                ResetSeashells();
            }
            RandomizeSeashells = false;
        }
    }
}