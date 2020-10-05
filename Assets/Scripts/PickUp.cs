using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    private Inventory inventory;
    public GameObject itemButton;

    private bool colliding = false;

    private void Start() 
    {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
    }

    IEnumerator Reset()
    {
        yield return new WaitForEndOfFrame();
        colliding = false;
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (colliding) return;
        if(other.CompareTag("Player") && !colliding)
        {
            for (int i = 0; i < inventory.slots.Length; i++)
            {
                if(inventory.isFull[i] == false)
                {
                    inventory.isFull[i] = true;
                    Instantiate(itemButton, inventory.slots[i].transform, false); //instancia o 'item' Button no mesmo local do slot
                    Destroy(gameObject);
                    colliding = true;
                    StartCoroutine(Reset()); 
                    break;
                }
            }       
        }
                    
    }
}

