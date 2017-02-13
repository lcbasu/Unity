using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Combat : NetworkBehaviour {

    public const int maxHealth = 100;
    
    [SyncVar]
    public int health = maxHealth;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void TakeDamage(int amount)
    {
        if (!isServer)
            return;

        health -= amount;

        if (health <=0)
        {
            health = 0;
            Debug.Log("Dead!");
        }
    }
}
