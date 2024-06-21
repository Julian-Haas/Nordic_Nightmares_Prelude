using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Splines;

public class NaddagilUtillitys : MonoBehaviour
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

    public static void ResetNaddiPosition(ref Naddagil naddi)
    {
        naddi.PatrolBehaviour.StartedPatrol = false;
        naddi.NaddiHearing.ResetSoundSum();
        naddi.AttackBehaviour.Agent.isStopped = true;
        naddi.StateMachiene.FinishedDigging();
        naddi.StartCoroutine(naddi.NaddiHearing.ListenerDelay());
    }

    public static void DisableRenderer(ref Naddagil naddi)
    {
        naddi.RendererEnabled = false;
        naddi.MeshRenderer.enabled = false;
    }

    public static List<T> ConvertToList<T>(ICollection<T> thingOfThinglol)
    {
        if(thingOfThinglol.Count <= 0)
            throw new System.NullReferenceException("Collection is null");

        List<T> listOfThings = new List<T>(thingOfThinglol);

        foreach (T thing in thingOfThinglol)
        {
            listOfThings.Add(thing); 
        }

        return listOfThings;
    }

    public static BezierKnot IsOnKnotPoint(ICollection<BezierKnot> knots, Vector3 position)
    {
        foreach (BezierKnot knot in knots)
        {
            Vector3 knotPos = knot.Position;
            if (Mathf.Abs((knotPos - position).magnitude) < 0.001f)
            {
                return knot; 
            }
        }
        Vector3 invalid = new Vector3(-999999, -999999, -999999); 
        return new BezierKnot(invalid); 
    }

}
