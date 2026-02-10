// ExploreData.cs (Simplified)

using UnityEngine;
using System.Collections.Generic;

// This is the only data container you need now.
[System.Serializable]
public class ExploreDataContent
{
    public string category;
    public string title;
    public List<ButtonData> buttons;
}

// This ScriptableObject reference remains the same.
[CreateAssetMenu(fileName = "New Explore Category", menuName = "Data/Explore Category")]
public class ExploreData : ScriptableObject
{
    public TextAsset jsonFile;
}

// This class for individual buttons remains the same.
[System.Serializable]
public class ButtonData
{
    public string name;
    [TextArea(3, 10)]
    public string content;
    public string imageStr;
    public string iconStr;
}