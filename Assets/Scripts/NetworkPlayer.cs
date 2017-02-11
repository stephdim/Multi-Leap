using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using Leap.Unity.Interaction;
using UnityEngine.Networking;
using Leap.Unity;

public class NetworkPlayer : NetworkBehaviour {
    [SyncVar]
    public string playerID;
    Camera playerCam;

    public InteractionManager interactionManager;
    [SerializeField]
    GameObject _leapPrefab;

    void Awake() {
        playerCam = GetComponent<Camera>();
        //if (isLocalPlayer) {
        //    GameObject.Find("LeapHandController").transform.SetParent(transform);
        //}
        //else {
            playerCam.enabled = false;
        //}
    }

    //[Command]
    //void CmdSetPlayerID(string newID) {
    //    playerID = newID;
    //}

    public override void OnStartLocalPlayer() {
        //GameObject.Find("LeapHandController").transform.SetParent(transform);
        transform.GetChild(0).GetComponent<Renderer>().material.color = Color.blue;
        GameObject leap = Instantiate<GameObject>(_leapPrefab);
        leap.transform.parent = transform;
        leap.transform.localPosition = Vector3.zero;
        leap.transform.localRotation = Quaternion.identity;
        playerCam.enabled = true;
    }
}