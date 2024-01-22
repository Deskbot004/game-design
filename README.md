# RPS Extreme (Working Title)

_(RPS stands for Rock-Paper-Scissors.)_  
A game project created in the context of "Einführung in die Spielentwicklung" in the LUH. We didn't include any of the requirements for the extra points.

## Created by:
| Student | Matrikelnr. |
| ------ | ------ |
| Hoppe, Jonas | 10015015 |
| Deichsel, Marvin | 10013263 |
| Krivorutski, Julia | 10012457 |

#  Basic Description & Tutorial
The current version of the game features a card-based rock, paper, scissors variant. It is played with just the mouse.

## Main menu
### Play
After pushing the **Play**-Button, the player can chose a deck for themself and the enemy, each with unique card ratios.
Furthermore, the enemy has specific preferences of play, depending on the chosen deck (like preferring to play rock over other cards, filling slots from left to right instead of right to left, etc).
After pushing the **Start Game**-Button the game begins.

### Options
For now there are two specific options listed.
- *RGB-Mode*: Toggles ₊˚✧ Party ✧˚₊ Mode (only in main menu)
- *Fullscreen*: Changes to Fullscreen

### Credits
The contributors and asset sources are listed here.

### Quit
Closes the game.

## Gameplay
At any time, the player can press Esc to pause the game and open the menu.

### Types of Cards
There are two card types, distinguished by the form of the card window.
- *Basic Cards*: Cards with a round window and a rock, paper or scissors symbol. These cards have slots which can be filled with support cards. These cards can be played onto a slot in the middle of the table.
- *Support Cards*: These cards can not be played on their own and need to be attached to a basic card to give it special effects.

### Gameplay-Loop
1. The player and enemy draw cards.
2. The Enemy plays cards which sometimes have support cards attached (based on enemy preference and hand cards). While the enemy is playing, the player can already proceed with step 3 and 4.
3. (Optional) The player can attach support cards to basic cards. In order to do that, they need to right click on a basic card, then drag a support card into it. Support cards can only be attached, if their slot fits into the slot of the basic card. The player can remove all attached support cards by clicking **Detach**. Otherwise, the combination can be confirmed by clicking **Done**.
4. The player can fill as many slots as they likes with basic cards (with or without attached support cards), via drag and drop, and confirm the gamestate with **End Turn**.
5. Basic cards (with potential support cards) get evaluated and for each slot the loser takes damage.
6. The Cycle repeats. 

The loop ends after the player or the enemy hits a life total of 0 or below. After that the player is presented with the options to **Play Again** with the same decks or return to the **Main Menu**.

### Special Rules
If the hand of the player/enemy is empty in step 1, they draw extra cards (equal to the starting hand size). If the player/enemy played the "Draw Cards" support card, the extra cards are drawn before step 1 and this rule does not trigger.

### Known Bugs
- "Catching" cards while they are moving causes unwanted behavior that might break the game. In that case it needs to be restarted. Best not to touch cards while they're moving.
- Cards on hand are sometimes wrongly aligned
- Cards with multiple same support cards attached cause unwanted game states
- Two "Win on draw" cards clashing always grants the enemy the win instead of a draw
- Escape menu **Return** is buggy thus disabled

## Planned Extensions
- Expand the game to be a deck building tournament mode
- Rebuild gameplay to take turns "playing first"
- Add ingame Tutorial with one slot and hardcoded hands
- Extend with "Lizard, Spock" Rules
- More Arenas
- More Effects
- Effects bound to slots
