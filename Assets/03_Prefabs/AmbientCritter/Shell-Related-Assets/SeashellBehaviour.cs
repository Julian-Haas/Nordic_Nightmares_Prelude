using System.Collections.Generic;
using System.Collections;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class SeashellBehaviour : MonoBehaviour
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
    public bool RandomizeSeashells = false;

    SphereCollider _reactionArea;
    public GameObject seashell;

    public List<GameObject> _seashells;

    void Start()
    {
        _reactionArea = transform.GetComponent<SphereCollider>();
        _reactionArea.radius = RadiusOfArea + ReactionArea;
    }

    void ResetSeashells()
    {
#if UNITY_EDITOR
        seashell.SetActive(true);
        ClearSeashellList();

        for (int i = 0; i < NumberOfSeashells; i++)
        {
            GameObject newShell = Instantiate(seashell);
            RandomizeSeashellTransforms(newShell);
            _seashells.Add(newShell);
        }
        RandomizeSeashells = false;
        seashell.SetActive(false);
#endif
    }

    void ClearSeashellList()
    {
#if UNITY_EDITOR
        _seashells.RemoveAll(shell => shell == null);

        while (_seashells.Count > 0)
        {
            DestroyImmediate(_seashells[0]);
            _seashells.RemoveAt(0);
        }
        _seashells.Clear();
#endif
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

        if (RandomizeSeashells && !Application.isPlaying)
        {
            ResetSeashells();
        }
    }
}


/*
 using System.Collections.Generic;
using System.Collections;
using UnityEditor;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;

#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class SeashellBehaviour : MonoBehaviour
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
    public bool RandomizeSeashells = false;

    SphereCollider _reactionArea;
    public GameObject seashell;
    public Material Seashell_Material;
    public Mesh Seashell_Mesh;
    private static Mesh _seashellMesh;

    private Mesh GetSeashellMesh()
    {
        if (_seashellMesh == null)
        {
            // If the mesh hasn't been loaded yet, load it from a file or create it dynamically
            _seashellMesh = LoadSeashellMesh(); // Implement this method to load or create the mesh
        }

        return _seashellMesh;
    }

    private Mesh LoadSeashellMesh()
    {
        // Path to the FBX file relative to the "Assets" folder
        string fbxPath = "Assets/06_PersonalSpaces/Tony/seashell_02.fbx";

        // Load the FBX file as a GameObject
        GameObject fbxObject = Resources.Load<GameObject>(fbxPath);

        // Get the MeshFilter component from the GameObject
        MeshFilter meshFilter = fbxObject.GetComponent<MeshFilter>();

        if (meshFilter != null && meshFilter.sharedMesh != null)
        {
            // Return the shared mesh of the MeshFilter
            return meshFilter.sharedMesh;
        }
        else
        {
            Debug.LogError("Failed to load seashell mesh from " + fbxPath);
            return null;
        }
    }


    public List<GameObject> _seashells;// = new List<GameObject>();

    /// Stuff multi-threading

    // Define the compute buffer
    ComputeBuffer seashellBuffer;

    struct SeashellMaterialData
    {
        public Material material;
        public NativeArray<Color32> textureData; // Hold the texture data
        public int textureWidth; // Width of the texture
        public int textureHeight; // Height of the texture
                                  // Add other material properties as needed
    }

    struct SeashellData
    {
        public Vector3 position;
        public Vector3 scale;
        public Quaternion rotation;
    }

    struct PrepareSeashellDataJob : IJobParallelFor
    {
        public NativeArray<Vector3> positions;
        public NativeArray<Vector3> scales;
        public NativeArray<Quaternion> rotations;

        public void Execute(int index)
        {
            // No need to access GameObjects directly here
            // Use the provided NativeArrays directly
        }
    }
    struct RenderSeashellsJob : IJobParallelFor
    {
        public NativeArray<Vector3> positions;
        public NativeArray<Vector3> scales;
        public NativeArray<Quaternion> rotations;
        public SeashellMaterialData materialData;

        public void Execute(int index)
        {
            // Get the position, scale, and rotation of the current seashell
            Vector3 position = positions[index];
            Vector3 scale = scales[index];
            Quaternion rotation = rotations[index];

            // Set the transformation matrix based on the position, scale, and rotation
            Matrix4x4 matrix = Matrix4x4.TRS(position, rotation, scale);

            // Reconstruct the texture
            Texture2D texture = new Texture2D(materialData.textureWidth, materialData.textureHeight);
            texture.SetPixels32(materialData.textureData.ToArray());
            texture.Apply();

            // Set the material's texture
            materialData.material.SetTexture("_MainTex", texture);

            // Draw the seashell using the material
            Graphics.DrawMeshNow(GetSeashellMesh(), matrix);
        }

        private Mesh GetSeashellMesh()
        {
            // Access the method from the SeashellBehaviour instance stored in materialData
            return materialData.seashellBehaviour.GetSeashellMesh();
        }
    }

    /// 


    void Start()
    {
        _reactionArea = transform.GetComponent<SphereCollider>();
        _reactionArea.radius = RadiusOfArea + ReactionArea;

        // Initialize seashellBuffer
        int count = _seashells.Count;
        int stride = sizeof(float) * 10;
        seashellBuffer = new ComputeBuffer(count, stride);

        if (RandomizeSeashells || Application.isPlaying)
        {
            PrepareSeashellData();
        }
    }


    void Update()
    {
        if (RandomizeSeashells || Application.isPlaying)
        {
            PrepareSeashellData();
            RenderSeashells(Seashell_Material, Seashell_Mesh); // Pass the material here
        }
    }

    void PrepareSeashellData()
    {
        NativeArray<Vector3> positions = new NativeArray<Vector3>(_seashells.Count, Allocator.TempJob);
        NativeArray<Vector3> scales = new NativeArray<Vector3>(_seashells.Count, Allocator.TempJob);
        NativeArray<Quaternion> rotations = new NativeArray<Quaternion>(_seashells.Count, Allocator.TempJob);

        for (int i = 0; i < _seashells.Count; i++)
        {
            GameObject seashell = _seashells[i];
            positions[i] = seashell.transform.position;
            scales[i] = seashell.transform.localScale;
            rotations[i] = seashell.transform.rotation;
        }

        PrepareSeashellDataJob job = new PrepareSeashellDataJob
        {
            positions = positions,
            scales = scales,
            rotations = rotations
        };

        JobHandle handle = job.Schedule(_seashells.Count, 64);
        handle.Complete();

        positions.Dispose();
        scales.Dispose();
        rotations.Dispose();
    }

    void RenderSeashells(Material seashellMat, Mesh seashellM) // Modify the method signature
    {
        // Create material data
        Texture2D seashellTexture = (Texture2D)seashellMat.mainTexture;
        Color32[] textureData = seashellTexture.GetPixels32();
        int textureWidth = seashellTexture.width;
        int textureHeight = seashellTexture.height;


        SeashellMaterialData materialData;
        materialData.material = seashellMat;
        materialData.textureData = new NativeArray<Color32>(textureData, Allocator.TempJob);
        materialData.textureWidth = textureWidth;
        materialData.textureHeight = textureHeight;


        NativeArray<Vector3> positions = new NativeArray<Vector3>(_seashells.Count, Allocator.TempJob);
        NativeArray<Vector3> scales = new NativeArray<Vector3>(_seashells.Count, Allocator.TempJob);
        NativeArray<Quaternion> rotations = new NativeArray<Quaternion>(_seashells.Count, Allocator.TempJob);

        for (int i = 0; i < _seashells.Count; i++)
        {
            GameObject seashell = _seashells[i];
            positions[i] = seashell.transform.position;
            scales[i] = seashell.transform.localScale;
            rotations[i] = seashell.transform.rotation;
        }

        RenderSeashellsJob job = new RenderSeashellsJob
        {
            positions = positions,
            scales = scales,
            rotations = rotations,
            materialData = materialData,
            seashellBehaviour = this  // Pass the current instance of SeashellBehaviour
        };

        JobHandle handle = job.Schedule(_seashells.Count, 64);
        handle.Complete();

        positions.Dispose();
        scales.Dispose();
        rotations.Dispose();
        materialData.textureData.Dispose();
    }

    void OnDestroy()
    {
        // Release the compute buffer when the object is destroyed
        if (seashellBuffer != null)
        {
            seashellBuffer.Release();
        }
    }

    void ResetSeashells()
    {
#if UNITY_EDITOR
        seashell.SetActive(true);
        ClearSeashellList();

        for (int i = 0; i < NumberOfSeashells; i++)
        {
            GameObject newShell = Instantiate(seashell);
            RandomizeSeashellTransforms(newShell);
            _seashells.Add(newShell);
        }
        RandomizeSeashells = false;
        seashell.SetActive(false);
#endif
    }

    void ClearSeashellList()
    {
#if UNITY_EDITOR
        _seashells.RemoveAll(shell => shell == null);

        while (_seashells.Count > 0)
        {
            DestroyImmediate(_seashells[0]);
            _seashells.RemoveAt(0);
        }
        _seashells.Clear();
#endif
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

        if (RandomizeSeashells && !Application.isPlaying)
        {
            ResetSeashells();
        }
    }
}
 */

////Responsible for this script: Tony Meis
//using System.Collections.Generic;
//using System.Collections;
//using UnityEditor;
//using UnityEngine;
//using Unity.Collections;
//using Unity.Jobs;

//#if UNITY_EDITOR
//[ExecuteInEditMode]
//#endif
//public class SeashellBehaviour : MonoBehaviour
//{
//    [Header("Adjustments for Seashells")]
//    [Range(1, 25)]
//    public int NumberOfSeashells = 5;
//    [Range(0.01f, 0.5f)]
//    public float MinimumSeashellSize = 0.1f;
//    [Range(0.51f, 1.0f)]
//    public float MaximumSeashellSize = 2.0f;
//    [Range(0.1f, 10f)]
//    public float RadiusOfArea = 1.0f;
//    [Range(0f, 10f)]
//    public float ReactionArea = 2.0f;
//    public bool RandomizeSeashells = false;

//    SphereCollider _reactionArea;
//    public GameObject seashell;

//    public List<GameObject> _seashells;// = new List<GameObject>();

//    /// Stuff multi-threading

//    ComputeBuffer seashellBuffer;
//    public Material seashellMaterial;

//    public struct SeashellData
//    {
//        public Vector3 position;
//        public Vector3 scale;
//        public Quaternion rotation;
//    }

//    struct SeashellDataJob : IJobParallelFor
//    {
//        [ReadOnly] public NativeArray<int> seashells;
//        public NativeArray<SeashellData> seashellDataArray;
//        public GameObject[] seashellArray;

//        public void Execute(int index)
//        {
//            // Retrieve the GameObject reference using the index
//            GameObject seashell = seashellArray[seashells[index]];

//            // Prepare data for seashell and store it in seashellDataArray[index]
//            SeashellData data = new SeashellData
//            {
//                position = seashell.transform.position,
//                scale = seashell.transform.localScale,
//                rotation = seashell.transform.rotation
//                // Add more data preparation as needed
//            };
//            seashellDataArray[index] = data;
//        }
//    }
//    /// 

//    void Start()
//    {
//        _reactionArea = transform.GetComponent<SphereCollider>();
//        _reactionArea.radius = RadiusOfArea + ReactionArea;
//    }

//    void ResetSeashells()
//    {
//#if UNITY_EDITOR
//        seashell.SetActive(true);
//        ClearSeashellList();

//        for (int i = 0; i < NumberOfSeashells; i++)
//        {
//            GameObject newShell = Instantiate(seashell);
//            RandomizeSeashellTransforms(newShell);
//            _seashells.Add(newShell);
//        }
//        RandomizeSeashells = false;
//        seashell.SetActive(false);
//#endif
//    }

//    void ClearSeashellList()
//    {
//#if UNITY_EDITOR
//        _seashells.RemoveAll(shell => shell == null);

//        while (_seashells.Count > 0)
//        {
//            DestroyImmediate(_seashells[0]);
//            _seashells.RemoveAt(0);
//        }
//        _seashells.Clear();
//#endif
//    }

//    void RandomizeSeashellTransforms(GameObject obj)
//    {
//        Vector3 randPos = GenerateRandomPosition();
//        float randomAngle = Random.Range(0f, 360f);
//        Quaternion randomRotation = Quaternion.Euler(0f, randomAngle, 0f);
//        float randomScale = Random.Range(MinimumSeashellSize, MaximumSeashellSize);

//        obj.transform.position = randPos;
//        obj.transform.rotation = randomRotation;
//        obj.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f) * randomScale * 0.5f;

//        int loopCount = 0;
//        while (loopCount < 25 && !IsPositionValid(obj))
//        {
//            obj.transform.position = GenerateRandomPosition();
//            loopCount++;
//        }
//    }

//    Vector3 GenerateRandomPosition()
//    {
//        float randomAngle = Random.Range(0f, Mathf.PI * 2f);
//        float randomRadius = Mathf.Sqrt(Random.value) * RadiusOfArea;
//        Vector3 randPos = new Vector3(transform.position.x + randomRadius * Mathf.Cos(randomAngle), transform.position.y, transform.position.z + randomRadius * Mathf.Sin(randomAngle)); ;

//        return randPos;
//    }

//    bool IsPositionValid(GameObject check)
//    {
//        foreach (GameObject obj in _seashells)
//        {
//            float dist = Vector3.Distance(check.transform.position, obj.transform.position);
//            if ((check.transform.localScale.x + obj.transform.localScale.x) > dist)
//            {
//                return false;
//            }
//        }
//        return true;
//    }

//    private void OnTriggerEnter(Collider other)
//    {
//        if (Application.isPlaying && other.CompareTag("Player"))
//        {
//            foreach (var v in _seashells)
//            {
//                StartCoroutine(DelayAnimation(v, "Armature|Close_Mouth"));
//            }
//        }
//    }

//    private void OnTriggerExit(Collider other)
//    {
//        if (Application.isPlaying && other.CompareTag("Player"))
//        {
//            foreach (var v in _seashells)
//            {
//                StartCoroutine(DelayAnimation(v, "Armature|Open_Mouth"));
//            }
//        }
//    }

//    IEnumerator DelayAnimation(GameObject shell, string animation)
//    {
//        float tmp = Random.Range(0f, 1f);

//        yield return new WaitForSeconds(tmp);

//        shell.GetComponent<Animator>().Play(animation);
//    }


//    void PrepareSeashellData()
//    {
//        // Convert the _seashells list to an array of GameObjects
//        GameObject[] seashellsArray = _seashells.ToArray();

//        // Allocate a NativeArray to hold the prepared data
//        NativeArray<SeashellData> seashellDataArray = new NativeArray<SeashellData>(seashellsArray.Length, Allocator.TempJob);

//        // Create an array to hold the indices of _seashells
//        int[] seashellIndices = new int[seashellsArray.Length];
//        for (int i = 0; i < seashellsArray.Length; i++)
//        {
//            seashellIndices[i] = i;
//        }

//        // Define and schedule the job
//        SeashellDataJob job = new SeashellDataJob
//        {
//            seashells = new NativeArray<int>(seashellIndices, Allocator.TempJob),
//            seashellDataArray = seashellDataArray,
//            seashellArray = seashellsArray
//        };

//        JobHandle handle = job.Schedule(seashellIndices.Length, 64);

//        // Wait for the job to complete
//        handle.Complete();

//        // Allocate a compute buffer to hold seashell data
//        seashellBuffer = new ComputeBuffer(seashellDataArray.Length, sizeof(float) * 10); // We need 10 floats for position, scale, and rotation

//        // Pass data from seashellDataArray to the compute buffer
//        seashellBuffer.SetData(seashellDataArray);
//        seashellMaterial.SetBuffer("_SeashellBuffer", seashellBuffer);

//        // Now you can use the seashellDataArray for further processing or rendering
//        // Don't forget to release the NativeArrays when done
//        seashellDataArray.Dispose();
//    }

//    void OnDestroy()
//    {
//        // Release the compute buffer when the object is destroyed
//        if (seashellBuffer != null)
//        {
//            seashellBuffer.Release();
//        }
//    }




//    void OnDrawGizmos()
//    {
//        Handles.color = Color.red;
//        Handles.DrawWireDisc(transform.position, transform.up.normalized, RadiusOfArea);
//        Handles.color = Color.green;
//        Handles.DrawWireDisc(transform.position, transform.up.normalized, RadiusOfArea + ReactionArea);

//        if (RandomizeSeashells && !Application.isPlaying)
//        {
//            ResetSeashells();
//        }
//    }
//}







//Responsible for this script: Tony Meis