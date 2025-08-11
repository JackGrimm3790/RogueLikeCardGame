using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the player's hand, including drawing, selecting, and discarding cards.
/// </summary>
public class HandManager : MonoBehaviour {
    public DeckManager deckManager; // Reference to the DeckManager to access the draw pile
    public DynamicButtonManager buttonManager; // Reference to the DynamicButtonManager
    public List<Card> hand = new List<Card>(); // List to store the current hand
    public List<Image> handImageSlots = new List<Image>(); // UI slots to display cards in the hand
    public Color highlightColor = Color.yellow; // Color for highlighting the selected card
    public int selectedCardIndex = -1; // Index of the selected card (-1 means no selection)
    public TurnManager turnManager; // Reference to the TurnManager
    public FieldManager fieldManager; // Reference to the FieldManager
    public EnemyManager enemyManager; // Reference to the EnemyManager
    public CardInspect cardInspectManager; // Reference to the CardInspect manager
    public float highlightStrength; // Strength of the highlight blending
    public Color baseColor = Color.white; // Base color for the hand slots
    public Color finalColor = Color.white; // Final color used for highlighting

    /// <summary>
    /// Draws a single card into the player's hand.
    /// </summary>
    public void DrawCardToHand() {
        if (hand.Count < 5) {
            Card drawnCard = deckManager.DrawCard(); 
            if (drawnCard != null) {
                hand.Add(drawnCard);          
                drawnCard.resetHasAttacked();
                drawnCard.resetHealth();
                
                UpdateHandImages();
            } else Debug.LogWarning("Draw pile is empty! No more cards to draw.");
        } else Debug.Log("Hand is full. Cannot draw more cards.");
    }

    /// <summary>
    /// Draws up to 5 cards into the player's hand.
    /// </summary>
    public void DrawHand() {
        for (int i = 0; i < 5; i++) {
            if(hand.Count >= 5) return;
            DrawCardToHand();
        }
    }

    /// <summary>
    /// For debug purposes only prints out the hand to the debug screen
    /// </summary>
    private void DisplayHand() {
        string handContents = "Current Hand: ";
        foreach (Card card in hand) {
            handContents += card.cardName + ", ";
        }
        Debug.Log(handContents);
    }

    /// <summary>
    /// Updates the visuals of the hand slots based on the current hand.
    /// </summary>
    public void UpdateHandImages() {
        buttonManager.UpdateButtons();
        for (int i = 0; i < handImageSlots.Count; i++) {
            if (i < hand.Count) {
                handImageSlots[i].sprite = hand[i].artwork;
                handImageSlots[i].enabled = true;
                if(selectedCardIndex != -1) finalColor = Color.Lerp(baseColor, highlightColor, highlightStrength);
                handImageSlots[i].color = (i == selectedCardIndex) ? finalColor : Color.white;
            } else {
                handImageSlots[i].sprite = null;
                handImageSlots[i].enabled = false;
                handImageSlots[i].color = Color.white;
            }
        }
    }


    /// <summary>
    /// Allows the player to select a card from their hand.
    /// </summary>
    /// <param name="index">The index of the card to select.</param>
    public void ChooseCard(int index) {
        if (!turnManager.IsPlayerTurn()) {
            Debug.LogWarning("Cannot select a card. It's not the player's turn.");
            return;
        }

        if (index >= 0 && index < hand.Count) {
            fieldManager.DeselectAll();
            enemyManager.Deselect();
            cardInspectManager.OnCardClicked(GetCard(index));
            if (selectedCardIndex != index) {
                selectedCardIndex = index; 
            } else {
                selectedCardIndex = -1;
                fieldManager.RemovePlaySelection();
                buttonManager.DeactivateAll();
            }

            UpdateHandImages(); 
        } else Debug.LogWarning("Invalid card index selected." + hand.Count);
    }

    /// <summary>
    /// Gets the currently selected card in the hand.
    /// </summary>
    /// <returns>The selected card or null if no card is selected.</returns>
    public Card GetSelectedCard() {
        if (selectedCardIndex >= 0 && selectedCardIndex < hand.Count) {
            return hand[selectedCardIndex];
        }
        return null;
    }

    /// <summary>
    /// Discards the selected card, granting mana based on its discard value.
    /// </summary>
    public void DiscardSelectedCard() {
        if (selectedCardIndex >= 0 && selectedCardIndex < hand.Count) {
            Card selectedCard = hand[selectedCardIndex]; 
            hand.RemoveAt(selectedCardIndex); 
            deckManager.discardPile.Push(selectedCard); 
            buttonManager.DeactivateAll();
            Deselect();
            UpdateHandImages();
            fieldManager.addMana(selectedCard.manaOnDiscard);
        } else Debug.LogWarning("No card selected to discard.");
    }

    /// <summary>
    /// Removes the card at a specified index.
    /// </summary>
    /// <param name="index">The index of the card to be removed.</param>
    public void RemoveCardAt(int index){

        if (index >= 0 && index < hand.Count) {
            hand.RemoveAt(index);
            UpdateHandImages();
            Deselect();
            buttonManager.DeactivateAll();
        }
        else Debug.LogWarning("please fix me");
    }

    /// <summary>
    /// Get method for the card at a specified index.
    /// </summary>
    /// <param name="index">The card at the index or null if the index is out of bounds or the card does not exist</param>
    public Card GetCard(int index) {
        if(index >=0 && index < hand.Count) return hand[index];
        return null;
    }

    public bool HasCard(int index) {
        if(index < 0 || index >= hand.Count) return false;
        if(hand[index] == null) return false;
        return true;
    }

    /// <summary>
    /// Deselects the selected card in the hand.
    /// </summary>
    public void Deselect() {
        selectedCardIndex = -1;
        UpdateHandImages();
    }

    /// <summary>
    /// Highlights a card in the hand.
    /// </summary>
    /// <param name="index">The index of the card to be highlighted</param>
    public void HighlightOnHover(int index) {  
        if (index >= 0 && index < handImageSlots.Count) {
            if (index != selectedCardIndex) {
                Color tempColor = Color.Lerp(baseColor, highlightColor, highlightStrength/2);
                handImageSlots[index].color = tempColor;
            }
        } else Debug.LogWarning("Invalid index for HighlightOnHover.");
    }

    /// <summary>
    /// Removes the highlight of a card in the hand.
    /// </summary>
    /// <param name="index">The index of the card to be un-highlighted</param>
    public void ResetHoverHighlight(int index) {
        if (index >= 0 && index < handImageSlots.Count)  {
            if (index != selectedCardIndex)
            {
                handImageSlots[index].color = baseColor;
            }
        } else Debug.LogWarning("Invalid index for ResetHoverHighlight.");
    }

    /// <summary>
    /// Get method for the index of the selected card.
    /// </summary>
    /// <returns>The index of the selected card</returns>
    public int GetSelectedCardIndex () {
        return selectedCardIndex;
    }

}
