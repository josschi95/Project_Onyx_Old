using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXManager : MonoBehaviour
{
    #region - Singleton -
    public static VFXManager instance { get; private set; }
    void Awake()
    {
        instance = this;
    }
    #endregion

    [Header("Movement")]
    public GameObject step_FX;
    public GameObject jump_FX;

    [Header("Combat")]
    public GameObject swingEffect_1;
    public GameObject swingEffect_2;
    public GameObject swingEffect_3;
    [Space]
    public GameObject bloodSplatter_small;
    public GameObject bloodSplatter_large;
    public GameObject shieldStrike;
}
