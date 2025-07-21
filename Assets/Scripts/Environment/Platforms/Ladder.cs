using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    private PlayerMove player;

    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerMove>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(player != null)
        {
            player.onLadder = true;
        }   
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(player != null)
        {
            player.onLadder = false;
        }   
    }
}
