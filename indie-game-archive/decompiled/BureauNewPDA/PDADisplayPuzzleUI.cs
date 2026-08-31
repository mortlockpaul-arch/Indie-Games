using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BureauNewPDA;

public class PDADisplayPuzzleUI
{
	public enum baseState
	{
		SelectObjects,
		OrderObjects
	}

	public enum transitionState
	{
		Initialize,
		Starting,
		Started,
		Waiting,
		Stopping,
		Processing,
		Checking,
		CheckingFailed,
		CheckPassed,
		OrderCorrect,
		OrderFailed,
		RestartingWithVideo,
		Stopped,
		NA
	}

	public baseState baseStateType;

	public transitionState currentState = transitionState.NA;

	public PuzzleDataControl.PuzzleData currentPuzzle = new PuzzleDataControl.PuzzleData();

	public CoreDisplayElements myCoreDisplayElements = new CoreDisplayElements();

	private List<DisplayText> displayText = new List<DisplayText>();

	private List<DisplayData> displayList = new List<DisplayData>();

	private DisplayText t = new DisplayText();

	private bool isActive;

	private Vector2 tempPosition = Vector2.Zero;

	private int currentSelection = 1;

	private double joyProgressA;

	private TimeSpan joyTimeSpan = TimeSpan.FromMilliseconds(160.0);

	private double joyProgressB;

	private bool isSelectionComplete;

	private int delayCount;

	private int checkSelection;

	private Random myRandom = new Random();

	public string pendingVideo = "";

	public double playPosition;

	private bool waitingForVideoFinish;

	public bool isPuzzleFinishedCorrect;

	private bool correctItemCorrect;

	private bool waitingForVideoToEnd;

	private string lastVideo = "";

	private DisplayData d = new DisplayData();

	public bool update(GameTime gameTime, GamePadControl myGamePad, List<string> playSimpleSFX)
	{
		if (baseStateType == baseState.SelectObjects)
		{
			updateSelectObject(gameTime, myGamePad, playSimpleSFX);
		}
		else if (baseStateType == baseState.OrderObjects)
		{
			updateOrderObjects(gameTime, myGamePad, playSimpleSFX);
		}
		if (!isActive)
		{
			return true;
		}
		return false;
	}

	public void updateOrderObjects(GameTime gameTime, GamePadControl myGamePad, List<string> playSimpleSFX)
	{
		switch (currentState)
		{
		case transitionState.Initialize:
			isActive = true;
			displayText.Clear();
			displayList.Clear();
			currentSelection = 1;
			currentState = transitionState.Starting;
			break;
		case transitionState.Starting:
			addOrderObjectText(showInstructions: true);
			currentState = transitionState.Started;
			break;
		case transitionState.Started:
			addGraphicOrdering(aPressed: false);
			currentState = transitionState.Waiting;
			break;
		case transitionState.Waiting:
			if ((myGamePad.anyDirection != GamePadControl.direction.NotSet) & (myGamePad.joyRightDirection == GamePadControl.direction.NotSet))
			{
				joyProgressA += gameTime.ElapsedGameTime.TotalMilliseconds / joyTimeSpan.TotalMilliseconds;
				if (joyProgressA > 1.0 && moveForOrder(myGamePad.anyDirection))
				{
					playSimpleSFX.Add("ScrollE");
					joyProgressA = 0.0;
					addGraphicOrdering(aPressed: false);
					addTriggerOrderGraphic();
				}
			}
			if (myGamePad.joyRightDirection != GamePadControl.direction.NotSet)
			{
				joyProgressB += gameTime.ElapsedGameTime.TotalMilliseconds / joyTimeSpan.TotalMilliseconds;
				if (joyProgressB > 1.0 && performMoveOrder(myGamePad.joyRightDirection))
				{
					playSimpleSFX.Add("blip1");
					displayText.Clear();
					addOrderObjectText(showInstructions: true);
					joyProgressB = 0.0;
					addGraphicOrdering(aPressed: false);
					addTriggerOrderGraphic();
				}
			}
			if (myGamePad.rightTrigger > 0.1f)
			{
				playSimpleSFX.Add("Arcade Action 05");
				displayList.Clear();
				addGraphicOrderingChecking();
				currentState = transitionState.Checking;
				delayCount = 0;
				checkSelection = 1;
				displayText.Clear();
				addOrderObjectText(showInstructions: false);
				pendingVideo = "PuzzleChangeOrder";
			}
			if (myGamePad.padXPressed & (currentPuzzle.puzzleId == 1))
			{
				displayList.Clear();
				displayText.Clear();
				pendingVideo = currentPuzzle.videoName;
				currentState = transitionState.RestartingWithVideo;
				waitingForVideoFinish = true;
			}
			break;
		case transitionState.Checking:
			isPuzzleFinishedCorrect = false;
			checkOrder(playPosition);
			break;
		case transitionState.OrderFailed:
			if (myGamePad.padAPressed)
			{
				currentState = transitionState.Stopping;
			}
			if (myGamePad.padXPressed & (currentPuzzle.puzzleId == 1))
			{
				pendingVideo = currentPuzzle.videoName;
				currentState = transitionState.RestartingWithVideo;
				waitingForVideoFinish = true;
			}
			break;
		case transitionState.RestartingWithVideo:
			if ((playPosition != -1.0) & waitingForVideoFinish)
			{
				waitingForVideoFinish = false;
			}
			else if ((playPosition == -1.0) & !waitingForVideoFinish)
			{
				pendingVideo = "PhoneSidewaysFadeOnA";
				currentState = transitionState.Initialize;
			}
			break;
		case transitionState.OrderCorrect:
			isPuzzleFinishedCorrect = true;
			currentState = transitionState.Stopping;
			break;
		case transitionState.Stopping:
			currentState = transitionState.Stopped;
			break;
		case transitionState.Stopped:
			isActive = false;
			currentState = transitionState.Stopped;
			break;
		case transitionState.Processing:
		case transitionState.CheckingFailed:
		case transitionState.CheckPassed:
			break;
		}
	}

	private void checkOrder(double playPosition)
	{
		delayCount++;
		if (playPosition > 1000.0)
		{
			if (checkIfObjectInRightOrder(1))
			{
				addGraphicOrderingChecking();
			}
			else
			{
				addGraphicOrderingChecking();
			}
		}
		if (playPosition > 2750.0)
		{
			if (checkIfObjectInRightOrder(2))
			{
				addGraphicOrderingChecking();
			}
			else
			{
				addGraphicOrderingChecking();
			}
		}
		if (playPosition > 4125.0)
		{
			if (checkIfObjectInRightOrder(3))
			{
				addGraphicOrderingChecking();
			}
			else
			{
				addGraphicOrderingChecking();
			}
		}
		if (!((playPosition == -1.0) & (delayCount > 100)))
		{
			return;
		}
		delayCount = 0;
		if (checkIfObjectInRightOrder(1) & checkIfObjectInRightOrder(2) & checkIfObjectInRightOrder(3))
		{
			currentPuzzle.isFinishedOrder = true;
			currentState = transitionState.OrderCorrect;
			checkOrderPassDisplay();
			return;
		}
		currentPuzzle.isFinishedOrder = false;
		currentState = transitionState.OrderFailed;
		checkOrderFailedDisplay();
		if ((currentPuzzle.puzzleId == 1) | (currentPuzzle.retries > 0))
		{
			pendingVideo = "PhoneTurnedSelectSidewaysFlash";
			currentState = transitionState.Waiting;
			if (currentPuzzle.retries > 0)
			{
				currentPuzzle.retries--;
			}
		}
	}

	private bool checkIfObjectInRightOrder(int order)
	{
		foreach (PuzzleDataControl.objectData @object in currentPuzzle.objectList)
		{
			if (@object.order == order)
			{
				if (@object.order == @object.correctOrder)
				{
					@object.isOrderCorrect = true;
					@object.isOrderWrong = false;
					return true;
				}
				@object.isOrderWrong = true;
				@object.wrongOrderOrder = @object.order;
				return false;
			}
		}
		return false;
	}

	private void replaceValueInObjectList(List<PuzzleDataControl.objectData> l, int findValue, int replaceValue)
	{
		foreach (PuzzleDataControl.objectData item in l)
		{
			if (item.order == findValue)
			{
				item.order = replaceValue;
				break;
			}
		}
	}

	private bool performMoveOrder(GamePadControl.direction direction)
	{
		if ((direction == GamePadControl.direction.N) & (currentSelection == 1))
		{
			return false;
		}
		if ((direction == GamePadControl.direction.S) & (currentSelection == 3))
		{
			return false;
		}
		switch (direction)
		{
		case GamePadControl.direction.N:
			if (currentSelection == 2)
			{
				replaceValueInObjectList(currentPuzzle.objectList, 2, -1);
				replaceValueInObjectList(currentPuzzle.objectList, 1, 2);
				replaceValueInObjectList(currentPuzzle.objectList, -1, 1);
				currentSelection = 1;
			}
			else if (currentSelection == 3)
			{
				replaceValueInObjectList(currentPuzzle.objectList, 3, -1);
				replaceValueInObjectList(currentPuzzle.objectList, 2, 3);
				replaceValueInObjectList(currentPuzzle.objectList, -1, 2);
				currentSelection = 2;
			}
			break;
		case GamePadControl.direction.S:
			if (currentSelection == 1)
			{
				replaceValueInObjectList(currentPuzzle.objectList, 1, -1);
				replaceValueInObjectList(currentPuzzle.objectList, 2, 1);
				replaceValueInObjectList(currentPuzzle.objectList, -1, 2);
				currentSelection = 2;
			}
			else if (currentSelection == 2)
			{
				replaceValueInObjectList(currentPuzzle.objectList, 2, -1);
				replaceValueInObjectList(currentPuzzle.objectList, 3, 2);
				replaceValueInObjectList(currentPuzzle.objectList, -1, 3);
				currentSelection = 3;
			}
			break;
		}
		return true;
	}

	public void updateSelectObject(GameTime gameTime, GamePadControl myGamePad, List<string> playSimpleSFX)
	{
		switch (currentState)
		{
		case transitionState.Initialize:
			isActive = true;
			displayText.Clear();
			displayList.Clear();
			currentSelection = 1;
			currentState = transitionState.Starting;
			break;
		case transitionState.Starting:
			displayText.Clear();
			addObjectText();
			currentState = transitionState.Started;
			break;
		case transitionState.Started:
			addGraphic(aPressed: false, playSimpleSFX);
			updateSelectionCount();
			currentState = transitionState.Waiting;
			break;
		case transitionState.Waiting:
			if (myGamePad.anyDirection != GamePadControl.direction.NotSet)
			{
				joyProgressA += gameTime.ElapsedGameTime.TotalMilliseconds / joyTimeSpan.TotalMilliseconds;
				if (joyProgressA > 1.0 && moveForSelection(myGamePad.anyDirection))
				{
					playSimpleSFX.Add("ScrollE");
					joyProgressA = 0.0;
					addGraphic(aPressed: false, playSimpleSFX);
					updateSelectionCount();
					if (isSelectionComplete)
					{
						addTriggerGraphic();
					}
				}
			}
			if (myGamePad.padAPressed)
			{
				playSimpleSFX.Add("retro_click");
				addGraphic(aPressed: true, playSimpleSFX);
				updateSelectionCount();
				if (isSelectionComplete)
				{
					addTriggerGraphic();
				}
			}
			else if ((myGamePad.rightTrigger > 0.1f) & isSelectionComplete)
			{
				playSimpleSFX.Add("Arcade Action 05");
				currentSelection = -1;
				updateSelectionCount();
				currentState = transitionState.Processing;
				delayCount = 0;
				displayList.Clear();
			}
			break;
		case transitionState.Processing:
			addGraphicProcessing();
			if (fadeOutTextButSelected())
			{
				currentState = transitionState.Checking;
			}
			break;
		case transitionState.Checking:
			isPuzzleFinishedCorrect = false;
			checking();
			break;
		case transitionState.CheckingFailed:
			if (!myGamePad.padAPressed)
			{
				break;
			}
			if ((currentPuzzle.puzzleId == 1) | (currentPuzzle.retries > 0))
			{
				pendingVideo = "PhoneTurnedSelectSidewaysFlash";
				currentState = transitionState.Initialize;
				if (currentPuzzle.retries > 0)
				{
					currentPuzzle.retries--;
				}
			}
			else
			{
				currentState = transitionState.Stopping;
			}
			break;
		case transitionState.CheckPassed:
			if (myGamePad.padAPressed)
			{
				baseStateType = baseState.OrderObjects;
				currentState = transitionState.Initialize;
			}
			break;
		case transitionState.Stopping:
			currentState = transitionState.Stopped;
			break;
		case transitionState.Stopped:
			isActive = false;
			currentState = transitionState.Stopped;
			break;
		case transitionState.OrderCorrect:
		case transitionState.OrderFailed:
		case transitionState.RestartingWithVideo:
			break;
		}
	}

	public int getRandom(int max)
	{
		return myRandom.Next(max) + 1;
	}

	private void checkOrderPassDisplay()
	{
	}

	private void checkOrderFailedDisplay()
	{
		if (currentPuzzle.puzzleId == 1)
		{
			removeSpecialText();
			t = new DisplayText();
			Color white = Color.White;
			string text = "(Sorry - You have the order incorrect.  Since you are still getting used to this - please try again.  You can view the video again by pressing (X) button)";
			t.addTextRaw(DisplayText.GroupTextType.Header, text, white, new Vector2(300f, 400f), isTypedAnimated: false, myCoreDisplayElements.myPDAFontHeader, 700);
			displayText.Add(t);
		}
		else if (currentPuzzle.retries > 0)
		{
			removeSpecialText();
			t = new DisplayText();
			Color white2 = Color.White;
			string text2 = "(Sorry - You have the order incorrect.  Your exercise has helped keep your mind focused - please try again.  Please use your joysticks to re-order items.)";
			t.addTextRaw(DisplayText.GroupTextType.Header, text2, white2, new Vector2(300f, 400f), isTypedAnimated: false, myCoreDisplayElements.myPDAFontHeader, 700);
			displayText.Add(t);
		}
		else
		{
			removeSpecialText();
			t = new DisplayText();
			Color white3 = Color.White;
			string text3 = "(Sorry - You do not have these items in the correct order.  You will need to spend some time and try that again.)";
			t.addTextRaw(DisplayText.GroupTextType.Header, text3, white3, new Vector2(300f, 400f), isTypedAnimated: false, myCoreDisplayElements.myPDAFontHeader, 700);
			displayText.Add(t);
			addPressAButton();
			t = new DisplayText();
			t.addTextRaw(DisplayText.GroupTextType.TextBoxSpecial, "Continue:", Color.White, new Vector2(990f, 600f), isTypedAnimated: false, myCoreDisplayElements.myPDAFontRegular, 700);
			displayText.Add(t);
		}
	}

	private void checkPassDisplay()
	{
		removeSpecialText();
		t = new DisplayText();
		Color white = Color.White;
		string text = "(Good job.   You have selected the correct items that are key to solving this action item.)";
		t.addTextRaw(DisplayText.GroupTextType.Header, text, white, new Vector2(300f, 400f), isTypedAnimated: false, myCoreDisplayElements.myPDAFontHeader, 700);
		displayText.Add(t);
		addPressAButton();
		t = new DisplayText();
		t.addTextRaw(DisplayText.GroupTextType.TextBoxSpecial, "Continue:", Color.White, new Vector2(990f, 600f), isTypedAnimated: false, myCoreDisplayElements.myPDAFontRegular, 700);
		displayText.Add(t);
	}

	private void checkFailedDisplay()
	{
		if (currentPuzzle.puzzleId == 1)
		{
			removeSpecialText();
			t = new DisplayText();
			Color white = Color.White;
			string text = "(Sorry - You did not select the right items.  Normally, you would need to start again but this is still practice.   Please try again.)";
			t.addTextRaw(DisplayText.GroupTextType.Header, text, white, new Vector2(300f, 400f), isTypedAnimated: false, myCoreDisplayElements.myPDAFontHeader, 700);
			displayText.Add(t);
			addPressAButton();
			t = new DisplayText();
			t.addTextRaw(DisplayText.GroupTextType.TextBoxSpecial, "Continue:", Color.White, new Vector2(990f, 600f), isTypedAnimated: false, myCoreDisplayElements.myPDAFontRegular, 700);
			displayText.Add(t);
		}
		else if (currentPuzzle.retries > 0)
		{
			removeSpecialText();
			t = new DisplayText();
			Color white2 = Color.White;
			string text2 = "(Sorry - You did not select the right items.  Since you used the yoga exercise, you can keep focus to try again.)";
			t.addTextRaw(DisplayText.GroupTextType.Header, text2, white2, new Vector2(300f, 400f), isTypedAnimated: false, myCoreDisplayElements.myPDAFontHeader, 700);
			displayText.Add(t);
			addPressAButton();
			t = new DisplayText();
			t.addTextRaw(DisplayText.GroupTextType.TextBoxSpecial, "Continue:", Color.White, new Vector2(990f, 600f), isTypedAnimated: false, myCoreDisplayElements.myPDAFontRegular, 700);
			displayText.Add(t);
		}
		else
		{
			removeSpecialText();
			t = new DisplayText();
			Color white3 = Color.White;
			string text3 = "(Sorry - You did not select the right items.  You will need to spend some time and try that again.)";
			t.addTextRaw(DisplayText.GroupTextType.Header, text3, white3, new Vector2(300f, 400f), isTypedAnimated: false, myCoreDisplayElements.myPDAFontHeader, 700);
			displayText.Add(t);
			addPressAButton();
			t = new DisplayText();
			t.addTextRaw(DisplayText.GroupTextType.TextBoxSpecial, "Continue:", Color.White, new Vector2(990f, 600f), isTypedAnimated: false, myCoreDisplayElements.myPDAFontRegular, 700);
			displayText.Add(t);
		}
	}

	private bool checkIfObjectSelectionCorrectAndMarkIt(int orderId)
	{
		foreach (PuzzleDataControl.objectData @object in currentPuzzle.objectList)
		{
			if (@object.order == currentSelection)
			{
				if (@object.correctOrder != -1)
				{
					@object.hasBeenMarkedCorrect = true;
					return true;
				}
				@object.hasBeenMarkedInCorrect = true;
				return false;
			}
		}
		return false;
	}

	private double getTimeFromFrames(int frames)
	{
		return (float)frames / 24f * 1000f;
	}

	private bool checking()
	{
		if (currentSelection == -1)
		{
			currentSelection = getRandomSelection();
			correctItemCorrect = checkIfObjectSelectionCorrectAndMarkIt(currentSelection);
			if (correctItemCorrect)
			{
				if (lastVideo == "PuzzleCorrect")
				{
					pendingVideo = "PuzzleCorrectB";
				}
				else
				{
					pendingVideo = "PuzzleCorrect";
				}
				lastVideo = pendingVideo;
			}
			else
			{
				pendingVideo = "PuzzleWrong";
			}
			playPosition = 0.0;
		}
		else if (waitingForVideoToEnd & (playPosition == -1.0))
		{
			waitingForVideoToEnd = false;
			if (correctItemCorrect)
			{
				if (checkIfAllItemsSelected())
				{
					currentPuzzle.isCorrectSelected = true;
					currentState = transitionState.CheckPassed;
					checkPassDisplay();
				}
				currentSelection = -1;
			}
			else
			{
				currentPuzzle.isCorrectSelected = false;
				checkFailedDisplay();
				currentState = transitionState.CheckingFailed;
			}
		}
		else if ((playPosition > 1000.0) & (pendingVideo == ""))
		{
			waitingForVideoToEnd = true;
			if (correctItemCorrect)
			{
				updateGraphicsChecking();
			}
			else
			{
				updateGraphicsChecking();
			}
		}
		else if ((playPosition == -1.0) & (pendingVideo == ""))
		{
			waitingForVideoToEnd = true;
			if (correctItemCorrect)
			{
				updateGraphicsChecking();
			}
			else
			{
				updateGraphicsChecking();
			}
		}
		return true;
	}

	private bool checkIfAllItemsSelected()
	{
		foreach (PuzzleDataControl.objectData @object in currentPuzzle.objectList)
		{
			if ((@object.correctOrder != -1) & !@object.hasBeenMarkedCorrect)
			{
				return false;
			}
		}
		return true;
	}

	private void updateGraphicsChecking()
	{
		foreach (DisplayData display in displayList)
		{
			display.isDisplayed = false;
		}
		foreach (PuzzleDataControl.objectData @object in currentPuzzle.objectList)
		{
			if (@object.hasBeenMarkedCorrect)
			{
				addInitialGraphics(@object.order, "Correct", getButtonPosition(@object.order), new Vector2(167.5f, 25.5f), 0f, 1f, 0.3f, startHalfTransparent: true);
			}
			else if (@object.hasBeenMarkedInCorrect)
			{
				addInitialGraphics(@object.order, "Wrong", getButtonPosition(@object.order), new Vector2(167.5f, 25.5f), 0f, 1f, 0.3f, startHalfTransparent: true);
			}
			else if (@object.isUserSelected)
			{
				addInitialGraphics(@object.order, "Selected", getButtonPosition(@object.order), new Vector2(167.5f, 25.5f), 0f, 1f, 0.3f, startHalfTransparent: true);
			}
		}
	}

	private int getRandomSelection()
	{
		int random = getRandom(12);
		foreach (PuzzleDataControl.objectData @object in currentPuzzle.objectList)
		{
			if ((@object.order >= random) & @object.isUserSelected & !@object.hasBeenMarkedCorrect)
			{
				return @object.order;
			}
		}
		foreach (PuzzleDataControl.objectData object2 in currentPuzzle.objectList)
		{
			if (object2.isUserSelected & !object2.hasBeenMarkedCorrect)
			{
				return object2.order;
			}
		}
		return -1;
	}

	private bool fadeOutTextButSelected()
	{
		bool result = true;
		foreach (DisplayText item in displayText)
		{
			if (item.groupType == DisplayText.GroupTextType.Header)
			{
				if (!isSelected(item.spriteTextRaw))
				{
					if (item.color.R > 20)
					{
						result = false;
						item.color = new Color(item.color.R - 3, item.color.G - 3, item.color.B - 3, item.color.A - 3);
					}
					else
					{
						item.color = new Color(0, 0, 0, 0);
					}
				}
			}
			else if (item.color.R > 20)
			{
				result = false;
				item.color = new Color(item.color.R - 10, item.color.G - 10, item.color.B - 10, item.color.A - 10);
			}
			else
			{
				item.color = new Color(0, 0, 0, 0);
			}
		}
		return result;
	}

	private bool isSelected(string name)
	{
		foreach (PuzzleDataControl.objectData @object in currentPuzzle.objectList)
		{
			if (@object.name == name)
			{
				if (@object.isUserSelected)
				{
					return true;
				}
				if (@object.hasBeenMarkedCorrect)
				{
					return true;
				}
				if (@object.hasBeenMarkedInCorrect)
				{
					return true;
				}
			}
		}
		return false;
	}

	private void addTriggerOrderGraphic()
	{
		t = new DisplayText();
		t.addTextRaw(DisplayText.GroupTextType.TextBoxSpecial, "Press Right Trigger to process selection:", Color.White, new Vector2(360f, 500f), isTypedAnimated: false, myCoreDisplayElements.myPDAFontRegular, 700);
		displayText.Add(t);
		d = new DisplayData();
		d.objectId = 2;
		d.baseImageName = "RightTrigger";
		d._textureName = "PDA";
		d.position = new Vector2(830f, 470f);
		d.origin = new Vector2(0f, 0f);
		d.isDisplayed = true;
		d.depth = 0.71f;
		d.scale = 1f;
		d.objectId = 113;
		d.rotation = 0f;
		d.myColor = Color.White;
		addGraphic(d);
	}

	private void addTriggerGraphic()
	{
		t = new DisplayText();
		t.addTextRaw(DisplayText.GroupTextType.TextBoxSpecial, "Press Right Trigger to process selection:", Color.White, new Vector2(360f, 500f), isTypedAnimated: false, myCoreDisplayElements.myPDAFontRegular, 700);
		displayText.Add(t);
		d = new DisplayData();
		d.objectId = 2;
		d.baseImageName = "RightTrigger";
		d._textureName = "PDA";
		d.position = new Vector2(830f, 470f);
		d.origin = new Vector2(0f, 0f);
		d.isDisplayed = true;
		d.depth = 0.71f;
		d.scale = 1f;
		d.objectId = 113;
		d.rotation = 0f;
		d.myColor = Color.White;
		addGraphic(d);
	}

	private bool moveForOrder(GamePadControl.direction d)
	{
		bool result = false;
		switch (d)
		{
		case GamePadControl.direction.N:
			result = true;
			currentSelection--;
			if (currentSelection < 1)
			{
				currentSelection = 1;
				return false;
			}
			break;
		case GamePadControl.direction.S:
			result = true;
			currentSelection++;
			if (currentSelection > 3)
			{
				currentSelection = 3;
				return false;
			}
			break;
		}
		return result;
	}

	private bool moveForSelection(GamePadControl.direction d)
	{
		bool result = false;
		switch (d)
		{
		case GamePadControl.direction.N:
			result = true;
			currentSelection--;
			if (currentSelection < 1)
			{
				currentSelection = 12;
			}
			break;
		case GamePadControl.direction.S:
			result = true;
			currentSelection++;
			if (currentSelection > 12)
			{
				currentSelection = 1;
			}
			break;
		case GamePadControl.direction.E:
			result = true;
			currentSelection += 4;
			if (currentSelection > 12)
			{
				currentSelection -= 12;
			}
			break;
		case GamePadControl.direction.W:
			result = true;
			currentSelection -= 4;
			if (currentSelection == -8)
			{
				currentSelection = 12;
			}
			else if (currentSelection == -1)
			{
				currentSelection = 11;
			}
			else if (currentSelection == -2)
			{
				currentSelection = 10;
			}
			else if (currentSelection == -3)
			{
				currentSelection = 9;
			}
			else if (currentSelection < 1)
			{
				currentSelection = 12;
			}
			break;
		}
		return result;
	}

	private void addGraphicOrdering(bool aPressed)
	{
		foreach (DisplayData display in displayList)
		{
			display.isDisplayed = false;
		}
		removeSpecialText();
		foreach (PuzzleDataControl.objectData @object in currentPuzzle.objectList)
		{
			if ((@object.order == currentSelection) & @object.hasBeenMarkedCorrect)
			{
				if (@object.isOrderWrong & (@object.order == @object.wrongOrderOrder))
				{
					addInitialGraphics(@object.order, "Wrong", getButtonPositionOrder(@object.order), new Vector2(167.5f, 25.5f), 0f, 1f, 0.3f, startHalfTransparent: false);
				}
				else if (@object.isOrderCorrect & (@object.order == @object.correctOrder))
				{
					addInitialGraphics(@object.order, "Correct", getButtonPositionOrder(@object.order), new Vector2(167.5f, 25.5f), 0f, 1f, 0.3f, startHalfTransparent: false);
				}
				else if (@object.isOrderCorrect & (@object.order != @object.correctOrder))
				{
					addInitialGraphics(@object.order, "Wrong", getButtonPositionOrder(@object.order), new Vector2(167.5f, 25.5f), 0f, 1f, 0.3f, startHalfTransparent: false);
				}
				else
				{
					addInitialGraphics(@object.order, "Select", getButtonPositionOrder(currentSelection), new Vector2(167.5f, 25.5f), 0f, 1f, 0.3f, startHalfTransparent: false);
				}
			}
			else if (@object.hasBeenMarkedCorrect)
			{
				if (@object.isOrderWrong & (@object.order == @object.wrongOrderOrder))
				{
					addInitialGraphics(@object.order, "Wrong", getButtonPositionOrder(@object.order), new Vector2(167.5f, 25.5f), 0f, 1f, 0.3f, startHalfTransparent: true);
				}
				else if (@object.isOrderCorrect & (@object.order == @object.correctOrder))
				{
					addInitialGraphics(@object.order, "Correct", getButtonPositionOrder(@object.order), new Vector2(167.5f, 25.5f), 0f, 1f, 0.3f, startHalfTransparent: true);
				}
				else if (@object.isOrderCorrect & (@object.order != @object.correctOrder))
				{
					addInitialGraphics(@object.order, "Wrong", getButtonPositionOrder(@object.order), new Vector2(167.5f, 25.5f), 0f, 1f, 0.3f, startHalfTransparent: true);
				}
				else
				{
					addInitialGraphics(@object.order, "Selected", getButtonPositionOrder(@object.order), new Vector2(167.5f, 25.5f), 0f, 1f, 0.3f, startHalfTransparent: true);
				}
			}
		}
		d = new DisplayData();
		d.objectId = 2;
		d.baseImageName = "JoyLOrder";
		d._textureName = "PDA";
		d.position = new Vector2(930f, 270f);
		d.origin = new Vector2(0f, 0f);
		d.isDisplayed = true;
		d.depth = 0.71f;
		d.scale = 1f;
		d.objectId = 116;
		d.rotation = 0f;
		d.myColor = Color.White;
		addGraphic(d);
		d = new DisplayData();
		d.objectId = 2;
		d.baseImageName = "JoyRSelect";
		d._textureName = "PDA";
		d.position = new Vector2(230f, 270f);
		d.origin = new Vector2(0f, 0f);
		d.isDisplayed = true;
		d.depth = 0.71f;
		d.scale = 1f;
		d.objectId = 117;
		d.rotation = 0f;
		d.myColor = Color.White;
		addGraphic(d);
	}

	private void addGraphicOrderingChecking()
	{
		foreach (DisplayData display in displayList)
		{
			display.isDisplayed = false;
		}
		removeSpecialText();
		foreach (PuzzleDataControl.objectData @object in currentPuzzle.objectList)
		{
			if (@object.hasBeenMarkedCorrect)
			{
				if (@object.isOrderCorrect)
				{
					addInitialGraphics(@object.order, "Correct", getButtonPositionOrder(@object.order), new Vector2(167.5f, 25.5f), 0f, 1f, 0.3f, startHalfTransparent: false);
				}
				else if (@object.isOrderWrong)
				{
					addInitialGraphics(@object.order, "Wrong", getButtonPositionOrder(@object.order), new Vector2(167.5f, 25.5f), 0f, 1f, 0.3f, startHalfTransparent: false);
				}
				else
				{
					addInitialGraphics(@object.order, "Selected", getButtonPositionOrder(@object.order), new Vector2(167.5f, 25.5f), 0f, 1f, 0.3f, startHalfTransparent: false);
				}
			}
		}
	}

	private void addGraphic(bool aPressed, List<string> playSimpleSFX)
	{
		foreach (DisplayData display in displayList)
		{
			display.isDisplayed = false;
		}
		removeSpecialText();
		foreach (PuzzleDataControl.objectData @object in currentPuzzle.objectList)
		{
			if (@object.order == currentSelection)
			{
				if (aPressed & !@object.hasBeenMarkedCorrect & !@object.hasBeenMarkedInCorrect)
				{
					if (@object.isUserSelected)
					{
						@object.isUserSelected = false;
					}
					else if (!isSelectionComplete)
					{
						@object.isUserSelected = true;
					}
					else
					{
						playSimpleSFX.Add("UI_Misc16");
					}
				}
				if (@object.hasBeenMarkedCorrect)
				{
					addInitialGraphics(@object.order, "Correct", getButtonPosition(@object.order), new Vector2(167.5f, 25.5f), 0f, 1f, 0.3f, startHalfTransparent: false);
				}
				else if (@object.hasBeenMarkedInCorrect)
				{
					addInitialGraphics(@object.order, "Wrong", getButtonPosition(@object.order), new Vector2(167.5f, 25.5f), 0f, 1f, 0.3f, startHalfTransparent: false);
				}
				if (@object.isUserSelected & !@object.hasBeenMarkedCorrect)
				{
					t = new DisplayText();
					t.addTextRaw(DisplayText.GroupTextType.TextBoxSpecial, "Remove:", Color.White, new Vector2(1010f, 600f), isTypedAnimated: false, myCoreDisplayElements.myPDAFontRegular, 700);
					displayText.Add(t);
					addPressAButton();
					addInitialGraphics(@object.order, "Selected", getButtonPosition(currentSelection), new Vector2(167.5f, 25.5f), 0f, 1f, 0.3f, startHalfTransparent: false);
				}
				else if (!@object.hasBeenMarkedInCorrect & !@object.hasBeenMarkedCorrect)
				{
					t = new DisplayText();
					t.addTextRaw(DisplayText.GroupTextType.TextBoxSpecial, "Select:", Color.White, new Vector2(1010f, 600f), isTypedAnimated: false, myCoreDisplayElements.myPDAFontRegular, 700);
					displayText.Add(t);
					addPressAButton();
					addInitialGraphics(@object.order, "Select", getButtonPosition(currentSelection), new Vector2(167.5f, 25.5f), 0f, 1f, 0.3f, startHalfTransparent: false);
				}
			}
			else if (@object.hasBeenMarkedCorrect)
			{
				addInitialGraphics(@object.order, "Correct", getButtonPosition(@object.order), new Vector2(167.5f, 25.5f), 0f, 1f, 0.3f, startHalfTransparent: true);
				@object.isUserSelected = true;
			}
			else if (@object.hasBeenMarkedInCorrect)
			{
				addInitialGraphics(@object.order, "Wrong", getButtonPosition(@object.order), new Vector2(167.5f, 25.5f), 0f, 1f, 0.3f, startHalfTransparent: true);
				@object.isUserSelected = false;
			}
			else if (@object.isUserSelected)
			{
				addInitialGraphics(@object.order, "Selected", getButtonPosition(@object.order), new Vector2(167.5f, 25.5f), 0f, 1f, 0.3f, startHalfTransparent: true);
			}
			else
			{
				addInitialGraphics(@object.order, "Empty", getButtonPosition(@object.order), new Vector2(167.5f, 25.5f), 0f, 1f, 0.3f, startHalfTransparent: false);
			}
		}
	}

	private void addGraphicProcessing()
	{
		foreach (DisplayData display in displayList)
		{
			display.isDisplayed = false;
		}
		foreach (PuzzleDataControl.objectData @object in currentPuzzle.objectList)
		{
			if (@object.isUserSelected)
			{
				addInitialGraphics(@object.order, "Selected", getButtonPosition(@object.order), new Vector2(167.5f, 25.5f), 0f, 1f, 0.3f, startHalfTransparent: true);
			}
			else if (@object.hasBeenMarkedInCorrect)
			{
				addInitialGraphics(@object.order, "Wrong", getButtonPosition(@object.order), new Vector2(167.5f, 25.5f), 0f, 1f, 0.3f, startHalfTransparent: true);
			}
			else if (@object.hasBeenMarkedCorrect)
			{
				addInitialGraphics(@object.order, "Correct", getButtonPosition(@object.order), new Vector2(167.5f, 25.5f), 0f, 1f, 0.3f, startHalfTransparent: true);
			}
		}
	}

	private void removeSpecialText()
	{
		bool flag = true;
		while (flag)
		{
			flag = false;
			foreach (DisplayText item in displayText)
			{
				if (item.groupType == DisplayText.GroupTextType.TextBoxSpecial)
				{
					flag = true;
					displayText.Remove(item);
					break;
				}
			}
		}
	}

	private void addPressAButton()
	{
		d = new DisplayData();
		d.baseImageName = "ButtonA";
		d._textureName = "PDA";
		d.position = new Vector2(1100f, 600f);
		d.origin = new Vector2(0f, 0f);
		d.isDisplayed = true;
		d.depth = 0.71f;
		d.scale = 1f;
		d.objectId = 112;
		d.rotation = 0f;
		d.myColor = Color.White;
		addGraphic(d);
	}

	private Vector2 getButtonPosition(int s)
	{
		if (s <= 4)
		{
			return new Vector2(307.5f, 149.5f + (float)(50 * (s - 1)));
		}
		if (s <= 8)
		{
			return new Vector2(642.5f, 149.5f + (float)(50 * (s - 5)));
		}
		return new Vector2(977.5f, 149.5f + (float)(50 * (s - 9)));
	}

	private Vector2 getButtonPositionOrder(int s)
	{
		return new Vector2(642.5f, 149.5f + (float)(50 * (s - 1)));
	}

	private void addInitialGraphics(int id, string baseName, Vector2 position, Vector2 origin, float rotation, float scale, float depth, bool startHalfTransparent)
	{
		d = new DisplayData();
		d.baseImageName = baseName;
		d._textureName = "PDA";
		d.position = position;
		d.isDisplayed = true;
		d.depth = depth;
		d.origin = origin;
		if (startHalfTransparent)
		{
			d.myColor = new Color(100, 100, 100, 100);
		}
		else
		{
			d.myColor = Color.White;
		}
		d.objectId = id;
		addGraphic(d);
	}

	private void addGraphic(DisplayData newD)
	{
		bool flag = false;
		foreach (DisplayData display in displayList)
		{
			if (display.objectId == newD.objectId)
			{
				display.isDisplayed = true;
				display.myColor = newD.myColor;
				display.position = newD.position;
				display.rotation = newD.rotation;
				display.scale = newD.scale;
				display.baseImageName = newD.baseImageName;
				flag = true;
			}
		}
		if (!flag)
		{
			newD.isDisplayed = true;
			displayList.Add(newD);
		}
	}

	private void updateSelectionCount()
	{
		bool flag = false;
		Color white = Color.White;
		foreach (DisplayText item in displayText)
		{
			if (item.groupType == DisplayText.GroupTextType.Regular)
			{
				flag = true;
				item.addTextRaw(DisplayText.GroupTextType.Regular, getPuzzleSelectionString() + " Objects Selected", white, new Vector2(495f, 340f), isTypedAnimated: false, myCoreDisplayElements.myPDAFontHeader, 400);
				break;
			}
		}
		if (!flag)
		{
			t = new DisplayText();
			t.addTextRaw(DisplayText.GroupTextType.Regular, getPuzzleSelectionString() + " Objects Selected", white, new Vector2(495f, 340f), isTypedAnimated: false, myCoreDisplayElements.myPDAFontHeader, 400);
			displayText.Add(t);
		}
	}

	private string getPuzzleSelectionString()
	{
		int num = 0;
		int num2 = 0;
		foreach (PuzzleDataControl.objectData @object in currentPuzzle.objectList)
		{
			if (@object.isUserSelected)
			{
				num++;
			}
			if (@object.correctOrder != -1)
			{
				num2++;
			}
		}
		if (num == num2 && num2 != 0)
		{
			isSelectionComplete = true;
		}
		else
		{
			isSelectionComplete = false;
		}
		return "(" + num + "/" + num2 + ")";
	}

	private void addOrderObjectText(bool showInstructions)
	{
		int i = 1;
		int num = 1;
		float x = 495f;
		float num2 = 130f;
		for (; i <= 12; i++)
		{
			foreach (PuzzleDataControl.objectData @object in currentPuzzle.objectList)
			{
				if ((@object.order == i) & @object.hasBeenMarkedCorrect)
				{
					t = new DisplayText();
					Color white = Color.White;
					t.addTextRaw(DisplayText.GroupTextType.Header, @object.name, white, new Vector2(x, num2), isTypedAnimated: false, myCoreDisplayElements.myPDAFontHeader, 300);
					displayText.Add(t);
					num2 += 50f;
					@object.order = num;
					num++;
					break;
				}
			}
		}
		if (showInstructions)
		{
			t = new DisplayText();
			Color white = Color.White;
			string text = "(Final step - You must now put the items in the correct chronological order as seen in the video.  Use the left joystick to select and the right joystick to move.)";
			t.addTextRaw(DisplayText.GroupTextType.Header, text, white, new Vector2(300f, 400f), isTypedAnimated: false, myCoreDisplayElements.myPDAFontHeader, 700);
			displayText.Add(t);
		}
	}

	private void addObjectText()
	{
		int num = 1;
		float x = 155f;
		float num2 = 130f;
		Color white;
		while (num <= 12)
		{
			foreach (PuzzleDataControl.objectData @object in currentPuzzle.objectList)
			{
				if (@object.order == num)
				{
					t = new DisplayText();
					white = Color.White;
					t.addTextRaw(DisplayText.GroupTextType.Header, @object.name, white, new Vector2(x, num2), isTypedAnimated: false, myCoreDisplayElements.myPDAFontHeader, 300);
					displayText.Add(t);
				}
			}
			num++;
			num2 += 50f;
			switch (num)
			{
			case 5:
				x = 495f;
				num2 = 130f;
				break;
			case 9:
				x = 830f;
				num2 = 130f;
				break;
			}
		}
		t = new DisplayText();
		white = Color.White;
		string text = "(Please select the 3 items you saw in the video.  These are the key items that will help you solve this action.)";
		t.addTextRaw(DisplayText.GroupTextType.Header, text, white, new Vector2(300f, 400f), isTypedAnimated: false, myCoreDisplayElements.myPDAFontHeader, 700);
		displayText.Add(t);
	}

	public void draw(SpriteBatch spriteBatch)
	{
		if (!isActive)
		{
			return;
		}
		foreach (DisplayData display in displayList)
		{
			if (display.isDisplayed)
			{
				spriteBatch.Draw(myCoreDisplayElements.getTexture(display.textureName), display.position, myCoreDisplayElements.spriteRDM.getSpriteRectangle(display.textureName, display.baseImageName, display.currentFrame), display.myColor, display.rotation, display.origin, display.scale, SpriteEffects.None, display.depth);
			}
		}
		foreach (DisplayText item in displayText)
		{
			tempPosition = item.position;
			spriteBatch.DrawString(item.myFont, item.getText(), item.position, item.color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.994f);
			tempPosition.X += 2f;
			tempPosition.Y += 2f;
			spriteBatch.DrawString(item.myFont, item.getText(), tempPosition, new Color(0, 0, 0, item.color.A), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.992f);
			tempPosition.X -= 3f;
			tempPosition.Y -= 3f;
			if (!item.isFinishedDrawing)
			{
				break;
			}
		}
	}
}
