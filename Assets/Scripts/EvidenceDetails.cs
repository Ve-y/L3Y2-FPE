using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EvidenceDetails", menuName = "ScriptableObjects/EvidenceDetails", order = 1)]

public class EvidenceDetails : ScriptableObject
{
    public string EvidenceName;
    public string EvidenceID;

    public string ConnectingEvidence;
    public string Description;

    public Texture EvidenceImage;

    public GameObject CurrentlyConnectedEvidence;
    public Vector2 CurrentPosition;

    public List<EvidenceDialogue> DialogueOnConnect = new List<EvidenceDialogue>();
}
