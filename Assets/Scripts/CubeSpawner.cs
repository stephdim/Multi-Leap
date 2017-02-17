using UnityEngine;
using UnityEngine.Networking;

public class CubeSpawner : NetworkBehaviour {

	[SerializeField]
	Vector3 spawnPosition;

	[SerializeField]
	GameObject prefab;

	//public override void OnStartServer() {
	//NetworkServer.Spawn(Instantiate(prefab, t.position, t.rotation));
	//NetworkIdentity.AssignClientAuthority();
	//NetworkServer.SpawnWithClientAuthority(Instantiate(prefab, t.position, t.rotation), 
	// NetworkServer.connections[0].
	//}

	void Update() {
		if (isLocalPlayer && Input.GetKeyDown(KeyCode.Space)) {
			CmdSpawn();
		}
	}

	[Command]
	public void CmdSpawn() {
		GameObject go = (GameObject)Instantiate(prefab, transform.position + transform.forward * .25f, Quaternion.identity);
		// NetworkServer.SpawnWithClientAuthority(go, connectionToClient);
		NetworkServer.Spawn(go);
	}

	public void SetAuthority(NetworkIdentity ni) {
		CmdSetAuthority(ni);
	}

	[Command]
	void CmdSetAuthority(NetworkIdentity grabID) {
		if (grabID.clientAuthorityOwner != null)
			grabID.RemoveClientAuthority(grabID.clientAuthorityOwner);
		grabID.AssignClientAuthority(connectionToClient);
	}

	public void RemoveAuthority(NetworkIdentity ni) {
		CmdRemoveAuthority(ni);
	}

	[Command]
	void CmdRemoveAuthority(NetworkIdentity grabID) {
		grabID.RemoveClientAuthority(grabID.clientAuthorityOwner);
	}
}
