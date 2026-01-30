using UnityEngine;
using TMPro;

public class DialogueScript : MonoBehaviour
{
    public string IntendedDialogue;
    public TMP_Text DialogueBox;

    private int CurrentLetter;
    private float Cooldown;

    void Start()
    {
        DialogueBox.text = "";

        float x = Input.mousePosition.x;
        float y = Input.mousePosition.y;

        transform.position = new Vector3(x, y, 0);
        transform.SetParent(GameObject.Find("UI").transform);
    }

    // Update is called once per frame
    void Update()
    {
        if (DialogueBox.text != IntendedDialogue && Time.time>Cooldown)
        {
            CurrentLetter += 1;
            Cooldown = Time.time + 0.1f;

            DialogueBox.text = IntendedDialogue.Substring(0,CurrentLetter-1);
        }
        if (DialogueBox.text==IntendedDialogue && Time.time > Cooldown)
        {
            DialogueBox.GetComponent<Animator>().SetTrigger("Disapear");
            GameObject.Destroy(DialogueBox.transform.gameObject, 1f);
        }
    }
}
