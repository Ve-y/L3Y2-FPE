using UnityEngine;

[CreateAssetMenu(fileName = "InterrogateDialogue", menuName = "Scriptable Objects/InterrogateDialogue")]
public class InterrogateDialogue : ScriptableObject
{
    public string Dialogue;
    public string CrossExamineDialogue;

    public InterrogateDialogue Next;
}
