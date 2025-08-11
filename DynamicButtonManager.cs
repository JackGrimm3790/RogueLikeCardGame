using UnityEngine;
using TMPro; // Import for TextMeshPro
using UnityEngine.UI; // Import for UI components
using System.Collections.Generic;

/// <summary>
/// Manages dynamic button behavior, including activation and deactivation of buttons based on game state.
/// </summary>
public class DynamicButtonManager : MonoBehaviour {
    public FieldManager fieldManager; // Reference to the FieldManager
    public HandManager handManager; // Reference to the HandManager

    private bool handSelected = false; // Tracks if a card in the hand is selected
    private bool fieldSelected = false; // Tracks if a card on the field is selected
    private bool fieldHasCard; // Tracks if a selected field zone has a card

    public List<Button> buttonList; // List of buttons for actions
    public List<Button> attackButtonList; // List of attack-specific buttons

    /// <summary>
    /// Initializes the button states by deactivating all buttons at the start.
    /// </summary>
    void Start() {
    DeactivateAll();
    DeactivateAllAttack();
    }

    /// <summary>
    /// Updates the visibility and state of buttons based on the current card selections.
    /// </summary>
    public void UpdateButtons() {
        DeactivateAll();
        DeactivateAllAttack();
        handSelected = handManager.selectedCardIndex >= 0;
        fieldSelected = fieldManager.getSelectedZoneIndex() >= 0;

        if (fieldSelected) {
            fieldHasCard = fieldManager.fieldCards[fieldManager.getSelectedZoneIndex()] != null;
        }

        if (handSelected) {
            SetActive(handManager.GetSelectedCardIndex());
        }

        if (fieldSelected && fieldHasCard) {
            SetAttackActive(fieldManager.getSelectedZoneIndex());
        }
    }

    /// <summary>
    /// Activates buttons for a specific card index based on its state and properties.
    /// </summary>
    /// <param name="index">Index of the selected card in the hand.</param>
    public void SetActive(int index) {
        if (index < 0) return;
        if (buttonList == null || buttonList.Count <= 0) return;

        Button button1 = buttonList[2 * index];
        Button button2 = buttonList[2 * index + 1];
        if (button1 != null) button1.gameObject.SetActive(true);
        else Debug.LogWarning("No Button");
        if (button2 != null) button2.gameObject.SetActive(true);
        else Debug.LogWarning("No Button");

        if (!handManager.HasCard(index)) return;

        TMP_Text buttonText1 = button1.GetComponentInChildren<TMP_Text>();
        string text1 = "Play      \n-" + handManager.GetCard(index).manaRequired;
        if (buttonText1 != null) buttonText1.text = text1;

        TMP_Text buttonText2 = button2.GetComponentInChildren<TMP_Text>();
        string text2 = "Discard   +" + handManager.GetCard(index).manaOnDiscard;
        if (buttonText2 != null) buttonText2.text = text2;
    }

    /// <summary>
    /// Deactivates all buttons in the action button list.
    /// </summary>
    public void DeactivateAll() {
        foreach (Button button in buttonList) {
            button.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Activates the attack button for a specific field zone index.
    /// </summary>
    /// <param name="index">Index of the field zone to activate the attack button for.</param>
    public void SetAttackActive(int index) {
        if (attackButtonList == null || attackButtonList.Count <= 0) return;
        Button button = attackButtonList[index];
        if (button != null) button.gameObject.SetActive(true);
    }

    /// <summary>
    /// Deactivates all buttons in the attack button list.
    /// </summary>
    public void DeactivateAllAttack() {
        foreach (Button button in attackButtonList) {
            button.gameObject.SetActive(false);
        }
    }
}
