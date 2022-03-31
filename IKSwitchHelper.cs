using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKSwitchHelper : MonoBehaviour
{
    public IKHelperTool IKHelper2H;
    public IKHelperTool IKHelperPole;

    // Start is called before the first frame update
    void Start()
    {
        EquipmentManager.instance.onEquipmentChanged += SwitchActiveTool;
    }

    public void SwitchActiveTool(Equipment newItem, Equipment oldItem)
    {
        if (newItem is Weapon weapon)
        {
            if (weapon.weaponType == WeaponType.Heavy)
            {
                IKHelper2H.enabled = true;
                IKHelperPole.enabled = false;
            }
            else if (weapon.weaponType == WeaponType.Pole)
            {
                IKHelper2H.enabled = false;
                IKHelperPole.enabled = true;
            }
            else
            {
                IKHelper2H.enabled = false;
                IKHelperPole.enabled = false;
            }
        }
    }
}