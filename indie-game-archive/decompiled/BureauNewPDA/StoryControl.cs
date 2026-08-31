using System;
using System.Collections.Generic;
using System.Linq;
using BureauNewPDA.Data;
using BureauNewPDA.Helpers;
using BureauNewPDA.VideoData;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BureauNewPDA;

public class StoryControl
{
	private SampleData currentChapter = new SampleData();

	public StoryData currentStory = new StoryData();

	public List<string> playSimpleSFX = new List<string>();

	private FileIO myFileIO = new FileIO();

	private DisplayText myDisplayText = new DisplayText();

	private DisplayText myDisplayQuestion1 = new DisplayText();

	private DisplayText myDisplayQuestion2 = new DisplayText();

	private DisplayText myDisplayQuestion3 = new DisplayText();

	private DisplayText myDisplayQuestion4 = new DisplayText();

	private DisplayText myDisplayQuestion5 = new DisplayText();

	private Vector2 QuestionPosition1 = Vector2.Zero;

	private Vector2 QuestionPosition2 = Vector2.Zero;

	private Vector2 QuestionPosition3 = Vector2.Zero;

	private Vector2 QuestionPosition4 = Vector2.Zero;

	private Vector2 QuestionPosition5 = Vector2.Zero;

	private Color question1Color = Color.White;

	private Color question2Color = Color.White;

	private Color question3Color = Color.White;

	private Color question4Color = Color.White;

	private Color question5Color = Color.White;

	private bool isQuestion1;

	private bool isQuestion2;

	private bool isQuestion3;

	private bool isQuestion4;

	private bool isQuestion5;

	private int totalQuestions = -1;

	private int mainTextLines;

	private bool showDisplayText;

	private bool showQuestionText;

	private int currentSelection = 1;

	private Vector2 textPosition = new Vector2(250f, 500f);

	public CoreDisplayElements myCoreDisplayElements = new CoreDisplayElements();

	public CursorControl myCursorControl = new CursorControl();

	public bool startPDA;

	private Color questionSelectColor = Color.Yellow;

	private double joyProgress = 1.0;

	private TimeSpan joyTimeSpan = TimeSpan.FromMilliseconds(170.0);

	public SaveData saveData = new SaveData();

	public bool canSaveData;

	public int pendingLoadLevel = -1;

	public int pendingPlayVideoId = -1;

	public PlayerIndex myPlayer;

	public bool foundPlayer;

	public bool lookForPlayer;

	public bool isTrialMode = true;

	public bool checkedForTrialMode;

	public bool useStorage;

	public bool saveMasterLoaded;

	public SaveDataMaster saveDataMaster = new SaveDataMaster();

	public bool pendingDataLoad;

	public bool dataLoaded;

	public bool quitGame;

	public bool purchaseGame;

	public bool wasInTrialMode;

	private int currentLocationId = -1;

	private int sceneId = -1;

	private int videoControlReturnId = -1;

	private bool doItOnce;

	public void loadNewChapter(string chapterName, int startId)
	{
		if ((chapterName == "SR6-RameshCaseSolved") & isTrialMode)
		{
			chapterName = "SR6-RameshCaseSolvedTrialMode";
		}
		currentChapter = myFileIO.loadData(chapterName);
		loadNextStory(startId);
		currentSelection = 1;
	}

	public void loadNextStory(int nextStoryId)
	{
		foreach (StoryData myStory in currentChapter.myStoryList)
		{
			if (myStory.sceneId != nextStoryId)
			{
				continue;
			}
			currentStory = myStory;
			addText();
			if (currentStory.sceneType == "Condition")
			{
				pendingPlayVideoId = -1;
			}
			else if (currentStory.sceneType == "PathSpecial")
			{
				if ((currentStory.sceneId != 15) & (currentStory.sceneId != 19))
				{
					myCursorControl.activateCursor();
				}
				else if (myCursorControl.currentActionData.id != "")
				{
					myCursorControl.addItem(saveData, myCursorControl.currentActionData.id);
				}
				if ((myStory.chapter == "Path") & (myStory.sceneId == 13))
				{
					playSimpleSFX.Add("windA");
				}
				myCursorControl.addSceneData(myStory.chapter + myStory.sceneId, saveData);
				pendingPlayVideoId = myStory.displayStateId;
			}
			else
			{
				pendingPlayVideoId = myStory.displayStateId;
			}
			break;
		}
		if (pendingPlayVideoId == 66)
		{
			Console.WriteLine("TraceIt");
		}
	}

	public void addText()
	{
		if ((currentStory.sceneType == "BasicContinue") | (currentStory.sceneType == "BasicDialogue"))
		{
			myDisplayText.addTextRawForBuilder(currentStory.bodyText, myCoreDisplayElements.MainFontRegular, 800);
			showDisplayText = true;
			showQuestionText = false;
		}
		else if ((currentStory.sceneType == "BasicContinueToPDAHome") | (currentStory.sceneType == "BasicContinueToPDA"))
		{
			if (currentStory.bodyText != "")
			{
				myDisplayText.addTextRawForBuilder(currentStory.headerText + " " + currentStory.bodyText, myCoreDisplayElements.MainFontRegular, 800);
				showDisplayText = true;
				showQuestionText = false;
			}
			else
			{
				myDisplayText.addTextRawForBuilder("", myCoreDisplayElements.MainFontRegular, 800);
				showDisplayText = false;
				showQuestionText = false;
			}
		}
		else
		{
			showDisplayText = false;
			showQuestionText = false;
		}
		if (currentStory.sceneType == "BasicDialogue")
		{
			if ((currentStory.chapter == "LoadGame") | (currentStory.chapter == "OverrightGame") | (currentStory.chapter == "OptionsGame"))
			{
				addQuestionText(1, currentStory.questions, 200);
			}
			else
			{
				addQuestionText(1, currentStory.questions, 505);
			}
			showQuestionText = true;
		}
	}

	private void updateQuestionSelected(int selection)
	{
		question1Color = Color.White;
		question2Color = Color.White;
		question3Color = Color.White;
		question4Color = Color.White;
		question5Color = Color.White;
		switch (selection)
		{
		case 1:
			question1Color = questionSelectColor;
			updateTextPositionAll(1);
			break;
		case 2:
			question2Color = questionSelectColor;
			updateTextPositionAll(2);
			break;
		case 3:
			question3Color = questionSelectColor;
			updateTextPositionAll(3);
			break;
		case 4:
			question4Color = questionSelectColor;
			updateTextPositionAll(4);
			break;
		case 5:
			question5Color = questionSelectColor;
			updateTextPositionAll(5);
			break;
		}
	}

	private void updateTextPositionAll(int selection)
	{
		QuestionPosition1 = updateTextPosition(QuestionPosition1, 1, selection);
		QuestionPosition2 = updateTextPosition(QuestionPosition2, 2, selection);
		QuestionPosition3 = updateTextPosition(QuestionPosition3, 3, selection);
		QuestionPosition4 = updateTextPosition(QuestionPosition4, 4, selection);
		QuestionPosition5 = updateTextPosition(QuestionPosition5, 5, selection);
	}

	private Vector2 updateTextPosition(Vector2 questionPosition, int questionid, int selectId)
	{
		bool flag = false;
		if (questionid == selectId)
		{
			flag = true;
		}
		if (flag)
		{
			if (questionPosition.X > 250f)
			{
				questionPosition.X -= 5f;
			}
		}
		else if (questionPosition.X < 290f)
		{
			questionPosition.X += 5f;
		}
		return questionPosition;
	}

	private void addQuestionText(int selection, List<QuestionData> questions, int startY)
	{
		QuestionPosition1 = new Vector2(290f, startY + myDisplayText.returnLines * 27);
		QuestionPosition2 = new Vector2(290f, QuestionPosition1.Y + 27f);
		QuestionPosition3 = new Vector2(290f, QuestionPosition2.Y + 27f);
		QuestionPosition4 = new Vector2(290f, QuestionPosition3.Y + 27f);
		QuestionPosition5 = new Vector2(290f, QuestionPosition4.Y + 27f);
		myDisplayQuestion1 = new DisplayText();
		myDisplayQuestion2 = new DisplayText();
		myDisplayQuestion3 = new DisplayText();
		myDisplayQuestion4 = new DisplayText();
		myDisplayQuestion5 = new DisplayText();
		isQuestion1 = false;
		isQuestion2 = false;
		isQuestion3 = false;
		isQuestion4 = false;
		isQuestion5 = false;
		foreach (QuestionData question in questions)
		{
			switch (question.id)
			{
			case 1:
				addQuestionText(question.questionText, myDisplayQuestion1);
				isQuestion1 = true;
				break;
			case 2:
				addQuestionText(question.questionText, myDisplayQuestion2);
				isQuestion2 = true;
				break;
			case 3:
				addQuestionText(question.questionText, myDisplayQuestion3);
				isQuestion3 = true;
				break;
			case 4:
				addQuestionText(question.questionText, myDisplayQuestion4);
				isQuestion4 = true;
				break;
			case 5:
				addQuestionText(question.questionText, myDisplayQuestion5);
				isQuestion5 = true;
				break;
			}
		}
		totalQuestions = questions.Count();
	}

	public void addQuestionText(string text, DisplayText currentQuestionText)
	{
		currentQuestionText.addTextRawForBuilder(text, myCoreDisplayElements.MainFontRegular, 800);
	}

	public void drawText(SpriteBatch spriteBatch)
	{
		if (showDisplayText)
		{
			myDisplayText.advanceDisplayBuilderText();
			spriteBatch.DrawString(myCoreDisplayElements.MainFontRegular, myDisplayText.getDisplayBuilderText(), textPosition, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.995f);
			spriteBatch.DrawString(myCoreDisplayElements.MainFontRegular, myDisplayText.getDisplayBuilderText(), textPosition + new Vector2(2f, 2f), Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.994f);
		}
		if (showQuestionText)
		{
			if (myDisplayText.isFinishedDrawing & isQuestion1)
			{
				myDisplayQuestion1.advanceDisplayBuilderText();
				spriteBatch.DrawString(myCoreDisplayElements.MainFontRegular, myDisplayQuestion1.getDisplayBuilderText(), QuestionPosition1, question1Color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.995f);
				spriteBatch.DrawString(myCoreDisplayElements.MainFontRegular, myDisplayQuestion1.getDisplayBuilderText(), QuestionPosition1 + new Vector2(2f, 2f), Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.994f);
			}
			if (myDisplayQuestion1.isFinishedDrawing & isQuestion2)
			{
				myDisplayQuestion2.advanceDisplayBuilderText();
				spriteBatch.DrawString(myCoreDisplayElements.MainFontRegular, myDisplayQuestion2.getDisplayBuilderText(), QuestionPosition2, question2Color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.995f);
				spriteBatch.DrawString(myCoreDisplayElements.MainFontRegular, myDisplayQuestion2.getDisplayBuilderText(), QuestionPosition2 + new Vector2(2f, 2f), Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.994f);
			}
			if (myDisplayQuestion2.isFinishedDrawing & isQuestion3)
			{
				myDisplayQuestion3.advanceDisplayBuilderText();
				spriteBatch.DrawString(myCoreDisplayElements.MainFontRegular, myDisplayQuestion3.getDisplayBuilderText(), QuestionPosition3, question3Color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.995f);
				spriteBatch.DrawString(myCoreDisplayElements.MainFontRegular, myDisplayQuestion3.getDisplayBuilderText(), QuestionPosition3 + new Vector2(2f, 2f), Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.994f);
			}
			if (myDisplayQuestion3.isFinishedDrawing & isQuestion4)
			{
				myDisplayQuestion4.advanceDisplayBuilderText();
				spriteBatch.DrawString(myCoreDisplayElements.MainFontRegular, myDisplayQuestion4.getDisplayBuilderText(), QuestionPosition4, question4Color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.995f);
				spriteBatch.DrawString(myCoreDisplayElements.MainFontRegular, myDisplayQuestion4.getDisplayBuilderText(), QuestionPosition4 + new Vector2(2f, 2f), Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.994f);
			}
			if (myDisplayQuestion4.isFinishedDrawing & isQuestion5)
			{
				myDisplayQuestion5.advanceDisplayBuilderText();
				spriteBatch.DrawString(myCoreDisplayElements.MainFontRegular, myDisplayQuestion5.getDisplayBuilderText(), QuestionPosition5, question5Color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.995f);
				spriteBatch.DrawString(myCoreDisplayElements.MainFontRegular, myDisplayQuestion5.getDisplayBuilderText(), QuestionPosition5 + new Vector2(2f, 2f), Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.994f);
			}
		}
	}

	public int getNextStoryId(int questionId)
	{
		foreach (QuestionData question in currentStory.questions)
		{
			if (question.id == questionId)
			{
				return question.sceneId;
			}
		}
		return -1;
	}

	public void getNextChapterStoryId(int questionId)
	{
		foreach (QuestionData question in currentStory.questions)
		{
			if (question.id == questionId)
			{
				loadNewChapter(question.questionText, question.sceneId);
				currentLocationId = question.sceneId;
			}
		}
	}

	private void checkCondition()
	{
		for (int i = 1; i < 10; i++)
		{
			foreach (QuestionData question in currentStory.questions)
			{
				if (question.id == i && saveData.checkVariablesForQuestionCondition(question))
				{
					sceneId = question.sceneId;
					i = 10;
					break;
				}
			}
		}
		loadNextStory(sceneId);
	}

	private int checkForVideoOverAction()
	{
		foreach (QuestionData question in currentStory.questions)
		{
			if (question.startFrame == question.endFrame)
			{
				return question.sceneId;
			}
		}
		return -1;
	}

	private int checkForVideoControlAction(int type, double time)
	{
		foreach (QuestionData question in currentStory.questions)
		{
			if (question.startFrame != question.endFrame && ((question.triggerType == type) & (time >= question.startTime) & (time <= question.endTime)))
			{
				return question.sceneId;
			}
		}
		return -1;
	}

	private void addDialogueVariables(int questionId)
	{
		foreach (QuestionData question in currentStory.questions)
		{
			if (question.id != questionId)
			{
				continue;
			}
			foreach (VariableData item in question.variableAdded)
			{
				if ((!item.isNot & !item.isRequired) && !checkForSFX(item.variableName))
				{
					saveData.addVariables(item.variableName);
				}
			}
		}
	}

	private bool checkForSFX(string name)
	{
		if (name == "SFXRumble")
		{
			playSimpleSFX.Add("RumbleDebris");
			return true;
		}
		return false;
	}

	public void specialConditions(GamePadControl myGamePad, VideoControl.VideoStatus currentVideoStatus)
	{
		if (currentStory.sceneId == 1)
		{
			if ((foundPlayer & checkedForTrialMode) && currentVideoStatus == VideoControl.VideoStatus.Stopped)
			{
				loadNextStory(5);
				useStorage = true;
			}
			else if (!lookForPlayer)
			{
				lookForPlayer = true;
			}
		}
		else if (currentStory.sceneId == 2)
		{
			if (!lookForPlayer)
			{
				lookForPlayer = true;
			}
		}
		else if (currentStory.sceneId == 3)
		{
			if (currentVideoStatus == VideoControl.VideoStatus.Stopped)
			{
				loadNextStory(10);
			}
		}
		else if (currentStory.sceneId == 4)
		{
			if (!useStorage)
			{
				useStorage = true;
			}
			else if (saveMasterLoaded && currentVideoStatus == VideoControl.VideoStatus.Stopped)
			{
				loadNextStory(5);
			}
		}
		else if (currentStory.sceneId == 11)
		{
			loadNewChapter(saveData.currentScene, saveData.currentSceneId);
			saveData.newMusic = saveData.musicPlayingOnSave;
			myCursorControl.loadInventory(saveData);
			currentLocationId = saveData.currentSceneId;
		}
		else if (currentStory.sceneId == 21)
		{
			quitGame = true;
		}
		else if (currentStory.sceneId == 9)
		{
			purchaseGame = true;
			loadNextStory(8);
		}
		else if (currentStory.sceneId == 15)
		{
			saveData = new SaveData();
			saveData.gameInProgress = false;
			loadNextStory(8);
		}
		else if (currentStory.sceneId == 5 && currentVideoStatus == VideoControl.VideoStatus.Stopped)
		{
			if (!doItOnce)
			{
				saveData = new SaveData();
				pendingLoadLevel = saveDataMaster.lastSavedId;
				pendingDataLoad = true;
				dataLoaded = false;
				doItOnce = true;
			}
			if (pendingLoadLevel == -1 && currentVideoStatus == VideoControl.VideoStatus.Stopped)
			{
				useStorage = true;
				loadNextStory(8);
				doItOnce = false;
			}
		}
		else
		{
			if (currentStory.sceneId != 8 || currentVideoStatus != VideoControl.VideoStatus.Stopped)
			{
				return;
			}
			if (isTrialMode)
			{
				if (saveData.gameInProgress)
				{
					loadNextStory(7);
				}
				else
				{
					loadNextStory(19);
				}
				wasInTrialMode = true;
			}
			else if (saveData.gameInProgress)
			{
				loadNextStory(10);
			}
			else
			{
				loadNextStory(2);
			}
		}
	}

	private bool getNextSlot()
	{
		int i = 1;
		bool flag = false;
		for (; i < 5; i++)
		{
			flag = false;
			foreach (SaveDataMaster.saveDataShell save in saveDataMaster.saveList)
			{
				if (save.id == i)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				break;
			}
		}
		if (i < 5)
		{
			saveDataMaster.lastSavedId = i;
			return true;
		}
		return false;
	}

	public void updateLoadText()
	{
		int num = 1;
		QuestionData questionData = new QuestionData();
		foreach (StoryData myStory in currentChapter.myStoryList)
		{
			if (myStory.sceneId != 2)
			{
				continue;
			}
			myStory.questions.Clear();
			foreach (SaveDataMaster.saveDataShell save in saveDataMaster.saveList)
			{
				questionData = new QuestionData();
				questionData.id = save.id;
				questionData.questionText = save.id + ": Last Saved On - " + save.saveDateTime.ToLocalTime().ToString();
				questionData.sceneId = 10 + save.id;
				myStory.questions.Add(questionData);
				if (save.id > num)
				{
					num = save.id;
				}
			}
			questionData = new QuestionData();
			questionData.id = num + 1;
			questionData.questionText = "Back to Main Menu";
			questionData.sceneId = 3;
			myStory.questions.Add(questionData);
			break;
		}
	}

	public void updateDialogueText()
	{
		foreach (StoryData myStory in currentChapter.myStoryList)
		{
			if (myStory.sceneId != 2)
			{
				continue;
			}
			foreach (QuestionData question in myStory.questions)
			{
				if (question.id == 1)
				{
					if (saveDataMaster.vibrationOn)
					{
						question.questionText = "Vibration = On";
					}
					else
					{
						question.questionText = "Vibration = Off";
					}
				}
				else if (question.id == 2)
				{
					if (saveDataMaster.skipAnimation)
					{
						question.questionText = "Cutscene Fast Skip = On";
					}
					else
					{
						question.questionText = "Cutscene Fast Skip = Off";
					}
				}
				else if (question.id == 3)
				{
					if (saveDataMaster.fastTextSkip)
					{
						question.questionText = "Fast Text Skip = On";
					}
					else
					{
						question.questionText = "Fast Text Skip = Off";
					}
				}
			}
		}
	}

	public void doSave(string sceneName, int sceneId)
	{
		if (canSaveData && ((sceneName != "LoadGame") & (sceneName != "StartGame") & (sceneName != "OverrightGame") & (sceneName != "OptionsGame")))
		{
			saveData.pendingDataSave = true;
			saveData.currentScene = sceneName;
			saveData.currentSceneId = sceneId;
			saveDataMaster.saveData(1);
		}
	}

	private void setGameOptions()
	{
		if (currentStory.sceneId == 3)
		{
			if (currentSelection == 1)
			{
				saveDataMaster.vibrationOn = true;
			}
			else
			{
				saveDataMaster.vibrationOn = false;
			}
		}
		else if (currentStory.sceneId == 4)
		{
			if (currentSelection == 1)
			{
				saveDataMaster.skipAnimation = true;
			}
			else
			{
				saveDataMaster.skipAnimation = false;
			}
		}
		else if (currentStory.sceneId == 5)
		{
			if (currentSelection == 1)
			{
				saveDataMaster.fastTextSkip = true;
			}
			else
			{
				saveDataMaster.fastTextSkip = false;
			}
		}
		else if (currentStory.sceneId == 2 && currentSelection == 5)
		{
			textPosition = new Vector2(250f, 470f);
			if (saveData.checkForVariable("GameStarted"))
			{
				loadNextStory(23);
			}
			else if (isTrialMode)
			{
				loadNewChapter("StartGame", 3);
				useStorage = false;
			}
			else
			{
				loadNewChapter("StartGame", 4);
				useStorage = true;
			}
		}
	}

	private bool checkTextIsComplete()
	{
		if (saveDataMaster.fastTextSkip)
		{
			return true;
		}
		if (myDisplayText.isFinishedDrawing)
		{
			return true;
		}
		return false;
	}

	public void updateStory(GamePadControl myGamePad, VideoControl.VideoStatus currentVideoStatus, double currentVideoTime, GameTime gameTime)
	{
		myGamePad.turnOffVibrate();
		if ((currentStory.chapter == "StartLucky") & (currentStory.sceneType == "StartMenu"))
		{
			specialConditions(myGamePad, currentVideoStatus);
			return;
		}
		if ((currentStory.chapter == "StartLucky") & (currentStory.sceneId == 5))
		{
			specialConditions(myGamePad, currentVideoStatus);
			return;
		}
		if ((currentStory.chapter == "StartLucky") & (currentStory.sceneId == 11))
		{
			specialConditions(myGamePad, currentVideoStatus);
			return;
		}
		if ((currentStory.chapter == "StartLucky") & (currentStory.sceneId == 9))
		{
			specialConditions(myGamePad, currentVideoStatus);
			return;
		}
		if ((currentStory.chapter == "StartLucky") & (currentStory.sceneId == 21))
		{
			quitGame = true;
			return;
		}
		switch (currentStory.sceneType)
		{
		case "PassThru":
			if (currentVideoStatus == VideoControl.VideoStatus.Stopped)
			{
				addDialogueVariables(1);
				loadNextStory(getNextStoryId(1));
			}
			else if (myGamePad.pressStart & saveDataMaster.skipAnimation)
			{
				addDialogueVariables(1);
				loadNextStory(getNextStoryId(1));
			}
			break;
		case "BasicContinue":
			showDisplayText = true;
			if (myGamePad.padAPressed & checkTextIsComplete())
			{
				addDialogueVariables(1);
				if ((currentStory.chapter == "IanConversationScrew") & (currentStory.sceneId == 4))
				{
					myCursorControl.addItem(saveData, "screwDriver");
					saveData.addVariables("screwDriver");
					playSimpleSFX.Add("zingMix");
				}
				loadNextStory(getNextStoryId(1));
			}
			break;
		case "PathSpecial":
			showDisplayText = false;
			if (myCursorControl.isPuzzleGame && myGamePad.padAPressed && myCursorControl.puzzleGameToggle())
			{
				myCursorControl.isPuzzleGame = false;
				myCursorControl.isCheckingPuzzle = false;
				saveData.removeVariable("PuzzleCorrect");
				saveData.removeVariable("PuzzleWrong");
				if (myCursorControl.correctAnswers >= 4)
				{
					saveData.addVariables("PuzzleCorrect");
				}
				else
				{
					saveData.addVariables("PuzzleWrong");
				}
				loadNextStory(getNextStoryId(1));
			}
			if (myCursorControl.spinWheelSuccess)
			{
				myCursorControl.deactiveCursor();
				currentLocationId = 24;
				saveData.addVariables("GateFOpen");
				myCursorControl.spinWheelSuccess = false;
				loadNextStory(currentLocationId);
			}
			else if (myCursorControl.turnOnMap & myCursorControl.mapIsComplete & !myCursorControl.mapCompleteDisplayed)
			{
				myCursorControl.turnOnMap = false;
				myCursorControl.mapCompleteDisplayed = true;
				loadNextStory(19);
			}
			else if (myGamePad.padAPressed & myCursorControl.turnOnMap & myCursorControl.mapIsComplete & myCursorControl.mapCompleteDisplayed)
			{
				saveData.addVariables("MapComplete");
				saveData.addLocationData(myCursorControl.currentActionData.id);
				myCursorControl.addItem(saveData, myCursorControl.currentActionData.id);
				if (currentLocationId == -1)
				{
					currentLocationId = 1;
				}
				else if (currentLocationId == 16)
				{
					currentLocationId = 17;
				}
				else if (currentLocationId == 27)
				{
					currentLocationId = 3;
				}
				else if (currentLocationId == 18)
				{
					currentLocationId = 1;
				}
				loadNextStory(currentLocationId);
			}
			else if (myGamePad.padAPressed & myCursorControl.turnOnMap)
			{
				if (myCursorControl.toggleMapSelection(saveData))
				{
					playSimpleSFX.Add("zingMix");
				}
			}
			else if (myGamePad.padBPressed & myCursorControl.turnOnMap)
			{
				if (currentLocationId == -1)
				{
					currentLocationId = 1;
				}
				else if (currentLocationId == 16)
				{
					currentLocationId = 17;
				}
				else if (currentLocationId == 27)
				{
					currentLocationId = 3;
				}
				else if (currentLocationId == 18)
				{
					currentLocationId = 1;
				}
				loadNextStory(currentLocationId);
			}
			else if (myGamePad.padAPressed & myCursorControl.canClick)
			{
				if (myCursorControl.inventoryItemTarget)
				{
					if (myCursorControl.selectInventoryItem(currentStory.chapter))
					{
						myCursorControl.deactiveCursor();
						loadNextStory(15);
					}
					else if ((currentStory.chapter == "Path") & (currentStory.sceneId == 19))
					{
						saveData.addVariables("MapComplete");
						saveData.addLocationData(myCursorControl.currentActionData.id);
						myCursorControl.addItem(saveData, myCursorControl.currentActionData.id);
						if (currentLocationId == -1)
						{
							currentLocationId = 1;
						}
						else if (currentLocationId == 16)
						{
							currentLocationId = 17;
						}
						else if (currentLocationId == 27)
						{
							currentLocationId = 3;
						}
						else if (currentLocationId == 18)
						{
							currentLocationId = 1;
						}
						loadNextStory(currentLocationId);
					}
				}
				else
				{
					if (!myCursorControl.specialCheck())
					{
						break;
					}
					if (myCursorControl.currentActionData.collisionType == RectangleActionData.RectCollisionType.Spin)
					{
						saveData.addVariables("NeedScrewDriver");
						playSimpleSFX.Add("SpinWheel");
						myCursorControl.spinWheel();
					}
					else if ((myCursorControl.currentActionData.nextRefId == -1) & (currentStory.chapter != "Maze"))
					{
						if (myCursorControl.currentActionData.id == "Piece9")
						{
							currentLocationId = 27;
						}
						playSimpleSFX.Add("zingMix");
						saveData.addVariables(myCursorControl.currentActionData.id);
						saveData.addLocationData(myCursorControl.currentActionData.id);
						myCursorControl.addItem(saveData, myCursorControl.currentActionData.id);
						if (currentLocationId == 27)
						{
							myCursorControl.deactiveCursor();
							loadNextStory(currentLocationId);
						}
					}
					else if ((myCursorControl.currentActionData.nextRefId == -1) & (currentStory.chapter == "Maze"))
					{
						if (myCursorControl.currentActionData.id == "Red")
						{
							playSimpleSFX.Add("Arcade Action 05");
						}
						else if (myCursorControl.currentActionData.id == "Green")
						{
							playSimpleSFX.Add("Arcade Action 04");
						}
						else if (myCursorControl.currentActionData.id == "Back")
						{
							playSimpleSFX.Add("Arcade Beep 03");
						}
						else if (myCursorControl.currentActionData.id == "Done")
						{
							playSimpleSFX.Add("blip1");
						}
						else
						{
							playSimpleSFX.Add("Arcade Beep 02");
						}
					}
					else if (myCursorControl.currentActionData.chapterRef != "NA")
					{
						myCursorControl.deactiveCursor();
						loadNewChapter(myCursorControl.currentActionData.chapterRef, myCursorControl.currentActionData.nextRefId);
					}
					else
					{
						playSimpleSFX.Add("Arcade Action 05");
						myCursorControl.deactiveCursor();
						addDialogueVariables(1);
						currentLocationId = myCursorControl.currentActionData.nextRefId;
						loadNextStory(myCursorControl.currentActionData.nextRefId);
					}
				}
			}
			else if (myGamePad.padYPressed && myCursorControl.inventoryCount() > 0)
			{
				if (myCursorControl.toggleInventory())
				{
					playSimpleSFX.Add("BRIEFCASE-OPEN LATCH");
				}
				else
				{
					playSimpleSFX.Add("BRIEFCASE-CLOSE LATCH");
				}
			}
			break;
		case "BasicDialogue":
			joyProgress += gameTime.ElapsedGameTime.TotalMilliseconds / joyTimeSpan.TotalMilliseconds;
			if (joyProgress > 1.0)
			{
				if (myGamePad.anyDirection == GamePadControl.direction.S)
				{
					currentSelection++;
					if (currentSelection > totalQuestions)
					{
						currentSelection = totalQuestions;
					}
					else
					{
						joyProgress = 0.0;
					}
				}
				else if (myGamePad.anyDirection == GamePadControl.direction.N)
				{
					currentSelection--;
					if (currentSelection < 1)
					{
						currentSelection = 1;
					}
					else
					{
						joyProgress = 0.0;
					}
				}
			}
			updateQuestionSelected(currentSelection);
			showDisplayText = true;
			if (myGamePad.padAPressed & checkTextIsComplete())
			{
				addDialogueVariables(currentSelection);
				if (currentStory.chapter == "OptionsGame")
				{
					setGameOptions();
				}
				currentLocationId = getNextStoryId(currentSelection);
				loadNextStory(currentLocationId);
				currentSelection = 1;
			}
			break;
		case "BasicDialogueLoad":
			joyProgress += gameTime.ElapsedGameTime.TotalMilliseconds / joyTimeSpan.TotalMilliseconds;
			if (joyProgress > 1.0)
			{
				if (myGamePad.anyDirection == GamePadControl.direction.S)
				{
					currentSelection++;
					if (currentSelection > totalQuestions)
					{
						currentSelection = totalQuestions;
					}
					else
					{
						joyProgress = 0.0;
					}
				}
				else if (myGamePad.anyDirection == GamePadControl.direction.N)
				{
					currentSelection--;
					if (currentSelection < 1)
					{
						currentSelection = 1;
					}
					else
					{
						joyProgress = 0.0;
					}
				}
			}
			updateQuestionSelected(currentSelection);
			showDisplayText = true;
			if (myGamePad.padAPressed)
			{
				addDialogueVariables(currentSelection);
				currentLocationId = getNextStoryId(currentSelection);
				loadNextStory(currentLocationId);
				currentSelection = 1;
			}
			break;
		case "VideoControl":
			myGamePad.turnOnVibrate();
			if (currentVideoStatus == VideoControl.VideoStatus.Stopped)
			{
				videoControlReturnId = checkForVideoOverAction();
			}
			else if (myGamePad.rightTrigger > 0.1f)
			{
				videoControlReturnId = checkForVideoControlAction(1, currentVideoTime);
			}
			else if (myGamePad.leftTrigger > 0.1f)
			{
				videoControlReturnId = checkForVideoControlAction(2, currentVideoTime);
			}
			if (videoControlReturnId != -1)
			{
				addDialogueVariables(currentSelection);
				myGamePad.turnOffVibrate();
				loadNextStory(videoControlReturnId);
				videoControlReturnId = -1;
			}
			break;
		case "PassThruToNewScene":
			if (currentVideoStatus == VideoControl.VideoStatus.Stopped)
			{
				addDialogueVariables(1);
				getNextChapterStoryId(1);
				if (canSaveData)
				{
					doSave(currentStory.chapter, currentStory.sceneId);
				}
			}
			break;
		case "Condition":
			if (currentVideoStatus == VideoControl.VideoStatus.Stopped)
			{
				addDialogueVariables(currentSelection);
				checkCondition();
			}
			break;
		}
	}
}
