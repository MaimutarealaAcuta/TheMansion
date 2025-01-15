using UnityEngine;

[System.Serializable]
public class Goal
{
    public string goalID;
    public string goalTitle;
    [TextArea(2, 4)]
    public string goalDescription;

    public bool isCompleted;
    public bool isActive;

    // Constructor - optional
    public Goal(string id, string title, string desc)
    {
        goalID = id;
        goalTitle = title;
        goalDescription = desc;
        isCompleted = false;
        isActive = false;
    }
}
