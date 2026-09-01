using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using BureauNewPDA.Data;
using BureauNewPDA.Helpers;
using BureauNewPDA.VideoData;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;

namespace BureauNewPDA;

public class Game1 : Game
{
	public class imageNameId
	{
		public int frame = -1;

		public string frameName = "";
	}

	private GraphicsDeviceManager graphics;

	private SpriteBatch spriteBatch;

	private PDAGameComponent PDAgc;

	private FileIO myFileIO = new FileIO();

	private RefDumpClass myRefData = new RefDumpClass();

	private StoryControl myStoryControl = new StoryControl();

	private VideoControl videoControl = new VideoControl();

	private VideoPlayer videoPlayer;

	public List<TextureData> myCurrentTextures = new List<TextureData>();

	public SpriteRectableDisplayManager spriteRDM = new SpriteRectableDisplayManager();

	private Dictionary<string, Rectangle> spriteSourceRectanglesOld = new Dictionary<string, Rectangle>();

	private Dictionary<string, Dictionary<int, Rectangle>> newSpriteRectableLookup = new Dictionary<string, Dictionary<int, Rectangle>>();

	private SpriteFont myPDAFontHeader;

	private SpriteFont myPDAFontRegular;

	private SpriteFont MainFontRegular;

	private GamePadControl myGamePad = new GamePadControl();

	private CursorControl myCursorControl = new CursorControl();

	private CoreDisplayElements myCoreDisplayElements = new CoreDisplayElements();

	private VariableEngine vEngine = new VariableEngine();

	private List<string> playSimpleSFX = new List<string>();

	private bool firstTime = true;

	private List<imageNameId> saveDiskList = new List<imageNameId>(56);

	private int saveDiskCurrentFrame = 57;

	private bool pauseGame;

	private AudioEngine audioEngine;

	private WaveBank waveBank;

	private SoundBank soundBank;

	private WaveBank waveBankVoice;

	private SoundBank soundBankVoice;

	private Cue engineSound;

	private WaveBank waveBankMusic;

	private SoundBank soundBankMusic;

	private string currentMusic = "";

	private Cue engineSoundMusic;

	private AudioCategory defaultCategory;

	private AudioCategory musicCategory;

	private WaveBank waveBankStoryVoice;

	private SoundBank soundBankStoryVoice;

	private List<Cue> playingLoopQue = new List<Cue>();

	private bool playEngine;

	private string newLoop = "";

	private int currentError;

	private bool disableSaves;

	private string loadType = "L";

	private bool isStarting = true;

	private bool foundProfile;

	private bool foundStorage;

	private bool showSaveText;

	private bool gameStarted;

	private bool isStorageOperationPending;

	private bool getStorage;

	private bool loadStartMenu;

	private bool gamePurchasedCheck;

	private StorageDevice storageDevice;

	private IAsyncResult result;

	private string b = "";

	private bool turnedOnVibrate;

	private int i;

	public Game1()
	{
		graphics = new GraphicsDeviceManager(this);
		base.Content.RootDirectory = "Content";
		graphics.PreferredBackBufferWidth = 1280;
		graphics.PreferredBackBufferHeight = 720;
		PDAgc = new PDAGameComponent(this);
		base.Components.Add(PDAgc);
		base.Components.Add(new GamerServicesComponent(this));
		myCoreDisplayElements.spriteRDM = spriteRDM;
		PDAgc.myCoreDisplayElements = myCoreDisplayElements;
		myStoryControl.myCoreDisplayElements = myCoreDisplayElements;
		myCursorControl.myCoreDisplayElements = myCoreDisplayElements;
		myStoryControl.myCursorControl = myCursorControl;
		videoPlayer = new VideoPlayer();
		PDAgc.myVideoPlayer = videoPlayer;
		vEngine.addData();
		vEngine.update(PDAgc.saveData.activeVariables, PDAgc.PDATextScreen.tableDataList, 1234, isAtHome: false);
		PDAgc.vEngine = vEngine;
		PDAgc.playSimpleSFX = playSimpleSFX;
		PDAgc.saveData = myStoryControl.saveData;
		myStoryControl.playSimpleSFX = playSimpleSFX;
	}

	protected override void Initialize()
	{
		base.Initialize();
		myGamePad.initiate(GamePad.GetState(PlayerIndex.One));
	}

	protected override void LoadContent()
	{
		spriteBatch = new SpriteBatch(base.GraphicsDevice);
		myPDAFontHeader = base.Content.Load<SpriteFont>("PDAHeaderFont");
		myPDAFontRegular = base.Content.Load<SpriteFont>("PDARegularFont");
		MainFontRegular = base.Content.Load<SpriteFont>("MainFontSpriteFont1");
		myCoreDisplayElements.myPDAFontHeader = myPDAFontHeader;
		myCoreDisplayElements.myPDAFontRegular = myPDAFontRegular;
		myCoreDisplayElements.MainFontRegular = MainFontRegular;
		PDAgc.reset();
		PDAgc.myGamePad = myGamePad;
		TextureData textureData = new TextureData();
		textureData.texture = base.Content.Load<Texture2D>("Images/PDA");
		textureData.textureName = "PDA";
		textureData.textureLoaded = true;
		myCurrentTextures.Add(textureData);
		textureData = new TextureData();
		textureData.texture = base.Content.Load<Texture2D>("Images/arrows");
		textureData.textureName = "arrows";
		textureData.textureLoaded = true;
		myCurrentTextures.Add(textureData);
		textureData = new TextureData();
		textureData.texture = base.Content.Load<Texture2D>("Images/patternPuzzle");
		textureData.textureName = "patternPuzzle";
		textureData.textureLoaded = true;
		myCurrentTextures.Add(textureData);
		newSpriteRectableLookup = new Dictionary<string, Dictionary<int, Rectangle>>();
		loadRectangle("PDA");
		loadRectangle("arrows");
		loadRectangle("patternPuzzle");
		addNewSpriteSplitter(spriteSourceRectanglesOld);
		spriteRDM.addSpriteData("PDA", newSpriteRectableLookup);
		spriteRDM.addSpriteData("arrows", newSpriteRectableLookup);
		spriteRDM.addSpriteData("patternPuzzle", newSpriteRectableLookup);
		PDAgc.isActive = false;
		myCoreDisplayElements.myCurrentTextures = myCurrentTextures;
		myRefData = myFileIO.loadRefData();
		videoControl.videoPlayer = videoPlayer;
		audioEngine = new AudioEngine("Content\\Audio\\BureauSFX.xgs");
		waveBank = new WaveBank(audioEngine, "Content\\Audio\\SFXWaveBank.xwb");
		soundBank = new SoundBank(audioEngine, "Content\\Audio\\SFXSoundBank.xsb");
		waveBankMusic = new WaveBank(audioEngine, "Content\\Audio\\MusicWaveBank.xwb");
		soundBankMusic = new SoundBank(audioEngine, "Content\\Audio\\MusicSoundBank.xsb");
		audioEngine.Update();
		engineSound = soundBank.GetCue("ScrollE");
		engineSound.Play();
		currentMusic = "Element";
		engineSoundMusic = soundBankMusic.GetCue(currentMusic);
		engineSound = engineSoundMusic;
		engineSound.Play();
	}

	protected override void UnloadContent()
	{
	}

	private void storageLocal()
	{
		if (loadType == "X")
		{
			return;
		}
		if (loadType == "F")
		{
			try
			{
				if (!Guide.IsVisible)
				{
					result = StorageDevice.BeginShowSelector(myStoryControl.myPlayer, null, null);
					loadType = "WaitingToClose";
				}
			}
			catch
			{
			}
		}
		else if (loadType == "WaitingToClose")
		{
			try
			{
				if (result.IsCompleted & !Guide.IsVisible)
				{
					loadType = "InitialLoad";
					storageDevice = StorageDevice.EndShowSelector(result);
					gameStarted = true;
				}
			}
			catch
			{
			}
		}
		else if ((loadType == "InitialLoad") & !Guide.IsVisible)
		{
			bool flag = false;
			try
			{
				myStoryControl.saveMasterLoaded = true;
				IAsyncResult asyncResult = storageDevice.BeginOpenContainer("LuckyData", null, null);
				asyncResult.AsyncWaitHandle.WaitOne();
				StorageContainer storageContainer = storageDevice.EndOpenContainer(asyncResult);
				asyncResult.AsyncWaitHandle.Close();
				flag = true;
				string file = "saveHeader.sav";
				if (!storageContainer.FileExists(file))
				{
					storageContainer.Dispose();
					loadType = "CreateFirstTime";
					return;
				}
				Stream stream = storageContainer.OpenFile(file, FileMode.Open);
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(SaveDataMaster));
				myStoryControl.saveDataMaster = (SaveDataMaster)xmlSerializer.Deserialize(stream);
				myStoryControl.canSaveData = true;
				stream.Close();
				storageContainer.Dispose();
				loadType = "X";
			}
			catch
			{
				myStoryControl.saveMasterLoaded = false;
				loadType = "X";
				if (!flag)
				{
					currentError = 1;
				}
				else
				{
					currentError = 3;
				}
			}
		}
		else if (loadType == "LOADCHECKPOINT")
		{
			bool flag2 = false;
			try
			{
				IAsyncResult asyncResult2 = storageDevice.BeginOpenContainer("LuckyData", null, null);
				asyncResult2.AsyncWaitHandle.WaitOne();
				StorageContainer storageContainer2 = storageDevice.EndOpenContainer(asyncResult2);
				asyncResult2.AsyncWaitHandle.Close();
				flag2 = true;
				string file2 = "LuckySaveData" + myStoryControl.pendingLoadLevel + ".sav";
				if (!storageContainer2.FileExists(file2))
				{
					storageContainer2.Dispose();
					loadType = "X";
					myStoryControl.pendingLoadLevel = -1;
					return;
				}
				Stream stream2 = storageContainer2.OpenFile(file2, FileMode.Open);
				XmlSerializer xmlSerializer2 = new XmlSerializer(typeof(SaveData));
				myStoryControl.saveData = (SaveData)xmlSerializer2.Deserialize(stream2);
				PDAgc.saveData = myStoryControl.saveData;
				stream2.Close();
				storageContainer2.Dispose();
				myStoryControl.canSaveData = true;
				loadType = "X";
				myStoryControl.pendingLoadLevel = -1;
			}
			catch
			{
				loadType = "X";
				if (!flag2)
				{
					currentError = 1;
				}
				else
				{
					currentError = 13;
				}
			}
		}
		else if (loadType == "CreateFirstTime")
		{
			try
			{
				IAsyncResult asyncResult3 = storageDevice.BeginOpenContainer("LuckyData", null, null);
				asyncResult3.AsyncWaitHandle.WaitOne();
				StorageContainer storageContainer3 = storageDevice.EndOpenContainer(asyncResult3);
				asyncResult3.AsyncWaitHandle.Close();
				string file3 = "saveHeader.sav";
				if (storageContainer3.FileExists(file3))
				{
					storageContainer3.DeleteFile(file3);
				}
				Stream stream3 = storageContainer3.CreateFile(file3);
				XmlSerializer xmlSerializer3 = new XmlSerializer(typeof(SaveDataMaster));
				xmlSerializer3.Serialize(stream3, myStoryControl.saveDataMaster);
				stream3.Close();
				myStoryControl.canSaveData = true;
				storageContainer3.Dispose();
				loadType = "X";
			}
			catch
			{
				loadType = "X";
				currentError = 2;
			}
		}
		else if ((loadType == "SAVEGAME") & (myStoryControl.saveDataMaster.lastSavedId != -1))
		{
			try
			{
				IAsyncResult asyncResult4 = storageDevice.BeginOpenContainer("LuckyData", null, null);
				asyncResult4.AsyncWaitHandle.WaitOne();
				StorageContainer storageContainer4 = storageDevice.EndOpenContainer(asyncResult4);
				asyncResult4.AsyncWaitHandle.Close();
				string file4 = "saveHeader.sav";
				if (storageContainer4.FileExists(file4))
				{
					storageContainer4.DeleteFile(file4);
				}
				Stream stream4 = storageContainer4.CreateFile(file4);
				XmlSerializer xmlSerializer4 = new XmlSerializer(typeof(SaveDataMaster));
				xmlSerializer4.Serialize(stream4, myStoryControl.saveDataMaster);
				stream4.Close();
				storageContainer4.Dispose();
				loadType = "SAVEGAMEDATA";
				showSaveText = false;
			}
			catch
			{
				loadType = "X";
				currentError = 2;
			}
		}
		if (!((loadType == "SAVEGAMEDATA") & (myStoryControl.saveDataMaster.lastSavedId != -1)))
		{
			return;
		}
		try
		{
			IAsyncResult asyncResult5 = storageDevice.BeginOpenContainer("LuckyData", null, null);
			asyncResult5.AsyncWaitHandle.WaitOne();
			StorageContainer storageContainer5 = storageDevice.EndOpenContainer(asyncResult5);
			asyncResult5.AsyncWaitHandle.Close();
			string file5 = "LuckySaveData" + myStoryControl.saveDataMaster.lastSavedId + ".sav";
			if (storageContainer5.FileExists(file5))
			{
				storageContainer5.DeleteFile(file5);
			}
			Stream stream5 = storageContainer5.CreateFile(file5);
			XmlSerializer xmlSerializer5 = new XmlSerializer(typeof(SaveData));
			xmlSerializer5.Serialize(stream5, myStoryControl.saveData);
			stream5.Close();
			storageContainer5.Dispose();
			loadType = "X";
			showSaveText = true;
		}
		catch
		{
			loadType = "X";
			currentError = 2;
		}
	}

	private void currentErrorDisplay()
	{
		try
		{
			if (!((currentError != 0) & !Guide.IsVisible))
			{
				return;
			}
			myGamePad.turnOffVibrate();
			if (currentError == 1)
			{
				Guide.BeginShowMessageBox(myStoryControl.myPlayer, "No Storage Device Selected", "You did not select a device.  Without a device, you will not be able to play this game.", new string[2] { "Yes. Select new device.", "Exit Game" }, 0, MessageBoxIcon.None, updateError, null);
			}
			else if (currentError == 2)
			{
				Guide.BeginShowMessageBox(myStoryControl.myPlayer, "Problem Saving Game", "Do not remove storage device while playing game.  You will also get this error if your device is full.", new string[1] { "Exit Game" }, 0, MessageBoxIcon.None, updateError, null);
			}
			else if (currentError == 3)
			{
				Guide.BeginShowMessageBox(myStoryControl.myPlayer, "Loading Settings Error", "Sorry - an error occured loading your settings.  Default settings will be used and previous data has been lost.", new string[1] { "OK" }, 0, MessageBoxIcon.None, updateError, null);
			}
			else if (currentError == 4)
			{
				Guide.BeginShowMessageBox(myStoryControl.myPlayer, "Controller Disconnected", "Please reconnect your controller to continue the game.", new string[1] { "OK" }, 0, MessageBoxIcon.None, updateError, null);
			}
			else if (currentError == 5)
			{
				Guide.BeginShowMessageBox(myStoryControl.myPlayer, "Warning - Not Saving Game", "Saving has been disabled during this game session.  Please quit and restart to allow saves.", new string[1] { "OK" }, 0, MessageBoxIcon.None, updateError, null);
			}
			else if (currentError == 6)
			{
				Guide.BeginShowMessageBox(myStoryControl.myPlayer, "Warning - User is not signed in.", "Sorry - Game needs a valid profile to save games.", new string[2] { "Sign In", "Exit Game" }, 0, MessageBoxIcon.None, updateError, null);
			}
			else if (currentError == 7)
			{
				Guide.ShowSignIn(1, onlineOnly: false);
				currentError = 0;
			}
			else if (currentError == 8)
			{
				Guide.BeginShowMessageBox(myStoryControl.myPlayer, "Warning - Active user has logged off.", "Sorry - I will now exit the current game.", new string[1] { "Exit Game" }, 0, MessageBoxIcon.None, updateError, null);
			}
			else if (currentError == 10)
			{
				Guide.BeginShowMessageBox(myStoryControl.myPlayer, "Game Options", "Are you sure you want to quit?  (Your data is only saved at checkpoints.)", new string[2] { "Keep Playing", "Quit Current Game" }, 0, MessageBoxIcon.None, updateError, null);
			}
			else if (currentError == 11)
			{
				Guide.BeginShowMessageBox(myStoryControl.myPlayer, "Game Options - Trial Mode", "Thank you for supporting us and our game.", new string[2] { "Purchase Game", "Return to Game" }, 0, MessageBoxIcon.None, updateError, null);
			}
			else if (currentError == 12)
			{
				SignedInGamer signedInGamer = Gamer.SignedInGamers[myStoryControl.myPlayer];
				if (signedInGamer.Privileges.AllowPurchaseContent)
				{
					Guide.ShowMarketplace(myStoryControl.myPlayer);
				}
				else
				{
					Guide.BeginShowMessageBox(myStoryControl.myPlayer, "Profile Limitation", "Sorry - This profile does not have the ability to make purchases.", new string[1] { "OK" }, 0, MessageBoxIcon.None, updateError, null);
				}
				currentError = 0;
			}
			else if (currentError == 13)
			{
				Guide.BeginShowMessageBox(myStoryControl.myPlayer, "Error Loading or Finding Saved Data", "Sorry - I will now exit the game.  If you receive this error again, you may need to manually delete your save data.  (This is not the same as your game data - it will be called LuckySaveData).", new string[1] { "Exit Game" }, 0, MessageBoxIcon.None, updateError, null);
			}
			else if (currentError == 14)
			{
				if (!myStoryControl.saveDataMaster.invertY)
				{
					b = "Invert Cursor";
				}
				else
				{
					b = "Normal Cursor";
				}
				Guide.BeginShowMessageBox(myStoryControl.myPlayer, "(Game Paused)", "Please select one of the following options to resume game.", new string[3] { "Resume", b, "Quit Game" }, 0, MessageBoxIcon.None, updateError, null);
			}
			else if (currentError == 15)
			{
				string text = "Vibration = ";
				string text2 = "Fast Text = ";
				string text3 = "Skip Cutscene=";
				text = ((!myStoryControl.saveDataMaster.vibrationOn) ? (text + " Off") : (text + " On"));
				text2 = ((!myStoryControl.saveDataMaster.fastTextSkip) ? (text2 + " Off") : (text2 + " On"));
				text3 = ((!myStoryControl.saveDataMaster.skipAnimation) ? (text3 + " Off") : (text3 + " On"));
				Guide.BeginShowMessageBox(myStoryControl.myPlayer, "(Options)", "Please select one of the following options.  To exit this menu, press an option and select resume.", new string[3] { text, text2, text3 }, 0, MessageBoxIcon.None, updateError, null);
			}
			else if (currentError == 16)
			{
				Guide.BeginShowMessageBox(myStoryControl.myPlayer, "(Options - Vibration)", "Turn Controller vibration On or Off or Resume Game", new string[3] { "ON", "OFF", "Resume" }, 0, MessageBoxIcon.None, updateError, null);
			}
			else if (currentError == 17)
			{
				Guide.BeginShowMessageBox(myStoryControl.myPlayer, "(Options - Fast Text)", "When Fast Text is on - you can skip text by pressing the (A) button", new string[3] { "ON", "OFF", "Resume" }, 0, MessageBoxIcon.None, updateError, null);
			}
			else if (currentError == 18)
			{
				Guide.BeginShowMessageBox(myStoryControl.myPlayer, "(Options - Skip Cutscene)", "When On, press <Start> to skip most cutscenes.  Option does not work during video puzzles.", new string[3] { "ON", "OFF", "Resume" }, 0, MessageBoxIcon.None, updateError, null);
			}
			else if (currentError == 19)
			{
				Guide.BeginShowMessageBox(myStoryControl.myPlayer, "(Exit to Main Menu)", "Are you sure you want to exit this game?  Any data since the last autosave will be lost.", new string[2] { "Main Menu", "Resume" }, 0, MessageBoxIcon.None, updateError, null);
			}
			else if (currentError == 20)
			{
				Guide.BeginShowMessageBox(myStoryControl.myPlayer, "FBI PDA Access Error", "The Bureau PDA is not for personal use.  If you received this message in error, please file a form 2131-AF224F to your regional Director.", new string[1] { "OK" }, 0, MessageBoxIcon.None, updateError, null);
			}
		}
		catch
		{
		}
	}

	private void updateError(IAsyncResult result)
	{
		if (currentError == 1)
		{
			currentError = 0;
			int? num = Guide.EndShowMessageBox(result);
			int? num2 = num;
			if (num2.GetValueOrDefault() == 0 && num2.HasValue)
			{
				loadType = "F";
				isStorageOperationPending = false;
				currentError = 0;
			}
			else
			{
				currentError = 0;
				Exit();
			}
		}
		else if (currentError == 2)
		{
			currentError = 0;
			Guide.EndShowMessageBox(result);
			currentError = 0;
			Exit();
		}
		else if (currentError == 3)
		{
			currentError = 0;
			loadType = "CreateFirstTime";
		}
		else if (currentError == 4)
		{
			currentError = 0;
		}
		else if (currentError == 5)
		{
			currentError = 0;
			disableSaves = true;
			loadType = "X";
		}
		else if (currentError == 6)
		{
			currentError = 0;
			int? num3 = Guide.EndShowMessageBox(result);
			int? num4 = num3;
			if (num4.GetValueOrDefault() == 0 && num4.HasValue)
			{
				currentError = 7;
				myStoryControl.foundPlayer = false;
			}
			else if (num3 == 1)
			{
				Exit();
			}
		}
		else if (currentError == 8)
		{
			currentError = 0;
			Exit();
		}
		else if (currentError == 10)
		{
			currentError = 0;
			myStoryControl.saveData.currentError = 0;
			int? num5 = Guide.EndShowMessageBox(result);
			int? num6 = num5;
			if ((num6.GetValueOrDefault() != 0 || !num6.HasValue) && num5 == 1)
			{
				Exit();
			}
		}
		else if (currentError == 11)
		{
			currentError = 0;
			int? num7 = Guide.EndShowMessageBox(result);
			int? num8 = num7;
			if (num8.GetValueOrDefault() == 0 && num8.HasValue)
			{
				currentError = 12;
			}
			else if (num7 == 1)
			{
				myStoryControl.pendingLoadLevel = -1;
				loadType = "X";
				loadStartMenu = true;
			}
		}
		else if (currentError == 13)
		{
			currentError = 0;
			Exit();
		}
		else if (currentError == 14)
		{
			currentError = 0;
			int? num9 = Guide.EndShowMessageBox(result);
			int? num10 = num9;
			if (num10.GetValueOrDefault() == 0 && num10.HasValue)
			{
				pauseGame = false;
				currentError = 0;
			}
			else if (num9 == 1)
			{
				if (myStoryControl.saveDataMaster.invertY)
				{
					myStoryControl.saveDataMaster.invertY = false;
				}
				else
				{
					myStoryControl.saveDataMaster.invertY = true;
				}
			}
			else if (num9 == 2)
			{
				currentError = 0;
				Exit();
			}
		}
		else if (currentError == 15)
		{
			currentError = 0;
			int? num11 = Guide.EndShowMessageBox(result);
			int? num12 = num11;
			if (num12.GetValueOrDefault() == 0 && num12.HasValue)
			{
				currentError = 16;
			}
			else if (num11 == 1)
			{
				currentError = 17;
			}
			else if (num11 == 2)
			{
				currentError = 18;
			}
		}
		else if (currentError == 16)
		{
			currentError = 0;
			int? num13 = Guide.EndShowMessageBox(result);
			int? num14 = num13;
			if (num14.GetValueOrDefault() == 0 && num14.HasValue)
			{
				myStoryControl.saveDataMaster.vibrationOn = true;
				currentError = 15;
			}
			else if (num13 == 1)
			{
				myStoryControl.saveDataMaster.vibrationOn = false;
				currentError = 15;
			}
			else if (num13 == 2)
			{
				pauseGame = false;
				currentError = 0;
			}
		}
		else if (currentError == 17)
		{
			currentError = 0;
			int? num15 = Guide.EndShowMessageBox(result);
			int? num16 = num15;
			if (num16.GetValueOrDefault() == 0 && num16.HasValue)
			{
				myStoryControl.saveDataMaster.fastTextSkip = true;
				currentError = 15;
			}
			else if (num15 == 1)
			{
				myStoryControl.saveDataMaster.fastTextSkip = false;
				currentError = 15;
			}
			else if (num15 == 2)
			{
				pauseGame = false;
				currentError = 0;
			}
		}
		else if (currentError == 18)
		{
			currentError = 0;
			int? num17 = Guide.EndShowMessageBox(result);
			int? num18 = num17;
			if (num18.GetValueOrDefault() == 0 && num18.HasValue)
			{
				myStoryControl.saveDataMaster.skipAnimation = true;
				currentError = 15;
			}
			else if (num17 == 1)
			{
				myStoryControl.saveDataMaster.skipAnimation = false;
				currentError = 15;
			}
			else if (num17 == 2)
			{
				pauseGame = false;
				currentError = 0;
			}
		}
		else if (currentError == 19)
		{
			currentError = 0;
			int? num19 = Guide.EndShowMessageBox(result);
			int? num20 = num19;
			if (num20.GetValueOrDefault() == 0 && num20.HasValue)
			{
				myStoryControl.canSaveData = false;
				pauseGame = false;
				resetGame(loadMenu: true);
			}
			else if (num19 == 1)
			{
				currentError = 0;
				pauseGame = false;
			}
		}
		else if (currentError == 20)
		{
			currentError = 0;
		}
	}

	private void gamePadEvents(double elapsedTime)
	{
		if (!myStoryControl.foundPlayer & myStoryControl.lookForPlayer)
		{
			for (PlayerIndex playerIndex = PlayerIndex.One; playerIndex <= PlayerIndex.Four; playerIndex++)
			{
				if (GamePad.GetState(playerIndex).Buttons.A == ButtonState.Pressed)
				{
					myStoryControl.myPlayer = playerIndex;
					myStoryControl.foundPlayer = true;
					break;
				}
				if (GamePad.GetState(playerIndex).Buttons.Start == ButtonState.Pressed)
				{
					myStoryControl.myPlayer = playerIndex;
					myStoryControl.foundPlayer = true;
					break;
				}
			}
		}
		else if (myStoryControl.foundPlayer & !foundProfile)
		{
			OperatingSystem oSVersion = Environment.OSVersion;
			PlatformID platform = oSVersion.Platform;
			if (platform == PlatformID.Xbox)
			{
				SignedInGamer signedInGamer = Gamer.SignedInGamers[myStoryControl.myPlayer];
				if (signedInGamer != null)
				{
					foundProfile = true;
					getStorage = true;
				}
				else
				{
					currentError = 6;
				}
			}
			else
			{
				getStorage = true;
				foundProfile = true;
			}
		}
		else if (getStorage & !foundStorage)
		{
			loadType = "F";
			foundStorage = true;
		}
		if (!foundProfile)
		{
			return;
		}
		myStoryControl.checkedForTrialMode = true;
		myGamePad.getCurrentGamePad(GamePad.GetState(myStoryControl.myPlayer).Buttons, GamePad.GetState(myStoryControl.myPlayer).ThumbSticks, GamePad.GetState(myStoryControl.myPlayer).DPad, myStoryControl.myPlayer, GamePad.GetState(myStoryControl.myPlayer).Triggers, elapsedTime);
		if (myStoryControl.isTrialMode)
		{
			myStoryControl.isTrialMode = Guide.IsTrialMode;
		}
		if (!GamePad.GetState(myStoryControl.myPlayer).IsConnected & myStoryControl.foundPlayer)
		{
			currentError = 4;
		}
		else if (myStoryControl.foundPlayer & foundStorage)
		{
			SignedInGamer signedInGamer2 = Gamer.SignedInGamers[myStoryControl.myPlayer];
			if (signedInGamer2 == null)
			{
				currentError = 8;
			}
		}
	}

	protected override void Update(GameTime gameTime)
	{
		if (!pauseGame)
		{
			myCursorControl.updateCursor(myGamePad, gameTime, PDAgc.saveData, myStoryControl.saveDataMaster.invertY);
		}
		if (myCursorControl.playSFX)
		{
			myCursorControl.playSFX = false;
			playSimpleSFX.Add(myCursorControl.SFXName);
		}
		if (myStoryControl.quitGame)
		{
			Exit();
		}
		if (!pauseGame)
		{
			gamePadEvents(gameTime.ElapsedGameTime.TotalMilliseconds);
		}
		else if (!Guide.IsVisible & (currentError == 0))
		{
			pauseGame = false;
		}
		if (myStoryControl.saveData.pendingDataSave & (saveDiskCurrentFrame > 55) & (videoControl.currentVideoStatus == VideoControl.VideoStatus.Stopped))
		{
			if (myStoryControl.canSaveData)
			{
				myStoryControl.saveData.musicPlayingOnSave = currentMusic;
				loadType = "SAVEGAME";
				saveDiskCurrentFrame = 0;
			}
			myStoryControl.saveData.pendingDataSave = false;
		}
		else if (myStoryControl.pendingDataLoad & (myStoryControl.pendingLoadLevel != -1) & (loadType == "X"))
		{
			loadType = "LOADCHECKPOINT";
			saveDiskCurrentFrame = 0;
		}
		else if (myStoryControl.purchaseGame & !Guide.IsVisible)
		{
			currentError = 12;
			gamePurchasedCheck = true;
			myStoryControl.purchaseGame = false;
		}
		else if (gamePurchasedCheck)
		{
			if (!Guide.IsTrialMode)
			{
				myStoryControl.isTrialMode = false;
				myStoryControl.loadNextStory(8);
				gamePurchasedCheck = false;
				myStoryControl.purchaseGame = false;
				myStoryControl.canSaveData = true;
			}
		}
		else if (PDAgc.showPDAAccessError)
		{
			currentError = 20;
			PDAgc.showPDAAccessError = false;
		}
		if (myStoryControl.useStorage)
		{
			storageLocal();
		}
		if (GamePad.GetState(myStoryControl.myPlayer).Buttons.Back == ButtonState.Pressed)
		{
			if ((myStoryControl.currentStory.chapter != "StartLucky") & !pauseGame)
			{
				pauseGame = true;
				currentError = 14;
			}
			else if (pauseGame & Guide.IsVisible & (currentError == 0))
			{
				Guide.EndShowMessageBox(result);
				pauseGame = false;
				currentError = 0;
			}
		}
		currentErrorDisplay();
		if (myGamePad.vibrate & !turnedOnVibrate & myStoryControl.saveDataMaster.vibrationOn)
		{
			turnedOnVibrate = true;
			GamePad.SetVibration(myGamePad.myPlayer, myGamePad.vibrateA, myGamePad.vibrateB);
		}
		if (!myGamePad.vibrate & turnedOnVibrate)
		{
			turnedOnVibrate = false;
			GamePad.SetVibration(myGamePad.myPlayer, myGamePad.vibrateA, myGamePad.vibrateB);
		}
		if (myStoryControl.saveData != PDAgc.saveData)
		{
			PDAgc.saveData = myStoryControl.saveData;
		}
		if (firstTime)
		{
			myStoryControl.loadNewChapter("StartLucky", 1);
			if (myStoryControl.pendingPlayVideoId != -1)
			{
				loadPendingVideo(myStoryControl.pendingPlayVideoId);
			}
			firstTime = false;
		}
		else
		{
			if (myStoryControl.startPDA)
			{
				PDAgc.isActive = true;
				if (PDAgc.pendingVideo)
				{
					playPDAVideo();
				}
				if (PDAgc.pendingClosePDA)
				{
					PDAgc.isActive = false;
					myStoryControl.startPDA = false;
					PDAgc.pendingClosePDA = false;
					if (PDAgc.loadNewScene)
					{
						myStoryControl.loadNewChapter(vEngine.currentResearchData.gotoSceneName, vEngine.currentResearchData.gotoSceneId);
						if (myStoryControl.pendingPlayVideoId != -1)
						{
							loadPendingVideo(myStoryControl.pendingPlayVideoId);
						}
					}
				}
			}
			else
			{
				myStoryControl.updateStory(myGamePad, videoControl.currentVideoStatus, videoControl.videoPlayer.PlayPosition.TotalMilliseconds, gameTime);
				if (myStoryControl.pendingPlayVideoId != -1)
				{
					loadPendingVideo(myStoryControl.pendingPlayVideoId);
				}
			}
			videoControl.update(pauseGame);
		}
		if (playSimpleSFX.Count() > 0)
		{
			foreach (string item in playSimpleSFX)
			{
				engineSound = soundBank.GetCue(item);
				engineSound.Play();
			}
			playSimpleSFX.Clear();
		}
		if (pauseGame & engineSoundMusic.IsPlaying)
		{
			engineSoundMusic.Pause();
		}
		else if (!pauseGame & engineSoundMusic.IsPaused)
		{
			try
			{
				engineSoundMusic.Resume();
			}
			catch
			{
				myStoryControl.saveData.newMusic = "Element";
				Console.WriteLine("Error unpause");
			}
		}
		if (!pauseGame)
		{
			updateMusic();
		}
		audioEngine.Update();
		base.Update(gameTime);
	}

	private void updateMusic()
	{
		if (myStoryControl.saveData.newMusic != "")
		{
			if (!(myStoryControl.saveData.newMusic != "MusicStop"))
			{
				try
				{
					engineSoundMusic.Stop(AudioStopOptions.AsAuthored);
					currentMusic = "";
					myStoryControl.saveData.newMusic = "";
					return;
				}
				catch
				{
					Console.WriteLine("Error Stopping music");
					return;
				}
			}
			if (myStoryControl.saveData.newMusic != engineSoundMusic.Name)
			{
				engineSoundMusic = soundBankMusic.GetCue(myStoryControl.saveData.newMusic);
				engineSound = engineSoundMusic;
				currentMusic = myStoryControl.saveData.newMusic;
				engineSound.Play();
				myStoryControl.saveData.newMusic = "";
				return;
			}
			if (engineSoundMusic.IsStopped)
			{
				engineSoundMusic = soundBankMusic.GetCue(myStoryControl.saveData.newMusic);
				engineSound = engineSoundMusic;
				currentMusic = myStoryControl.saveData.newMusic;
				engineSound.Play();
			}
			myStoryControl.saveData.newMusic = "";
		}
		else if (engineSoundMusic.IsStopped)
		{
			if (engineSoundMusic.Name == "Feedback_Negative")
			{
				engineSoundMusic = soundBankMusic.GetCue("TenseMysteryTone");
				engineSound = engineSoundMusic;
				currentMusic = "TenseMysteryTone";
				engineSound.Play();
			}
			else if (engineSoundMusic.Name == "Feedback_Positive")
			{
				engineSoundMusic = soundBankMusic.GetCue("MysteryTone_Pt1");
				engineSound = engineSoundMusic;
				currentMusic = "MysteryTone_Pt1";
				engineSound.Play();
			}
			else if (engineSoundMusic.Name == "TenseMysteryTone")
			{
				engineSoundMusic = soundBankMusic.GetCue("Feedback_Positive");
				engineSound = engineSoundMusic;
				currentMusic = "Feedback_Positive";
				engineSound.Play();
			}
			else if (engineSoundMusic.Name == "Bureau2_Theme_Var2_UpbeatStart_Loop")
			{
				engineSoundMusic = soundBankMusic.GetCue("Bureau2_Theme_Var1_MysteryLoop");
				engineSound = engineSoundMusic;
				currentMusic = "Bureau2_Theme_Var1_MysteryLoop";
				engineSound.Play();
			}
		}
	}

	private void playPDAVideo()
	{
		if (PDAgc.pendingVideo & !PDAgc.loadedVideo)
		{
			videoControl.addPendingVideo(base.Content.Load<Video>("Video/PDA/" + PDAgc.loadVideoName), PDAgc.loadVideoName);
			PDAgc.pendingVideo = false;
			PDAgc.loadedVideo = true;
			myStoryControl.pendingPlayVideoId = -1;
		}
	}

	private void loadPendingVideo(int id)
	{
		foreach (RefDumpClass.VideoData myDump in myRefData.myDumpList)
		{
			if (myDump.refId == id)
			{
				videoControl.addPendingVideo(base.Content.Load<Video>("Video/" + myDump.refGroupName + "/" + myDump.refName), myDump.refName);
				myStoryControl.pendingPlayVideoId = -1;
				break;
			}
		}
	}

	private bool isNumber(string v, out int newNumber)
	{
		newNumber = -1;
		if (v.Length > 6)
		{
			try
			{
				return int.TryParse(v.Substring(v.Length - 5), out newNumber);
			}
			catch
			{
				return false;
			}
		}
		return false;
	}

	private void addNewSpriteSplitter(Dictionary<string, Rectangle> d)
	{
		Dictionary<int, Rectangle> dictionary = new Dictionary<int, Rectangle>();
		string text = "";
		bool flag = false;
		int newNumber = 0;
		foreach (KeyValuePair<string, Rectangle> item in d)
		{
			if (isNumber(item.Key, out newNumber))
			{
				if (text != item.Key.Substring(0, item.Key.Length - 6))
				{
					if (flag)
					{
						newSpriteRectableLookup.Add(text, dictionary);
					}
					dictionary = new Dictionary<int, Rectangle>();
				}
				flag = true;
				text = item.Key.Substring(0, item.Key.Length - 6);
				dictionary.Add(newNumber, item.Value);
			}
			else
			{
				if ((text != item.Key) & flag)
				{
					newSpriteRectableLookup.Add(text, dictionary);
				}
				dictionary = new Dictionary<int, Rectangle>();
				flag = false;
				text = item.Key;
				dictionary.Add(0, item.Value);
				newSpriteRectableLookup.Add(item.Key, dictionary);
			}
		}
		if (flag)
		{
			newSpriteRectableLookup.Add(text, dictionary);
		}
	}

	private void loadRectangle(string fileName)
	{
		List<string> list = new List<string>();
		try
		{
			using StreamReader streamReader = new StreamReader(TitleContainer.OpenStream("Content/SpriteSheetData/" + fileName + ".txt"));
			while (!streamReader.EndOfStream)
			{
				string text = streamReader.ReadLine();
				string[] array = text.Split('=');
				string[] array2 = array[1].Trim().Split(' ');
				Rectangle value = new Rectangle(int.Parse(array2[0]), int.Parse(array2[1]), int.Parse(array2[2]), int.Parse(array2[3]));
				list.Add(array[0].Trim());
				spriteSourceRectanglesOld.Add(array[0].Trim(), value);
			}
		}
		catch
		{
			Console.WriteLine("Error A345A in loading Recatable - filename = " + fileName);
		}
	}

	private void resetGame(bool loadMenu)
	{
		myStoryControl.saveData.newMusic = "Element";
		if (loadMenu)
		{
			myStoryControl.loadNewChapter("StartLucky", 8);
			myStoryControl.myCursorControl.deactiveCursor();
		}
	}

	private void drawSaveDisk(SpriteBatch spriteBatch)
	{
		if ((saveDiskCurrentFrame >= 0) & (saveDiskCurrentFrame <= 56))
		{
			spriteBatch.Draw(myCoreDisplayElements.getTexture("PDA"), new Vector2(1000f, 600f), myCoreDisplayElements.spriteRDM.getSpriteRectangle("PDA", "SaveDisk", saveDiskCurrentFrame), Color.White, 0f, new Vector2(64f, 64f), 1f, SpriteEffects.None, 0.99f);
		}
		saveDiskCurrentFrame++;
	}

	protected override void Draw(GameTime gameTime)
	{
		base.GraphicsDevice.Clear(Color.CornflowerBlue);
		base.GraphicsDevice.Clear(Color.Black);
		spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
		videoControl.draw(spriteBatch);
		myCursorControl.drawCursor(spriteBatch, PDAgc.saveData);
		if (saveDiskCurrentFrame <= 55)
		{
			drawSaveDisk(spriteBatch);
		}
		if (!PDAgc.isActive)
		{
			myStoryControl.drawText(spriteBatch);
		}
		spriteBatch.End();
		base.Draw(gameTime);
	}
}
