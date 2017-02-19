using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using Leap.Unity.Interaction;
using UnityEngine.Networking;
using Leap.Unity;

public class NetworkPlayer : NetworkBehaviour {

    [SyncVar]
    public Color playerColor;

    void Start() {
        //playerColor.Callback += OnColorSet;
        SetColor();
    }

    public override void OnStartClient() {
        if (isServer) {
            CmdPlayerColor();
        }
    }

    [Command]
    void CmdPlayerColor() {
        playerColor = new Color(Random.Range(0f, 1), Random.Range(0f, 1), Random.Range(0f, 1), 1);
    }

    void SetColor() {
        if (isLocalPlayer) {
            foreach (CustomHand hand in GetComponentsInChildren<CustomHand>()) {
                hand.PlayerColor = playerColor;
            }
        } else {
            foreach (LinkHandSpheres hand in GetComponentsInChildren<LinkHandSpheres>()) {
                hand.SetHandColor(playerColor);
            }
        }
    }
}