using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

/// <summary>
/// Manages the player's field, including playing and selecting
/// </summary>
public class FieldManager : MonoBehaviour {
    public HandManager handManager; // Reference to HandManager to get selected card
    public DeckManager deckManager; // Reference to DeckManager for discard pile
    public FieldManager fieldManager; // Self-reference for field interactions
    public EnemyManager enemyManager; // Reference to EnemyManager for enemy interactions
    public List<Card> fieldCards = new List<Card>(); // List to store cards on the player's field
    public List<Image> fieldZones = new List<Image>(); // UI slots to display cards on the player's field
    public Color selectedZoneColor = Color.green; // Color for highlighting selected field zone
    public Sprite playerPlaceholderSprite; // Placeholder sprite for empty player field zones
    public int selectedZoneIndex = -1; // Index of the selected player field zone (-1 means no selection)
    public DynamicButtonManager buttonManager; // Reference to the button manager
    public TurnManager turnManager; // Reference to the turn manager
    public TMP_Text manaText; // Text component to display mana
    public int manaCount; // Current mana count for the player
    public CardInspect cardInspectManager; // Reference to the CardInspect manager
    public float highlightStrength; // Strength of the highlight blending
    public Color baseColor = Color.white; // Base color for field zones
    public Color finalColor = Color.white; // Final color used for highlighting
    private Card selectedCardForPlay; // Card selected for play
    private int selectedIndexForPlay; // Index of the card selected for play

    void Start() {
    manaCount = 3;
    updateMana();
    InitializeFieldCards(); 
    UpdateFieldZones();
    }

    /// <summary>
    /// Initializes fieldCards to match the number of fieldZones.
    /// </summary>
    /// <param name="numberOfZones">Number of field zones to initialize.</param>
    public void InitializeFieldCards() {
        while (fieldCards.Count < fieldZones.Count) {
            fieldCards.Add(null);
        }
    }

    /// <summary>
    /// Selects or deselects a field zone.
    /// </summary>
    /// <param name="index">Index of the field zone to select or deselect.</param>
    public void SelectFieldZone(int index) {
        if(!turnManager.IsPlayerTurn()) {
            Debug.LogWarning("It is not your turn");
            return;
        }

        if (index >= 0 && index < fieldZones.Count) {
            Debug.Log("Field Card Selection at index of: " + index);
            cardInspectManager.OnCardClicked(GetPlayerCardAtIndex(index));
            handManager.Deselect();
            enemyManager.Deselect();
            selectedZoneIndex = (selectedZoneIndex == index) ? -1 : index;
            if(selectedZoneIndex == -1) {
                enemyManager.RemoveAttackSelection();
            }
            buttonManager.DeactivateAllAttack();
            buttonManager.DeactivateAll();
            UpdateFieldZones();
        }
        else {
            Debug.LogWarning("Selected field zone index is out of range.");
        }
    }

    /// <summary>
    /// Places a selected card from the hand into a field zone.
    /// </summary>
    /// <param name="index">Index of the field zone to place the card in.</param>
    public void PlaySelectedCardToField(int index) {

        if (fieldCards[index] != null) {
            Debug.LogWarning("Field zone is already occupied. Choose an empty zone.");
            return;
        }

        if (handManager == null) {
            Debug.LogWarning("HandManager is not assigned.");
            return;
        }

        if (selectedCardForPlay != null) {
            if(selectedCardForPlay.manaRequired > manaCount) {
                Debug.LogWarning("You do not have enough mana");
                return;
            }
            manaCount -= selectedCardForPlay.manaRequired;
            RemovePlaySelection();
            updateMana();
            selectedCardForPlay.resetHasAttacked();
            selectedCardForPlay.resetHealth();
            selectedCardForPlay.setSummonSickness(true);
            fieldCards[index] = selectedCardForPlay;
            fieldZones[index].sprite = selectedCardForPlay.artwork;
            fieldZones[index].enabled = true;
            buttonManager.DeactivateAll();
            handManager.RemoveCardAt(selectedIndexForPlay); 
            selectedZoneIndex = -1;       
           
            UpdateFieldZones();
        } else {
            Debug.LogWarning("No selected card in hand to play.");
        }
    }

    /// <summary>
    /// Updates the visuals of each field zone based on fieldCards.
    /// </summary>
    public void UpdateFieldZones() {
        buttonManager.UpdateButtons();
        if (fieldZones.Count != fieldCards.Count) {
            Debug.LogWarning("Mismatch between fieldZones and fieldCards count. Ensure they are the same length.");
            return;
        }

        for (int i = 0; i < fieldZones.Count; i++) {
            if (fieldCards[i] != null) {
                fieldZones[i].sprite = fieldCards[i].artwork;
                fieldZones[i].enabled = true;
            } else {
                fieldZones[i].sprite = playerPlaceholderSprite;
                fieldZones[i].enabled = true;
            }

            if(selectedZoneIndex != -1) finalColor = Color.Lerp(baseColor, selectedZoneColor, highlightStrength);
            fieldZones[i].color = (i == selectedZoneIndex) ? finalColor : Color.white;
        }
    }

    /// <summary>
    /// Deselects all field zones.
    /// </summary>
    public void DeselectAll() {
        selectedZoneIndex = -1;
        UpdateFieldZones();
    }

    /// <summary>
    /// Get method for the selected card
    /// </summary>
    /// <returns>The selected card or if none is selected null</returns>
    public Card GetSelectedFieldZone() {
        if (selectedZoneIndex >= 0 && selectedZoneIndex < fieldCards.Count) {
            return fieldCards[selectedZoneIndex];
        }
        Debug.LogWarning("No valid field zone selected.");
        return null;
    }

    /// <summary>
    /// Get method for a card on the field with a given index
    /// </summary>
    /// <returns>The card that is at the correct index or null if the index is out of range</returns>
    /// <param name="index">Index of the card to be given.</param>
    public Card GetPlayerCardAtIndex(int index) {
        if (index >= 0 && index < fieldCards.Count) {
            return fieldCards[index];
        }
        Debug.LogWarning("Index out of range in GetPlayerCardAtIndex.");
        return null;
    }

    /// <summary>
    /// Removes a player card at an input index
    /// </summary>
    /// <param name="index">Index of the card to be removed.</param>
    public void RemovePlayerCard(int index) {
        if (index >= 0 && index < fieldCards.Count) {
            Card cardToRemove = fieldCards[index];
            if (cardToRemove != null) {
                deckManager.discardPile.Push(cardToRemove);
            
                fieldCards[index] = null;   
                UpdateFieldZones();         
            } else Debug.LogWarning($"No card found in field zone {index} to remove.");
        } else Debug.LogWarning($"Invalid index {index} in RemovePlayerCard.");
    }

    /// <summary>
    /// Get method for the selected cards index
    /// </summary>
    /// <returns>The index of the selected card or if none is selected -1</returns>
    public int getSelectedZoneIndex() {
        return selectedZoneIndex;
    }

    /// <summary>
    /// Resets the has attacked for all cards on the field
    /// </summary>
    public void ResetFieldHasAttacked() {
        foreach (var card in fieldCards) {
            if (card != null) {
                card.resetHasAttacked(); 
            }
        }
    }

    /// <summary>
    /// Adds to the players mana
    /// </summary>
    /// <param name="ammount">The ammound to be added.</param>
    public void addMana(int ammount) {
        if(ammount <= 0) {
            return;
        }
        manaCount += ammount;
        updateMana();
    }

    /// <summary>
    /// Decreases the players mana
    /// </summary>
    /// <param name="ammount">The ammount to be removed.</param>
    public void useMana(int ammount) {
        if(ammount <= 0 || ammount > manaCount) {
            return;
        }
        manaCount -= ammount;
    }   

    /// <summary>
    /// Resets the players mana is used by the reset manager to setup the next game
    /// </summary>
    public void resetMana() {
        manaCount = 3;
        updateMana();
        turnManager.isFirstTurn = true;
    }

    /// <summary>
    /// Updates the mana text
    /// </summary>
    public void updateMana () {
        manaText.SetText(manaCount + "");
    }

    /// <summary>
    /// Highlights field zones on hover
    /// </summary>
    /// <param name="index">Index of the field zone to be highlighted.</param>
    public void HighlightOnHover(int index) {  
        if (index >= 0 && index < fieldZones.Count) {
           
            if (index != selectedZoneIndex) {
                fieldZones[index].color = Color.Lerp(baseColor, selectedZoneColor, highlightStrength/2);
            }
        } else Debug.LogWarning("Invalid index for HighlightOnHover.");
    }

    /// <summary>
    /// Resets the highlight of field zone after hover
    /// </summary>
    /// <param name="index">Index of the field zone highlight is to be removed from.</param>
    public void ResetHoverHighlight(int index) {
        if (index >= 0 && index < fieldZones.Count) {
            if (index != selectedZoneIndex) {
                fieldZones[index].color = baseColor;
            }
        } else Debug.LogWarning("Invalid index for ResetHoverHighlight.");
    }

    /// <summary>
    /// Adds a play selection event to a specific player field zone.
    /// This is the helper method for PlaySelectionMade() and gets 
    /// called for every player field zone
    /// </summary>
    /// <param name="image">Image component of the field zone.</param>
    /// <param name="index">Index of the field zone.</param>
    public void PlaySelectionMade(Image image, int index) {
        if (image == null) {
            Debug.LogWarning("Image is null. Cannot modify EventTrigger.");
            return;
        }
        image.gameObject.SetActive(false);
        image.gameObject.SetActive(true);

        EventTrigger eventTrigger = image.gameObject.GetComponent<EventTrigger>();
        if (eventTrigger == null) {
            eventTrigger = image.gameObject.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry clickEntry = eventTrigger.triggers.Find(entry => entry.eventID == EventTriggerType.PointerClick);
        if (clickEntry == null) {
            clickEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
            eventTrigger.triggers.Add(clickEntry);
        } else clickEntry.callback.RemoveAllListeners();
        image.gameObject.SetActive(false);
        image.gameObject.SetActive(true);

        clickEntry.callback.AddListener((eventData) =>
        {
            fieldManager.PlaySelectedCardToField(index); 
        });
        image.gameObject.SetActive(false);
        image.gameObject.SetActive(true);
    }


    public void PlaySelectionMade() {
        selectedCardForPlay = handManager.GetSelectedCard();
        selectedIndexForPlay = handManager.selectedCardIndex;
        int i = 0;
        foreach (Image image in fieldZones) {
            PlaySelectionMade(image, i);
            i++;
        }
    }

    /// <summary>
    /// Removes the play selection event from a field zone.
    /// This is the helper method for RemovePlaySelection() and gets 
    /// called for every player field zone
    /// </summary>
    /// <param name="image">Image component of the field zone.</param>
    /// <param name="index">Index of the field zone.</param>
    public void RemovePlaySelection(Image image, int index) {
        if (image == null) {
        Debug.LogWarning("Image is null. Cannot modify EventTrigger.");
        return;
        }

        EventTrigger eventTrigger = image.gameObject.GetComponent<EventTrigger>();
        if (eventTrigger == null) {
            Debug.LogWarning("No EventTrigger found on the specified Image.");
            return;
        }

        var clickEntry = eventTrigger.triggers.Find(entry => entry.eventID == EventTriggerType.PointerClick);
        if (clickEntry != null) eventTrigger.triggers.Remove(clickEntry); 
        UnityEngine.EventSystems.EventSystem.current.UpdateModules();

        EventTrigger.Entry newClickEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
        newClickEntry.callback.AddListener((eventData) =>
        {
            SelectFieldZone(index);
        });
        eventTrigger.triggers.Add(newClickEntry);
        UnityEngine.EventSystems.EventSystem.current.UpdateModules();

        DeselectAll();
    }



    public void RemovePlaySelection() {
        int i = 0;
        foreach (Image image in fieldZones) {
            RemovePlaySelection(image, i);
            i++;
        }
    }

    public void NoSummonSickness() {
        foreach (Card card in fieldCards) {
            if(card != null) card.setSummonSickness(false);
        }
    }

    



}
