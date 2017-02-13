using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerMove : NetworkBehaviour {

    public GameObject bulletePrefab;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if (!isLocalPlayer)
            return;

        float x = Input.GetAxis("Horizontal") * 0.1f;
        float z = Input.GetAxis("Vertical") * 0.1f;

        transform.Translate(x, 0, z);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Command function is called from the client but invoked on the server
            CmdFire();
        }
	}

    public override void OnStartLocalPlayer()
    {
        GetComponent<MeshRenderer>().material.color = Color.red;
        base.OnStartLocalPlayer();
    }

    [Command]
    void CmdFire()
    {
        // Create the bullete from the bullete prefab

        GameObject bullete = Instantiate(bulletePrefab, transform.position - transform.forward, Quaternion.identity) as GameObject;

        // Make the bullete move away in front of the player
        bullete.GetComponent<Rigidbody>().velocity = -transform.forward * 4;

        // spawn the bullete on the clients
        NetworkServer.Spawn(bullete);

        // make the bullete disappear after 2 seconds
        Destroy(bullete, 2.0f);
    }
}
