using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContainerManager : MonoBehaviour
{
    #region - Singleton -

    public static ContainerManager instance;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of Container Manager found");
            return;
        }

        instance = this;
    }

    #endregion

    public GameObject containerMenu;
    public Transform containerPanelItem;
    public GameObject containerItemElement;
    public Button takeAllButton;
    public Button closeButton;
}
