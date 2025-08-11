using UnityEngine;

/// <summary>
/// Represents a card with various properties such as name, description, type, and stats.
/// </summary>
[CreateAssetMenu(fileName = "NewCard", menuName = "Card")]
public class Card : ScriptableObject {
    /// <summary>
    /// Basic card details including name, dexription level etc.
    /// </summary>
    public string cardName;

    public string description;

    public int level;

    public Sprite artwork;

    public CardType cardType;

    public AttackType attackType;

    public int attackMin;

    public int attackMax;

    public int health;

    public int defaultHealth; // The default health of the card used to reset the cards on starting a new game.

    public bool hasAttacked; // Indicates whether the card has already attacked during the current turn

    public int manaRequired; // The amount of mana required to play the card.

    public int manaOnDiscard; // The amount of mana gained when the card is discarded.

    public bool summonSickness; // Variable to stop cards from attack on the turn they were summoned

    /// <summary>
    /// Resets the card's health to its default value.
    /// </summary>
    public void resetHealth() {
        health = defaultHealth;
    }

    /// <summary>
    /// Resets the card's attack status, allowing it to attack again used on turn end and on starting a new game.
    /// </summary>
    public void resetHasAttacked() {
        hasAttacked = false;
    }

    public void setSummonSickness (bool what) {
        summonSickness = what;
    }

    /// <summary>
    /// Reduces the card's health by the specified amount.
    /// </summary>
    /// <param name="ammount">The amount of damage to inflict.</param>
    public void takeDamage(int ammount) {
        health -= ammount;
    }

    /// <summary>
    /// Heals the card by the specified amount, up to its default health not currently used
    /// as card effects have yet to be added.
    /// </summary>
    /// <param name="ammount">The amount of health to restore.</param>
    public void heal(int ammount) {
        health += ammount;
        if (health > defaultHealth) health = defaultHealth;
    }
}

/// <summary>
/// Represents the various types a card can belong to.
/// </summary>
public enum CardType {
    Fire,
    Earth,
    Water,
    Dark,
    Light,
    Air,
    Human,
    Undead,
    Spirit
}

/// <summary>
/// Represents the different types of attacks a card can perform.
/// </summary>
public enum AttackType {
    Slashing,
    Bludgeoning,
    Burning,
    Magic,
    Ice,
    Poison,
    Corruption,
    Acid,
    Piercing,
    Light,
    Lightning
}
