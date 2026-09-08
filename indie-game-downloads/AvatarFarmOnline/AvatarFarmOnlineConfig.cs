using AvatarFarmOnline.Logic.Stage;
using AvatarFarmOnline.Scores;
using Quasar.GameUtils.Config;

namespace AvatarFarmOnline;

internal class AvatarFarmOnlineConfig : ConfigFile
{
	private const string INVERT_Y_AXIS = "Invert_Y_Axis";

	private const string P2P_SCORES_ENABLED = "P2P_Scores_Enabled";

	private const string CAMERA = "Camera";

	private static AvatarFarmOnline.AvatarFarmOnlineConfig instance;

	private GeneralConfigSection generalConfig;

	public static AvatarFarmOnline.AvatarFarmOnlineConfig Instance => instance;

	public static bool VibrationEnabled
	{
		get
		{
			return instance.generalConfig.VibrationEnabled;
		}
		set
		{
			instance.generalConfig.VibrationEnabled = value;
		}
	}

	public bool P2PEnabled
	{
		get
		{
			return GetItem<bool>("P2P_Scores_Enabled").Value;
		}
		set
		{
			GetItem<bool>("P2P_Scores_Enabled").Value = value;
		}
	}

	public AvatarFarmOnline.Logic.Stage.LocalPlayer.CameraStates CameraMode
	{
		get
		{
			return (AvatarFarmOnline.Logic.Stage.LocalPlayer.CameraStates)GetItem<int>("Camera").Value;
		}
		set
		{
			GetItem<int>("Camera").Value = (int)value;
		}
	}

	public bool InvertYAxis
	{
		get
		{
			return GetItem<bool>("Invert_Y_Axis").Value;
		}
		set
		{
			GetItem<bool>("Invert_Y_Axis").Value = value;
		}
	}

	static AvatarFarmOnlineConfig()
	{
		instance = new AvatarFarmOnline.AvatarFarmOnlineConfig();
	}

	public AvatarFarmOnlineConfig()
		: base("config.xml", "Config")
	{
		generalConfig = new GeneralConfigSection(1f, 0.6f);
		AddItem(generalConfig);
		AddItem(ConfigParameter.CreateIntParameter("Camera", 0));
		AddItem(ConfigParameter.CreateBoolParameter("Invert_Y_Axis", defaultValue: false));
		AddItem(ConfigParameter.CreateBoolParameter("P2P_Scores_Enabled", defaultValue: true));
	}

	public static void SaveConfig()
	{
		instance.Save();
	}

	public static void LoadConfig()
	{
		instance.Load();
	}

	protected override void Gather()
	{
		base.Gather();
		GetItem<bool>("P2P_Scores_Enabled").Value = AvatarFarmOnline.Scores.GameScoreManager.EnableP2P;
	}

	public override void Apply()
	{
		base.Apply();
		AvatarFarmOnline.Scores.GameScoreManager.EnableP2P = GetItem<bool>("P2P_Scores_Enabled").Value;
	}
}
