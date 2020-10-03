using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

	public int health = 100;

	public GameObject deathEffect;
	private GameObject clone;

	public void TakeDamage (int damage)
	{
		health -= damage;

		if (health <= 0)
		{
			Die();
		}
	}

	void Die ()
	{
		clone = (GameObject)Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
		Destroy(clone, 0.5f);
	}

}