using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionItem : MonoBehaviour
{
    public GameObject effect;
    private CharacterController2D player;

    private void Start() {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController2D>();
    }

    public void Use ()
    {
        Instantiate(effect, player.transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
