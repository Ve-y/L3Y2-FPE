using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EvidenceButtonScript : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public EvidencePiece Piece;
    public InteractionSystem IS;

    private bool IsPressed;
    private float ButtonTime;

    private bool BeingLinked;

    private bool WasDragging;

    void Awake()
    {
        IS = GameObject.Find("PlayerCamera").GetComponent<InteractionSystem>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        IsPressed = true;
        ButtonTime = Time.time+0.35f;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        IsPressed = false;

        if (!WasDragging)
        {
            IS.ButtonCheck(this.GetComponent<Button>(), transform.parent.gameObject.GetComponent<EvidencePiece>().Stats);
        }
        else
        {
            Piece.BeingDragged = false;
        }

        WasDragging = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsPressed)
        {
            if (ButtonTime<Time.time)
            {
                WasDragging = true;
                Piece.BeingDragged = true;
            }
        }
    }
}
