# Roguelike Deckbuilding Game

## Game Overview

This is a roguelike deckbuilding game where the player uses cards to defeat enemies by playing them onto the field, attacking, and managing resources like mana. The game includes mechanics for drawing cards, managing decks, and executing enemy AI turns.

### Key Features
- **Deck Management**: Draw cards from a deck and manage a discard pile.
- **Mana System**: Play cards using mana, gain mana each turn, and discard cards for mana.
- **Card Combat**: Cards on the field can attack enemy cards or directly attack the enemy if no cards are on their field.
- **Turn-Based Gameplay**: Players and enemies alternate turns, with each side executing specific actions.

---

## How the Game Works

1. **Start of Player Turn**:
   - The player draws a card to their hand.
   - The player gains mana.
   - Any necessary resets (like "hasAttacked" flags) are applied to player cards.

2. **Player Actions**:
   - The player can play cards from their hand onto the field, attack enemy cards, or discard cards for mana.

3. **End of Player Turn**:
   - Once the player ends their turn, control passes to the enemy.

4. **Enemy Turn**:
   - The enemy draws cards and plays them onto their field.
   - Enemy cards attack player cards or attack directly if no player cards are on the field.

5. **Win/Lose Conditions**:
   - The player wins if all enemy cards are defeated and the player initiates another attack.
   - The player loses if their field is wiped, and they cannot defend against an incoming attack.

---

## Script Overview

### **Core Gameplay Scripts**
1. **Card**  
   Represents individual cards in the game, storing information like name, attack, health, and mana cost. Includes methods for resetting health, managing attack state, and handling damage.

2. **DeckManager**  
   Handles the player's deck, including shuffling, drawing cards, and managing the discard pile.

3. **HandManager**  
   Manages the player's hand, updating card slots and enabling/disabling UI elements based on the current state of the game.

4. **FieldManager**  
   Manages the player's field, allowing cards to be played and updated. Tracks selected zones for playing cards or attacking.

5. **EnemyManager**  
   Manages the enemy’s field, deck, and actions. Includes methods for shuffling the enemy deck, playing cards, and executing attacks.

6. **BattleManager**  
   Manages the combat mechanics between player and enemy cards, resolving attacks and damage calculations.

7. **TurnManager**  
   Controls the flow of turns between the player and the enemy, ensuring the correct sequence of actions for both sides.

8. **Level**  
   Contains the logic for enemy AI behavior. Executes enemy actions based on the current turn number.

### **UI and Interaction Scripts**
9. **CardInspect**  
   Allows players to inspect a card by double-clicking it, opening a detailed view of its stats and artwork.

10. **DynamicButtonManager**  
    Dynamically manages buttons for playing, discarding, and attacking based on the current game state.

11. **ResetGameManager**  
    Handles game resets, moving all cards back to decks and restarting the game.



---

- The game uses a modular system, with each script responsible for a specific aspect of the gameplay.
- Scripts communicate with each other through references, ensuring seamless integration of mechanics like deck management, combat, and turn-based actions.
