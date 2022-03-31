using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Inventory/Apparel/Accessories")]

public class Apparel_Accessories : Apparel
{
    [Header("Accessories")]
    public AccessoryType accessoryType;
    [Space]
    public SkinnedMeshRenderer pauldronMesh_L;
    public SkinnedMeshRenderer pauldronMesh_R;
    [Space]
    public SkinnedMeshRenderer capeMesh;
    [Space]
    public SkinnedMeshRenderer elbowMesh_L;
    public SkinnedMeshRenderer elbowMesh_R;
    [Space]
    public SkinnedMeshRenderer kneeMesh_L;
    public SkinnedMeshRenderer kneeMesh_R;

    [Range(0,1)]
    public int capeBoneStruct;
}

public enum AccessoryType { Pauldrons, Cape, Elbowpads, Kneepads }
