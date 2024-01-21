# R P S Extreme

A game project created in the context of "Einführung in die Spielentwicklung" in the LUH.

## Created by:
| Student | Matrikelnr. |
| ------ | ------ |
| Hoppe, Jonas | 10015015 |
| Deichsel, Marvin | 10013263 |
| Krivorutski, Julia | 10012457 |

#  Basic Description & Tutorial
The current version of the game features a card based rock, paper, scissors variant.

## Main menu
### Play
After pushing the **Play**-Button the user can chose a deck for himself and the enemy, each with unique card ratios.
Furthermore the enemy has specific preferences of play depending on the chosen deck. E.g. Tries to play rock with the highest probability, likes to fill slots from left to right instead of right to left.
After pushing the **Start Game**-Button the game begins.

### Options
For now there are two specific options listed.
- *RGB-Mode*: Changes the RGB-Color of the background in the main menu gradually.
- *Fullscreen*: Changes to Fullscreen (WIP)

### Credits
Here are the contributors and sources of assets listed.

### Quit
Closes the game.

## Gameplay
### Types of Cards
There are two card types. Distinguished by Form.
- *Basic Cards*: Cards with a round window and a symbol corresponding to the rock, paper, scissors game. These cards have slots which can be filled with support cards. These cards can be played onto a slot in the middle of the table.
- *Support Cards*: These cards can not be played on their own and need to be attached to a basic card. These cards have special effects which affect the resolution of play.

### Gameplay-Loop
After the cards are dealt the Game-loop begins:
1. Cards for the turn are drawn.
2. The Enemy plays cards with (Based on enemy preference and hand cards) support cards attached.
3. (Optional) The User can attach support cards by right clicking a basic card and then dragging a fitting support card (indicated by the slot to be filled) onto the card. If the user wants to remove the attached support card **Detach** should be clicked. Else the combination is to be confirmed by **Done**.
4. The User can fill as many slots as he likes with basic cards, via drag and drop, and confirms the gamestate with **End Turn**.
5. Basic cards with their support cards get evaluated and for each loss on each slot the loser takes damage.
6. The Cycle repeats.

The loop ends after a player hits a lifetotal of 0 or below and the Option to **Restart** with the same decks or **Return** to the main menu is presented.

### Special Rules

## Planned Extensions
- Rebuild the game to be a deck building tournament mode
- Rebuild gameplay to take turns "playing first"
- Add ingame Tutorial with one slot and hardcoded hands
- Extend with "Lizard, Spock" Rules
- More Arenas
- More Effects
- Effects bound on slots
