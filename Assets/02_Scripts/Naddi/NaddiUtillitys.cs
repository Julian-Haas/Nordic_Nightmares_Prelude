using UnityEngine;

public class NaddiUtillitys : MonoBehaviour
{
    public static void SetFlags(ref bool flagOne, ref bool flagTwo, bool val1, bool val2)
    {
        flagOne = val1;
        flagTwo = val2;
    }

    public static void SetFlags(ref bool flagOne, ref bool flagTwo, ref bool flagThree, bool val1, bool val2, bool val3)
    {
        flagOne = val1;
        flagTwo = val2;
        flagThree = val3;
    }

    public static float RandomOffset(float min, float max)
    {
        float val = Random.Range(min, max);
        float signOffset = Random.Range(0, 1);
        if (signOffset == 0)
            val *= -1;
        return val;
    }
    public static bool IsValidNavMesh(Vector3 position)
    {
        UnityEngine.AI.NavMeshHit hit;
        return UnityEngine.AI.NavMesh.SamplePosition(position, out hit, 1.0f, UnityEngine.AI.NavMesh.AllAreas);
    }

    public static void ResetNaddiPosition(ref Naddi naddi)
    {
        naddi.StartedPatrol = false;
        naddi.NaddiHearing.ResetSoundSum();
        naddi.Agent.isStopped = true;
        naddi.StateMachiene.FinishedDigging();
        naddi.StartCoroutine(naddi.NaddiHearing.ListenerDelay());
    }

    public static void DisableRenderer(ref Naddi naddi)
    {
        naddi.RendererEnabled = false;
        naddi.StateMachiene.GetNaddiMeshRenderer.enabled = false;
    }
}
