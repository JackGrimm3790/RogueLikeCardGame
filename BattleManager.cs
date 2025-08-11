using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages battle interactions between player and enemy, including attacking, defending,
/// and determining victory or defeat conditions.
/// </summary>
public class BattleManager : MonoBehaviour {

    /// <summary>
    /// mainCanvas, VictoryCanvas, and defeatCanvas are used to open a canvas on defeat/victory 
    /// and give the player options for going to the main menu or playing another game
    /// </summary>

    public Canvas mainCanvas;

    public Canvas victoryCanvas;

    public Canvas defeatCanvas;


    public string menuSceneName = "MainMenu"; // menuSceneName is a string that stores the name of the menu scene and is used to return to the main menu

    public float delayBeforeCanvas = 1.0f; // delayBeforeCanvas is a length of a pause implemented before opening either the victory or defeat canvas

    /// <summary>
    /// fieldManager, EnemyManager, and buttonManager are all references to other scripts used in the 
    /// game so there methods and variables can be used
    /// the fieldManager is used to manage player cards on the field
    /// enemyManager is used to controll basic enemy functionality and to manage it's cards on the field
    /// buttonManager is used to controll what buttons the player has access to
    /// </summary>

    public FieldManager fieldManager;

    public EnemyManager enemyManager;

    public DynamicButtonManager buttonManager;

    /// <summary>
    /// start methods are called as soon as the game starts
    /// this one manages the canvases and ensures the victory and defeat canvases are not visible
    /// </summary>
    private void Start() {
        if (mainCanvas != null) {
            mainCanvas.gameObject.SetActive(true);
        } else {
            Debug.LogWarning("Main canvas is not assigned.");
        }

        if (victoryCanvas != null) {
            victoryCanvas.gameObject.SetActive(false);
        } else {
            Debug.LogWarning("Victory canvas is not assigned.");
        }

        if (defeatCanvas != null) {
            defeatCanvas.gameObject.SetActive(false);
        } else {
            Debug.LogWarning("Defeat canvas is not assigned.");
        }
    }

    /// <summary>
    /// Executes an attack from the player to an enemy.
    /// </summary>
    /// <param name="index">The index of the enemy card being targeted.</param>
    public void Attack(int index) {
        if (fieldManager == null) {
            Debug.LogWarning("FieldManager is not assigned.");
            return;
        }

        Card playerCard = enemyManager.selectedCardForAttack;
        if (playerCard == null) {
            Debug.LogWarning("No player card selected for attack.");
            return;
        }

        if(playerCard.summonSickness) {
            Debug.LogWarning("Cards cannot attack the turn they were played onto the field");
            return;
        }

        if (playerCard.hasAttacked) {
            Debug.LogWarning($"Player card {playerCard.cardName} has already attacked this turn.");
            return;
        }

        if (enemyManager == null || enemyManager.enemyFieldCards == null) {
            Debug.LogWarning("EnemyManager or enemyFieldCards not set.");
            return;
        }

        bool enemyHasCardsLeft = false;
        foreach (var card in enemyManager.enemyFieldCards) {
            if (card != null) {
                enemyHasCardsLeft = true;
                break;
            }
        }

        enemyManager.RemoveAttackSelection();

        if (!enemyHasCardsLeft) {
            Victory();
            return;
        }

        Card enemyCard = enemyManager.GetCard(index);
        if (enemyCard == null) {
            Debug.LogWarning("No enemy card selected as target.");
            return;
        }

        int damage = Random.Range(playerCard.attackMin, playerCard.attackMax);
        enemyCard.takeDamage(damage);

        playerCard.hasAttacked = true;

        if (enemyCard.health <= 0 && index != -1) {
            enemyManager.RemoveEnemyCard(index);
        }

        fieldManager.DeselectAll();
        buttonManager.DeactivateAllAttack();
        enemyManager.Deselect();
    }

    /// <summary>
    /// Handles the enemy attacking a player card or directly if none exist.
    /// </summary>
    /// <param name="enemyCard">The card the enemy is using to attack.</param>
    /// <param name="playerCardIndex">The index of the player card being targeted, or -1 for a direct attack.</param>
    public void Defend(Card enemyCard, int playerCardIndex) {
        if (enemyCard == null) {
            Debug.LogWarning("No enemy card provided for defense.");
            return;
        }

        if (fieldManager == null) {
            Debug.LogWarning("FieldManager is not assigned.");
            return;
        }

        if (playerCardIndex == -1) {
            bool playerHasCardsLeft = false;

            foreach (var card in fieldManager.fieldCards) {
                if (card != null) {
                    playerHasCardsLeft = true;
                    break;
                }
            }

            if (!playerHasCardsLeft) {
                Defeat();
                return;
            }
        }
        else {
            Card playerCard = fieldManager.GetPlayerCardAtIndex(playerCardIndex);

            if (playerCard == null) {
                Debug.LogWarning($"Player card not found at index: {playerCardIndex}");
                return;
            }

            int damage = Random.Range(enemyCard.attackMin, enemyCard.attackMax + 1);
            playerCard.takeDamage(damage);

            if (playerCard.health <= 0) {
                fieldManager.RemovePlayerCard(playerCardIndex);
            }
            Debug.Log(playerCard.cardName + " at index of: " + playerCardIndex + " took dammage: " + damage);
        }
    }

    /// <summary>
    /// Displays the victory screen.
    /// </summary>
    public void Victory() {
        StartCoroutine(VictoryCoroutine());
    }

    private IEnumerator VictoryCoroutine() {
        yield return new WaitForSeconds(delayBeforeCanvas);

        if (victoryCanvas != null && mainCanvas != null) {
            victoryCanvas.gameObject.SetActive(true);
        } else {
            Debug.LogWarning("Either the victory or the main canvas is not assigned.");
        }
    }

    /// <summary>
    /// Displays the defeat screen.
    /// </summary>
    public void Defeat() {
        StartCoroutine(DefeatCoroutine());
    }

    private IEnumerator DefeatCoroutine() {
        yield return new WaitForSeconds(delayBeforeCanvas);

        if (defeatCanvas != null) {
            defeatCanvas.gameObject.SetActive(true);
        } else {
            Debug.LogWarning("Defeat canvas is not assigned.");
        }
    }

    /// <summary>
    /// Returns to the main menu after a delay.
    /// </summary>
    public void ReturnToMenuAfterDelay() {
        StartCoroutine(ReturnToMenuCoroutine());
    }

    private IEnumerator ReturnToMenuCoroutine() {
        yield return new WaitForSeconds(delayBeforeCanvas);

        if (!string.IsNullOrEmpty(menuSceneName)) {
            SceneManager.LoadScene(menuSceneName);
        } else {
            Debug.LogWarning("Menu scene name is not set.");
        }
    }
}
