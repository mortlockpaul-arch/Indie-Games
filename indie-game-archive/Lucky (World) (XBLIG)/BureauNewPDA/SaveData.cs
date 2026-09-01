using System.Collections.Generic;
using System.Linq;
using BureauNewPDA.Data;
using Microsoft.Xna.Framework;

namespace BureauNewPDA;

public class SaveData
{
	public class mapSaveData
	{
		public string mapPieceName = "";

		public Vector2 currentPosition;

		public Vector2 origin;

		public bool isCorrect;

		public bool isOver;

		public bool isSelected;
	}

	public bool gameInProgress;

	public List<string> activeVariables = new List<string>();

	public List<mapSaveData> currentMapSaveData = new List<mapSaveData>();

	public List<string> inventorySaveList = new List<string>();

	public string newMusic = "";

	public string currentScene = "";

	public int currentSceneId = -1;

	private bool found;

	public int currentError;

	public string musicPlayingOnSave = "";

	public bool pendingDataSave;

	private int hours;

	private double minutes;

	private int day;

	private string temp = "";

	private int h;

	private int m;

	public void addVariables(string v)
	{
		v = specialProcess(v);
		if (v != "")
		{
			found = false;
			foreach (string activeVariable in activeVariables)
			{
				if (activeVariable == v)
				{
					found = true;
					break;
				}
			}
			if (!found)
			{
				activeVariables.Add(v);
			}
		}
		if (activeVariables.Count() == 1)
		{
			activeVariables.Add("MapPieces1");
		}
		else if (activeVariables.Count() == 3)
		{
			activeVariables.Remove("MapPieces1");
			activeVariables.Add("MapPieces2");
		}
		else if (activeVariables.Count() == 4)
		{
			activeVariables.Remove("MapPieces2");
			activeVariables.Add("MapPieces3Plus");
		}
	}

	public bool checkForVariable(string v)
	{
		foreach (string activeVariable in activeVariables)
		{
			if (activeVariable == v)
			{
				return true;
			}
		}
		return false;
	}

	public void clearVars()
	{
		activeVariables.Clear();
	}

	private string specialProcess(string v)
	{
		switch (v)
		{
		case "ResetGame":
			return "";
		case "StartGame":
			gameInProgress = true;
			return "";
		case "MusicElement":
			newMusic = "Element";
			return "";
		case "MusicDrone":
			newMusic = "Drone01";
			return "";
		case "MusicForest":
			newMusic = "AfternoonAmbienceSimple_03";
			return "";
		case "MusicDesert":
			newMusic = "weather-wind-ghost-town";
			return "";
		case "MusicDemon":
			newMusic = "Demons";
			return "";
		case "MusicDungeon":
			newMusic = "horror-dungeon-ambience-1";
			return "";
		case "MusicAbout":
			newMusic = "About";
			return "";
		case "AddPiece4":
			addLocationData("Piece4");
			return "";
		case "MusicStop":
			newMusic = "Blank";
			return "";
		case "MusicTheme":
			newMusic = "Bureau2_Theme";
			return "";
		case "MusicColonies":
			newMusic = "Colonies Theme";
			return "";
		case "WrongTurnUndo":
			removeVariable("WrongTurn");
			return "";
		case "RestartPit":
			removeVariable("I_PitKeyA");
			return "";
		case "RestartMaze":
			removeVariable("MazeAStart");
			removeVariable("FailedMazeA");
			removeVariable("MazeBStart");
			removeVariable("FailedMazeB");
			removeVariable("MazeCStart");
			removeVariable("FailedMazeC");
			return "";
		default:
			return v;
		}
	}

	public void addLocationData(string name)
	{
		if (!((name == "screwDriver") | (name == "shovel") | (name == "I_PitKeyA")) && !checkForLocation(name))
		{
			mapSaveData mapSaveData2 = new mapSaveData();
			mapSaveData2.mapPieceName = name;
			switch (name)
			{
			case "Piece1":
				mapSaveData2.currentPosition = new Vector2(1000f, 400f);
				mapSaveData2.origin = getHalfOrgin(269f, 279f);
				break;
			case "Piece2":
				mapSaveData2.currentPosition = new Vector2(900f, 500f);
				mapSaveData2.origin = getHalfOrgin(330f, 343f);
				break;
			case "Piece3":
				mapSaveData2.currentPosition = new Vector2(600f, 500f);
				mapSaveData2.origin = getHalfOrgin(345f, 255f);
				break;
			case "Piece4":
				mapSaveData2.currentPosition = new Vector2(800f, 500f);
				mapSaveData2.origin = getHalfOrgin(382f, 219f);
				break;
			case "Piece5":
				mapSaveData2.currentPosition = new Vector2(600f, 450f);
				mapSaveData2.origin = getHalfOrgin(356f, 240f);
				break;
			case "Piece6":
				mapSaveData2.currentPosition = new Vector2(500f, 550f);
				mapSaveData2.origin = getHalfOrgin(245f, 410f);
				break;
			case "Piece7":
				mapSaveData2.currentPosition = new Vector2(800f, 500f);
				mapSaveData2.origin = getHalfOrgin(282f, 298f);
				break;
			case "Piece8":
				mapSaveData2.currentPosition = new Vector2(800f, 550f);
				mapSaveData2.origin = getHalfOrgin(286f, 334f);
				break;
			case "Piece9":
				mapSaveData2.currentPosition = new Vector2(200f, 500f);
				mapSaveData2.origin = getHalfOrgin(291f, 314f);
				break;
			}
			currentMapSaveData.Add(mapSaveData2);
		}
	}

	private Vector2 getHalfOrgin(float x, float y)
	{
		Vector2 zero = Vector2.Zero;
		zero.X = x * 0.5f;
		zero.Y = y * 0.5f;
		return zero;
	}

	public bool checkForLocation(string name)
	{
		foreach (mapSaveData currentMapSaveDatum in currentMapSaveData)
		{
			if (currentMapSaveDatum.mapPieceName == name)
			{
				return true;
			}
		}
		return false;
	}

	public void removeVariable(string v)
	{
		foreach (string activeVariable in activeVariables)
		{
			if (activeVariable == v)
			{
				activeVariables.Remove(activeVariable);
				break;
			}
		}
	}

	public bool checkVariablesForQuestionCondition(QuestionData question)
	{
		foreach (VariableData item in question.variableAdded)
		{
			if (item.isRequired & !item.isNot)
			{
				if (!checkForVariable(item.variableName))
				{
					return false;
				}
			}
			else if ((item.isNot & item.isRequired) && !checkNotForVariable(item.variableName))
			{
				return false;
			}
		}
		return true;
	}

	private bool checkNotForVariable(string s)
	{
		foreach (string activeVariable in activeVariables)
		{
			if (activeVariable == s)
			{
				return false;
			}
		}
		return true;
	}
}
