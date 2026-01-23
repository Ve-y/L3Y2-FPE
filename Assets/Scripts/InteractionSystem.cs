using LineworkLite.FreeOutline;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface IInteractable
{
    public void Interact();
}

public class InteractionSystem : MonoBehaviour
{
    [SerializeField] private GameObject Camera;
    [SerializeField] private FreeOutlineSettings OutlineSettings;

    private List<EvidenceDetails> CollectedEvidence = new List<EvidenceDetails>();

    private GameObject CurrentSelectedInteraction;
    private LayerMask layerMask;
    private int PreviousLayer;

    public InputHandler PlayerInput;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CurrentSelectedInteraction = null;
        layerMask = LayerMask.GetMask("PlayerModel", "PlayerHitbox");
    }

    void PickupEvidence(GameObject Evi)
    {
        EvidenceDetails Details = Evi.GetComponent<EvidenceScript>().Evidence;

        Details.CurrentlyConnectedEvidence = null;
        Details.CurrentPosition = new Vector2(Random.Range(-10,10), Random.Range(-10, 10));
        CollectedEvidence.Add(Details);

        Destroy(Evi);
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
    }
}
