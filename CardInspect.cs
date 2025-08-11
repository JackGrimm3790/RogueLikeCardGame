using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages the card inspection functionality, including detecting double-clicks and displaying card details.
/// </summary>
public class CardInspect : MonoBehaviour {
    public Canvas inspectCanvas; // Canvas used to display card details.

    public Image cardImage; // Image component used to display the card's artwork.

    public TMP_Text nameText; // Text component to display the card's name.

    public TMP_Text description; // Text component to display the card's detailed description.

    public TurnManager turnManager; // Reference to the TurnManager for managing game flow during inspection

    public bool canInspect; // Indicates whether card inspection is currently allowed.

    private Card currentCard; // The currently selected card

    private bool isAwaitingDoubleClick = false; // Tracks whether the script is waiting for a second click

    private float doubleClickTimer = 0f; // Timer for detecting double-clicks

    private readonly float doubleClickThreshold = 0.3f; // Maximum time allowed between clicks for a double-click

    /// <summary>
    /// Ensures the inspect canvas is hidden at the start of the game.
    /// </summary>
    void Start() {
        if (inspectCanvas != null)
        {
            inspectCanvas.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Called when a card is clicked. Tracks clicks and opens the inspect canvas on a double-click.
    /// </summary>
    /// <param name="selectedCard">The card that was clicked.</param>
    public void OnCardClicked(Card selectedCard) {
        if (selectedCard == null) {
            Debug.LogWarning("No card selected.");
            return;
        }

        if (!canInspect) return;

        if (isAwaitingDoubleClick && currentCard == selectedCard) {
            OpenInspectCanvas();
            ResetDoubleClickState();
            return;
        }

        isAwaitingDoubleClick = true;
        currentCard = selectedCard;
        doubleClickTimer = doubleClickThreshold;
    }

    /// <summary>
    /// Updates the double-click timer, resetting if the time threshold is exceeded.
    /// </summary>
    void Update() {
        if (isAwaitingDoubleClick) {
            doubleClickTimer -= Time.deltaTime;

            if (doubleClickTimer <= 0f) {
                ResetDoubleClickState();
            }
        }
    }

    /// <summary>
    /// Resets the double-click tracking variables.
    /// </summary>
    private void ResetDoubleClickState() {
        isAwaitingDoubleClick = false;
        doubleClickTimer = 0f;
        currentCard = null;
    }

    /// <summary>
    /// Opens the inspect canvas and displays the details of the currently selected card.
    /// </summary>
    private void OpenInspectCanvas() {
        if (inspectCanvas == null || currentCard == null) {
            Debug.LogWarning("Inspect canvas or current card data is not set.");
            return;
        }

        turnManager.isPlayerTurn = false;
        nameText.SetText(currentCard.cardName);
        nameText.fontSize = currentCard.cardName.Length >= 18 ? 6 : 8;
        inspectCanvas.gameObject.SetActive(true);
        generateCardText(currentCard);
        cardImage.sprite = currentCard.artwork;
    }

    /// <summary>
    /// Closes the inspect canvas and resumes the player's turn.
    /// </summary>
    public void CloseInspectCanvas() {
        if (inspectCanvas != null) {
            inspectCanvas.gameObject.SetActive(false);
            turnManager.isPlayerTurn = true;
        }
    }

    /// <summary>
    /// Generates and sets the card description text.
    /// </summary>
    /// <param name="card">The card for which the description is generated.</param>
    public void generateCardText(Card card) {
        string cardText = $"Level: {card.level}\nType: {card.cardType}\nHealth: {card.health}/{card.defaultHealth}\n" +
                          $"Attack Type: {card.attackType}\nAttack Damage: {card.attackMin}-{card.attackMax}";
        description.SetText(cardText);
    }
}
