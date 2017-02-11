using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkSync : MonoBehaviour {

    [SerializeField]
    GameObject handPool;

    [SerializeField]
    bool b;
	
    void OnValidate() {
        if (b) {
            if (gameObject.GetComponent<NetworkTransformChild>() != null) return;
            foreach (Transform t in handPool.transform) {
                foreach(Transform child in t) {
                    gameObject.AddComponent<NetworkTransformChild>().target = child;
                }
            }
        }
    }
}
