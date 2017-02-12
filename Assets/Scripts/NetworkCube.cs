using UnityEngine;
using UnityEngine.Networking;

public class NetworkCube : NetworkBehaviour {

	[SerializeField] Material red, blue, green;
	Renderer _renderer;
	Rigidbody _rigidbody;
	Collider _collider;
	NetworkIdentity _networkIdentity;

	void Awake() {
		_renderer = GetComponent<Renderer>();
		_rigidbody = GetComponent<Rigidbody>();
		_collider = GetComponent<Collider>();
		_networkIdentity = GetComponent<NetworkIdentity>();
	}

	public override void OnStartClient() {
		_rigidbody.isKinematic = true; // isKinematic <=> enabled
	}

	// If we have the local authority, disable trigering and enable physics interactions
	public override void OnStartAuthority() {
		_collider.isTrigger = false;
		_rigidbody.isKinematic = false; // isKinematic <=> enabled
	}

	// If we haven't the local authority, disable physics interactions and enable trigering
	public override void OnStopAuthority() {
		_collider.isTrigger = true;
		_rigidbody.isKinematic = true; // isKinematic <=> enabled
	}

	// On each collision, the local player is changed
	void OnTriggerEnter(Collider collider) {
		if (hasAuthority) return;

		// remove the authority from the previous owner
		// CmdRemoveAuthority(_networkIdentity);

		// assign the authority to ourself
		// CmdAssignAuthority(_networkIdentity);

		// HACK
		// we haven't curently the authority on the object, so we can't send command to server
		// remove the authority from the previous owner
		// assign the authority to ourself
		CubeSpawner cubeSpawner = collider.GetComponentInParent<CubeSpawner>();
        if (cubeSpawner) cubeSpawner.SetAuthority(GetComponent<NetworkIdentity>());
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
}
