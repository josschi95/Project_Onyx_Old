using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Inventory/Apparel/Head")]
public class Apparel_Head : Apparel
{
    [Header("Head")]
    public CoverType coverType;

    public SkinnedMeshRenderer fullMesh;
    [Tooltip("0 for most, 1 for crown, 2 for capotain, 3 for tudor, 4 for witch")]
    public SkinnedMeshRenderer baseHairMesh;
    [Tooltip("0 for most, 1 for open_faced and wolf, 2 for Bear")]
    public SkinnedMeshRenderer noHairMesh;
    [Tooltip("0 for tiaras, 1 for bandana1, 2 for bandana2")]
    public SkinnedMeshRenderer noFacialHairMesh;
    [Space]
    public SkinnedMeshRenderer headAttachmentMesh;

    public bool hasAttachment = false;
    [Range(0, 4)]
    public int boneStructure;
}

public enum CoverType { Base_Hair, No_Hair, No_Face_Hair, Full }
