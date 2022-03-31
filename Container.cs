using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Container : Interactable
{
    public bool isChest = false;
    public bool isLocked = false;
    bool isEmpty = false;

    public List<Item> contents = new List<Item>();

    GameObject containerMenu;
    Transform containerPanelItem;
    GameObject containerItemElement;
    Button takeAllButton;
    Button closeButton;

    Inventory inventory;
    ContainerManager containerManager;

    private void Awake()
    {
        if (isLocked)
        {
            interactionMethod = InteractionMethod.Unlock;
        }
        if (contents.Count == 0 && !isEmpty)
        {
            isEmpty = true;
            gameObject.name = name + " (Empty)";
        }
    }

    public void Start()
    {
        inventory = Inventory.instance;
        containerManager = ContainerManager.instance;

        containerMenu = containerManager.containerMenu;
        containerPanelItem = containerManager.containerPanelItem;
        containerItemElement = containerManager.containerItemElement;
        takeAllButton = containerManager.takeAllButton;
        closeButton = containerManager.closeButton;
    }

    public override void Interact()
    {
        base.Interact();

        containerMenu.SetActive(true);
        DisplayContents();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        takeAllButton.onClick.AddListener(TakeAll);
        closeButton.onClick.AddListener(CloseMenu);
    }

    public void UpdateContainerMenu()
    {
        DisplayContents();
    }

    public void ClearContainerPanelItems()
    {
        foreach (Transform child in containerPanelItem.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void DisplayContents()
    {
        ClearContainerPanelItems();

        foreach (Item item in contents)
        {
            GameObject newItem = Instantiate(containerItemElement) as GameObject;
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
            {
                inventory.AddItem(item);
                contents.Remove(item);
                DisplayContents();
                if (contents.Count == 0)
                {
                    CloseMenu();
                }
            });

            txtItem.equippedFrame.enabled = false;
            newItem.transform.SetParent(containerPanelItem);
        }
    }

    public void TakeAll()
    {
        foreach (Item item in contents)
        {
            inventory.AddItem(item);
        }
        contents.Clear();

        CloseMenu();
    }

    public void CloseMenu()
    {
        GameMaster.instance.ResumeGame();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        ClearContainerPanelItems();
        containerMenu.SetActive(false);
        if (contents.Count == 0 && !isEmpty)
        {
            isEmpty = true;
            gameObject.name = name + " (Empty)";
        }

    }
}
