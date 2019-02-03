﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Input;

public enum EquipmentSlot { PRIMARY, SECONDARY, HEAD, CHEST, WAIST, HANDS, FEET, FINGERONE, FINGERTWO, NECK }

public class InventoryPanel : UIPanel
{
    [SerializeField] private Inventory inventoryContainer;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private GameObject inventorySlotParent;
    [SerializeField] private GameObject itemPrefab;

    private InventoryManager inventoryManager;

    public Inventory InventoryContainer
    {
        get;
    }

    protected override void Awake()
    {
        base.Awake();
        inventoryManager = AssetManager.Instance.GetManager<InventoryManager>() ?? new InventoryManager();
        inventoryManager.InventoryPanel = this;
    }

    private void Start()
    {
        InputManager manager = AssetManager.Instance.GetManager<InputManager>();
        //void callback(InputAction.CallbackContext _) => ToggleVisibility();
        manager.AddListener(manager.Input.UI.InventoryOpen, ctx => ToggleVisibility());

        InstantiateInventory();
    }

    private void InstantiateInventory()
    {
        for (int slot = 0; slot < inventoryContainer.Size; slot++)
        {
            inventoryContainer.StorageSlots.Add(new StorageSlot
            {
                Position = slot,
                Slot = Instantiate(slotPrefab, inventorySlotParent.transform).GetComponent<UIElementHandler>()
            });
        }
    }

    public bool AddItemToInventory(int stackAmount, ItemContainer container)
    {
        if (stackAmount > 0 && container != null)
        {
            for (int position = 0; position < inventoryContainer.StorageSlots.Count; position++)
            {
                StorageSlot tempSlot = inventoryContainer.StorageSlots[position];
                if (container.ItemName.Equals(tempSlot.Content) && container.StackingLimit >= (stackAmount + tempSlot.Amount))
                {
                    tempSlot.Amount += stackAmount;
                    return true;
                }
                else if (inventoryContainer.StorageSlots[position].Amount == 0)
                {
                    tempSlot.Amount = stackAmount;
                    tempSlot.Content = container.ItemName;
                    return true;
                }
            }
        }
        return false;
    }

    public bool RemoveItemsFromInventory(int position, out ItemContainer container, out int amount)
    {
        StorageSlot tempSlot = inventoryContainer.StorageSlots[position];
        container = null;
        amount = tempSlot.Amount;
        if (tempSlot.Amount != 0)
        {
            container = LoadItemContainer(inventoryContainer.StorageSlots[position].Content);
            tempSlot.EmptySlot();
            return true;
        }
        return false;
    }

    public bool RemoveSingleItemFromInventory(int position, out ItemContainer container)
    {
        StorageSlot tempSlot = inventoryContainer.StorageSlots[position];
        container = null;
        if (tempSlot.Amount != 0)
        {
            container = LoadItemContainer(inventoryContainer.StorageSlots[position].Content);
            tempSlot.Amount -= 1;
            return true;
        }
        return false;
    }

    public bool EquipItem(int position, out EquipmentSlot slot)
    {
        StorageSlot tempInventorySlot = inventoryContainer.StorageSlots[position];
        slot = 0;
        if (tempInventorySlot.Amount != 0)
        {
            ItemContainer item = LoadItemContainer(inventoryContainer.StorageSlots[position].Content);
            if (item.GetItemType().Equals(ItemType.EQUIPMENT))
            {
                slot = ((EquipmentContainer)item).EquipmentType;
                tempInventorySlot.EmptySlot();
                AddItemToInventory(1, inventoryContainer.CurrentSelectedEntity.GetEquippedItem(slot));
                inventoryContainer.CurrentSelectedEntity.SetEquippedItem(slot, (EquipmentContainer)item);
                return true;
            }
        }
        return false;
    }

    public bool UnequipItem(EquipmentSlot slot)
    {
        if (AddItemToInventory(1, inventoryContainer.CurrentSelectedEntity.GetEquippedItem(slot)))
        {
            inventoryContainer.CurrentSelectedEntity.SetEquippedItem(slot, null);
        }
        return false;
    }

    private ItemContainer LoadItemContainer(string name)
    {
        return AssetManager.Instance.LoadBundle<ItemContainer>(AssetManager.Instance.Paths.ItemsPath, name);
    }
}