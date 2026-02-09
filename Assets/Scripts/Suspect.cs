using UnityEngine;

[CreateAssetMenu(fileName = "Suspect", menuName = "Scriptable Objects/Suspect")]
public class Suspect : ScriptableObject
{
    public InterrogateDialogue StartingDialogue;
    public Sprite Looks;

    public float Stress; // guilty if reaches 100. stress goes up by 25 for every cross examine
}
