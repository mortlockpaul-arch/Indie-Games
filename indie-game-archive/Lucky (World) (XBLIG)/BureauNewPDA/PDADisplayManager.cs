using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BureauNewPDA;

internal class PDADisplayManager
{
	public enum PDAStatusDisplay
	{
		starting,
		caseMenu,
		caseResults
	}

	private enum statusEnum
	{
		Initialize,
		Starting,
		Started,
		Waiting,
		Stopping,
		Stopped,
		Returning
	}

	public PDAStatusDisplay PDAStatus;

	private statusEnum status;

	private List<DisplayData> displayList = new List<DisplayData>();

	private List<DisplayText> displayText = new List<DisplayText>();

	public bool isReturnResults;

	private Color fadeColor = new Color(0, 0, 0, 0);

	private byte fadeColorBase1;

	private byte fadeColorBase2;

	private byte fadeColorBase3;

	private double progressA;

	private TimeSpan messageTimeSpan = TimeSpan.FromMilliseconds(600.0);

	private double fadeProgressA;

	private TimeSpan fadeTimeSpan = TimeSpan.FromMilliseconds(200.0);

	private double rotationProgressA;

	private TimeSpan rotationTimeSpan = TimeSpan.FromMilliseconds(1000.0);

	private double displayStateProgress;

	private TimeSpan displayStateTimeSpan = TimeSpan.FromMilliseconds(1000.0);

	private double fadeProgressB;

	private double fadeProgressC;

	private DisplayData d = new DisplayData();

	private int endCountForDisplay = -1;

	private int displayBoxOffset = 200;

	private bool hasStopped;

	private bool finished;

	private Vector2 basePosition = Vector2.Zero;

	private Vector2 tempPosition = Vector2.Zero;

	private float tempRotation;

	private float tempScale = 1f;

	private float tempScaleB = 0.7f;

	private int startingPhase = 1;

	private int pauseCounter;

	private int playCount;

	public void start(List<DisplayData> _displayList, List<DisplayText> displayText, CoreDisplayElements myCoreDisplayElements, int currentTime, PDATextSelectScreen PDATextScreen)
	{
		this.displayText = displayText;
		displayList = _displayList;
		fadeColorBase1 = 0;
		fadeColorBase2 = 0;
		fadeColorBase3 = 0;
		fadeProgressA = 0.0;
		fadeProgressB = 0.0;
		fadeProgressC = 0.0;
		progressA = 0.0;
		PDATextScreen.myCoreDisplayElements = myCoreDisplayElements;
		PDATextScreen.reset(currentTime);
		displayBoxOffset = 200;
	}

	public bool update(GameTime gameTime, GamePadControl myGamePad, int currentTime, List<string> playSimpleSound, PDATextSelectScreen PDATextScreen, VariableEngine vEngine, SaveData saveData)
	{
		hasStopped = false;
		switch (PDAStatus)
		{
		case PDAStatusDisplay.starting:
			updatePDATextScreen(gameTime, PDATextScreen, myGamePad, currentTime, playSimpleSound, PDATextScreen, vEngine, saveData);
			if (status == statusEnum.Stopped)
			{
				hasStopped = true;
				status = statusEnum.Returning;
			}
			break;
		case PDAStatusDisplay.caseResults:
			updatePDATextScreenReturn(gameTime, PDATextScreen, myGamePad, currentTime, playSimpleSound, PDATextScreen, saveData);
			if (PDAStatus == PDAStatusDisplay.starting)
			{
				return true;
			}
			break;
		}
		return hasStopped;
	}

	public bool updateLoadScreen(GameTime gameTime, GamePadControl myGamePad, int currentTime, List<string> playSimpleSound, PDATextSelectScreen PDATextScreen, VariableEngine vEngine, SaveData saveData)
	{
		hasStopped = false;
		if (PDAStatus == PDAStatusDisplay.starting)
		{
			updatePDATextScreenLoad(gameTime, PDATextScreen, myGamePad, currentTime, playSimpleSound, PDATextScreen, vEngine, saveData);
			if (status == statusEnum.Stopped)
			{
				hasStopped = true;
				status = statusEnum.Returning;
			}
		}
		return hasStopped;
	}

	private void fadeGraphicByID(int objId, int fadeId, double speedMilliseconds, GameTime gameTime, bool dontGenerate)
	{
		foreach (DisplayData display in displayList)
		{
			if (display.objectId == objId)
			{
				display.isDisplayed = true;
				display.myColor = incrementFade(fadeId, speedMilliseconds, gameTime, dontGenerate);
				break;
			}
		}
	}

	private Color incrementFade(int id, double speedMilliseconds, GameTime gameTime, bool dontGenerate)
	{
		fadeTimeSpan = TimeSpan.FromMilliseconds(speedMilliseconds);
		switch (id)
		{
		case 1:
			if ((fadeProgressA < 1.0) & !dontGenerate)
			{
				fadeProgressA += gameTime.ElapsedGameTime.TotalMilliseconds / fadeTimeSpan.TotalMilliseconds;
				if (fadeProgressA > 1.0)
				{
					fadeProgressA = 1.0;
					fadeColorBase1 = byte.MaxValue;
				}
				else
				{
					fadeColorBase1 = (byte)(255f * MathHelper.SmoothStep(0f, 1f, (float)fadeProgressA));
				}
			}
			if (fadeColorBase1 < byte.MaxValue)
			{
				return new Color(fadeColorBase1, fadeColorBase1, fadeColorBase1, fadeColorBase1);
			}
			return Color.White;
		case 2:
			if ((fadeProgressB < 1.0) & !dontGenerate)
			{
				fadeProgressB += gameTime.ElapsedGameTime.TotalMilliseconds / fadeTimeSpan.TotalMilliseconds;
				if (fadeProgressB > 1.0)
				{
					fadeProgressB = 1.0;
					fadeColorBase1 = byte.MaxValue;
				}
				else
				{
					fadeColorBase2 = (byte)(255f * MathHelper.SmoothStep(0f, 1f, (float)fadeProgressB));
				}
			}
			if (fadeColorBase2 < byte.MaxValue)
			{
				return new Color(fadeColorBase2, fadeColorBase2, fadeColorBase2, fadeColorBase2);
			}
			return Color.White;
		case 3:
			if ((fadeProgressC < 1.0) & !dontGenerate)
			{
				fadeProgressC += gameTime.ElapsedGameTime.TotalMilliseconds / fadeTimeSpan.TotalMilliseconds;
				if (fadeProgressC > 1.0)
				{
					fadeProgressC = 1.0;
					fadeColorBase3 = byte.MaxValue;
				}
				else
				{
					fadeColorBase3 = (byte)(255f * MathHelper.SmoothStep(0f, 1f, (float)fadeProgressC));
				}
			}
			if (fadeColorBase3 < byte.MaxValue)
			{
				return new Color(fadeColorBase3, fadeColorBase3, fadeColorBase3, fadeColorBase3);
			}
			return Color.White;
		default:
			return Color.White;
		}
	}

	private void addInitialGraphics(int id, string baseName, Vector2 position, Vector2 origin, float rotation, float scale, float depth, bool startOpacityZero)
	{
		d = new DisplayData();
		d.baseImageName = baseName;
		d._textureName = "PDA";
		d.position = position;
		d.isDisplayed = true;
		d.depth = depth;
		d.origin = origin;
		if (startOpacityZero)
		{
			d.myColor = new Color(0, 0, 0, 0);
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
				flag = true;
			}
		}
		if (!flag)
		{
			newD.isDisplayed = true;
			displayList.Add(newD);
		}
	}

	private void applyRotationScale(int id, float rotation, float scale)
	{
		foreach (DisplayData display in displayList)
		{
			if (display.objectId == id)
			{
				Vector2 vector = new Vector2(640f, 360f);
				Matrix matrix = Matrix.CreateRotationZ(rotation);
				matrix *= Matrix.CreateScale(scale);
				Vector2 vector2 = Vector2.Transform(display.position - vector, matrix);
				display.position = vector2 + vector;
				display.rotation = rotation;
				display.scale = scale;
				break;
			}
		}
	}

	private void addUpdateButtonVertical(int id, int position, string baseName, Color myColor, float scale, float rotation)
	{
		switch (position)
		{
		case 1:
			basePosition = new Vector2(565f, 200f);
			break;
		case 2:
			basePosition = new Vector2(705f, 200f);
			break;
		case 3:
			basePosition = new Vector2(565f, 370f);
			break;
		case 4:
			basePosition = new Vector2(705f, 370f);
			break;
		}
		d = new DisplayData();
		d.baseImageName = "ButtonGeneric";
		d._textureName = "PDA";
		d.position = Vector2.SmoothStep(new Vector2(basePosition.X, basePosition.Y + 740f), basePosition, (float)progressA);
		d.origin = new Vector2(95f, 98.5f);
		d.isDisplayed = true;
		d.depth = 0.51f;
		d.scale = scale;
		d.objectId = id;
		d.rotation = rotation;
		d.myColor = myColor;
		if (rotation != 0f)
		{
			Vector2 vector = new Vector2(640f, 360f);
			Matrix matrix = Matrix.CreateRotationZ(rotation);
			matrix *= Matrix.CreateScale(scale);
			Vector2 vector2 = Vector2.Transform(d.position - vector, matrix);
			d.position = vector2 + vector;
		}
		addGraphic(d);
	}

	private DisplayData getDisplayData(int id)
	{
		foreach (DisplayData display in displayList)
		{
			if (display.objectId == id)
			{
				return display;
			}
		}
		return new DisplayData();
	}

	private void updatePosition(int id, Vector2 newPosition)
	{
		foreach (DisplayData display in displayList)
		{
			if (display.objectId == id)
			{
				display.position = newPosition;
				break;
			}
		}
	}

	private void updatePDATextScreen(GameTime gameTime, PDATextSelectScreen PDAText, GamePadControl myGamePad, int currentTime, List<string> playSimpleSound, PDATextSelectScreen PDATextScreen, VariableEngine vEngine, SaveData saveData)
	{
		if (status == statusEnum.Initialize)
		{
			PDAText.reset(currentTime);
			PDAText.addHeaderText("Available Activities", 30, startOpacity: true);
			PDAText.addHeaderText("Time Needed", 600, startOpacity: true);
			PDAText.updateHeaderTextColor(Color.Black);
			PDAText.updateLineItemColors(new Color(0, 0, 0, 0), noRemoved: false);
			PDAText.updateTextList();
			PDAText.addTime();
			displayStateProgress = 0.0;
			startingPhase = 1;
			status = statusEnum.Starting;
			PDAText.isActive = true;
		}
		else if (status == statusEnum.Starting)
		{
			pauseCounter++;
			if (startingPhase == 1)
			{
				PDATextScreen.fadeInAllGraphicsText();
				if (PDAText.checkNextPhaseForUpdating(ResearchControlData.ResearchData.DisplayState.Updating))
				{
					PDAText.updateTextList();
				}
				if (PDAText.updateTextBoxColor(ResearchControlData.ResearchData.DisplayState.Updating))
				{
					startingPhase = 2;
				}
			}
			else if (startingPhase == 2)
			{
				PDATextScreen.fadeInAllGraphicsText();
				if (PDAText.checkNextPhaseForUpdating(ResearchControlData.ResearchData.DisplayState.Remove))
				{
					PDAText.updatePhaseState(ResearchControlData.ResearchData.DisplayState.Updating, ResearchControlData.ResearchData.DisplayState.Updated);
					PDAText.updateTextList();
				}
				if (PDAText.updateTextBoxColor(ResearchControlData.ResearchData.DisplayState.Remove))
				{
					startingPhase = 3;
				}
			}
			else if (startingPhase == 3)
			{
				PDATextScreen.fadeInAllGraphicsText();
				if (PDAText.checkNextPhaseForUpdating(ResearchControlData.ResearchData.DisplayState.Adding))
				{
					PDAText.updatePhaseState(ResearchControlData.ResearchData.DisplayState.Remove, ResearchControlData.ResearchData.DisplayState.Removed);
					PDAText.updateTextList();
				}
				if (PDAText.updateTextBoxColor(ResearchControlData.ResearchData.DisplayState.Adding))
				{
					startingPhase = 4;
				}
			}
			else if ((startingPhase == 4) & PDATextScreen.fadeInAllGraphicsText())
			{
				PDAText.updateLineItemColors(Color.White, noRemoved: true);
				PDAText.updatePhaseState(ResearchControlData.ResearchData.DisplayState.Adding, ResearchControlData.ResearchData.DisplayState.Added);
				status = statusEnum.Waiting;
			}
		}
		else if (status == statusEnum.Waiting)
		{
			if (myGamePad.anyDirection != GamePadControl.direction.NotSet)
			{
				if (myGamePad.anyDirection == GamePadControl.direction.S)
				{
					PDATextScreen.updateSelect(1, gameTime);
				}
				else if (myGamePad.anyDirection == GamePadControl.direction.N)
				{
					PDATextScreen.updateSelect(-1, gameTime);
				}
				PDATextScreen.updateArrows();
			}
			else if (myGamePad.padAPressed)
			{
				playSimpleSound.Add("Arcade Beep 02");
				PDAText.getCurrentItem(PDATextScreen.selectedChoice());
				if (PDATextScreen.currentCaseDataItem.isAvailable)
				{
					status = statusEnum.Stopping;
				}
			}
		}
		else if (status == statusEnum.Stopping)
		{
			if (PDATextScreen.fadeOutAllGraphicsText())
			{
				vEngine.getCurrentResearchData(PDAText.currentCaseDataItem.id);
				PDAText.currentResearchData = vEngine.currentResearchData;
				PDAText.isActive = false;
				status = statusEnum.Stopped;
			}
		}
		else if (status != statusEnum.Stopped && status == statusEnum.Returning)
		{
			status = statusEnum.Initialize;
		}
	}

	private void updatePDATextScreenLoad(GameTime gameTime, PDATextSelectScreen PDAText, GamePadControl myGamePad, int currentTime, List<string> playSimpleSound, PDATextSelectScreen PDATextScreen, VariableEngine vEngine, SaveData saveData)
	{
		if (status == statusEnum.Initialize)
		{
			PDAText.reset(currentTime);
			PDAText.addHeaderText("Save Slots", 30, startOpacity: true);
			PDAText.updateHeaderTextColor(Color.Black);
			PDAText.updateLineItemColors(new Color(0, 0, 0, 0), noRemoved: false);
			PDAText.updateTextList();
			PDAText.addTime();
			displayStateProgress = 0.0;
			startingPhase = 1;
			status = statusEnum.Starting;
			PDAText.isActive = true;
		}
		else if (status == statusEnum.Starting)
		{
			if (startingPhase == 1)
			{
				PDATextScreen.fadeInAllGraphicsText();
				if (PDAText.checkNextPhaseForUpdating(ResearchControlData.ResearchData.DisplayState.Updating))
				{
					PDAText.updateTextList();
				}
				if (PDAText.updateTextBoxColor(ResearchControlData.ResearchData.DisplayState.Updating))
				{
					startingPhase = 2;
				}
			}
			else if ((startingPhase == 2) & PDATextScreen.fadeInAllGraphicsText())
			{
				PDAText.updateLineItemColors(Color.White, noRemoved: true);
				PDAText.updatePhaseState(ResearchControlData.ResearchData.DisplayState.Adding, ResearchControlData.ResearchData.DisplayState.Added);
				status = statusEnum.Waiting;
			}
		}
		else if (status == statusEnum.Waiting)
		{
			if (myGamePad.anyDirection != GamePadControl.direction.NotSet)
			{
				if (myGamePad.anyDirection == GamePadControl.direction.S)
				{
					PDATextScreen.updateSelect(1, gameTime);
				}
				else if (myGamePad.anyDirection == GamePadControl.direction.N)
				{
					PDATextScreen.updateSelect(-1, gameTime);
				}
				PDATextScreen.updateArrows();
			}
			else if (myGamePad.padAPressed)
			{
				playSimpleSound.Add("Arcade Beep 02");
				PDAText.getCurrentItem(PDATextScreen.selectedChoice());
				if (PDATextScreen.currentCaseDataItem.isAvailable)
				{
					status = statusEnum.Stopping;
				}
			}
		}
		else if (status == statusEnum.Stopping)
		{
			if (PDATextScreen.fadeOutAllGraphicsText())
			{
				vEngine.getCurrentResearchData(PDAText.currentCaseDataItem.id);
				PDAText.currentResearchData = vEngine.currentResearchData;
				PDAText.isActive = false;
				status = statusEnum.Stopped;
			}
		}
		else if (status != statusEnum.Stopped && status == statusEnum.Returning)
		{
			status = statusEnum.Initialize;
		}
	}

	private void updatePDATextScreenReturn(GameTime gameTime, PDATextSelectScreen PDAText, GamePadControl myGamePad, int currentTime, List<string> playSimpleSound, PDATextSelectScreen PDATextScreen, SaveData saveData)
	{
		if (status == statusEnum.Initialize)
		{
			PDAText.addHeaderText("Results", 30, startOpacity: true);
			PDAText.updateHeaderTextColor(Color.White);
			PDAText.updateTextListOnReturn();
			displayStateProgress = 0.0;
			startingPhase = 1;
			status = statusEnum.Starting;
			PDAText.isActive = false;
		}
		else if (status == statusEnum.Starting)
		{
			if (myGamePad.padAPressed)
			{
				PDAText.textBoxSummary.finishAnimationNow = true;
			}
			if (PDAText.textBoxSummary.isFinishedDrawing)
			{
				PDAText.addResultsButton();
				status = statusEnum.Waiting;
				return;
			}
			playCount++;
			if ((playCount > 3) & !PDAText.textBoxSummary.isSpace)
			{
				playCount = 0;
				playSimpleSound.Add("Type");
			}
		}
		else if (status == statusEnum.Waiting)
		{
			if (myGamePad.padAPressed)
			{
				status = statusEnum.Initialize;
				PDAText.reset();
				PDAStatus = PDAStatusDisplay.starting;
				PDAText.isReturnDisplay = false;
			}
		}
		else if (status == statusEnum.Returning)
		{
			status = statusEnum.Initialize;
			PDAText.reset();
			PDAText.isReturnDisplay = true;
		}
	}

	private void adjustTableDataListOnReturn(PDATextSelectScreen PDATextScreen)
	{
		bool flag = true;
		while (flag)
		{
			flag = false;
			foreach (PDATextListData tableData in PDATextScreen.tableDataList)
			{
				if (tableData.orderId != PDATextScreen.selectedPosition)
				{
					flag = true;
					PDATextScreen.tableDataList.Remove(tableData);
					break;
				}
			}
		}
		foreach (PDATextListData tableData2 in PDATextScreen.tableDataList)
		{
			tableData2.orderId = 1;
		}
		PDATextScreen.selectedPosition = 1;
	}

	private void updatePDATextScreenOld(GameTime gameTime, PDATextSelectScreen PDAText, GamePadControl myGamePad, PDATextSelectScreen PDATextScreen)
	{
		if (status == statusEnum.Initialize)
		{
			addInitialGraphics(1, "ButtonB", Vector2.Zero, Vector2.Zero, 0f, 1f, 0.1f, startOpacityZero: true);
			PDAText.updateBasePosition(0f, 200f);
			PDAText.addHeaderText("Available Activities", 30, startOpacity: true);
			PDAText.addHeaderText("Hours Needed", 600, startOpacity: true);
			PDAText.addTime();
			PDAText.updateTextList();
			PDAText.updateColors(new Color(0, 0, 0, 0));
			status = statusEnum.Starting;
		}
		else if (status == statusEnum.Starting)
		{
			PDAText.updateHeaderTextColor(Color.White);
			fadeGraphicByID(1, 1, 400.0, gameTime, dontGenerate: false);
			if ((progressA < 1.0) & (fadeColorBase1 > 55))
			{
				progressA += gameTime.ElapsedGameTime.TotalMilliseconds / messageTimeSpan.TotalMilliseconds;
				if (progressA > 1.0)
				{
					progressA = 1.0;
				}
				tempPosition = Vector2.SmoothStep(new Vector2(640f, 1100f), new Vector2(640f, 360f), (float)progressA);
				updatePosition(2, tempPosition);
				updatePosition(3, tempPosition);
				PDAText.updateBasePosition(0f, tempPosition.Y - 360f);
				PDAText.updatePositions();
			}
			if (fadeColorBase1 > 155)
			{
				fadeGraphicByID(2, 2, 600.0, gameTime, dontGenerate: false);
				fadeGraphicByID(3, 2, 200.0, gameTime, dontGenerate: true);
				PDAText.updateColors(new Color(fadeColorBase2, fadeColorBase2, fadeColorBase2, fadeColorBase2));
			}
			if ((fadeColorBase1 == byte.MaxValue) & (progressA == 1.0))
			{
				turnOnPDA();
				status = statusEnum.Waiting;
			}
		}
		else
		{
			if (status != statusEnum.Waiting)
			{
				return;
			}
			turnOnPDA();
			if (myGamePad.anyDirection != GamePadControl.direction.NotSet)
			{
				if (myGamePad.anyDirection == GamePadControl.direction.S)
				{
					PDATextScreen.updateSelect(1, gameTime);
				}
				else if (myGamePad.anyDirection == GamePadControl.direction.N)
				{
					PDATextScreen.updateSelect(-1, gameTime);
				}
			}
		}
	}

	private void turnOnPDA()
	{
		foreach (DisplayData display in displayList)
		{
			display.isDisplayed = true;
		}
	}

	private void updateStartingPDA(GameTime gameTime)
	{
		if (status == statusEnum.Initialize)
		{
			addInitialGraphics(1, "PDABG", Vector2.Zero, Vector2.Zero, 0f, 1f, 0.1f, startOpacityZero: true);
			addInitialGraphics(2, "VerticalPhoneBG", new Vector2(640f, 360f), new Vector2(158.5f, 268f), 0f, 1f, 0.15f, startOpacityZero: true);
			addInitialGraphics(3, "VerticalPhoneFrame", new Vector2(640f, 360f), new Vector2(175f, 335f), 0f, 1f, 0.26f, startOpacityZero: true);
			addUpdateButtonVertical(4, 1, "ButtonGeneric", new Color(0, 0, 0, 0), 0.7f, 0f);
			addUpdateButtonVertical(5, 2, "ButtonGeneric", new Color(0, 0, 0, 0), 0.7f, 0f);
			addUpdateButtonVertical(6, 3, "ButtonGeneric", new Color(0, 0, 0, 0), 0.7f, 0f);
			addUpdateButtonVertical(7, 4, "ButtonGeneric", new Color(0, 0, 0, 0), 0.7f, 0f);
			status = statusEnum.Starting;
		}
		else if (status == statusEnum.Starting)
		{
			fadeGraphicByID(1, 1, 400.0, gameTime, dontGenerate: false);
			if ((progressA < 1.0) & (fadeColorBase1 > 55))
			{
				progressA += gameTime.ElapsedGameTime.TotalMilliseconds / messageTimeSpan.TotalMilliseconds;
				if (progressA > 1.0)
				{
					progressA = 1.0;
				}
				tempPosition = Vector2.SmoothStep(new Vector2(640f, 1100f), new Vector2(640f, 360f), (float)progressA);
				updatePosition(2, tempPosition);
				updatePosition(3, tempPosition);
				addUpdateButtonVertical(4, 1, "ButtonGeneric", new Color(0, 0, 0, 0), 0.7f, 0f);
				addUpdateButtonVertical(5, 2, "ButtonGeneric", new Color(0, 0, 0, 0), 0.7f, 0f);
				addUpdateButtonVertical(6, 3, "ButtonGeneric", new Color(0, 0, 0, 0), 0.7f, 0f);
				addUpdateButtonVertical(7, 4, "ButtonGeneric", new Color(0, 0, 0, 0), 0.7f, 0f);
			}
			if (fadeColorBase1 > 55)
			{
				fadeGraphicByID(2, 2, 600.0, gameTime, dontGenerate: false);
				fadeGraphicByID(3, 2, 200.0, gameTime, dontGenerate: true);
				fadeGraphicByID(4, 2, 200.0, gameTime, dontGenerate: true);
				fadeGraphicByID(5, 2, 200.0, gameTime, dontGenerate: true);
				fadeGraphicByID(6, 2, 200.0, gameTime, dontGenerate: true);
				fadeGraphicByID(7, 2, 200.0, gameTime, dontGenerate: true);
			}
			if ((fadeColorBase1 == byte.MaxValue) & (progressA == 1.0))
			{
				status = statusEnum.Started;
				rotationProgressA = 0.0;
				tempRotation = 0f;
				tempScale = 1f;
				tempScaleB = 0.7f;
			}
		}
		else
		{
			if (status != statusEnum.Started)
			{
				return;
			}
			fadeGraphicByID(1, 1, 400.0, gameTime, dontGenerate: false);
			fadeGraphicByID(2, 2, 600.0, gameTime, dontGenerate: false);
			fadeGraphicByID(3, 2, 200.0, gameTime, dontGenerate: true);
			fadeGraphicByID(4, 2, 200.0, gameTime, dontGenerate: true);
			fadeGraphicByID(5, 2, 200.0, gameTime, dontGenerate: true);
			fadeGraphicByID(6, 2, 200.0, gameTime, dontGenerate: true);
			fadeGraphicByID(7, 2, 200.0, gameTime, dontGenerate: true);
			if (rotationProgressA < 1.0)
			{
				rotationProgressA += gameTime.ElapsedGameTime.TotalMilliseconds / rotationTimeSpan.TotalMilliseconds;
				if (rotationProgressA > 1.0)
				{
					rotationProgressA = 1.0;
				}
			}
			float num = MathHelper.ToRadians(-90f);
			if (tempRotation > num)
			{
				tempRotation = MathHelper.SmoothStep(0f, num, (float)rotationProgressA);
				tempScale = MathHelper.SmoothStep(1f, 1.66f, (float)rotationProgressA);
				tempScaleB = MathHelper.SmoothStep(0.7f, 1f, (float)rotationProgressA);
			}
			else
			{
				tempRotation = num;
				tempScale = 1.66f;
				tempScaleB = 1f;
			}
			applyRotationScale(2, tempRotation, tempScale);
			applyRotationScale(3, tempRotation, tempScale);
			addUpdateButtonVertical(4, 1, "ButtonGeneric", Color.White, tempScaleB, tempRotation);
			addUpdateButtonVertical(5, 2, "ButtonGeneric", Color.White, tempScaleB, tempRotation);
			addUpdateButtonVertical(6, 3, "ButtonGeneric", Color.White, tempScaleB, tempRotation);
			addUpdateButtonVertical(7, 4, "ButtonGeneric", Color.White, tempScaleB, tempRotation);
		}
	}

	public void draw(SpriteBatch spriteBatch, PDATextSelectScreen PDATextScreen)
	{
		PDATextScreen.draw(spriteBatch);
	}
}
