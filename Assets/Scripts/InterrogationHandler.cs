using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class InterrogationHandler : MonoBehaviour
{
    public Suspect CurrentlyInterrogating;
    public List<Suspect> CurrentSuspects = new List<Suspect>();

    private bool IsInterrogating;

    public GameObject InterrogationRoomPrefab;
    public GameObject CurrentRoom;
    public void BeginInterrogation()
    {
        Vector3 RoomPosition = new Vector3(250,100,250);

        CurrentRoom = Instantiate(InterrogationRoomPrefab);
        CurrentRoom.transform.position = RoomPosition;

        GameObject Player = GameObject.Find("PlayerObject");

        Player.GetComponent<PlayerMovement>().teleport = CurrentRoom.transform.Find("PlayerSpawn").transform.position;
    }
}
