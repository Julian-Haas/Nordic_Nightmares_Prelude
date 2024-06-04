using System.Reflection;
using UnityEngine;
using TMPro;
using System.Collections.Generic; 
public class EditorHelper
{
    /// <summary>
    ///   <para>clears the Debug console!</para>
    /// </summary>
    public static void ClearConsoleLogs()
    {
        var assembly = Assembly.GetAssembly(typeof(UnityEditor.ActiveEditorTracker));
        var type = assembly.GetType("UnityEditorInternal.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
    }
    /// <summary>
    ///   <para>static functions used to update a specific TextmeshPro Debug text. Value should be convertable to string!</para>
    /// </summary>
    public static void SetDebugText<T>(ref TextMeshProUGUI txt, T value) 
    {
        if (txt == null)
        {
            throw new System.NullReferenceException("You need to assign the Debug Text in the Unity inspector.");
        }
        txt.text = value.ToString();
    }

    /// <summary>
    ///   <para>this overload allows you to add a string for more specific informations tot the Debug TMPro text. Value should be convertable to string.</para>
    /// </summary>
    public static void SetDebugText<T>(ref TextMeshProUGUI txt, string txtInfo, T value)
    {
        if(txt == null)
        {
            throw new System.NullReferenceException("You need to assign the Debug Text in the Unity inspector.");
        }
        txt.text = txtInfo + " " + value.ToString();
    }
}
