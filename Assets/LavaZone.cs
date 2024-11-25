using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MarioController player = other.GetComponent<MarioController>();
            if (player != null)
            {
                player.Die();
            }
        }
    }
}