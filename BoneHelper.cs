using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneHelper : MonoBehaviour
{
    SkinnedMeshRenderer mesh;
    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<SkinnedMeshRenderer>();

        foreach (Transform bone in mesh.bones)
        {
            Debug.Log(gameObject.name + " " + bone.name);
        }
        Debug.Log(gameObject.name + " Root Bone " + mesh.rootBone.name + "________________");
    }
}
