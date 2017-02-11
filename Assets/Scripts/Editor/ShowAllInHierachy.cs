using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class ShowAllInHierachy : Editor {

    [MenuItem("Custom/ShowAll")]
	static void ShowAll() {
        GameObject[] root = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();

        foreach(GameObject child in root) {
            ShowAllTransform(child.transform);
        }
    }

    static void ShowAllTransform(Transform transform) {
        foreach (Transform child in transform) {
            child.hideFlags &= ~HideFlags.HideInHierarchy;
            ShowAllTransform(child);
        }
    }

    [MenuItem("Custom/RemoveNTC")]
    static void RemoveAllNetworkTransformChild() {
        foreach (NetworkTransformChild child in Selection.activeGameObject.GetComponents<NetworkTransformChild>()) {
            DestroyImmediate(child);
        }
    }
}
