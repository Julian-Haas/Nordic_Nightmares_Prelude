using System.Collections;
using System.Collections.Generic;
using UnityEngine;

        //Grass, Sand, Dirt, Stone, Gravel
public class Player_Ground_Texture_Check : MonoBehaviour
{
    [SerializeField] CapsuleCollider _collider;
    //[SerializeField] NoiseEmitter _emitter;
    int layer_mask;

    [Header("Noise Range depending on Ground")]
    //[SerializeField] float Grass_Quiet = 0.0f;
    //[SerializeField] float Grass_Loud = 0.0f;
    //[SerializeField] float Sand_Quiet = 1.0f;
    //[SerializeField] float Sand_Loud = 3.0f;
    //[SerializeField] float Dirt_Quiet = 1.5f;
    //[SerializeField] float Dirt_Loud = 3.5f;
    //[SerializeField] float Stone_Quiet = 2.0f;
    //[SerializeField] float Stone_Loud = 4.0f;
    //[SerializeField] float Gravel_Quiet = 3.0f;
    //[SerializeField] float Gravel_Loud = 5.0f;

    [SerializeField] PlayerControl playerControl;
    public string CurrentTexture;
    private void Start()
    {
        playerControl = GetComponent<PlayerControl>();
        _collider = GetComponent<CapsuleCollider>();
        layer_mask = LayerMask.GetMask("whatIsGround");
        //_emitter = GetComponentInChildren<NoiseEmitter>();
    }

    public void CheckGroundTexture()
    {
        if(Physics.Raycast(transform.position + new Vector3 (0, 0.5f * _collider.height + 0.5f * _collider.radius,0 ), 
                Vector3.down, 
                out RaycastHit hit, 
                10f,
                layer_mask))
        {
            if(hit.collider.TryGetComponent<Terrain>(out Terrain terrain)) 
            {
                //Debug.Log("Player is walking on " + GetMainTextureFromTerrain(terrain, hit.point));
                CurrentTexture = GetMainTextureFromTerrain(terrain, hit.point);
                //_emitter.MakeSound();
            }
            else if ( hit.collider != null)
            {
                //Debug.Log("Player walking on " +  hit.collider.name);
            }
        }
    }

    public string GetMainTextureFromTerrain(Terrain terrain, Vector3 HitPoint)
    {
        Vector3 terrainPos = HitPoint - terrain.transform.position;
        Vector3 splatPos = new Vector3(terrainPos.x / terrain.terrainData.size.x, 0, terrainPos.z / terrain.terrainData.size.z);

        int xCoord = Mathf.FloorToInt(splatPos.x * terrain.terrainData.alphamapWidth);
        int zCoord = Mathf.FloorToInt(splatPos.z * terrain.terrainData.alphamapHeight);

        float[,,] alphaMap = terrain.terrainData.GetAlphamaps(xCoord, zCoord, 1, 1);

        int primaryIndex = 0;
        for(int i = 1; i < alphaMap.Length; i++)
        {
            if (alphaMap[0,0,i] > alphaMap[0,0, primaryIndex]) 
            {
                primaryIndex = i;
            }
        }

        switch (terrain.terrainData.terrainLayers[primaryIndex].diffuseTexture.name)
        {
            case "T_Grass_1 1":
                //_emitter.AdjustNoiseParamaters(Grass_Quiet, Grass_Loud, 0);
                break;
            case "T_Grass_01":
                //_emitter.AdjustNoiseParamaters(Grass_Quiet, Grass_Loud, 0);
                break;

            case "T_Sand_01":
                //_emitter.AdjustNoiseParamaters(Sand_Quiet, Sand_Loud, 5);
                break;

            case "T_Base_Ground":
                //_emitter.AdjustNoiseParamaters(Dirt_Quiet, Dirt_Loud, 5.5f);
                break;
            case "T_Dirt_00":
                //_emitter.AdjustNoiseParamaters(Dirt_Quiet, Dirt_Loud, 5.5f);
                break;
            case "T_Dirt_01":
                //_emitter.AdjustNoiseParamaters(Dirt_Quiet, Dirt_Loud, 5.5f);
                break;

            case "T_Stone_Placeholder":
                //_emitter.AdjustNoiseParamaters(Stone_Quiet, Stone_Loud, 6);
                break;
            case "T_Stone_Placeholder_Ina":
                //_emitter.AdjustNoiseParamaters(Stone_Quiet, Stone_Loud, 6);
                break;

            case "T_Gravel_02":
                //_emitter.AdjustNoiseParamaters(Gravel_Quiet, Gravel_Loud, 7.5f);
                break;
            case "T_Gravel_03":
                //_emitter.AdjustNoiseParamaters(Gravel_Quiet, Gravel_Loud, 7.5f);
                break;


            default:
                break;
        }

        return terrain.terrainData.terrainLayers[primaryIndex].diffuseTexture.name;
    }
}