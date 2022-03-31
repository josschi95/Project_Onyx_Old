using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EquipmentHelper : MonoBehaviour
{
    #region - Singleton -

    public static EquipmentHelper instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of EquipmentHelper found");
            return;
        }
        instance = this;
    }

    #endregion

    [Header("Weapons")]
    public Transform RH_Retargeter;
    public Transform LH_Retargeter;
    public Transform arrowSpawn;

    #region - Head -
    [Header("Head")]
    //Full Helms
    [Space]
    public Transform head_Full_RootBone;
    public Transform[] head_Full_Bones;
    [Space]

    #region - Base Hair -
    //Base_Hair
    [Tooltip("Majority")]
    public Transform head_Base_RootBone_v0;
    public Transform[] head_Base_Bones_v0;
    [Space]
    [Tooltip("Crown")]
    public Transform head_Base_RootBone_v1;
    public Transform[] head_Base_Bones_v1;
    [Space]
    [Tooltip("Capotain")]
    public Transform head_Base_RootBone_v2;
    public Transform[] head_Base_Bones_v2;
    [Space]
    [Tooltip("Tudor")]
    public Transform head_Base_RootBone_v3;
    public Transform[] head_Base_Bones_v3;
    [Space]
    [Tooltip("Witch")]
    public Transform head_Base_RootBone_v4;
    public Transform[] head_Base_Bones_v4;
    [Space]
    #endregion

    #region - No Facial Hair -
    //No Facial Hair
    public Transform head_noFace_RootBone_v0;
    public Transform[] head_noFace_Bones_v0;
    [Space]
    public Transform head_noFace_RootBone_v1;
    public Transform[] head_noFace_Bones_v1;
    [Space]
    public Transform head_noFace_RootBone_v2;
    public Transform[] head_noFace_Bones_v2;
    [Space]
    #endregion

    #region - No Hair -
    //No Hair
    public Transform head_noHair_RootBone_v0;
    public Transform[] head_noHair_Bones_v0;
    [Space]
    public Transform head_noHair_RootBone_v1;
    public Transform[] head_noHair_Bones_v1;
    [Space]
    public Transform head_noHair_RootBone_v2;
    public Transform[] head_noHair_Bones_v2;
    [Space]
    #endregion

    public Transform head_Attach_RootBone;
    public Transform[] head_Attach_Bones;
    [Space]
    #endregion

    [Header("Chest")]
    public Transform chestRootBone;
    public Transform[] chestBones;
    [Header("Legs")]
    public Transform legsRootBone;
    public Transform[] legBones_v0;
    public Transform[] legBones_v1;
    [Header("Upper Arms")]
    public Transform UA_L_rootBone;
    public Transform UA_R_rootBone;
    public Transform[] UA_L_Bones;
    public Transform[] UA_R_Bones;
    [Header("Lower Arms")]
    public Transform LA_L_rootBone;
    public Transform LA_R_rootBone;
    public Transform[] LA_L_Bones;
    public Transform[] LA_R_Bones;
    [Header("Hands")]
    public Transform hand_L_rootBone;
    public Transform hand_R_rootBone;
    public Transform[] hand_L_Bones;
    public Transform[] hand_R_Bones;
    [Header("Feet")]
    public Transform LowerLeg_L_rootBone;
    public Transform LowerLeg_R_rootBone;
    public Transform[] LowerLeg_L_Bones;
    public Transform[] LowerLeg_R_Bones;
    [Header("Shoulders")]
    public Transform Shoulder_L_rootBone;
    public Transform Shoulder_R_rootBone;
    public Transform[] Shoulder_L_Bones;
    public Transform[] Shoulder_R_Bones;
    [Header("Elbows")]
    public Transform Elbow_L_rootBone;
    public Transform Elbow_R_rootBone;
    public Transform[] Elbow_L_Bones;
    public Transform[] Elbow_R_Bones;
    [Header("Knees")]
    public Transform Knee_L_rootBone;
    public Transform Knee_R_rootBone;
    public Transform[] Knee_L_Bones;
    public Transform[] Knee_R_Bones;
    [Header("Capes")]
    public Transform cape_rootBone;
    public Transform[] cape_Bones_v0;
    public Transform[] cape_Bones_v1;
}

//var allBones = GetComponentsInChildren<Transform>();
//var bone = allBones.Where(b => b.gameObject.name == "Neck").FirstOrDefault();
//headRootBone = bone;