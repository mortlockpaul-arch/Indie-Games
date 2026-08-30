using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using MathTools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;
using MusicPlayer;
using Renderer;
using Screens;
using Skeleton;

namespace BabyMaker;

public class Game1 : Game
{
	public struct optionSet
	{
		public int SaveVersion;

		public int MusicVol;

		public int SFXVol;

		public List<string> highscoreNames;

		public List<int> highscoreNums;

		public bool MessageDisplayed;

		public int MessageDisplayMinute;

		public int TimePlayed;
	}

	private TimeTracker m_tUpdateTracker;

	private TimeTracker m_tDrawTracker;

	private GraphicsDeviceManager graphics;

	private SpriteBatch spriteBatch;

	private static List<Screen> m_stkScreens;

	private static BasicEffect b;

	private VertexDeclaration positionColorVertexDeclaration;

	private Matrix worldMatrix;

	private Matrix viewMatrix;

	private static Rectangle projectionRect;

	private static Matrix projectionMatrix;

	private Vector2 cameraOffset;

	private RenderSprite m_fps;

	private string[] m_fpsStrings;

	private List<int> Last60FramesDrawTimer;

	private double avgDrawRate;

	private static IAsyncResult getDeviceResult;

	private static StorageDevice m_StorageDevice;

	private static bool m_bSaveOptionsRequested;

	private static bool m_bLoadOptionsRequested;

	private static bool m_bAdvancedOptions;

	private static bool m_bIsShowingDeviceSelector;

	private static bool m_bExit;

	private RenderSprite screenSprite;

	private static bool m_bMessageDisplayed;

	private static int m_iMessageDisplayMinute;

	private static int m_iTimePlayed;

	private static List<string> m_highScoreNames;

	private static List<int> m_highScores;

	private static int m_iTrialTimePlayed;

	private static bool m_bCanSave;

	private static bool m_bDisplayedFailure;

	public static Profilehelper Profiler;

	private static AvatarHandler m_avatar;

	private static bool isTrial = true;

	private static bool sm_bIsDamaged;

	private static int m_iLevelReached;

	private static int m_iPlayTime;

	private static int m_iCoinsCollected;

	private static int m_iEnemiesKilled;

	private static int m_iPlayerDeaths;

	public static float DepthDivisor => 667f;

	public static float SFXVolume
	{
		get
		{
			return SoundEffect.MasterVolume;
		}
		set
		{
			SoundEffect.MasterVolume = value;
		}
	}

	public Game1()
	{
		m_avatar = null;
		m_iTrialTimePlayed = 0;
		m_bCanSave = false;
		m_bDisplayedFailure = false;
		m_bExit = false;
		getDeviceResult = null;
		m_StorageDevice = null;
		m_bSaveOptionsRequested = false;
		m_bLoadOptionsRequested = false;
		m_bAdvancedOptions = false;
		m_bIsShowingDeviceSelector = false;
		graphics = new GraphicsDeviceManager(this);
		base.Components.Add(new GamerServicesComponent(this));
		if (GraphicsAdapter.DefaultAdapter.IsWideScreen)
		{
			graphics.PreferredBackBufferHeight = 720;
			graphics.PreferredBackBufferWidth = 1280;
		}
		else
		{
			graphics.PreferredBackBufferHeight = 768;
			graphics.PreferredBackBufferWidth = 1024;
		}
		base.Content.RootDirectory = "Content";
		SetOptionsDefault();
	}

	protected override void Initialize()
	{
		Profiler = new Profilehelper();
		m_tUpdateTracker = new TimeTracker();
		m_tDrawTracker = new TimeTracker();
		ControlManager.Initialize();
		VectorTools.Initialize();
		m_stkScreens = new List<Screen>();
		positionColorVertexDeclaration = new VertexDeclaration(base.GraphicsDevice, VertexPositionColor.VertexElements);
		b = new BasicEffect(base.GraphicsDevice, null);
		b.VertexColorEnabled = true;
		viewMatrix = Matrix.CreateLookAt(new Vector3(0f, 0f, 1f), Vector3.Zero, Vector3.Up);
		float num = 1280f / (float)base.GraphicsDevice.Viewport.Width * (float)base.GraphicsDevice.Viewport.Height;
		projectionRect = new Rectangle(0, (int)(((float)base.GraphicsDevice.Viewport.Height - num) / 2f), 1280, (int)num);
		projectionMatrix = Matrix.CreateOrthographicOffCenter(projectionRect.Left, projectionRect.Right, projectionRect.Bottom, projectionRect.Top, 1f, 1000f);
		SceneRenderer.SetProjectionRectSpace(projectionRect);
		cameraOffset = new Vector2(0f, 0f);
		worldMatrix = Matrix.CreateTranslation(0f, 0f, -1f);
		b.World = worldMatrix;
		b.View = viewMatrix;
		b.Projection = projectionMatrix;
		m_stkScreens = new List<Screen>();
		Last60FramesDrawTimer = new List<int>();
		avgDrawRate = 0.0;
		SoundEffect.MasterVolume = 0.7f;
		base.Initialize();
	}

	protected override void LoadContent()
	{
		MediaPlayer.IsRepeating = false;
		spriteBatch = new SpriteBatch(base.GraphicsDevice);
		SceneRenderer.SetGraphicsDevice(base.GraphicsDevice, graphics, b, spriteBatch, base.Content);
		SpriteManager.Initialize(base.GraphicsDevice, base.Content);
		SoundManager.Initialize(base.Content);
		CharacterResourceManager.Initialize(base.Content);
		GameScreen gameScreen = new GameScreen();
		new SplashScreen(5000);
		new LoadingScreen(gameScreen.GetPropPool());
		m_fps = SpriteManager.GetSprite("images/whitesquare", new Vector2(100f, 100f), 1f);
		m_fpsStrings = new string[61];
		for (int i = 0; i < 61; i++)
		{
			m_fpsStrings[i] = i.ToString();
		}
		screenSprite = SpriteManager.GetSprite("images/whitesquare", new Vector2(0f, 0f), 0.9f);
		screenSprite.Alpha = 0.5f;
		ParticleManager.Initialize();
	}

	protected override void UnloadContent()
	{
	}

	public static void ExitGame()
	{
		m_bExit = true;
	}

	public static bool TrialTimeUp()
	{
		return m_iTrialTimePlayed > 420000;
	}

	protected override void OnExiting(object sender, EventArgs args)
	{
		for (int i = 0; i < m_stkScreens.Count; i++)
		{
			if (m_stkScreens[i] is GameScreen)
			{
				((GameScreen)m_stkScreens[i]).EndPhysicsLoop();
			}
		}
		base.OnExiting(sender, args);
	}

	protected override void Update(GameTime gameTime)
	{
		Profiler.StartTimer(ProfileTypes.PROF_UPDATE);
		if (!m_bMessageDisplayed)
		{
			m_iTimePlayed += gameTime.ElapsedGameTime.Milliseconds;
		}
		if (IsTrial())
		{
			m_iTrialTimePlayed += gameTime.ElapsedGameTime.Milliseconds;
		}
		if (m_bExit)
		{
			Exit();
		}
		HandleSaveLoadOptions();
		m_tUpdateTracker.Update(gameTime);
		ControlManager.UpdateInput(gameTime);
		Mp3MusicPlayer.Update(m_tUpdateTracker, Guide.IsVisible);
		if (!Guide.IsVisible)
		{
			SoundManager.Update(m_tUpdateTracker);
			m_stkScreens.Last().HandleInput(m_tUpdateTracker);
			int num = m_stkScreens.Count - 1;
			while (m_stkScreens[num].HandleInputParent)
			{
				num--;
				m_stkScreens[num].HandleInput(m_tUpdateTracker);
			}
			m_stkScreens.Last().Update(m_tUpdateTracker);
			num = m_stkScreens.Count - 1;
			while (m_stkScreens[num].UpdateParent)
			{
				num--;
				m_stkScreens[num].Update(m_tUpdateTracker);
			}
			ParticleManager.Update(m_tUpdateTracker);
		}
		SpawnTestDataScreen();
		if (m_avatar != null)
		{
			m_avatar.Update();
		}
		base.Update(gameTime);
		Profiler.EndTimer(ProfileTypes.PROF_UPDATE);
	}

	protected override void Draw(GameTime gameTime)
	{
		Profiler.StartTimer(ProfileTypes.PROF_DRAW);
		while (Last60FramesDrawTimer.Count >= 20)
		{
			Last60FramesDrawTimer.RemoveAt(0);
		}
		Last60FramesDrawTimer.Add(gameTime.ElapsedGameTime.Milliseconds);
		avgDrawRate = 0.0;
		foreach (int item in Last60FramesDrawTimer)
		{
			avgDrawRate += item;
		}
		avgDrawRate /= Last60FramesDrawTimer.Count();
		m_tDrawTracker.Update(gameTime);
		base.GraphicsDevice.Clear(new Color(33, 28, 43));
		base.GraphicsDevice.SamplerStates[0].AddressU = TextureAddressMode.Wrap;
		base.GraphicsDevice.SamplerStates[0].AddressV = TextureAddressMode.Wrap;
		GameBeginDraw();
		SceneRenderer.SetActiveTexture(null);
		spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None, SceneRenderer.GetBatchMatrix());
		SceneRenderer.IniFrame(b: false);
		SceneRenderer.GetEffect();
		int num = m_stkScreens.Count - 1;
		while (num >= 0 && m_stkScreens[num].DrawParent)
		{
			num--;
			if (num >= 0)
			{
				SpriteManager.Draw(gameTime);
				spriteBatch.End();
				GameEndDraw();
				GameBeginDraw();
				SceneRenderer.SetActiveTexture(null);
				spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None, SceneRenderer.GetBatchMatrix());
				SceneRenderer.IniFrame(b: false);
				m_stkScreens[num].Draw(m_tDrawTracker);
			}
		}
		SpriteManager.Draw(gameTime);
		spriteBatch.End();
		GameEndDraw();
		GameBeginDraw();
		SceneRenderer.SetActiveTexture(null);
		spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None, SceneRenderer.GetBatchMatrix());
		SceneRenderer.IniFrame(b: false);
		m_stkScreens.Last().Draw(m_tDrawTracker);
		ParticleManager.Draw(m_tDrawTracker);
		_ = Color.LightBlue;
		if (gameTime.ElapsedGameTime.Milliseconds > 300)
		{
			_ = Color.Red;
		}
		if (1000.0 / avgDrawRate >= 59.0)
		{
			m_fps.Color = Color.Lime;
		}
		else if (1000.0 / avgDrawRate >= 50.0)
		{
			m_fps.Color = Color.Yellow;
		}
		else if (1000.0 / avgDrawRate >= 40.0)
		{
			m_fps.Color = Color.Red;
		}
		else
		{
			m_fps.Color = Color.Purple;
		}
		string output = Profiler.GetOutput();
		SceneRenderer.DrawString(fonts.OUTLINE_FONT, output, SceneRenderer.GetCameraPosition() - new Vector2(40f, 220f), Color.White, 9000f);
		SpriteManager.Draw(gameTime);
		spriteBatch.End();
		GameEndDraw();
		base.Draw(gameTime);
		Profiler.EndTimer(ProfileTypes.PROF_DRAW);
	}

	private void GameBeginDraw()
	{
	}

	private void GameEndDraw()
	{
	}

	public static void PushScreen(Screen s)
	{
		m_stkScreens.Add(s);
	}

	public static void PopScreen(string s)
	{
		m_stkScreens.Remove(m_stkScreens.Last());
		if (m_stkScreens.Count > 0)
		{
			m_stkScreens.Last().OnRegainFocus(s);
		}
	}

	public static Screen PeekScreen()
	{
		return m_stkScreens.Last();
	}

	public static void ResetLevelsWon()
	{
	}

	public static void ResetProgress()
	{
		ResetLevelsWon();
	}

	public static void SpawnTestDataScreen()
	{
	}

	public static int GetTimePlayed()
	{
		return m_iTimePlayed / 1000 / 60;
	}

	public static void ResetSaveState()
	{
		getDeviceResult = null;
		m_StorageDevice = null;
		m_bSaveOptionsRequested = false;
		m_bLoadOptionsRequested = false;
		m_bAdvancedOptions = false;
		m_bIsShowingDeviceSelector = false;
	}

	public static StorageDevice GetStorageDevice(out bool isRetrieving)
	{
		bool flag = false;
		if (m_StorageDevice != null && !flag)
		{
			isRetrieving = false;
			return m_StorageDevice;
		}
		if (m_StorageDevice != null)
		{
			isRetrieving = false;
			return m_StorageDevice;
		}
		if (getDeviceResult == null && !Guide.IsVisible)
		{
			getDeviceResult = Guide.BeginShowStorageDeviceSelector(null, null);
			m_bIsShowingDeviceSelector = true;
		}
		if (getDeviceResult.IsCompleted)
		{
			if (m_bIsShowingDeviceSelector)
			{
				m_StorageDevice = Guide.EndShowStorageDeviceSelector(getDeviceResult);
				m_bIsShowingDeviceSelector = false;
			}
			isRetrieving = false;
			return m_StorageDevice;
		}
		isRetrieving = true;
		return null;
	}

	public static bool IsTrial()
	{
		return Guide.IsTrialMode;
	}

	public static void ResetDamageRegistry()
	{
		sm_bIsDamaged = false;
	}

	public static void RegisterDamage()
	{
		sm_bIsDamaged = true;
	}

	public static bool NoDamageRegistered()
	{
		return !sm_bIsDamaged;
	}

	public static void SaveGlobalOptions()
	{
		if (!Guide.IsVisible && !m_bSaveOptionsRequested)
		{
			m_bSaveOptionsRequested = true;
			GetStorageDevice(out var _);
		}
	}

	public static void LoadGlobalOptions()
	{
		if (!Guide.IsVisible && !m_bLoadOptionsRequested)
		{
			m_bLoadOptionsRequested = true;
			GetStorageDevice(out var _);
		}
	}

	public void HandleSaveLoadOptions()
	{
		if (m_bSaveOptionsRequested)
		{
			try
			{
				StorageDevice storageDevice = GetStorageDevice(out var isRetrieving);
				if (!isRetrieving && storageDevice != null && storageDevice.IsConnected)
				{
					optionSet optionSet2 = new optionSet
					{
						SaveVersion = 1,
						MusicVol = (int)Math.Round(Mp3MusicPlayer.Volume * 10f),
						SFXVol = (int)Math.Round(SFXVolume * 10f),
						highscoreNames = m_highScoreNames,
						highscoreNums = m_highScores,
						MessageDisplayed = m_bMessageDisplayed,
						MessageDisplayMinute = m_iMessageDisplayMinute,
						TimePlayed = m_iTimePlayed
					};
					StorageContainer storageContainer = storageDevice.OpenContainer("BabyMakerStorage");
					string path = Path.Combine(storageContainer.Path, "SaveData.sav");
					FileStream fileStream = File.Open(path, FileMode.Create);
					XmlSerializer xmlSerializer = new XmlSerializer(typeof(optionSet));
					xmlSerializer.Serialize(fileStream, optionSet2);
					fileStream.Close();
					storageContainer.Dispose();
				}
				if (!isRetrieving)
				{
					m_bSaveOptionsRequested = false;
					UpdateCanSave();
					if (!CanSave())
					{
						DisplaySaveFailure();
					}
				}
				return;
			}
			catch (Exception)
			{
				return;
			}
		}
		if (!m_bLoadOptionsRequested)
		{
			return;
		}
		StorageDevice storageDevice2 = GetStorageDevice(out var isRetrieving2);
		if (!isRetrieving2 && storageDevice2 != null && storageDevice2.IsConnected)
		{
			try
			{
				StorageContainer storageContainer2 = storageDevice2.OpenContainer("BabyMakerStorage");
				string path2 = Path.Combine(storageContainer2.Path, "SaveData.sav");
				m_bCanSave = true;
				if (File.Exists(path2))
				{
					FileStream fileStream2 = File.Open(path2, FileMode.Open);
					optionSet optionSet3 = default(optionSet);
					XmlSerializer xmlSerializer2 = new XmlSerializer(typeof(optionSet));
					optionSet3 = (optionSet)xmlSerializer2.Deserialize(fileStream2);
					if (optionSet3.SaveVersion == 1)
					{
						Mp3MusicPlayer.Volume = (float)optionSet3.MusicVol / 10f;
						SFXVolume = (float)optionSet3.SFXVol / 10f;
						m_bMessageDisplayed = optionSet3.MessageDisplayed;
						m_iMessageDisplayMinute = optionSet3.MessageDisplayMinute;
						m_iTimePlayed = optionSet3.TimePlayed;
						m_highScoreNames = optionSet3.highscoreNames;
						m_highScores = optionSet3.highscoreNums;
					}
					else
					{
						SetOptionsDefault();
					}
					fileStream2.Close();
				}
				else
				{
					SetOptionsDefault();
				}
				storageContainer2.Dispose();
			}
			catch (Exception)
			{
				SetOptionsDefault();
			}
		}
		else if (!isRetrieving2)
		{
			SetOptionsDefault();
			m_bCanSave = false;
		}
		if (!isRetrieving2)
		{
			m_bLoadOptionsRequested = false;
			m_bDisplayedFailure = false;
		}
	}

	public static bool CanSave()
	{
		return m_bCanSave;
	}

	public static void UpdateCanSave()
	{
		if (m_bCanSave)
		{
			StorageDevice storageDevice = GetStorageDevice(out var isRetrieving);
			if (!isRetrieving && (storageDevice == null || !storageDevice.IsConnected))
			{
				m_bCanSave = false;
			}
		}
	}

	public static void DisplaySaveFailure()
	{
		if (!m_bDisplayedFailure)
		{
			m_bDisplayedFailure = true;
			new GenericErrScreen(ControlManager.ActiveMenuIndex, "Either your memory device got\ndisconnected at some point or\nyou are not using a profile\nthat can save.\nSaving of progress has \nbeen disabled.");
		}
	}

	public static void SetOptionsDefault()
	{
		m_bMessageDisplayed = false;
		m_iMessageDisplayMinute = 8;
		m_iTimePlayed = 0;
		SFXVolume = 0.4f;
		Mp3MusicPlayer.Volume = 0.4f;
		m_highScoreNames = new List<string>();
		m_highScores = new List<int>();
		for (int num = 20; num > 0; num--)
		{
			m_highScoreNames.Add("Bob McGee");
			m_highScores.Add(num * 100);
		}
	}

	public static void AddHighScore(string name, int value)
	{
		for (int i = 0; i < m_highScores.Count; i++)
		{
			if (m_highScores[i] < value)
			{
				m_highScores.Insert(i, value);
				m_highScoreNames.Insert(i, name);
				m_highScores.RemoveAt(m_highScores.Count - 1);
				m_highScoreNames.RemoveAt(m_highScoreNames.Count - 1);
				break;
			}
		}
	}

	public static List<string> GetHighScoreNames()
	{
		return m_highScoreNames;
	}

	public static List<int> GetHighScores()
	{
		return m_highScores;
	}

	public static bool CanSendMessageToFriend()
	{
		PlayerIndex playerIndex = ControlManager.GetPlayerIndex(ControlManager.ActiveMenuIndex);
		SignedInGamer signedInGamer = null;
		foreach (SignedInGamer signedInGamer2 in Gamer.SignedInGamers)
		{
			if (signedInGamer2.PlayerIndex == playerIndex)
			{
				signedInGamer = signedInGamer2;
			}
		}
		if (signedInGamer == null || !signedInGamer.IsSignedInToLive || signedInGamer.Privileges.AllowCommunication == GamerPrivilegeSetting.Blocked)
		{
			return false;
		}
		return true;
	}

	public static void SendMessageToFriend(string text)
	{
		PlayerIndex playerIndex = ControlManager.GetPlayerIndex(ControlManager.ActiveMenuIndex);
		SignedInGamer signedInGamer = null;
		foreach (SignedInGamer signedInGamer2 in Gamer.SignedInGamers)
		{
			if (signedInGamer2.PlayerIndex == playerIndex)
			{
				signedInGamer = signedInGamer2;
			}
		}
		if (signedInGamer == null || !signedInGamer.IsSignedInToLive || signedInGamer.Privileges.AllowCommunication == GamerPrivilegeSetting.Blocked)
		{
			new GenericErrScreen(ControlManager.ActiveMenuIndex, "The controller you are\ntrying to use is not using\nan account that can message\nother users.\nPlease use an XBox Live account\ncapable of messaging.");
		}
		else
		{
			Guide.ShowComposeMessage(ControlManager.GetPlayerIndex(ControlManager.ActiveMenuIndex), text, null);
		}
	}

	public static void ShowPurchaseScreen(int controlIndex)
	{
		PlayerIndex playerIndex = ControlManager.GetPlayerIndex(controlIndex);
		SignedInGamer signedInGamer = null;
		foreach (SignedInGamer signedInGamer2 in Gamer.SignedInGamers)
		{
			if (signedInGamer2.PlayerIndex == playerIndex)
			{
				signedInGamer = signedInGamer2;
			}
		}
		if (signedInGamer == null || !signedInGamer.IsSignedInToLive || !signedInGamer.Privileges.AllowPurchaseContent)
		{
			new GenericErrScreen(controlIndex, "The controller you are trying\nto use is not using an account\nthat can purchase XBox Indie Games.\nPlease use an XBox Live account\ncapable of purchasing content.");
		}
		else
		{
			Guide.ShowMarketplace(playerIndex);
		}
	}

	public static Color GetPurchaseColor()
	{
		return new Color(114, byte.MaxValue, 37);
	}

	public static int GetCurrentLevel()
	{
		return m_iLevelReached;
	}

	public static void SetLevel(int i)
	{
		m_iLevelReached = i;
	}

	public static int GetPlayPercent()
	{
		return (int)(100f * (float)m_iPlayTime / (float)(m_iMessageDisplayMinute * 1000 * 60));
	}

	public static void RecordPlayTime(int i)
	{
		m_iPlayTime += i;
	}

	public static int GetCoinsCollected()
	{
		return m_iCoinsCollected;
	}

	public static void SetCoin(int i)
	{
		m_iCoinsCollected = i;
	}

	public static int GetEnemiesDestroyed()
	{
		return m_iEnemiesKilled;
	}

	public static void RegisterEnemyDeath()
	{
		m_iEnemiesKilled++;
	}

	public static int GetPlayerDeaths()
	{
		return m_iPlayerDeaths;
	}

	public static void PlayerDeath()
	{
		m_iPlayerDeaths++;
	}

	public static string GetPlayerName(int playerId)
	{
		PlayerIndex playerIndex = ControlManager.GetPlayerIndex(playerId);
		SignedInGamer signedInGamer = null;
		foreach (SignedInGamer signedInGamer2 in Gamer.SignedInGamers)
		{
			if (signedInGamer2.PlayerIndex == playerIndex)
			{
				signedInGamer = signedInGamer2;
			}
		}
		if (signedInGamer != null)
		{
			return signedInGamer.Gamertag;
		}
		return "Anonymous";
	}

	public static void LoadAvatar()
	{
		m_avatar = new AvatarHandler(ControlManager.ActiveMenuIndex);
		SceneRenderer.Avatar = m_avatar;
	}

	public static void ResetAvatar()
	{
		m_avatar = null;
		SceneRenderer.Avatar = m_avatar;
	}

	public static AvatarHandler GetAvatar()
	{
		return m_avatar;
	}
}
