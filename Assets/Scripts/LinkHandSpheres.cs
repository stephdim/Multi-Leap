using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Networking;

public class LinkHandSpheres : NetworkBehaviour {

    private const float CYLINDER_RADIUS = 0.006f;
    [SerializeField]
    private int _cylinderResolution = 12;

    private Transform[] _jointSpheres;
    private List<Transform> _sphereATransforms;
    private List<Transform> _sphereBTransforms;
    private List<Transform> _cylinderTransforms;
    private bool _hasGeneratedMeshes;
    private Transform mockThumbJointSphere;
    private Transform palmPositionSphere;

    private Transform wristPositionSphere;

    // Use this for initialization
    void Awake () {
		_jointSpheres = new Transform[4 * 5];
        mockThumbJointSphere = transform.FindChild("MockJoint");
        palmPositionSphere = transform.FindChild("PalmPosition");
        wristPositionSphere = transform.FindChild("WristPosition");

        for (int i = 0; i < _jointSpheres.Length; ++i) {
            _jointSpheres[i] = transform.GetChild(i);
        }
        _cylinderTransforms = new List<Transform>();
        _sphereATransforms = new List<Transform>();
        _sphereBTransforms = new List<Transform>();
        _hasGeneratedMeshes = false;

        BuildCylinders();
    }

    void Update() {
        updateCylinders();
    }

    private void updateCylinders() {
        for (int i = 0; i < _cylinderTransforms.Count; i++) {
            Transform cylinder = _cylinderTransforms[i];
            Transform sphereA = _sphereATransforms[i];
            Transform sphereB = _sphereBTransforms[i];
            Vector3 delta = sphereA.position - sphereB.position;

            if (!_hasGeneratedMeshes) {
                MeshFilter filter = cylinder.GetComponent<MeshFilter>();
                filter.sharedMesh = generateCylinderMesh(delta.magnitude / transform.lossyScale.x);
            }

            cylinder.position = sphereA.position;

            if (delta.sqrMagnitude <= Mathf.Epsilon) {
                //Two spheres are at the same location, no rotation will be found
                continue;
            }

            cylinder.LookAt(sphereB);
        }

        _hasGeneratedMeshes = true;
    }

    private int getFingerJointIndex(int fingerIndex, int jointIndex) {
        return fingerIndex * 4 + jointIndex;
    }

    private void BuildCylinders() {

        Transform[] fingerJoints = GetComponentsInChildren<Transform>().Where(c => c.name == "Finger Joint").ToArray();
        int k = 0;

        //Create cylinders between finger joints
        for (int i = 0; i < 5; i++) {
            for (int j = 0; j < 3; j++) {
                int keyA = getFingerJointIndex(i, j);
                int keyB = getFingerJointIndex(i, j + 1);

                Transform sphereA = _jointSpheres[keyA];
                Transform sphereB = _jointSpheres[keyB];
                buildCylinder(sphereA, sphereB, fingerJoints[k++]);
                //createCylinder("Finger Joint", sphereA, sphereB);
            }
        }

        Transform[] handJoints = GetComponentsInChildren<Transform>().Where(c => c.name == "Hand Joints").ToArray();
        k = 0;
        //Create cylinders between finger knuckles
        for (int i = 0; i < 4; i++) {
            int keyA = getFingerJointIndex(i, 0);
            int keyB = getFingerJointIndex(i + 1, 0);

            Transform sphereA = _jointSpheres[keyA];
            Transform sphereB = _jointSpheres[keyB];
            buildCylinder(sphereA, sphereB, handJoints[k++]);

            //createCylinder("Hand Joints", sphereA, sphereB);
        }

        //Create the rest of the hand
        Transform thumbBase = _jointSpheres[0]; // 0 * 4 = THUMB_BASE_INDEX
        Transform pinkyBase = _jointSpheres[16]; // 4 * 4 = PINKY_BASE_INDEX
        buildCylinder(thumbBase, mockThumbJointSphere, transform.FindChild("Hand Bottom"));
        buildCylinder(pinkyBase, mockThumbJointSphere, transform.FindChild("Hand Side"));
        //createCylinder("Hand Bottom", thumbBase, mockThumbJointSphere);
        //createCylinder("Hand Side", pinkyBase, mockThumbJointSphere);

        //createCylinder("ArmFront", armFrontLeft, armFrontRight, true);
        //createCylinder("ArmBack", armBackLeft, armBackRight, true);
        //createCylinder("ArmLeft", armFrontLeft, armBackLeft, true);
        //createCylinder("ArmRight", armFrontRight, armBackRight, true);
    }

    private void buildCylinder(Transform sphereA, Transform sphereB, Transform joint) {
        _sphereATransforms.Add(sphereA);
        _sphereBTransforms.Add(sphereB);
        _cylinderTransforms.Add(joint);

    }

    private Mesh generateCylinderMesh(float length) {
        Mesh mesh = new Mesh();
        mesh.name = "GeneratedCylinder";
        mesh.hideFlags = HideFlags.DontSave;

        List<Vector3> verts = new List<Vector3>();
        List<Color> colors = new List<Color>();
        List<int> tris = new List<int>();

        Vector3 p0 = Vector3.zero;
        Vector3 p1 = Vector3.forward * length;
        for (int i = 0; i < _cylinderResolution; i++) {
            float angle = (Mathf.PI * 2.0f * i) / _cylinderResolution;
            float dx = CYLINDER_RADIUS * Mathf.Cos(angle);
            float dy = CYLINDER_RADIUS * Mathf.Sin(angle);

            Vector3 spoke = new Vector3(dx, dy, 0);

            verts.Add((p0 + spoke) * transform.lossyScale.x);
            verts.Add((p1 + spoke) * transform.lossyScale.x);

            colors.Add(Color.white);
            colors.Add(Color.white);

            int triStart = verts.Count;
            int triCap = _cylinderResolution * 2;

            tris.Add((triStart + 0) % triCap);
            tris.Add((triStart + 2) % triCap);
            tris.Add((triStart + 1) % triCap);

            tris.Add((triStart + 2) % triCap);
            tris.Add((triStart + 3) % triCap);
            tris.Add((triStart + 1) % triCap);
        }

        mesh.SetVertices(verts);
        mesh.SetIndices(tris.ToArray(), MeshTopology.Triangles, 0);
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        ;
        mesh.UploadMeshData(true);

        return mesh;
    }

    public void SetHandColor(Color color) {
        for (int i = 0; i < _jointSpheres.Length; ++i) {
            _jointSpheres[i].GetComponent<Renderer>().material.color = color;
        }
        mockThumbJointSphere.GetComponent<Renderer>().material.color = color;
        palmPositionSphere.GetComponent<Renderer>().material.color = color;
        wristPositionSphere.GetComponent<Renderer>().material.color = color;
    }
}
