using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeetTrigger : MonoBehaviour
{
public bool isTouchingPlatform = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Platform"))
        {
            isTouchingPlatform = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Platform"))
        {
            isTouchingPlatform = false;
        }
    }
}
