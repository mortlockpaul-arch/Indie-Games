#define TRACE
using System;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Storage;
using RacingGame.Helpers;

namespace RacingGame.Properties;

[Serializable]
public class GameSettings
{
	private const string SettingsFilename = "RacingGameSettings.xml";

	public const int MinimumResolutionWidth = 640;

	public const int MinimumResolutionHeight = 480;

	private static GameSettings defaultInstance = new GameSettings();

	private static bool needSave = false;

	private string highscores = "";

	public static string playerName = "Player";

	private int resolutionWidth;

	private int resolutionHeight;

	private bool fullscreen = true;

	private bool postScreenEffects = true;

	private bool shadowMapping = true;

	private bool highDetail = true;

	private float soundVolume = 0.8f;

	private float musicVolume = 0.6f;

	private float controllerSensitivity = 0.5f;

	public static GameSettings Default => defaultInstance;

	public string Highscores
	{
		get
		{
			return highscores;
		}
		set
		{
			if (highscores != value)
			{
				needSave = true;
			}
			highscores = value;
		}
	}

	public string PlayerName
	{
		get
		{
			return playerName;
		}
		set
		{
			if (playerName != value)
			{
				needSave = true;
			}
			playerName = value;
		}
	}

	public int ResolutionWidth
	{
		get
		{
			return resolutionWidth;
		}
		set
		{
			if (resolutionWidth != value)
			{
				needSave = true;
			}
			resolutionWidth = value;
		}
	}

	public int ResolutionHeight
	{
		get
		{
			return resolutionHeight;
		}
		set
		{
			if (resolutionHeight != value)
			{
				needSave = true;
			}
			resolutionHeight = value;
		}
	}

	public bool Fullscreen
	{
		get
		{
			return fullscreen;
		}
		set
		{
			if (fullscreen != value)
			{
				needSave = true;
			}
			fullscreen = value;
		}
	}

	public bool PostScreenEffects
	{
		get
		{
			return postScreenEffects;
		}
		set
		{
			if (postScreenEffects != value)
			{
				needSave = true;
			}
			postScreenEffects = value;
		}
	}

	public bool ShadowMapping
	{
		get
		{
			return shadowMapping;
		}
		set
		{
			if (shadowMapping != value)
			{
				needSave = true;
			}
			shadowMapping = value;
		}
	}

	public bool HighDetail
	{
		get
		{
			return highDetail;
		}
		set
		{
			if (highDetail != value)
			{
				needSave = true;
			}
			highDetail = value;
		}
	}

	public float SoundVolume
	{
		get
		{
			return soundVolume;
		}
		set
		{
			if (soundVolume != value)
			{
				needSave = true;
			}
			soundVolume = value;
		}
	}

	public float MusicVolume
	{
		get
		{
			return musicVolume;
		}
		set
		{
			if (musicVolume != value)
			{
				needSave = true;
			}
			musicVolume = value;
		}
	}

	public float ControllerSensitivity
	{
		get
		{
			return controllerSensitivity;
		}
		set
		{
			if (controllerSensitivity != value)
			{
				needSave = true;
			}
			controllerSensitivity = value;
		}
	}

	private GameSettings()
	{
	}

	public static void Initialize()
	{
		Load();
	}

	public static void Load()
	{
		bool flag = false;
		needSave = false;
		FileHelper.StorageContainerMRE.WaitOne();
		FileHelper.StorageContainerMRE.Reset();
		try
		{
			StorageDevice xnaUserDevice = FileHelper.XnaUserDevice;
			if (xnaUserDevice != null && xnaUserDevice.IsConnected)
			{
				StorageContainer val = xnaUserDevice.OpenContainer("2060 Cyber Racer");
				try
				{
					string path = Path.Combine(val.Path, "RacingGameSettings.xml");
					if (File.Exists(path))
					{
						using FileStream fileStream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
						if (fileStream.Length > 0)
						{
							GameSettings gameSettings = (GameSettings)new XmlSerializer(typeof(GameSettings)).Deserialize(fileStream);
							if (gameSettings != null)
							{
								defaultInstance = gameSettings;
							}
						}
						else
						{
							needSave = true;
							flag = true;
						}
					}
					else
					{
						needSave = true;
					}
				}
				finally
				{
					((IDisposable)val)?.Dispose();
				}
			}
		}
		catch (Exception ex)
		{
			Trace.WriteLine("Settings Load Failure: " + ex.ToString());
		}
		FileHelper.StorageContainerMRE.Set();
		if (flag)
		{
			Save();
		}
	}

	public static void Save()
	{
		if (!needSave)
		{
			return;
		}
		needSave = false;
		FileHelper.StorageContainerMRE.WaitOne();
		FileHelper.StorageContainerMRE.Reset();
		try
		{
			StorageDevice xnaUserDevice = FileHelper.XnaUserDevice;
			if (xnaUserDevice != null && xnaUserDevice.IsConnected)
			{
				StorageContainer val = xnaUserDevice.OpenContainer("2060 Cyber Racer");
				try
				{
					string path = Path.Combine(val.Path, "RacingGameSettings.xml");
					using FileStream stream = File.Open(path, FileMode.Create, FileAccess.Write);
					new XmlSerializer(typeof(GameSettings)).Serialize(stream, defaultInstance);
				}
				finally
				{
					((IDisposable)val)?.Dispose();
				}
			}
		}
		catch (Exception ex)
		{
			Trace.WriteLine("Settings Load Failure: " + ex.ToString());
		}
		FileHelper.StorageContainerMRE.Set();
	}

	public static void SetMinimumGraphics()
	{
		Default.ResolutionWidth = 640;
		Default.ResolutionHeight = 480;
		Default.ShadowMapping = false;
		Default.HighDetail = false;
		Default.PostScreenEffects = false;
		Save();
	}
}
