﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPGames
{
	class BattleShipsMenu : GameMenu
	{

		private BattleShips game;
		private MenuOptions currentMenu;
		private Ship[] ships;
		private Ship currentShip;
		private int currentShipIndex;
		private bool allShipsPlaced;
		private bool switchedPlayer;


		// Constructor
		public BattleShipsMenu()
		{
			currentMenu = MenuOptions.ChooseGameMode;
			switchedPlayer = false;

			ships = new Ship[2];
			ships[0] = new Ship(5, "Aircraft Carrier");
			ships[1] = new Ship(4, "BattleShip");
			//ships[2] = new Ship(4, "BattleShip");
			//ships[3] = new Ship(3, "Destroyer");
			//ships[4] = new Ship(3, "Destroyer");
			//ships[5] = new Ship(3, "Submarine");
			//ships[6] = new Ship(2, "Patrol Boat");
			//ships[7] = new Ship(2, "Patrol Boat");
			//ships[8] = new Ship(2, "Patrol Boat");

			currentShipIndex = 0;
			currentShip = ships[0];
		}

		/// <summary>
		/// Displays the current menu, using the MenuOption enum.
		/// </summary>
		public void DisplayCurrentMenu()
		{
			switch(currentMenu)
			{
				case MenuOptions.ChooseGameMode:
					ShowGameModes();
					break;
				case MenuOptions.HandleGameActions:
					ShowGameActions();
					break;
			}
		}

		/// <summary>
		/// Uses a string input that contains the action based on the shown menu.
		/// <para> Returns true if the action was to quit to the main menu. </para>
		/// </summary>
		/// <param name="input">A string that contains the action based on the current menu</param>
		/// <returns>Returns true if the action was to quit to the main menu</returns>
		public bool HandleInput(string input)
		{
			bool quitToMainMenu = false;
			bool quitToChooseGameMode = false;

			switch(currentMenu)
			{
				case MenuOptions.ChooseGameMode:
					quitToMainMenu = ChooseGameMode(input);
					break;
				case MenuOptions.HandleGameActions:
					quitToChooseGameMode = HandleGameAction(input);
					break;
			}

			if(quitToChooseGameMode)
			{
				currentMenu = MenuOptions.ChooseGameMode;
			}

			return quitToMainMenu;
		}

		//------------- Show actions menu's ---------------

		private void ShowGameModes()
		{
			Console.WriteLine("Choose game mode:");
			Console.WriteLine("1. Standard Mode");
			Console.WriteLine("2. Variation Mode");
			Console.WriteLine("0. Quit to Main Menu");
		}

		private void ShowGameActions()
		{

			ShowGameBoard();
			Console.WriteLine("Player " + game.CurrentPlayer + ", it is your turn.");
			if(allShipsPlaced != true)
			{
				Console.WriteLine("Place: " + currentShip.Name + " with length: " + currentShip.Size +
					" at [X,Y,(v Or h)]" + "  (v) Vertical. (h) Horizontal.");
			}
			else
			{
				Console.WriteLine("Place bomb at coordinate: [X,Y]");
			}

		}

		//------------- Handle Input menu's ---------------

		private bool ChooseGameMode(string input)
		{
			// Hvis vores menu er "vælg gamemode", så check følgende input:
			if(input == "1")
			{
				CreateGame(GameMode.Standard);
			}
			else if(input == "2")
			{
				CreateGame(GameMode.Variation);
			}
			else if(input == "0")
			{
				return true;
			}

			return false;
		}


		private bool HandleGameAction(string input)
		{
			// Hvis vores input er 0, så skal vi returnere til Hoved menuen.
			if(input == "0")
			{
				return true;
			}

			string[] shipCoordinates = input.Split(',');

			//TODO: Refactor nested if-statements.
			if(allShipsPlaced != true)
			{
				if(shipCoordinates.Length == 3)
				{
					if(IsCoordinateValid(shipCoordinates[0], shipCoordinates[1]))
					{
						int x = Convert.ToInt32(shipCoordinates[0]) - 1; // minus 1 fordi at gameboardet er fra 0 til 9.
						int y = Convert.ToInt32(shipCoordinates[1]) - 1;
						char axis = Convert.ToChar(shipCoordinates[2]);

						if(game.PlaceShip(x, y, axis, currentShip)) // If we could place a ship, then update current ship, to the next ship in list.
						{
							UpdateCurrentShip();
						}
					}
				}
			}
			else // If we placed all ships, then we can place bombs.
			{
				//TODO: Place bombs
				if(shipCoordinates.Length == 2)
				{
					if(IsCoordinateValid(shipCoordinates[0], shipCoordinates[1]))
					{
						int x = Convert.ToInt32(shipCoordinates[0]) - 1; // minus 1 fordi at gameboardet er fra 0 til 9.
						int y = Convert.ToInt32(shipCoordinates[1]) - 1;

						if(game.PlaceBomb(x, y)) // If we could place a ship, then update current ship, to the next ship in list.
						{
							
							if(game.CheckWinCondition())
							{
								// TODO: print out win
							}

							SwitchPlayer();
						}
					}
				}
			}

			return false;
		}


		// ---------- Uncategorized methods --------------

		// TODO: Update Current ship and switch player if he placed all ships.
		private void UpdateCurrentShip()
		{
			currentShipIndex++;
			if(currentShipIndex < ships.Length) // Place all ships until no more.
			{
				currentShip = ships[currentShipIndex];
			}
			else 
			{
				// If both players has placed all ships, then set allShipsPlaced to true.
				if(switchedPlayer)
				{
					allShipsPlaced = true;
				}

				// switch player and set index to 0.				
				SwitchPlayer();
				switchedPlayer = true;
				currentShipIndex = 0;
			}
		}

		private void CreateGame(GameMode mode)
		{
			game = new BattleShips(mode);
			currentMenu = MenuOptions.HandleGameActions;
		}

		private void ShowWinner()
		{
			Console.Clear();
			ShowGameBoard();
			Console.WriteLine("Winner winner chicken dinner! Player " + game.CurrentPlayer + " won the game!");
			PressKeyToContinue();
		}

		private void PressKeyToContinue()
		{
			Console.WriteLine("Press any key to continue");
			Console.ReadKey();
		}

		private void ShowGameBoard()
		{
			Console.WriteLine(game.GetGameBoardView());
		}

		private bool IsCoordinateValid(string x, string y)
		{
			bool isXValid = false;
			bool isYValid = false;

			for(int i = 1; i <= game.GridSize; i++)
			{
				if(isXValid != true && x == i.ToString()) // if x is not valid yet, then check if it is. If x is already valid, then don't check.
				{
					isXValid = true;
				}

				if(isYValid != true && y == i.ToString())
				{
					isYValid = true;
				}
			}

			return isXValid && isYValid; // Both x & y should be true
		}

		private void SwitchPlayer()
		{
			Console.Clear();
			Console.WriteLine("\nPlayer " + game.GetNextPlayer() + "! Your turn! Look away Player " + game.CurrentPlayer);
			PressKeyToContinue();
			game.ChangePlayer();
		}
	}
}
