#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Text;
using System.Collections.Generic;

public class HierarchyCopier
{
    [MenuItem("Tools/Copy UI Hierarchy to Clipboard")]
    public static void CopyHierarchy()
    {
        StringBuilder sb = new StringBuilder();
        GameObject[] selectedObjects = Selection.gameObjects;

        if (selectedObjects.Length == 0)
        {
            EditorUtility.DisplayDialog("Nothing Selected", "Please select a GameObject (like your Canvas or Main Panel) in the Hierarchy first!", "OK");
            return;
        }

        sb.AppendLine("--- Copied UI Hierarchy ---");
        
        foreach (GameObject root in selectedObjects)
        {
            TraverseHierarchy(root.transform, 0, sb);
        }

        // Copy to clipboard
        GUIUtility.systemCopyBuffer = sb.ToString();
        
        Debug.Log("Hierarchy successfully copied to clipboard! You can now paste it.");
    }

    private static void TraverseHierarchy(Transform t, int depth, StringBuilder sb)
    {
        // Create the visual tree indentation
        string indent = "";
        for (int i = 0; i < depth; i++) indent += "   ";
        if (depth > 0) indent += "↳ ";

        // Grab all attached components to see the Layout Groups
        string componentInfo = GetComponentInfo(t.gameObject);

        // Append to the text block
        sb.AppendLine($"{indent}{t.name} {componentInfo}");

        // Loop through children recursively
        foreach (Transform child in t)
        {
            TraverseHierarchy(child, depth + 1, sb);
        }
    }

    private static string GetComponentInfo(GameObject go)
    {
        Component[] components = go.GetComponents<Component>();
        List<string> compNames = new List<string>();

        foreach (Component c in components)
        {
            if (c == null) continue; // Skip missing scripts
            
            string name = c.GetType().Name;
            
            // Filter out the obvious ones to keep the text clean
            if (name != "Transform" && name != "GameObject" && name != "CanvasRenderer")
            {
                compNames.Add(name);
            }
        }

        if (compNames.Count > 0)
        {
            return $"[{string.Join(", ", compNames)}]";
        }
        return "";
    }
}
#endif