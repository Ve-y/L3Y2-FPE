using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
using Unity.Mathematics;

public class EvidencePiece : MonoBehaviour
{
    public bool BeingLinked;
    public bool BeingDragged;

    public TMP_Text Name;
    public TMP_Text Description;
    public RawImage Icon;
    public Button ThreadButton;

    public GameObject PinPrefab;

    private GameObject CurrentPin;

    public InteractionSystem IS;

    private GameObject Thread=null;
    public GameObject ThreadPrefab;

    private GameObject ThreadOverride;

    public Canvas CurrentCanvas;

    public EvidenceDetails Stats;

    public bool CorrectLink;

    public void SetupPiece()
    {
        CurrentCanvas = GameObject.Find("UI").GetComponent<Canvas>();

        Name.text = Stats.EvidenceName;
        Description.text = Stats.Description;

        Icon.texture = Stats.EvidenceImage;
        transform.name = Stats.EvidenceID;

        transform.GetComponent<RectTransform>().anchoredPosition = Stats.CurrentPosition;
        IS = GameObject.Find("PlayerCamera").GetComponent<InteractionSystem>();
    }

    public void StartLinking()
    {
        if (CurrentCanvas==null)
        {
            CurrentCanvas = GameObject.Find("UI").GetComponent<Canvas>();
            
        }

        if (CurrentPin != null)
        {
            CurrentPin.GetComponent<Animator>().SetTrigger("Despawn");
            GameObject.Destroy(CurrentPin, 0.25f);

            CurrentPin = null;
        }

        if (Thread)
        {
            GameObject.Destroy(Thread);
            Thread = null;
            ThreadButton.image.color = new Color(1, 0, 0);

            ThreadOverride = null;
        }

        Thread = Instantiate(ThreadPrefab);
        Thread.transform.SetParent(CurrentCanvas.transform);
        Thread.transform.SetAsFirstSibling();

        CurrentCanvas.transform.Find("EvidenceScreen").SetAsFirstSibling();
    }
    public void LinkEvidence(GameObject EvidenceToLink, EvidenceDetails LinkedStats)
    {
        if (BeingLinked)
        {
            CurrentPin = Instantiate(PinPrefab);
            CurrentPin.GetComponent<RectTransform>().position = EvidenceToLink.GetComponent<RectTransform>().position + new Vector3(2.5f, 2.5f, 10);
            CurrentPin.transform.SetParent(EvidenceToLink.transform.parent);

            BeingLinked = false;

            if (Stats.ConnectingEvidence==LinkedStats.EvidenceID || LinkedStats.ConnectingEvidence==Stats.EvidenceID)
            {
                Stats.CurrentlyConnectedEvidence = EvidenceToLink.transform.parent.gameObject;

                if (EvidenceToLink.transform.parent.gameObject.GetComponent<EvidencePiece>().Stats.CurrentlyConnectedEvidence==null || EvidenceToLink.transform.parent.gameObject.GetComponent<EvidencePiece>().Stats.CurrentlyConnectedEvidence!=transform.gameObject)
                {
                    IS.CorrectEvidence += 1;
                    Debug.Log("Correct Link");
                }

                CorrectLink = true;
                ThreadButton.image.color = new Color(0, 1, 0);

                Thread.GetComponent<RawImage>().color = new Color(0, 1, 0);

                ThreadOverride = EvidenceToLink.transform.parent.Find("ThreadButton").gameObject;
            }
            else
            {
                LinkedStats.CurrentlyConnectedEvidence = null;
                if (CorrectLink)
                {
                    IS.CorrectEvidence = math.max(IS.CorrectEvidence-1,0);
                }

                CorrectLink = false;
                ThreadButton.image.color = new Color(1, 0, 0);

                if (Stats.EvidenceID==LinkedStats.EvidenceID)
                {
                    if (Thread != null)
                    {
                        GameObject.Destroy(Thread);
                        Thread = null;
                    }
                }
                else
                {
                    Thread.GetComponent<RawImage>().color = new Color(1, 0, 0);
                    ThreadOverride = EvidenceToLink.transform.parent.Find("ThreadButton").gameObject;
                }
            }
        }
    }
    void DragEvidence()
    {
        float x = Input.mousePosition.x;
        float y = Input.mousePosition.y;

        float yOffset = 60;

        transform.position = new Vector3(x, y - yOffset , 0);
    }
    void Update()
    {
        if (BeingDragged)
        {
            DragEvidence();
        }
        if (Thread!=null)
        {
            Vector2 mousePos;
            if (ThreadOverride==null)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                CurrentCanvas.transform as RectTransform,
                Input.mousePosition,
                CurrentCanvas.worldCamera,
                out mousePos);
            }
            else
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                CurrentCanvas.transform as RectTransform,
                ThreadOverride.GetComponent<RectTransform>().position,
                CurrentCanvas.worldCamera,
                out mousePos);
            }

            Vector2 startPos = transform.GetComponent<RectTransform>().anchoredPosition + new Vector2(0, 60);
            Thread.GetComponent<RectTransform>().anchoredPosition = startPos;

            Vector2 direction = mousePos - startPos;
            float distance = direction.magnitude;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            Thread.GetComponent<RectTransform>().sizeDelta = new Vector2(distance, Thread.GetComponent<RectTransform>().sizeDelta.y);
            Thread.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, angle);
        }
    }

}
