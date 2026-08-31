using System.Collections.Generic;
using System.Linq;

namespace BureauNewPDA;

public class VariableEngine
{
	public class variableData
	{
		public int id = -1;

		public string variableName = "";
	}

	private List<ResearchControlData.ResearchData> masterResearchList = new List<ResearchControlData.ResearchData>();

	private List<variableData> variableList = new List<variableData>();

	private ResearchControlData researchControlData = new ResearchControlData();

	private PuzzleDataControl puzzleDataControl = new PuzzleDataControl();

	private List<PuzzleDataControl.PuzzleData> puzzleDataList = new List<PuzzleDataControl.PuzzleData>();

	private PDATextListData PDAd = new PDATextListData();

	private int count;

	public ResearchControlData.ResearchData currentResearchData = new ResearchControlData.ResearchData();

	private string unlockText = "";

	private int puzzleCount;

	private bool found;

	public void addData()
	{
		researchControlData.masterResearchList = masterResearchList;
		researchControlData.variableList = variableList;
		researchControlData.addData();
		puzzleDataControl.puzzleDataList = puzzleDataList;
		puzzleDataControl.addData();
	}

	public void update(List<string> activeVariables, List<PDATextListData> caseDataList, int currentTime, bool isAtHome)
	{
		count = 0;
		caseDataList.Clear();
		foreach (ResearchControlData.ResearchData masterResearch in masterResearchList)
		{
			if (isExcluded(masterResearch, activeVariables) || !isAvailable(masterResearch, activeVariables))
			{
				continue;
			}
			PDAd = new PDATextListData();
			if (!isComplete(masterResearch, activeVariables) && !(!isAtHome & masterResearch.hasLocationRequirement))
			{
				count++;
				if (masterResearch.displayState != ResearchControlData.ResearchData.DisplayState.Added)
				{
					masterResearch.displayState = ResearchControlData.ResearchData.DisplayState.Adding;
				}
				PDAd.addData2Column(masterResearch.id, count, masterResearch.headerTxt, minutesToHouse(masterResearch.baseDurationMinutes), 0, 650, masterResearch.bodyTxt, PDATextListData.type.BasicResearch, isAvailable: true, isComplete: false);
				PDAd.displayState = masterResearch.displayState;
				caseDataList.Add(PDAd);
			}
		}
		foreach (ResearchControlData.ResearchData masterResearch2 in masterResearchList)
		{
			if (masterResearch2.id == 27)
			{
				masterResearch2.id = 27;
			}
			if (!(!isExcluded(masterResearch2, activeVariables) & (masterResearch2.type == ResearchControlData.ResearchData.activateType.PlayVideoPuzzle)))
			{
				continue;
			}
			puzzleCount = isCompleteCount(masterResearch2, activeVariables);
			if ((masterResearch2.requiredVariables.Count > 1) & (puzzleCount < masterResearch2.requiredVariables.Count) & (puzzleCount != 0))
			{
				if (masterResearch2.displayState != ResearchControlData.ResearchData.DisplayState.Added)
				{
					masterResearch2.displayState = ResearchControlData.ResearchData.DisplayState.Adding;
				}
				count++;
				PDAd = new PDATextListData();
				if (masterResearch2.type == ResearchControlData.ResearchData.activateType.PlayVideoPuzzle)
				{
					unlockText = "Locked - (" + isCompleteCount(masterResearch2, activeVariables) + "/" + masterResearch2.requiredVariables.Count() + ") found.  ";
				}
				else
				{
					unlockText = "";
				}
				PDAd.addData2Column(masterResearch2.id, count, masterResearch2.headerTxt, minutesToHouse(masterResearch2.baseDurationMinutes), 0, 650, unlockText + masterResearch2.bodyTxt, PDATextListData.type.BasicResearch, isAvailable: false, isComplete: false);
				PDAd.displayState = masterResearch2.displayState;
				caseDataList.Add(PDAd);
			}
		}
		foreach (ResearchControlData.ResearchData masterResearch3 in masterResearchList)
		{
			if (isExcluded(masterResearch3, activeVariables) || !isAvailable(masterResearch3, activeVariables))
			{
				continue;
			}
			PDAd = new PDATextListData();
			if (!isComplete(masterResearch3, activeVariables) && (!isAtHome & masterResearch3.hasLocationRequirement))
			{
				count++;
				if (masterResearch3.displayState != ResearchControlData.ResearchData.DisplayState.Added)
				{
					masterResearch3.displayState = ResearchControlData.ResearchData.DisplayState.Adding;
				}
				unlockText = "Must be at Home or FBI Office to do this activity - ";
				PDAd.addData2Column(masterResearch3.id, count, masterResearch3.headerTxt, minutesToHouse(masterResearch3.baseDurationMinutes), 0, 650, unlockText + masterResearch3.bodyTxt, PDATextListData.type.BasicResearch, isAvailable: false, isComplete: false);
				PDAd.displayState = masterResearch3.displayState;
				caseDataList.Add(PDAd);
			}
		}
		foreach (ResearchControlData.ResearchData masterResearch4 in masterResearchList)
		{
			if (isExcluded(masterResearch4, activeVariables) || !isAvailable(masterResearch4, activeVariables))
			{
				continue;
			}
			PDAd = new PDATextListData();
			if (isComplete(masterResearch4, activeVariables))
			{
				count++;
				if (masterResearch4.displayState != ResearchControlData.ResearchData.DisplayState.Updated)
				{
					masterResearch4.displayState = ResearchControlData.ResearchData.DisplayState.Updating;
				}
				PDAd.addData2Column(masterResearch4.id, count, masterResearch4.completedheaderTxt, minutesToHouse(masterResearch4.baseDurationMinutes), 0, 650, masterResearch4.completedBodyTxt, PDATextListData.type.BasicResearch, isAvailable: false, isComplete: true);
				PDAd.displayState = masterResearch4.displayState;
				caseDataList.Add(PDAd);
			}
		}
	}

	public bool findInCaseList(int id, List<PDATextListData> caseList)
	{
		foreach (PDATextListData @case in caseList)
		{
			if (@case.id == id)
			{
				return true;
			}
		}
		return false;
	}

	public PuzzleDataControl.PuzzleData getCurrentPuzzleData()
	{
		foreach (PuzzleDataControl.PuzzleData puzzleData in puzzleDataList)
		{
			if (puzzleData.puzzleId == currentResearchData.playVideoPuzzleId)
			{
				return puzzleData;
			}
		}
		return new PuzzleDataControl.PuzzleData();
	}

	public bool checkIfPuzzleSelectComplete()
	{
		foreach (PuzzleDataControl.PuzzleData puzzleData in puzzleDataList)
		{
			if (puzzleData.puzzleId == currentResearchData.playVideoPuzzleId)
			{
				return checkPuzzleObjectList(puzzleData.objectList);
			}
		}
		return false;
	}

	public bool checkPuzzleObjectList(List<PuzzleDataControl.objectData> l)
	{
		bool result = true;
		foreach (PuzzleDataControl.objectData item in l)
		{
			if ((item.correctOrder != -1) & !item.hasBeenMarkedCorrect)
			{
				result = false;
				break;
			}
		}
		return result;
	}

	public bool checkIfSelectionCorrect(int id, SaveData saveData)
	{
		return false;
	}

	public void finishPuzzleAddVariables(List<PDATextListData> caseDataList, SaveData saveData)
	{
		if (getCurrentPuzzleData().isCorrectSelected & getCurrentPuzzleData().isFinishedOrder)
		{
			foreach (string resultingVariable in currentResearchData.resultingVariables)
			{
				saveData.addVariables(resultingVariable);
			}
		}
		update(saveData.activeVariables, caseDataList, 123, isAtHome: false);
	}

	private string minutesToHouse(int minutes)
	{
		if (minutes == 0)
		{
			return "";
		}
		int num = (int)((double)minutes / 60.0);
		double num2 = minutes - num * 60;
		if (num == 0)
		{
			return "        " + num2 + " minutes";
		}
		if (num2 == 0.0)
		{
			if (num == 1)
			{
				return "        60 minutes";
			}
			return num + " hours";
		}
		return num + " hours " + num2 + " minutes";
	}

	public int isCompleteCount(ResearchControlData.ResearchData r, List<string> activeVarList)
	{
		int num = 0;
		foreach (string requiredVariable in r.requiredVariables)
		{
			foreach (string activeVar in activeVarList)
			{
				if (activeVar == requiredVariable)
				{
					num++;
					break;
				}
			}
		}
		return num;
	}

	public bool isComplete(ResearchControlData.ResearchData r, List<string> activeVarList)
	{
		if (r.resultingVariables.Count == 0)
		{
			return false;
		}
		foreach (string resultingVariable in r.resultingVariables)
		{
			found = false;
			foreach (string activeVar in activeVarList)
			{
				if (activeVar == resultingVariable)
				{
					found = true;
					break;
				}
			}
			if (!found)
			{
				return false;
			}
		}
		return true;
	}

	public bool isAvailable(ResearchControlData.ResearchData r, List<string> activeVarList)
	{
		foreach (string requiredVariable in r.requiredVariables)
		{
			found = false;
			foreach (string activeVar in activeVarList)
			{
				if (activeVar == requiredVariable)
				{
					found = true;
					break;
				}
			}
			if (!found)
			{
				return false;
			}
		}
		return true;
	}

	public bool isExcluded(ResearchControlData.ResearchData r, List<string> activeVarList)
	{
		foreach (string excludeIfVariable in r.excludeIfVariables)
		{
			foreach (string activeVar in activeVarList)
			{
				if (activeVar == excludeIfVariable)
				{
					return true;
				}
			}
		}
		return false;
	}

	public ResearchControlData.ResearchData.activateType getResearchTypeById(int id)
	{
		foreach (ResearchControlData.ResearchData masterResearch in masterResearchList)
		{
			if (masterResearch.id == id)
			{
				return masterResearch.type;
			}
		}
		return ResearchControlData.ResearchData.activateType.PlayVideoReturn;
	}

	public string getVideoNameTypeById(int id)
	{
		foreach (ResearchControlData.ResearchData masterResearch in masterResearchList)
		{
			if (masterResearch.id == id)
			{
				return masterResearch.playVideoName;
			}
		}
		return "";
	}

	public void getCurrentResearchData(int id)
	{
		foreach (ResearchControlData.ResearchData masterResearch in masterResearchList)
		{
			if (masterResearch.id == id)
			{
				currentResearchData = masterResearch;
				break;
			}
		}
	}

	private void addVar(int id, string name)
	{
		variableData variableData2 = new variableData();
		variableData2.id = id;
		variableData2.variableName = name;
		variableList.Add(variableData2);
	}
}
