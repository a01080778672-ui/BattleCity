using UnityEngine;

[CreateAssetMenu(menuName = "SO/Event Data")]
public class EventData : ScriptableObject
{
    public string eventID;
    public string eventName;

    [TextArea]
    public string description;

    public Sprite eventImage;
    public GameObject eventPrefab;
}