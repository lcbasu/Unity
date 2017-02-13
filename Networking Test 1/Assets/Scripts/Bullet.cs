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

        PlayerMove hitPlayer = hit.GetComponent<PlayerMove>();

        if (hitPlayer != null)
        {
            // Reduce the health of the hit player
            Combat combat = hit.GetComponent<Combat>();
            combat.TakeDamage(10);

            Destroy(gameObject);
        }
    }
}
