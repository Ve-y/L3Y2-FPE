using LineworkLite.FreeOutline;
using Unity.VisualScripting;
using UnityEngine;

public class InteractionSystem : MonoBehaviour
{
    [SerializeField] private GameObject Camera;
    [SerializeField] private FreeOutlineSettings OutlineSettings;

    private GameObject CurrentSelectedInteraction;
    private LayerMask layerMask;
    private int PreviousLayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CurrentSelectedInteraction = null;
        layerMask = LayerMask.GetMask("PlayerModel", "PlayerHitbox");
    }

    void CheckInteraction()
    {
        RaycastHit hit;
        Vector3 OffsetPosition = transform.position + (transform.forward * 1);

        OutlineSettings.Outlines[0].width = Mathf.Lerp(OutlineSettings.Outlines[0].width, 4, 6 * Time.deltaTime);

        if (Physics.Raycast(OffsetPosition, transform.forward, out hit, Mathf.Infinity))
        {
            if (hit.transform.tag=="Interaction" || hit.transform.tag=="Evidence")
            {
                if (CurrentSelectedInteraction==null)
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
        CheckInteraction();
    }
}
