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
}
