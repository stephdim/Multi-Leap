using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EnableOnLocalPlayer : NetworkBehaviour {

    [SerializeField]
    GameObject leapController;
    [SerializeField]
    Camera playerCam;

    public override void OnStartLocalPlayer() {
        leapController.SetActive(true);
        playerCam.enabled = true;
    }
}
