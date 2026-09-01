using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace BureauNewPDA;

public class PDAGameComponent(Game game) : DrawableGameComponent(game)
{
	public enum PDAState
	{
		StartUp,
		CurrentCase,
		CurrentCaseReturnAction,
		CurrentCaseReturnStory,
		CurrentCaseReturnVideo,
		CurrentCaseReturnVideoPuzzle,
		CurrentCaseLoadScene,
		VideoPuzzleSelect,
		VideoPuzzleOrder,
		DisplayReturnResults,
		Load,
		Options,
		Exit
	}

	private SpriteBatch spriteBatch;

	private GraphicsDevice device;

	public bool isActive;

	private List<DisplayData> displayList = new List<DisplayData>();

	private List<DisplayText> displayText = new List<DisplayText>();

	public CoreDisplayElements myCoreDisplayElements = new CoreDisplayElements();

	public PDATextSelectScreen PDATextScreen = new PDATextSelectScreen();

	public GamePadControl myGamePad = new GamePadControl();

	private PDADisplayPuzzleUI PDADisplayPuzzle = new PDADisplayPuzzleUI();

	private PDAState currentPDAState;

	private PDAVideoControl myPDAVideo = new PDAVideoControl();

	public SaveData saveData = new SaveData();

	public List<string> playSimpleSFX = new List<string>();

	public VideoPlayer myVideoPlayer;

	public bool loadedVideo;

	public bool pendingVideo;

	public string loadVideoName = "";

	public bool pendingClosePDA;

	public VariableEngine vEngine = new VariableEngine();

	private PDADisplayManager myPDADM = new PDADisplayManager();

	private Vector2 tempPosition = Vector2.Zero;

	public bool loadNewScene;

	public bool showPDAAccessError;

	public override void Initialize()
	{
		base.Initialize();
	}

	public void fullReset()
	{
		myPDADM = new PDADisplayManager();
		PDATextScreen = new PDATextSelectScreen();
		PDADisplayPuzzle = new PDADisplayPuzzleUI();
		myPDAVideo.resetVideoControl();
	}

	public void reset()
	{
		myPDADM.start(displayList, displayText, myCoreDisplayElements, 1234, PDATextScreen);
		myPDAVideo.myCoreDisplayElements = myCoreDisplayElements;
		PDADisplayPuzzle.myCoreDisplayElements = myCoreDisplayElements;
		pendingVideo = false;
		loadedVideo = false;
		loadVideoName = "";
		currentPDAState = PDAState.StartUp;
		pendingClosePDA = false;
		myPDAVideo.resetVideoControl();
	}

	protected override void LoadContent()
	{
		spriteBatch = new SpriteBatch(base.GraphicsDevice);
	}

	public override void Update(GameTime gameTime)
	{
		if (isActive)
		{
			resetDisplay();
			switch (currentPDAState)
			{
			case PDAState.StartUp:
				myPDAVideo.update(myVideoPlayer, myGamePad, currentPDAState, playSimpleSFX);
				checkForNewMovieToPlay();
				if (myPDAVideo.currentState == PDAVideoControl.State.NA)
				{
					currentPDAState = myPDAVideo.pendingVideoState;
					specialEndCheck(saveData);
				}
				if (myPDAVideo.showErrorCode != -1)
				{
					showPDAAccessError = true;
					myPDAVideo.showErrorCode = -1;
				}
				break;
			case PDAState.CurrentCase:
				myPDAVideo.update(myVideoPlayer, myGamePad, currentPDAState, playSimpleSFX);
				checkForNewMovieToPlay();
				if (myPDAVideo.currentState == PDAVideoControl.State.Waiting && myPDADM.update(gameTime, myGamePad, 1234, playSimpleSFX, PDATextScreen, vEngine, saveData))
				{
					currentPDAState = PDAState.CurrentCaseReturnAction;
					myPDAVideo.loadVideo = "PhoneSidewayFadeOutA";
				}
				break;
			case PDAState.CurrentCaseReturnAction:
				checkForNewMovieToPlay();
				if (!((myVideoPlayer.State == MediaState.Stopped) & loadedVideo))
				{
					break;
				}
				if (vEngine.getResearchTypeById(PDATextScreen.currentCaseDataItem.id) == ResearchControlData.ResearchData.activateType.PlayVideoReturn)
				{
					myPDAVideo.loadVideo = vEngine.getVideoNameTypeById(PDATextScreen.currentCaseDataItem.id);
					if (PDATextScreen.currentResearchData.id == 9)
					{
						saveData.newMusic = "Bureau2_Theme_Var2_UpbeatStart_Loop";
					}
					currentPDAState = PDAState.CurrentCaseReturnVideo;
				}
				else if (vEngine.getResearchTypeById(PDATextScreen.currentCaseDataItem.id) == ResearchControlData.ResearchData.activateType.PlayVideoPuzzle)
				{
					saveData.newMusic = "Bureau2_PuzzlesLoop";
					myPDAVideo.loadVideo = vEngine.getVideoNameTypeById(PDATextScreen.currentCaseDataItem.id);
					currentPDAState = PDAState.CurrentCaseReturnVideoPuzzle;
				}
				else if (vEngine.getResearchTypeById(PDATextScreen.currentCaseDataItem.id) == ResearchControlData.ResearchData.activateType.GotoScene)
				{
					vEngine.getCurrentResearchData(PDATextScreen.currentCaseDataItem.id);
					myPDAVideo.loadVideo = "PhoneSidewayFadeOutA";
					currentPDAState = PDAState.CurrentCaseLoadScene;
				}
				break;
			case PDAState.DisplayReturnResults:
				checkForNewMovieToPlay();
				if (((myVideoPlayer.State == MediaState.Stopped) & loadedVideo) && myPDADM.update(gameTime, myGamePad, 1234, playSimpleSFX, PDATextScreen, vEngine, saveData))
				{
					currentPDAState = PDAState.CurrentCase;
					myPDAVideo.loadVideo = "PhoneSidewaysFadeOnA";
					specialEndCheck(saveData);
				}
				break;
			case PDAState.VideoPuzzleSelect:
				myPDAVideo.update(myVideoPlayer, myGamePad, currentPDAState, playSimpleSFX);
				checkForNewMovieToPlay();
				if (myVideoPlayer.State == MediaState.Playing)
				{
					PDADisplayPuzzle.playPosition = myVideoPlayer.PlayPosition.TotalMilliseconds;
				}
				else
				{
					PDADisplayPuzzle.playPosition = -1.0;
				}
				if (myPDAVideo.currentState != PDAVideoControl.State.Waiting)
				{
					break;
				}
				if (PDADisplayPuzzle.update(gameTime, myGamePad, playSimpleSFX))
				{
					vEngine.finishPuzzleAddVariables(PDATextScreen.tableDataList, saveData);
					currentPDAState = PDAState.DisplayReturnResults;
					if (PDADisplayPuzzle.isPuzzleFinishedCorrect)
					{
						saveData.newMusic = "Bureau2_Theme_Var2_UpbeatStart_Loop";
						myPDADM.PDAStatus = PDADisplayManager.PDAStatusDisplay.caseResults;
						saveData.pendingDataSave = true;
					}
					else
					{
						saveData.newMusic = "Bureau2_PlotThemeLoop";
						currentPDAState = PDAState.CurrentCase;
						myPDADM.PDAStatus = PDADisplayManager.PDAStatusDisplay.starting;
						saveData.pendingDataSave = true;
					}
				}
				if ((PDADisplayPuzzle.pendingVideo != "") & (myVideoPlayer.State == MediaState.Stopped))
				{
					myPDAVideo.loadVideo = PDADisplayPuzzle.pendingVideo;
					PDADisplayPuzzle.pendingVideo = "";
				}
				break;
			case PDAState.CurrentCaseLoadScene:
				myPDAVideo.update(myVideoPlayer, myGamePad, currentPDAState, playSimpleSFX);
				checkForNewMovieToPlay();
				if ((myVideoPlayer.State == MediaState.Stopped) & (myPDAVideo.loadVideo == ""))
				{
					currentPDAState = PDAState.Exit;
					loadNewScene = true;
				}
				break;
			case PDAState.Load:
				myPDAVideo.update(myVideoPlayer, myGamePad, currentPDAState, playSimpleSFX);
				checkForNewMovieToPlay();
				if (myPDAVideo.currentState == PDAVideoControl.State.Waiting && myPDADM.updateLoadScreen(gameTime, myGamePad, 1234, playSimpleSFX, PDATextScreen, vEngine, saveData))
				{
					currentPDAState = PDAState.CurrentCaseReturnAction;
					myPDAVideo.loadVideo = "PhoneSidewayFadeOutA";
				}
				break;
			case PDAState.Exit:
				if (myVideoPlayer.State == MediaState.Stopped)
				{
					pendingClosePDA = true;
					currentPDAState = PDAState.CurrentCaseReturnStory;
				}
				break;
			}
		}
		base.Update(gameTime);
	}

	public void updateLoadList(SaveDataMaster saveDataMaster)
	{
		PDATextScreen.tableDataList.Clear();
		PDATextListData pDATextListData = new PDATextListData();
		int num = 1;
		foreach (SaveDataMaster.saveDataShell save in saveDataMaster.saveList)
		{
			pDATextListData = new PDATextListData();
			pDATextListData.addData2Column(save.id, save.id, save.id + ": Last Saved On - " + save.saveDateTime.ToLocalTime().ToString(), "", 0, 650, "", PDATextListData.type.NA, isAvailable: true, isComplete: true);
			PDATextScreen.tableDataList.Add(pDATextListData);
			num++;
		}
		pDATextListData = new PDATextListData();
		pDATextListData.addData2Column(num, num, num + ": Empty Slot", "", 0, 650, "", PDATextListData.type.NA, isAvailable: true, isComplete: false);
		PDATextScreen.tableDataList.Add(pDATextListData);
	}

	private void specialEndCheck(SaveData saveData)
	{
	}

	private void addKendallDoingWell(SaveData saveData)
	{
		int num = 0;
		saveData.activeVariables.Remove("MakingProgress");
		if (saveData.checkForVariable("CompletedJacobVideoPuzzle"))
		{
			num++;
		}
		if (saveData.checkForVariable("CompletedWilliamVideoPuzzle"))
		{
			num++;
		}
		if (saveData.checkForVariable("CompletedMollyVideoPuzzle"))
		{
			num++;
		}
		if (saveData.checkForVariable("ViewedJacobEvidence"))
		{
			num++;
		}
		if (saveData.checkForVariable("ViewLobbyTape"))
		{
			num++;
		}
		if (num >= 3)
		{
			saveData.addVariables("MakingProgress");
		}
	}

	private void checkForNewMovieToPlay()
	{
		if (myPDAVideo.loadVideo != "")
		{
			loadVideoName = myPDAVideo.loadVideo;
			myPDAVideo.loadVideo = "";
			pendingVideo = true;
			loadedVideo = false;
			if (loadVideoName == "PhoneTurnedOffMainScreen")
			{
				currentPDAState = PDAState.Exit;
			}
		}
	}

	private void resetDisplay()
	{
		foreach (DisplayData display in displayList)
		{
			display.isDisplayed = false;
		}
	}

	public override void Draw(GameTime gameTime)
	{
		if (isActive)
		{
			spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
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
				spriteBatch.DrawString(item.myFont, item.getText(), tempPosition, Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.992f);
				tempPosition.X -= 3f;
				tempPosition.Y -= 3f;
				if (!item.isFinishedDrawing)
				{
					break;
				}
			}
			myPDAVideo.drawUI(spriteBatch);
			myPDADM.draw(spriteBatch, PDATextScreen);
			PDADisplayPuzzle.draw(spriteBatch);
			spriteBatch.End();
		}
		base.Draw(gameTime);
	}
}
