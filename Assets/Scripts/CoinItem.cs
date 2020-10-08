using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinItem : MonoBehaviour
{   
     private CharacterController2D player;
     
    private void Start() {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController2D>();
    }

    public void Use ()
    {
        player.Currency();
        Destroy(gameObject);
    }
}
