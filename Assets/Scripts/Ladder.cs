using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    private PlayerMove4 player;

    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerMove4>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //PlayerMove2 player = collision.GetComponent<PlayerMove2>();

        if(player != null)
        {
            player.onLadder = true;
        }   
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //PlayerMove2 player = collision.GetComponent<PlayerMove2>();

        if(player != null)
        {
            player.onLadder = false;
        }   
    }
}
