using Leap;
using Leap.Unity.Interaction;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkCube : NetworkBehaviour {

	[SerializeField] Material red, blue, green;
    Color playerColor;
    Renderer _renderer;
	//Collider _collider;
	NetworkIdentity _networkIdentity;
	InteractionBehaviour _interactionBehaviour;

	void Awake() {
		_renderer = GetComponent<Renderer>();
		//_collider = GetComponentInChildren<Collider>();
		_networkIdentity = GetComponent<NetworkIdentity>();
		_interactionBehaviour = GetComponent<InteractionBehaviour>();
    }

	void SetRigidbodyEnabled(bool enabled) {
		_interactionBehaviour.isKinematic = !enabled;
		_interactionBehaviour.useGravity = enabled;
	}

	public override void OnStartClient() {
		InteractionManager interactionManager = FindObjectOfType<InteractionManager>();
		GetComponent<InteractionBehaviour>().Manager = interactionManager;

		//SetRigidbodyEnabled(false);
	}

	// If we have the local authority, disable trigering and enable physics interactions
	public override void OnStartAuthority() {
		//_collider.isTrigger = false;
		SetRigidbodyEnabled(true);
	}

	// If we haven't the local authority, disable physics interactions and enable trigering
	public override void OnStopAuthority() {
		//_collider.isTrigger = true;
		SetRigidbodyEnabled(false);
	}

	public void OnGraspBegin() {
		if (hasAuthority) return;
		CubeSpawner cubeSpawner = FindObjectOfType<InteractionManager>() // TODO cache
			.GetComponentInParent<CubeSpawner>();
		if (cubeSpawner) cubeSpawner.SetAuthority(_networkIdentity);
	}

	public void OnGraspEnd(Hand lastHand) {
		//if (!hasAuthority || isServer) return;

		//if (GetComponent<NetworkCubeInteractionBehaviour>().IsBeingGrasped) // TODO cache
		//	return;

		//// Transfert rigidbody informations (velocity, angularVelocity) to server
		//Rigidbody rigidbody = GetComponent<Rigidbody>(); // TODO cache
  //      CmdTransfertRigidbodyState(rigidbody.velocity, rigidbody.angularVelocity);

		//CubeSpawner cubeSpawner = FindObjectOfType<InteractionManager>() // TODO cache
		//.GetComponentInParent<CubeSpawner>();
		//if (cubeSpawner) cubeSpawner.RemoveAuthority(_networkIdentity);
	}

	[Command]
	void CmdTransfertRigidbodyState(Vector3 velocity, Vector3 angularVelocity) {
		GetComponent<Rigidbody>().velocity = velocity;
		GetComponent<Rigidbody>().angularVelocity= angularVelocity;
	}


	void Update() {
		if (hasAuthority) {
			if (_renderer.material != green) _renderer.material = green;
		} else {
			if (_renderer.material != red) _renderer.material = red;
		}
	}

    //[Command]
    //void CmdRemoveAuthority(NetworkIdentity grabID) {
    //	grabID.RemoveClientAuthority(grabID.clientAuthorityOwner);
    //}

    //[Command]
    //void CmdAssignAuthority(NetworkIdentity grabID) {
    //	grabID.AssignClientAuthority(connectionToClient);
    //}

    void OnTriggerEnter(Collider other) {
        if (other.name == "DestroyTrigger") {
            CmdDestroy();
        }
    }

    [Command]
    void CmdDestroy() {
        _networkIdentity.RemoveClientAuthority(_networkIdentity.clientAuthorityOwner);
        GameObject go = NetworkServer.FindLocalObject(_networkIdentity.netId);
        NetworkServer.Destroy(go);
    }
}
