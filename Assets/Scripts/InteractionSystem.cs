using LineworkLite.FreeOutline;
using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public interface IInteractable
{
    public void Interact();
}

public class InteractionSystem : MonoBehaviour
{
    [SerializeField] private FreeOutlineSettings OutlineSettings;

    public GameObject EvidencePiecePrefab;
    Canvas CurrentUI;

    private List<EvidenceDetails> CollectedEvidence = new List<EvidenceDetails>();
    public float CorrectEvidence;

    public GameObject DialoguePrefab;
    private GameObject CurrentSelectedInteraction;
    private LayerMask layerMask;
    private int PreviousLayer;

    public GameObject EvidenceCard;

    public string CurrentEvidenceLinking;

    public InputHandler PlayerInput;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CurrentSelectedInteraction = null;
        CurrentUI = GameObject.Find("UI").GetComponent<Canvas>();
        layerMask = LayerMask.GetMask("PlayerModel", "PlayerHitbox");
    }

    public void ButtonCheck(Button ClickedButton, EvidenceDetails Details)
    {
        if (CurrentEvidenceLinking!=null && CurrentEvidenceLinking!="")
        {
            EvidencePiece LinkedEvidence = GameObject.Find(CurrentEvidenceLinking).GetComponent<EvidencePiece>();
            if (LinkedEvidence.GetComponent<EvidencePiece>().Stats.DialogueOnConnect.Count>0)
            {
                foreach (EvidenceDialogue PossibleDialogue in LinkedEvidence.GetComponent<EvidencePiece>().Stats.DialogueOnConnect)
                {
                    if (PossibleDialogue.EvidenceID==Details.EvidenceID)
                    {
                        GameObject DialogueBox = Instantiate(DialoguePrefab);
                        DialogueBox.GetComponent<DialogueScript>().IntendedDialogue = PossibleDialogue.DialogueToSay;
                    }
                }
            }

            LinkedEvidence.LinkEvidence(ClickedButton.gameObject,Details);
            CurrentEvidenceLinking = "";
        }
        else
        {
            EvidencePiece clickedPiece = ClickedButton.transform.parent.GetComponent<EvidencePiece>();

            CurrentEvidenceLinking = Details.EvidenceID;
            clickedPiece.BeingLinked = true;

            clickedPiece.StartLinking();
        }

        Debug.Log(CorrectEvidence);
        if (CorrectEvidence==CollectedEvidence.Count-1)
        {
            Debug.Log("You won!!");
        }
    }

    void NavigateEvidenceScreen()
    {
        Vector2 MouseMovement = PlayerInput.MouseInput;
        bool Navigating = PlayerInput.lookingAround;

        if (CurrentUI.enabled)
        {
            if (Navigating)
            {
                Cursor.lockState = CursorLockMode.Confined;
                foreach (Transform child in CurrentUI.transform)
                {
                    if (child.gameObject.name != "EvidenceScreen" && child.gameObject.name != "Hint")
                    {
                        RectTransform trans = child.GetComponent<RectTransform>();
                        trans.position += new Vector3(MouseMovement.x, MouseMovement.y, 0);
                    }
                }
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
            }
        }
    }

    void OpenEvidenceScreen()
    {
        Canvas UI = GameObject.Find("UI").GetComponent<Canvas>();
        CorrectEvidence = 0;

        if (UI.enabled)
        {
            UI.enabled = false;
            foreach (Transform child in UI.transform)
            {
                if (child.gameObject.name != "EvidenceScreen" && child.gameObject.name != "Hint")
                {
                    GameObject.Destroy(child.gameObject);
                }
            }
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            UI.enabled = true;

            foreach (EvidenceDetails Evi in CollectedEvidence)
            {
                GameObject ClonedPrefab = Instantiate(EvidencePiecePrefab);
                EvidencePiece PieceScript = ClonedPrefab.GetComponent<EvidencePiece>();

                PieceScript.Stats = Evi;
                ClonedPrefab.transform.SetParent(UI.transform);

                PieceScript.SetupPiece();
                Debug.Log(Evi);
            }
            Cursor.lockState = CursorLockMode.None;
        }
    }

    void PickupEvidence(GameObject Evi)
    {
        EvidenceDetails Details = Evi.GetComponent<EvidenceScript>().Evidence;

        Details.CurrentlyConnectedEvidence = null;
        Details.CurrentPosition = new Vector2(Random.Range(-350,350), Random.Range(-150, 150));
        CollectedEvidence.Add(Details);

        Evi.tag="Untagged";
        GameObject InstancedTag = Instantiate(EvidenceCard);
        InstancedTag.transform.Find("Card").Find("TextPlane").GetComponent<TMP_Text>().text = CollectedEvidence.Count.ToString();

        if (Evi.transform.Find("CardPlacement"))
        {
            Transform Placement = Evi.transform.Find("CardPlacement");

            InstancedTag.transform.position = Placement.position;
            InstancedTag.transform.rotation = Placement.rotation * Quaternion.Euler(new Vector3(0, -90, -90));
            InstancedTag.transform.SetParent(Placement);
        }
        else
        {
            InstancedTag.transform.position = Evi.transform.position;
            InstancedTag.transform.rotation = Evi.transform.rotation * Quaternion.Euler(new Vector3(0, -90, -90));
            InstancedTag.transform.SetParent(Evi.transform);
        }
    }

    void Interact()
    {
        RaycastHit hit;
        Vector3 OffsetPosition = transform.position + (transform.forward * 1);

        if (Physics.Raycast(OffsetPosition, transform.forward, out hit, Mathf.Infinity))
        {
            if (hit.collider.gameObject.TryGetComponent(out IInteractable intObj))
            {
                intObj.Interact();
                OpenEvidenceScreen();
            }
            else
            {
                if (hit.transform.tag == "Evidence")
                {
                    PickupEvidence(hit.collider.gameObject);
                }
            }
        }
    }
    void CheckInteraction()
    {
        RaycastHit hit;
        Vector3 OffsetPosition = transform.position + (transform.forward * 1);

        OutlineSettings.Outlines[0].width = Mathf.Lerp(OutlineSettings.Outlines[0].width, 4, 6 * Time.deltaTime);

        if (Physics.Raycast(OffsetPosition, transform.forward, out hit, Mathf.Infinity))
        {
            if (hit.transform.tag == "Interaction" || hit.transform.tag == "Evidence")
            {
                if (CurrentSelectedInteraction == null)
                {
                    OutlineSettings.Outlines[0].width = 0;

                    CurrentSelectedInteraction = hit.transform.gameObject;
                    PreviousLayer = hit.transform.gameObject.layer;

                    hit.transform.gameObject.layer = 7;

                    Debug.Log("Interaction found");
                }
            }
            else
            {
                if (CurrentSelectedInteraction)
                {
                    CurrentSelectedInteraction.layer = PreviousLayer;
                }

                CurrentSelectedInteraction = null;
            }
        }
        else
        {
            if (CurrentSelectedInteraction)
            {
                CurrentSelectedInteraction.layer = PreviousLayer;
            }

            CurrentSelectedInteraction = null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerInput.interactionTriggered)
        {
            Interact();
        }
        CheckInteraction();
        NavigateEvidenceScreen();
    }
}
