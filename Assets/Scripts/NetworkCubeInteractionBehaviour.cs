using Leap;
using Leap.Unity.Interaction;
using UnityEngine;

[RequireComponent(typeof(NetworkCube))]
public class NetworkCubeInteractionBehaviour : InteractionBehaviour {

	NetworkCube _networkCube;

	protected override void Awake() {
		base.Awake();
		_networkCube = GetComponent<NetworkCube>();
    }

	protected override void OnGraspBegin() {
		base.OnGraspBegin();
		_networkCube.OnGraspBegin();
    }

	protected override void OnGraspEnd(Hand lastHand) {
		base.OnGraspEnd(lastHand);
		_networkCube.OnGraspEnd(lastHand);
	}
}
