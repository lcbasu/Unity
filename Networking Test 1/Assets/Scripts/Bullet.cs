using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter(Collision collision)
    {
        GameObject hit = collision.gameObject;

        Combat hitCombat = hit.GetComponent<Combat>();

        if (hitCombat != null)
        {
            // Reduce the health of the hit combat
            hitCombat.TakeDamage(10);

            Destroy(gameObject);
        }
    }
}
