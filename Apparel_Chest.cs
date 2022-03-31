using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Inventory/Apparel/Body")]

public class Apparel_Chest : Apparel
{
    [Header("Body")]
    public SkinnedMeshRenderer bodyMesh;
    public bool legsVariant = false;
    public SkinnedMeshRenderer legMesh;
    [Space]
    public bool hasArms = false;
    public SkinnedMeshRenderer upperArmMesh_L;
    public SkinnedMeshRenderer upperArmMesh_R;
    [Space]
    public bool longSleeves = false;
    public SkinnedMeshRenderer lowerArmMesh_L;
    public SkinnedMeshRenderer lowerArmMesh_R;
}

