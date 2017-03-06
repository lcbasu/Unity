using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;

public class PlayerSetup : NetworkBehaviour {

    public List<Behaviour> componentsToDisable = null;

    Camera sceneCamera = null;

    void Start()
    {
        if (!isLocalPlayer)
        {
            foreach (Behaviour b in componentsToDisable)
            {
                b.enabled = false;
            }
        }
        else
        {
            sceneCamera = Camera.main;
            if (sceneCamera != null)
            {
                sceneCamera.gameObject.SetActive(false);
            }
        }
    }

    void OnDisable()
    {
        if (sceneCamera != null)
        {
            sceneCamera.gameObject.SetActive(true);
        }
    }

}
