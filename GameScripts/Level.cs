using UnityEngine;
using System.Collections;

/// <summary>
/// Manages the enemy's actions during their turn in the game.
/// Handles card plays, attacks, and turn progression with delays.
/// </summary>
public class Level : MonoBehaviour {
    public EnemyManager enemyManager; // Reference to EnemyManager for managing the enemy's deck and field
    public BattleManager battleManager; // Reference to BattleManager for executing attacks
    public FieldManager fieldManager; // Reference to FieldManager for player cards
    public TurnManager turnManager; // Reference to TurnManager to start and end turns

    public float actionDelay = 1.5f; // Delay in seconds between each enemy action

    /// <summary>
    /// Executes the enemy's turn by determining actions based on the turn number.
    /// </summary>
    public void EnemyTurn() {
        if (enemyManager == null || battleManager == null || fieldManager == null) {
            Debug.LogWarning("One or more required components are missing in Level.");
            return;
        }

        int turnNumber = enemyManager.GetEnemyTurnNumber(); // Retrieve current turn number from EnemyManager
        Debug.Log($"Executing Enemy Turn {turnNumber}");

        StartCoroutine(ExecuteEnemyTurnWithPauses(turnNumber)); // Start the coroutine to handle the turn
    }

    /// <summary>
    /// Coroutine to execute the enemy's turn with pauses between actions.
    /// </summary>
    /// <param name="turnNumber">The current turn number.</param>
    private IEnumerator ExecuteEnemyTurnWithPauses(int turnNumber) {
        switch (turnNumber) {
            case 1:
                yield return StartCoroutine(FirstTurnActions());
                break;
            case 2:
                yield return StartCoroutine(SecondTurnActions());
                break;
            case 3:
                yield return StartCoroutine(ThirdTurnActions());
                break;
            default:
                yield return StartCoroutine(DefaultTurnActions());
                break;
        }

        turnManager.EndEnemyTurn(); // End the enemy turn and start the player's turn
    }

    /// <summary>
    /// Coroutine for the enemy's first turn actions.
    /// Plays multiple cards onto the field.
    /// </summary>
    private IEnumerator FirstTurnActions() {
        Play();
        yield return new WaitForSeconds(actionDelay);

        Play();
        yield return new WaitForSeconds(actionDelay);

        Play();
        yield return new WaitForSeconds(actionDelay);
    }

    /// <summary>
    /// Coroutine for the enemy's second turn actions.
    /// Plays a card and performs an attack.
    /// </summary>
    private IEnumerator SecondTurnActions() {
        Play();
        yield return new WaitForSeconds(actionDelay);

        Attack();
        yield return new WaitForSeconds(actionDelay);
    }

    /// <summary>
    /// Coroutine for the enemy's third turn actions.
    /// Performs two consecutive attacks.
    /// </summary>
    private IEnumerator ThirdTurnActions() {
        Attack();
        yield return new WaitForSeconds(actionDelay);

        Attack();
        yield return new WaitForSeconds(actionDelay);
    }

    /// <summary>
    /// Coroutine for default enemy turn actions.
    /// Plays a card and performs an attack.
    /// </summary>
    private IEnumerator DefaultTurnActions() {
        Play();
        yield return new WaitForSeconds(actionDelay);

        Attack();
        yield return new WaitForSeconds(actionDelay);
    }

    /// <summary>
    /// Plays a card from the enemy's deck onto the field if there is an available slot.
    /// </summary>
    public void Play() {
        if (enemyManager.enemyDeck == null || enemyManager.enemyDeck.Count == 0) {
            Debug.LogWarning("Enemy deck is empty, no card played.");
            return;
        }

        Card cardToPlay = enemyManager.enemyDeck[0];
        cardToPlay.resetHealth();
        enemyManager.enemyDeck.RemoveAt(0);

        int availableZoneIndex = GetFirstAvailableEnemyFieldZone();
        if (availableZoneIndex == -1) {
            Debug.LogWarning("No available field zone for enemy to play a card.");
            return;
        }

        enemyManager.enemyFieldCards[availableZoneIndex] = cardToPlay;
        enemyManager.enemyFieldZones[availableZoneIndex].sprite = cardToPlay.artwork;
        enemyManager.enemyFieldZones[availableZoneIndex].enabled = true;

        Debug.Log($"Enemy plays card {cardToPlay.cardName} in zone {availableZoneIndex}");
    }

    /// <summary>
    /// Attacks the first available player card or performs a direct attack.
    /// </summary>
    public void Attack() {
        Card enemyCard = SelectEnemyCardToAttack();
        int playerCardIndex = SelectPlayerTarget();

        if (enemyCard != null) {
            Debug.Log($"Enemy card {enemyCard.cardName} attacks player card in zone {playerCardIndex}");
            battleManager.Defend(enemyCard, playerCardIndex);
        } else Debug.LogWarning("No available enemy card to perform attack.");
    }

    /// <summary>
    /// Gets the first available field zone for the enemy to place a card.
    /// </summary>
    /// <returns>The index of the first available field zone or -1 if none are available.</returns>
    private int GetFirstAvailableEnemyFieldZone() {
        if (enemyManager.enemyFieldCards == null) {
            Debug.LogWarning("enemyFieldCards list is null.");
            return -1;
        }

        for (int i = 0; i < enemyManager.enemyFieldCards.Count; i++) {
            if (enemyManager.enemyFieldCards[i] == null) {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// Selects an enemy card for attacking.
    /// </summary>
    /// <returns>The selected enemy card or null if no cards are available.</returns>
    private Card SelectEnemyCardToAttack() {
        if (enemyManager.enemyFieldCards == null) {
            Debug.LogWarning("enemyFieldCards list is null.");
            return null;
        }

        foreach (var card in enemyManager.enemyFieldCards) {
            if (card != null) {
                return card;
            }
        }
        return null;
    }

    /// <summary>
    /// Selects the first available player card on the field as a target.
    /// </summary>
    /// <returns>The index of the target player card or -1 if none are available.</returns>
    private int SelectPlayerTarget() {
        if (fieldManager.fieldCards == null) {
            Debug.LogWarning("fieldCards list is null.");
            return -1;
        }

        for (int i = 0; i < fieldManager.fieldCards.Count; i++) {
            if (fieldManager.fieldCards[i] != null) {
                return i;
            }
        }
        return -1;
    }
}
