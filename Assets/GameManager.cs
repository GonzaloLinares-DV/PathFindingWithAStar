using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    public Vector3 playerPos;
    public Vector3 FoundIn;
    void Update()
    {
        playerPos = player.GetComponent<PlayerNode>().transform.position;
    }
}
