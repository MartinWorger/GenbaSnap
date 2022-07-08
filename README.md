# GenbaSnap
Snap card game to Genba Digital requirements

# Specification

## Snap Game

> We would like you to spend some time creating a simple C# application which plays the card game “Snap” (https://en.wikipedia.org/wiki/Snap_(card_game)). The time you take to do this is entirely up to you and depends on how far you want to take your implementation…
> Since we are particularly interested in the technical approach and creative processes undertaken to implement the above, the only firm requirements are:
> -	The code compiles and runs in Visual Studio 2019 (or above)
> -	The correctness of the application can be verified by running (passing!) unit tests
> -	The game elements and interactions are implemented in an object-orientated manner (any demonstrable use of the SOLID principles would be a plus!)
> -	The solution can be cloned by us from a public Git repository

# Solution

In its initial state the rules are as follows:
- It is a game for 2 or more players
- The game is played with a standard deck of 52 cards (4 suits, Ace - King)
- The cards are shuffled and dealt to the players starting with player number 1
- Player number 1 takes the first turn
- Each player has a stack of cards facing up (i.e. the cards in play) and a stack facing down (the "hand")
- If a player runs out of cards in their hand they can no longer play more cards
- A snap may be called at any time, however it will only be successful if at least 2 play stacks have a topmost card of the same value
- On a successful snap call, all play stack cards within the snap match will be moved to the players hand
- A player may continue to call snap while they have _any_ cards - either in play or in their hand
- When a player has no more cards, they are out of the game
- The winner is the last person to have an unplayed card in their hand

## Random Factor

Within the console application simulator there may be random calls made:
- Players may attempt to play a card out of turn, even if they are out of the game
- Players may call snap at random, even if they are out of the game
- In a snap situation there is a small chance that no players will call snap

## Game Play

The solution has been designed without user interface.
A console application is provided to simulate game play, 
and feedback of progress provided via logging to the console


## Dependencies

The solution has been developed in Visual Studio 2022 Community Edition

It makes use of some Microsoft extension packages
- Microsoft.Extensions.Caching.Memory
- Microsoft.Extensions.DependencyInjection
- Microsoft.Extensions.Logging

Unit testing dependencies
- Microsoft.NET.Test.Sdk
- MSTest.TestAdapter
- MSTest.TestFramework
- Moq


---

Martin Worger 08/Jul/2022