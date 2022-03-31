using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Inventory/Apparel/Feet")]
public class Apparel_Feet : Apparel
{
    [Header("Feet")]
    public SkinnedMeshRenderer footMesh_L;
    public SkinnedMeshRenderer footMesh_R;
}
