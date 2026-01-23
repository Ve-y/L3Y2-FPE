using UnityEngine;

public class BoxTest : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        transform.localScale *= 1.001f;
    }
}
