using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Manages the deck, including the draw pile, discard pile, and card initialization.
/// Provides functionality for shuffling, drawing, and resetting the deck.
/// </summary>
public class DeckManager : MonoBehaviour
{
    public HandManager handManager; // Reference to the HandManager to handle player card drawing
    public List<Card> allAvailableCards; // List of all available cards to initialize the deck
    public List<Card> drawPile = new List<Card>(); // List representing the draw pile
    public Stack<Card> discardPile = new Stack<Card>(); // Stack representing the discard pile

    /// <summary>
    /// Initializes the deck and deals the starting hand to the player.
    /// </summary>
    void Start() {
    InitializeDeck();
    handManager.DrawHand();
    }

    /// <summary>
    /// Initializes the deck by adding all available cards to the draw pile and shuffling it.
    /// </summary>
    public void InitializeDeck() {
        drawPile.Clear();
        foreach (Card card in allAvailableCards) {
            drawPile.Add(card);
        }

        ShuffleDrawPile();
    }

    /// <summary>
    /// Shuffles the draw pile randomly.
    /// </summary>
    public void ShuffleDrawPile()
    {
        int n = drawPile.Count;
        for (int i = 0; i < n; i++)
        {
            // Pick a random index between the current index and the end of the list
            int randomIndex = Random.Range(i, n);

            // Swap the current element with the randomly chosen element
            Card temp = drawPile[i];
            drawPile[i] = drawPile[randomIndex];
            drawPile[randomIndex] = temp;
        }
    }


    /// <summary>
    /// Draws a card from the top of the draw pile.
    /// </summary>
    /// <returns>The drawn card, or null if the draw pile is empty.</returns>
    public Card DrawCard() {
        if (drawPile.Count == 0) {
            Debug.LogWarning("Draw pile is empty!");
            return null;
        }

        Card drawnCard = drawPile[0];
        drawPile.RemoveAt(0);
        return drawnCard;
    }

    /// <summary>
    /// Resets the deck by reinitializing all cards and shuffling the draw pile.
    /// </summary>
    public void ResetDeck() {
        InitializeDeck();
    }

    /// <summary>
    /// Provides a summary of the deck, including card details used for debug purposes and not for game play.
    /// </summary>
    /// <returns>A string representing the deck summary.</returns>
    public override string ToString() {
        string deckSummary = "Deck Summary:\n";
        deckSummary += $"Total Cards in Draw Pile: {drawPile.Count}\n";
        
        foreach (Card card in drawPile) {
            deckSummary += $"{card.cardName} - Cost: {card.level}, Type: {card.cardType}, Element: {card.attackType}\n";
        }

        return deckSummary;
    }
}
