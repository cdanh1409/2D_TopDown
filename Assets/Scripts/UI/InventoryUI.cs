using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TopDownSurvival.Core;
using System.Collections.Generic;

namespace TopDownSurvival.UI
{
    public class InventoryUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Inventory playerInventory;
        [SerializeField] private GameObject uiPanel;
        [SerializeField] private Transform slotsContainer;

        private List<Image> slotIconImages = new List<Image>();
        private List<Text> slotCountTexts = new List<Text>();

        private void Start()
        {
            if (playerInventory == null)
            {
                var player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    playerInventory = player.GetComponent<Inventory>();
                }
            }

            if (playerInventory != null)
            {
                playerInventory.OnInventoryChanged += Redraw;
            }

            // Cache slot elements
            if (slotsContainer != null)
            {
                for (int i = 0; i < slotsContainer.childCount; i++)
                {
                    Transform slotObj = slotsContainer.GetChild(i);
                    Transform iconTrans = slotObj.Find("Icon");
                    Transform countTrans = slotObj.Find("Count");

                    if (iconTrans != null)
                    {
                        Image img = iconTrans.GetComponent<Image>();
                        slotIconImages.Add(img);
                    }
                    if (countTrans != null)
                    {
                        Text txt = countTrans.GetComponent<Text>();
                        slotCountTexts.Add(txt);
                    }
                }
            }

            // Initial Draw
            Redraw();

            // Default closed
            if (uiPanel != null)
            {
                uiPanel.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            if (playerInventory != null)
            {
                playerInventory.OnInventoryChanged -= Redraw;
            }
        }

        private void Update()
        {
            if (Keyboard.current == null) return;

            // Toggle Inventory with Tab, I, or Escape
            if (Keyboard.current.tabKey.wasPressedThisFrame || Keyboard.current.iKey.wasPressedThisFrame || Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                Toggle();
            }
        }

        public void Toggle()
        {
            if (uiPanel != null)
            {
                bool active = !uiPanel.activeSelf;
                uiPanel.SetActive(active);
                if (active)
                {
                    Redraw();
                }
            }
        }

        public void Show()
        {
            if (uiPanel != null)
            {
                uiPanel.SetActive(true);
                Redraw();
            }
        }

        public void Hide()
        {
            if (uiPanel != null)
            {
                uiPanel.SetActive(false);
            }
        }

        public void Redraw()
        {
            if (playerInventory == null) return;

            List<ItemStack> slots = playerInventory.GetSlots();

            for (int i = 0; i < slotIconImages.Count; i++)
            {
                if (i >= slots.Count) break;

                ItemStack slot = slots[i];
                if (slot.item != null && slot.count > 0)
                {
                    slotIconImages[i].sprite = slot.item.icon;
                    slotIconImages[i].enabled = true;
                    slotIconImages[i].color = Color.white;

                    if (slotCountTexts[i] != null)
                    {
                        slotCountTexts[i].text = slot.count > 1 ? slot.count.ToString() : "";
                        slotCountTexts[i].enabled = true;
                    }
                }
                else
                {
                    slotIconImages[i].sprite = null;
                    slotIconImages[i].enabled = false;

                    if (slotCountTexts[i] != null)
                    {
                        slotCountTexts[i].text = "";
                        slotCountTexts[i].enabled = false;
                    }
                }
            }
        }
    }
}
