using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages resetting the game state, including player and enemy cards, and restarting the game.
/// </summary>
public class ResetGameManager : MonoBehaviour {
    public DeckManager playerDeckManager; // Reference to the player's DeckManager
    public HandManager playerHandManager; // Reference to the player's HandManager
    public FieldManager playerFieldManager; // Reference to the player's FieldManager
    public EnemyManager enemyManager; // Reference to EnemyManager for managing enemy cards
    public TurnManager turnManager; // Reference to TurnManager to control turns
    public Canvas mainCanvas; // Reference to Main Game Canvas
    public Canvas victoryCanvas; // Reference to Victory Canvas
    public Canvas defeatCanvas; // Reference to Defeat Canvas
    public DynamicButtonManager buttonManager; // Reference to DynamicButtonManager for UI interactions

    /// <summary>
    /// Resets the game by moving all cards back to the decks, resetting health, and clearing selections.
    /// </summary>
    public void ResetGame() {

        DeselectAllSelections(); // Clear all previous selections
        ResetMana(); // Reset mana values

        ResetPlayerCards(); // Reset player cards to their initial states
        ResetEnemyCards(); // Reset enemy cards to their initial states

        enemyManager.enemyTurnNumber = 0;

        buttonManager.DeactivateAll(); // Deactivate UI buttons
        buttonManager.DeactivateAllAttack();

    }

    /// <summary>
    /// Resets and restarts the game after a win or defeat.
    /// </summary>
    public void PlayAgain() {
        Debug.Log("Game will start again shortly...");

        ResetGame(); // Reset all game states
        playerHandManager.DrawHand(); // Draw the starting hand for the player
        enemyManager.InitializeFieldCards(); // Initialize enemy field cards
        enemyManager.UpdateEnemyFieldZones(); // Update enemy field visuals
        playerFieldManager.InitializeFieldCards(); // Initialize player field cards
        playerFieldManager.UpdateFieldZones(); // Update player field visuals
        turnManager.StartPlayerTurn(); // Start the player's turn

        // Manage canvas visibility
        if (mainCanvas != null) {
            mainCanvas.gameObject.SetActive(true);
        }
        if (victoryCanvas != null) {
            victoryCanvas.gameObject.SetActive(false);
        }
        if (defeatCanvas != null) {
            defeatCanvas.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Resets mana for the player field.
    /// </summary>
    private void ResetMana() {
        if (playerFieldManager == null) {
            Debug.LogWarning("Player FieldManager is not assigned.");
            return;
        }
        playerFieldManager.resetMana();
    }

    /// <summary>
    /// Resets all player cards to their initial states and moves them back to the deck.
    /// </summary>
    public void ResetPlayerCards() {
        if (playerDeckManager == null || playerHandManager == null || playerFieldManager == null) {
            Debug.LogWarning("One or more player managers are not assigned.");
            return;
        }


        MoveCardsToDeck(playerHandManager.hand, playerDeckManager.drawPile, "Player Hand");
        MoveCardsToDeck(playerFieldManager.fieldCards, playerDeckManager.drawPile, "Player Field");
        EmptyDiscardPiles(playerDeckManager.discardPile, playerDeckManager.drawPile, "Player Discard Pile");

        playerDeckManager.ShuffleDrawPile(); // Shuffle the deck
    }

    /// <summary>
    /// Resets all enemy cards to their initial states and moves them back to the deck.
    /// </summary>
    public void ResetEnemyCards() {
        if (enemyManager == null) {
            Debug.LogWarning("EnemyManager is not assigned.");
            return;
        }


        MoveCardsToDeck(enemyManager.enemyFieldCards, enemyManager.enemyDeck, "Enemy Field");
        EmptyDiscardPiles(enemyManager.enemyDiscardPile, enemyManager.enemyDeck, "Enemy Discard Pile");

        enemyManager.ShuffleDeck(); // Shuffle the deck
    }

    /// <summary>
    /// Moves cards from a specified list back to the specified deck.
    /// </summary>
    /// <param name="sourceList">List containing the cards to move.</param>
    /// <param name="deck">Deck to receive the cards.</param>
    /// <param name="sourceName">Name of the source for logging.</param>
    private void MoveCardsToDeck(List<Card> sourceList, List<Card> deck, string sourceName) {
        if (sourceList == null || deck == null) return;

        foreach (Card card in sourceList) {
            if (card != null) {
                card.resetHealth(); // Reset card health
                card.resetHasAttacked(); // Reset attack status
                deck.Add(card);
            }
        }
        sourceList.Clear(); // Clear the source list
    }

    /// <summary>
    /// Empties the discard piles into the deck.
    /// </summary>
    /// <param name="sourceList">Stack containing discarded cards.</param>
    /// <param name="deck">Deck to receive the cards.</param>
    /// <param name="sourceName">Name of the source for logging.</param>
    private void EmptyDiscardPiles(Stack<Card> sourceList, List<Card> deck, string sourceName) {
        if (sourceList == null || deck == null) return;

        foreach (Card card in sourceList) {
            if (card != null) {
                card.resetHealth(); // Reset card health
                card.resetHasAttacked(); // Reset attack status
                deck.Add(card);
            }
        }
        sourceList.Clear(); // Clear the source stack
    }

    /// <summary>
    /// Deselects all previously selected cards and field zones, including hand selection.
    /// </summary>
    private void DeselectAllSelections() {
        if (playerFieldManager != null) {
            playerFieldManager.DeselectAll();
        }

        if (enemyManager != null) {
            enemyManager.Deselect();
        }

        if (playerHandManager != null) {
            playerHandManager.Deselect();
        }
    }
}
