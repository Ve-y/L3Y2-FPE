using UnityEngine;

[CreateAssetMenu(fileName = "EvidenceDetails", menuName = "ScriptableObjects/EvidenceDetails", order = 1)]

public class EvidenceDetails : ScriptableObject
{
    public string EvidenceName;
    public string EvidenceID;

    public string ConnectingEvidence;
    public string Description;

    public Sprite EvidenceImage;

    public GameObject CurrentlyConnectedEvidence;
    public Vector2 CurrentPosition;
}
