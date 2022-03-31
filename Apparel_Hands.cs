using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Inventory/Apparel/Hands")]

public class Apparel_Hands : Apparel
{
    [Header("Hands")]
    public bool hasHands = false;
    public SkinnedMeshRenderer handMesh_L;
    public SkinnedMeshRenderer handMesh_R;
    [Space]
    public bool hasForeArms = false;
    public SkinnedMeshRenderer forearmMesh_L;
    public SkinnedMeshRenderer forearmMesh_R;
}
