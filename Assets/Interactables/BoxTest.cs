using UnityEngine;

public class BoxTest : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        GameObject.Destroy(transform.gameObject);
    }
}
