using UnityEngine;
using UnityEngine.Networking;

public class EnableOnLocalPlayer : NetworkBehaviour {

    [SerializeField]
    GameObject leapController;
    [SerializeField]
    Camera playerCam;
	[SerializeField]
	GameObject[] rigidHands;

    public override void OnStartLocalPlayer() {
        leapController.SetActive(true);
        playerCam.enabled = true;
		foreach(var rigidHand in rigidHands) {
			rigidHand.SetActive(true);
		}
    }
}
