using UnityEngine;

/// <summary>
/// Manages the game's turn system, including switching between player and enemy turns.
/// </summary>
public class TurnManager : MonoBehaviour {
    public FieldManager fieldManager; // Reference to FieldManager for player actions
    public EnemyManager enemyManager; // Reference to EnemyManager for enemy actions
    public HandManager handManager; // Reference to HandManager for managing the player's hand
    public bool isPlayerTurn = true; // Tracks whether it's currently the player's turn
    public bool isFirstTurn = true; // Tracks if it's the first turn of the game
    public DynamicButtonManager buttonManager; // Reference to DynamicButtonManager for UI interactions

    /// <summary>
    /// Starts the game with the player's turn.
    /// </summary>
    void Start() {
    StartPlayerTurn();
    }

    /// <summary>
    /// Checks if it's currently the player's turn.
    /// </summary>
    /// <returns>True if it's the player's turn, otherwise false.</returns>
    public bool IsPlayerTurn() {
        return isPlayerTurn;
    }

    /// <summary>
    /// Starts the player's turn and performs necessary initializations.
    /// </summary>
    public void StartPlayerTurn() {
        isPlayerTurn = true;
        Debug.Log("Player's Turn Started");

        if (!isItFirstTurn()) fieldManager.addMana(2); // Add mana unless it's the first turn

        if (fieldManager != null) {
            fieldManager.ResetFieldHasAttacked(); // Reset the hasAttacked flag for player cards
            if(!isItFirstTurn()) fieldManager.NoSummonSickness();
            Debug.Log("Reset hasAttacked for all cards on the player's field.");
        } else Debug.LogWarning("FieldManager is not assigned.");

        // Draw a card at the start of the player's turn
        fieldManager.handManager.DrawCardToHand();
    }

    /// <summary>
    /// Ends the player's turn and transitions to the enemy's turn.
    /// </summary>
    private void EndPlayerTurn() {
        fieldManager.DeselectAll();
        handManager.Deselect();
        enemyManager.Deselect();
        Debug.Log("Player's Turn Ended");
        StartEnemyTurn();
    }

    /// <summary>
    /// Starts the enemy's turn.
    /// </summary>
    private void StartEnemyTurn() {
        isPlayerTurn = false;
        Debug.Log("Enemy's Turn Started");

        // Execute the enemy's turn actions
        enemyManager.ExecuteEnemyTurn();
    }

    /// <summary>
    /// Ends the enemy's turn and transitions back to the player's turn.
    /// </summary>
    public void EndEnemyTurn() {
        Debug.Log("Enemy's Turn Ended");
        StartPlayerTurn();
    }

    /// <summary>
    /// Manually ends the player's turn. This can be triggered by a UI button.
    /// </summary>
    public void EndPlayerTurnButton() {
        Debug.LogWarning("Ending turn...");
        if (isPlayerTurn) {
            isFirstTurn = false;
            isPlayerTurn = false;
            fieldManager.RemovePlaySelection();
            enemyManager.RemoveAttackSelection();
            buttonManager.DeactivateAll();
            buttonManager.DeactivateAllAttack();
            EndPlayerTurn();
        } else Debug.LogWarning("It is not your turn");
    }

    /// <summary>
    /// Checks if it is currently the first turn of the game.
    /// </summary>
    /// <returns>True if it is the first turn, otherwise false.</returns>
    public bool isItFirstTurn()
    {
        return isFirstTurn;
    }
}
