using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkSync : MonoBehaviour {

    [SerializeField]
    GameObject handPool;

    [SerializeField]
    bool b;

    readonly string[] spheresName = new string[]{"Joint", "MockJoint", "PalmPosition", "WristPosition" };
    void OnValidate() {
        if (b) {
            if (gameObject.GetComponent<NetworkTransformChild>() != null) return;
            foreach (Transform t in handPool.transform) {
                foreach(Transform child in t) {
                    if (spheresName.Contains(child.name)) {
                        NetworkTransformChild ntc = gameObject.AddComponent<NetworkTransformChild>();
                        ntc.target = child;
                        ntc.syncRotationAxis = NetworkTransform.AxisSyncMode.None;
                        ntc.sendInterval = 1/29f;
                    }
                }
            }
        }
    }
}
