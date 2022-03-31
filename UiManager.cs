using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class UiManager : MonoBehaviour
{
    #region - Singleton -

    public static UiManager instance;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of UI Manager found");
            return;
        }

        instance = this;
    }

    #endregion

    public TMP_Text carryCapacityText, goldText;

    public GameObject menu;
    [Tooltip("Root for inventory items")]
    public Transform InventoryPanelItem;
    [Tooltip("prefab representing inventory item UI")]
    public GameObject InventoryItemElement;
    public GameObject containerMenu;

    int activePanel = 0;

    public GameObject interactionPopUp;
    public TMP_Text popUpText;
    float interactionDistance = 7f;

    Camera cam;
    Inventory inventory;
    PlayerStats playerStats;
    Interactable interactTarget;
    [SerializeField] PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        inventory = Inventory.instance;
        playerStats = PlayerStats.instance;
        cam = Camera.main;

        inventory.onItemChangedCallback += UpdateMenu;
        //EquipmentManager.instance.onEquipmentChanged += UpdateMenu;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameMaster.instance.gamePaused == true || menu.activeSelf == true || containerMenu.activeSelf == true)
        {
            interactionPopUp.SetActive(false);
        }

        CheckForInteract();
    }

    #region - Interaction -
    void CheckForInteract()
    {
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hit;
        Debug.DrawRay(cam.transform.position, cam.transform.forward, Color.blue);
        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            Interactable interactable = hit.collider.GetComponentInParent<Interactable>();
            if (interactable != null && interactable.enabled == true)
            {
                //Ignore objects that are behind the player
                float playerDist = Vector3.Distance(cam.transform.position, playerController.transform.position);
                if (Vector3.Distance(cam.transform.position, hit.collider.transform.position) > playerDist * 0.8)
                {
                    Debug.DrawLine(cam.transform.position, hit.collider.transform.position, Color.black);
                    interactTarget = interactable;
                    if (!GameMaster.instance.gamePaused)
                    {
                        interactionPopUp.SetActive(true);
                        popUpText.text = interactable.interactionMethod.ToString() + ": " + interactable.name;
                    }
                    else
                    {
                        interactionPopUp.SetActive(false);
                    }
                }
            }
            else
            {
                interactTarget = null;
                interactionPopUp.SetActive(false);
            }
        }
        else
        {
            interactTarget = null;
            interactionPopUp.SetActive(false);
        }
    }

    public void Interact()
    {
        if (interactTarget == null || !playerController.acceptInput)
            return;

        interactionPopUp.SetActive(false);
        interactTarget.GetComponentInParent<Interactable>().Interact();

        Container container = interactTarget.GetComponent<Container>();
        if (container != null && container.isChest)
        {
            playerStats.animator.CrossFade("Loot", 0.1f, 0);
        }
    }
    #endregion

    public void ToggleMenu()
    {
        menu.SetActive(!menu.activeSelf);

        if (menu.activeSelf == true)
        {
            GameMaster.instance.PauseGame();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            interactionPopUp.SetActive(false);
            if (containerMenu.activeSelf)
            {
                containerMenu.SetActive(false);
            }
        }
        else
        {
            GameMaster.instance.ResumeGame();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        UpdateMenu();
    }

    //Updates all values of the menu 
    public void UpdateMenu()
    {
        float totalWeight = inventory.carryWeight + EquipmentManager.instance.apparelWeight - 1;
        carryCapacityText.text = totalWeight.ToString() + "/" + playerStats.carryCapacity.GetValue().ToString();
        goldText.text = inventory.gold.ToString();

        if (activePanel == 0)
        {
            DisplayAllCategories();
        }
        else if (activePanel == 1)
        {
            DisplayApparelCategory();
        }
        else if (activePanel == 2)
        {
            DisplayWeaponsCategory();
        }
        else if (activePanel == 3)
        {
            DisplayMagicCategory();
        }
        else if (activePanel == 4)
        {
            DisplayPotionsCategory();
        }
        else if (activePanel ==5)
        {
            DisplayMiscCategory();
        }
    }

    #region - Inventory UI Functions -
    public void ClearInventoryPanelItems()
    {
        foreach (Transform child in InventoryPanelItem.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void DisplayAllCategories()
    {
        activePanel = 0;

        ClearInventoryPanelItems();
        var combinedList = inventory.apparel.Union(inventory.weapons).Union(inventory.spells).Union(inventory.potions).Union(inventory.miscellaneous);
        combinedList = combinedList.OrderBy(go => go.name).ToList();
        foreach (Item item in combinedList)
        {
            GameObject newItem = Instantiate(InventoryItemElement) as GameObject;
            InventoryItemUi txtItem = newItem.GetComponent<InventoryItemUi>();
            txtItem.itemImage.sprite = item.icon;
            if (item.itemQuantity > 1)
            {
                txtItem.itemName.text = (item.name + " (" + item.itemQuantity + ")").ToString();
            }
            else
            {
                txtItem.itemName.text = (item.name).ToString();

            }
            txtItem.itemWeight.text = item.weight.ToString();
            txtItem.itemValue.text = item.value.ToString();

            if (item is Equipment equipment)
            {
                RightClick newInput = txtItem.itemButton.GetComponent<Button>().GetComponent<RightClick>();
                newInput.leftClick.AddListener(equipment.Use);
                newInput.rightClick.AddListener(equipment.UseSecondary);
            }
            else
            {
                //txtItem.itemButton.GetComponent<Button>().onClick.AddListener(() => item.Use());
                txtItem.itemButton.GetComponent<Button>().onClick.AddListener(item.Use);
            }
            if (item is Equipment equip && equip.isEquipped)
            {
                txtItem.equippedFrame.enabled = true;
            }
            else
            {
                txtItem.equippedFrame.enabled = false;
            }
            newItem.transform.SetParent(InventoryPanelItem);
        }
    }

    public void DisplayApparelCategory()
    {
        activePanel = 1;

        ClearInventoryPanelItems();
        foreach (Item item in inventory.apparel)
        {
            GameObject newItem = Instantiate(InventoryItemElement) as GameObject;
            InventoryItemUi txtItem = newItem.GetComponent<InventoryItemUi>();
            txtItem.itemImage.sprite = item.icon;
            if (item.itemQuantity > 1)
            {
                txtItem.itemName.text = (item.name + " (" + item.itemQuantity + ")").ToString();
            }
            else
            {
                txtItem.itemName.text = (item.name).ToString();

            }
            txtItem.itemWeight.text = item.weight.ToString();
            txtItem.itemValue.text = item.value.ToString();

            if (item is Equipment equipment)
            {
                RightClick newInput = txtItem.itemButton.GetComponent<Button>().GetComponent<RightClick>();
                newInput.leftClick.AddListener(equipment.Use);
                newInput.rightClick.AddListener(equipment.UseSecondary);
            }

            if (item is Equipment equip && equip.isEquipped)
            {
                txtItem.equippedFrame.enabled = true;
            }
            else
            {
                txtItem.equippedFrame.enabled = false;
            }

            newItem.transform.SetParent(InventoryPanelItem);
        }
    }

    public void DisplayWeaponsCategory()
    {
        activePanel = 2;

        ClearInventoryPanelItems();
        foreach (Item item in inventory.weapons)
        {
            GameObject newItem = Instantiate(InventoryItemElement) as GameObject;
            InventoryItemUi txtItem = newItem.GetComponent<InventoryItemUi>();
            txtItem.itemImage.sprite = item.icon;
            if (item.itemQuantity > 1)
            {
                txtItem.itemName.text = (item.name + " (" + item.itemQuantity + ")").ToString();
            }
            else
            {
                txtItem.itemName.text = (item.name).ToString();

            }
            txtItem.itemWeight.text = item.weight.ToString();
            txtItem.itemValue.text = item.value.ToString();

            if (item is Equipment equipment)
            {
                RightClick newInput = txtItem.itemButton.GetComponent<Button>().GetComponent<RightClick>();
                newInput.leftClick.AddListener(equipment.Use);
                newInput.rightClick.AddListener(equipment.UseSecondary);
            }

            if (item is Equipment equip && equip.isEquipped)
            {
                txtItem.equippedFrame.enabled = true;
            }
            else
            {
                txtItem.equippedFrame.enabled = false;
            }

            newItem.transform.SetParent(InventoryPanelItem);
        }
    }

    public void DisplayMagicCategory()
    {
        activePanel = 3;

        ClearInventoryPanelItems();
        foreach (Spell spell in inventory.spells)
        {
            GameObject newItem = Instantiate(InventoryItemElement) as GameObject;
            InventoryItemUi txtItem = newItem.GetComponent<InventoryItemUi>();
            txtItem.itemImage.sprite = spell.icon;
            txtItem.itemName.text = (spell.name).ToString();
            txtItem.itemWeight.text = spell.manaCost.ToString();
            //txtItem.itemValue.text = spell.value.ToString();

            RightClick newInput = txtItem.itemButton.GetComponent<Button>().GetComponent<RightClick>();
            newInput.leftClick.AddListener(spell.Use);
            newInput.rightClick.AddListener(spell.UseSecondary);

            if (spell is Equipment equip && equip.isEquipped)
            {
                txtItem.equippedFrame.enabled = true;
            }
            else
            {
                txtItem.equippedFrame.enabled = false;
            }


            newItem.transform.SetParent(InventoryPanelItem);
        }
    }

    public void DisplayPotionsCategory()
    {
        activePanel = 4;

        ClearInventoryPanelItems();
        foreach (Item item in inventory.potions)
        {
            GameObject newItem = Instantiate(InventoryItemElement) as GameObject;
            InventoryItemUi txtItem = newItem.GetComponent<InventoryItemUi>();
            txtItem.itemImage.sprite = item.icon;
            if (item.itemQuantity > 1)
            {
                txtItem.itemName.text = (item.name + " (" + item.itemQuantity + ")").ToString();
            }
            else
            {
                txtItem.itemName.text = (item.name).ToString();

            }
            txtItem.itemWeight.text = item.weight.ToString();
            txtItem.itemValue.text = item.value.ToString();
            txtItem.itemButton.GetComponent<Button>().onClick.AddListener(() =>
            item.Use());
            txtItem.equippedFrame.enabled = false;
            newItem.transform.SetParent(InventoryPanelItem);
        }
    }

    public void DisplayMiscCategory()
    {
        activePanel = 5;

        ClearInventoryPanelItems();
        foreach (Item item in inventory.miscellaneous)
        {
            GameObject newItem = Instantiate(InventoryItemElement) as GameObject;
            InventoryItemUi txtItem = newItem.GetComponent<InventoryItemUi>();
            txtItem.itemImage.sprite = item.icon;
            if (item.itemQuantity > 1)
            {
                txtItem.itemName.text = (item.name + " (" + item.itemQuantity + ")").ToString();
            }
            else
            {
                txtItem.itemName.text = (item.name).ToString();

            }
            txtItem.itemWeight.text = item.weight.ToString();
            txtItem.itemValue.text = item.value.ToString();
            txtItem.itemButton.GetComponent<Button>().onClick.AddListener(() =>
            item.Use());
            txtItem.equippedFrame.enabled = false;
            newItem.transform.SetParent(InventoryPanelItem);
        }
    }
    #endregion
}

//Possibly change so instead of after the item name it's the weight and value, change to "infoSpace1" or something and have that be.... damage, cost, etc.