# 2048
[![forthebadge](https://forthebadge.com/images/badges/made-with-c-sharp.svg)](https://forthebadge.com)
[![forthebadge](https://forthebadge.com/images/badges/contains-tasty-spaghetti-code.svg)](https://forthebadge.com)
[![forthebadge](https://forthebadge.com/images/badges/fixed-bugs.svg)](https://forthebadge.com)

[![ko-fi](https://ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/Q5Q0M8XY)

A not quite perfect and a bit quirky 2048 ripoff that can operate alone as just a game engine.

## Table of Contents <!-- omit in toc -->

- [Compiling](#compiling)
- [UI](#ui)
  - [Reference implementation](#reference-implementation)
  - [Configurability](#configurability)
- [Setup](#setup)
  - [Engine Classes](#engine-classes)
  - [Engine events](#engine-events)
  - [Engine methods](#engine-methods)
  - [Engine variables](#engine-variables)
  - [GameControls enum](#gamecontrols-enum)

## Compiling

Compiling the project with the reference Windows Form UI

Requirements:
- [.NET SDK 5.x.x](https://dotnet.microsoft.com/download)
- [.NET Desktop Runtime 5.x.x](https://dotnet.microsoft.com/download) (_if compiled without self containment_)

Compile to run with the .NET Desktop Runtime 5.x.x (_output size: ~150KB_): **RECOMMENDED**
>`dotnet publish -r win-x64 -p:PublishSingleFile=true --self-contained false`

Compile to run without the .NET Desktop Runtime 5.x.x (_output size: ~300MB_):
>`dotnet publish -r win-x64 -p:PublishSingleFile=true --self-contained true`

## UI

### Reference implementation

The UI is a minimalistic interface with 16 panels and labels for the game cells. The Undo and New Game buttons are made up of panels and labels as well.

### Configurability

The front-end UI and the back-end game logic are completely separate and can be moved freely. To run the game you need to set up a `GameInstance` _(more info in [Setup](#setup))_. 

## Setup

The game logic is set up in an object-oriented manner with the `GameInstance` class handling everything except the user interface.

### Engine classes

- **GameInstance**

    Handles most of the work and runs the needed functions and calculations.

- **Movement**

    Handles calculations for the cells' movements and merges. Must not be called manually.

- **GameControls**

    Defines the GameControls enum that is used to identify movement direction passed to the Game instance.

### Engine events

- **UPdDisp**

    Invoked when a cell's render update is needed.

    Arguments: 
    - `int x`: The x position of the cell
    - `int y`: The y position of the cell
    - `int value`: The new value of the cell

- **UpdScore**

    Invoked when the score is changed and should be updated in the UI.

    Arguments:
    - `int score`: The new score of the player

- **UpdHS**

    Invoked when the high score is changed hs should be updated in the UI.

    Arguments:
    - `int score`: The new score of the player

- **GenErr**

    Invoked when the engine is unable to generate a new cell.

    There are no arguments.

### Engine methods

- **`GameInstance.Reset`**
    This method should be called at the beginning of each game, including the initial one.

    There are no arguments.

    There are no return values.

- **`GameInstance.Move`**
    This method should be called when a move was submitted by the player.

    Arguments:
    - `GameControls key`: The key pressed, delivered in the [GameControls](#gamecontrols-enum) format.

    There are no return values.

- **`GameInstance.Restore`**
    This method should be called when a user wants to undo a move. The game will be restored to its state before the last move. Restore can only go back once in the timeline.

    There are no arguments.

    There are no return values.

### Engine variables

- **`int Score`**: The current score of the player.
- **`int HighScore`**: Gets or sets the high score of the user.
- **`int[4,4] State`**: The current state of the playing field. Values should not be set manually.

### Basic usage

For basic usage, see `Form1.cs`'s reference implementation.

For the engine to work correctly, the UI implementation must subscribe to:
- `GameInstance.UpdDisp`
- `GameInstance.GenErr`

and must have a way to call:
- `GameInstance.Reset`
- `GameInstance.Move`

### GameControls enum

The controls passed to `GameInstance.Move` must have a `GameControls` type.

Possible states:
- Up
- Down
- Left
- Right
- Debug

_Debug makes all possible cells display in order._
