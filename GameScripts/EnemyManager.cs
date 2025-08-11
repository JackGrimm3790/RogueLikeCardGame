using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Manages basic enemy functionality, including the enemies deck and the player selection of enemy cards for attack
/// </summary>
public class EnemyManager : MonoBehaviour {
    public List<Card> enemyDeck = new List<Card>(); // Enemy's deck of cards
    public FieldManager fieldManager; // Reference to the FieldManager
    public List<Card> enemyFieldCards = new List<Card>(); // Cards currently on the enemy's field
    public Stack<Card> enemyDiscardPile = new Stack<Card>(); // Discard pile for the enemy's removed cards
    public List<Image> enemyFieldZones = new List<Image>(); // UI slots to display cards on the enemy's field
    public Color enemyHighlightColor = Color.red; // Color for highlighting enemy zones
    public Sprite enemyPlaceholderSprite; // Placeholder sprite for empty enemy field zones
    public Level level; // Reference to LevelScript to manage enemy actions

    public int enemyTurnNumber = 0; // Tracks the current turn number for the enemy
    public int highlightedEnemyZoneIndex = -1; // Index of the highlighted enemy zone (-1 means no highlight)
    public DynamicButtonManager buttonManager; // Reference to the DynamicButtonManager
    public TurnManager turnManager; // Reference to the TurnManager
    public CardInspect cardInspectManager; // Reference to the CardInspect manager
    public HandManager handManager; // Reference to the HandManager
    public float highlightStrength; // Strength of the highlight color blending
    public Color baseColor = Color.white; // Base color for enemy field zones
    public Color finalColor = Color.white; // Final color used for highlighting
    public BattleManager battleManager; // Reference to the BattleManager
    public EnemyManager enemyManager; // Self-reference
    public Card selectedCardForAttack; // Selected player card for attack
    public int selectedIndexForAttack; // Index of the selected player card for attack

    void Start() {
    InitializeFieldCards();
    ShuffleDeck();
    UpdateEnemyFieldZones();
    }

    /// <summary>
    /// Initializes enemyFieldCards to match the number of enemyFieldZones.
    /// </summary>
    public void InitializeFieldCards() {
        while (enemyFieldCards.Count < enemyFieldZones.Count)
        {
            enemyFieldCards.Add(null); 
        }
    }

    /// <summary>
    /// Shuffles the enemy's deck.
    /// </summary>
    public void ShuffleDeck() {
        for (int i = 0; i < enemyDeck.Count; i++) {
            Card temp = enemyDeck[i];
            int randomIndex = Random.Range(i, enemyDeck.Count);
            enemyDeck[i] = enemyDeck[randomIndex];
            enemyDeck[randomIndex] = temp;
        }
    }

    /// <summary>
    /// Returns the current turn number for the enemy and increments it.
    /// </summary>
    /// <returns>The current turn number.</returns>
    public int GetEnemyTurnNumber() {
        return enemyTurnNumber++;
    }

    /// <summary>
    /// Executes the enemy's turn actions.
    /// </summary>
    public void ExecuteEnemyTurn() {
        level.EnemyTurn();
    }

    /// <summary>
    /// Updates the visuals for the enemy field zones.
    /// </summary>
    public void UpdateEnemyFieldZones() {
        buttonManager.UpdateButtons();

        for (int i = 0; i < enemyFieldZones.Count; i++) {
            if (i < enemyFieldCards.Count && enemyFieldCards[i] != null) {
                enemyFieldZones[i].sprite = enemyFieldCards[i].artwork;
                enemyFieldZones[i].enabled = true;
            } else {
                enemyFieldZones[i].sprite = enemyPlaceholderSprite;
                enemyFieldZones[i].enabled = true;
            }

            if(highlightedEnemyZoneIndex != -1) finalColor = Color.Lerp(baseColor, enemyHighlightColor, highlightStrength);
            enemyFieldZones[i].color = (i == highlightedEnemyZoneIndex) ? finalColor : Color.white;
        }
    }

    /// <summary>
    /// Highlights or deselects an enemy field zone.
    /// </summary>
    /// <param name="index">Index of the field zone to highlight.</param>
    public void HighlightEnemyZone(int index) {
        if(!turnManager.IsPlayerTurn()) {
            Debug.LogWarning("It is not your turn");
            return;
        }
        if(index >= 0 && index < enemyFieldZones.Count) {
            Debug.Log("Enemy Card Selection at index of: " + index);
            cardInspectManager.OnCardClicked(GetCard(index));
            handManager.Deselect();
            fieldManager.DeselectAll();
            highlightedEnemyZoneIndex = (highlightedEnemyZoneIndex == index) ? -1 : index;
            buttonManager.DeactivateAll();
            buttonManager.DeactivateAllAttack();
            UpdateEnemyFieldZones();
        }
    }

    /// <summary>
    /// Gets the currently selected enemy card.
    /// </summary>
    /// <returns>The selected enemy card or null if none is selected.</returns>
    public Card GetSelectedEnemyCard() {
        if (highlightedEnemyZoneIndex >= 0 && highlightedEnemyZoneIndex < enemyFieldCards.Count) {
            return enemyFieldCards[highlightedEnemyZoneIndex];
        }
        return null; 
    }

    /// <summary>
    /// Removes an enemy card from the specified index and adds it to the discard pile.
    /// </summary>
    /// <param name="index">Index of the card to remove.</param>
    public void RemoveEnemyCard(int index) {
        if (index >= 0 && index < enemyFieldCards.Count) {
            Card cardToRemove = enemyFieldCards[index];
            if (cardToRemove != null) {
                enemyDiscardPile.Push(cardToRemove);  
                enemyFieldCards[index] = null;       
                Deselect();

                Debug.Log($"Card {cardToRemove.cardName} removed from field and added to discard pile.");
            } else {
                Debug.LogWarning("No card to remove at this field index.");
            }
        } else {
            Debug.LogWarning("Invalid enemy field index.");
        }
    }


    /// <summary>
    /// Used in other scripts to find the index of the selected card
    /// </summary>
    /// <returns>The index of the selected card or -1 if no card is selected.</returns>
    public int GetHighlightedEnemyZoneIndex() {
        return highlightedEnemyZoneIndex;
    }

    /// <summary>
    /// Used in other scripts to find the card at a specified index
    /// </summary>
    /// <param name="index">Index of the card.</param>
    /// <returns>The card at the input index.</returns>
    public Card GetCard(int index) {
        return (index >= 0 && index < enemyFieldCards.Count) ? enemyFieldCards[index] : null;
    }

    /// <summary>
    /// Used to deselect the enemy zone
    /// </summary>
    public void Deselect() {
        highlightedEnemyZoneIndex = -1;
        UpdateEnemyFieldZones();
    }

    /// <summary>
    /// Highlights an enemy card on hover.
    /// </summary>
    /// <param name="index">Index of the hovered card.</param>
    public void HighlightOnHover(int index) {  
        if (index >= 0 && index < enemyFieldZones.Count) // Ensure the index is within bounds {
            // Temporarily highlight the hovered card unless it is already selected
            if (index != highlightedEnemyZoneIndex) {
                Color tempColor = Color.Lerp(baseColor, enemyHighlightColor, highlightStrength/2);
                enemyFieldZones[index].color = tempColor;
        } else {
            Debug.LogWarning("Invalid index for HighlightOnHover.");
        }
    }

    /// <summary>
    /// Resets the highlight of an enemy card after hover.
    /// </summary>
    /// <param name="index">Index of the card to reset.</param>
    public void ResetHoverHighlight(int index) {
        if (index >= 0 && index < enemyFieldZones.Count) // Ensure the index is within bounds {
            // Reset the color to its base state unless the card is selected
            if (index != highlightedEnemyZoneIndex) {
                enemyFieldZones[index].color = baseColor;
        } else {
            Debug.LogWarning("Invalid index for ResetHoverHighlight.");
        }
    }

    /// <summary>
    /// Adds an attack selection event to a specific enemy field zone.
    /// This is the helper method for MakeAttackSelection() and gets 
    /// called for every enemy field zone
    /// </summary>
    /// <param name="image">Image component of the enemy field zone.</param>
    /// <param name="index">Index of the enemy field zone.</param>
    public void MakeAttackSelection(Image image, int index) {
        if (image == null) {
            Debug.LogWarning("Image is null. Cannot modify EventTrigger.");
            return;
        }
    
        EventTrigger eventTrigger = image.gameObject.GetComponent<EventTrigger>();
        if (eventTrigger == null) {
            eventTrigger = image.gameObject.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry clickEntry = eventTrigger.triggers.Find(entry => entry.eventID == EventTriggerType.PointerClick);
        if (clickEntry == null) {
            clickEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
            eventTrigger.triggers.Add(clickEntry);
        } else {
            clickEntry.callback.RemoveAllListeners(); 
        }

        clickEntry.callback.AddListener((eventData) => 
        {
            battleManager.Attack(index);
        
        });

        Debug.Log($"PointerClick event added for BattleManager.Attack at index {index}.");
    }

    public void MakeAttackSelection() {
        int i = 0;
        if (!turnManager.IsPlayerTurn()) {
            Debug.LogWarning("It is not the player's turn.");
            return;
        }
        foreach (Image image in enemyFieldZones) {
            MakeAttackSelection(image, i);
            i++;
        }
        selectedCardForAttack = fieldManager.GetSelectedFieldZone();
        cardInspectManager.canInspect = false;
    }

    /// <summary>
    /// Removes the attack selection event from a specific enemy field zone.
    /// This is the helper method for MakeAttackSelection() and gets 
    /// called for every enemy field zone
    /// </summary>
    /// <param name="image">Image component of the enemy field zone.</param>
    /// <param name="index">Index of the enemy field zone.</param>
    public void RemoveAttackSelection(Image image, int index) {
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
        if (clickEntry != null) {
            eventTrigger.triggers.Remove(clickEntry);
        }

        EventTrigger.Entry newClickEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
        newClickEntry.callback.AddListener((eventData) =>
        {
            Debug.Log($"Highlighting enemy zone at index {index}");
            HighlightEnemyZone(index);
        });
        eventTrigger.triggers.Add(newClickEntry);

        Debug.Log($"PointerClick event reset to call HighlightEnemyZone({index}).");
    }

    public void RemoveAttackSelection() {
        int i =0;
        foreach(Image image in enemyFieldZones) {
            RemoveAttackSelection(image, i);
            i++;
        }
        Debug.Log("Removed attack selection from all enemy field zones.");
        cardInspectManager.canInspect = true;
    }
}
