using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Quasar.Audios;
using Quasar.GameUtils.Meshes;
using Quasar.GameUtils.Player;
using Quasar.GameUtils.Template;
using Quasar.Global;
using Quasar.Language;
using Quasar.Meshes;
using Quasar.Meshes.Text;
using Quasar.Shaders;
using Quasar.Textures;

namespace Quasar.GameUtils.Awards;

internal class BaseAwardNotificationNode : AwardNotificationNode
{
	private const int IMAGE_SIZE = 64;

	private const int WIDTH = 500;

	private const int HEIGHT = 100;

	private const int APPEAR_TIME = 150;

	private const int DISAPPEAR_TIME = 150;

	private const int DURATION = 5000;

	private const int PADDING = 18;

	private const int IMAGE_PADDING = 5;

	private const int TITLE_Y = 32;

	private const int TEXT_Y = -8;

	private const int TEXT_X = 37;

	private const int BORDER = 4;

	private const int PROGRESS_WIDTH = 200;

	private const int PROGRESS_HEIGHT = 30;

	private const int PROGRESS_X = -63;

	private const int PROGRESS_Y = -15;

	private const int PROGRESS_PADDING_X = 16;

	private const int PROGRESS_PADDING_Y = 12;

	private const int PROGRESS_BAR_WIDTH = 168;

	private const int PROGRESS_BAR_HEIGHT = 6;

	private SimpleAudio achievementUnlockedAudio;

	private Sized2DRectangleMesh image;

	private Sized2DRectangleMesh bgImage;

	private Sized2DRectangleMesh progressBar;

	private TextMesh title;

	private TextMesh awardName;

	private TextMesh progressText;

	private RenderItem item;

	private HUDDetailRectangle progressBG;

	private long lastNotifyTime;

	private float scale = 1f;

	public override AwardsScene.NotificationState State
	{
		get
		{
			if (!Visible)
			{
				return AwardsScene.NotificationState.Hidden;
			}
			return AwardsScene.NotificationState.Showing;
		}
	}

	public BaseAwardNotificationNode()
	{
		achievementUnlockedAudio = new SimpleAudio(Engine.ContentManager.Load<SoundEffect>("Sounds/AwardUnlocked"));
		item = new RenderItem();
		HUDRectangle m = new HUDRectangle(new Vector2(500f, 100f));
		item.addMesh(m);
		progressBG = new HUDDetailRectangle(new Vector2(200f, 30f));
		progressBG.Offset = new Vector2(-63f, -15f);
		item.addMesh(progressBG);
		progressBar = new Sized2DRectangleMesh(new Vector2(168f, 6f), GameTemplate.DialogTitleColor);
		progressBar.Offset = new Vector2(-63f, -15f);
		item.addMesh(progressBar);
		progressText = new TextMesh(GameTemplate.StandardFont, new TextDrawProperties(new Vector2(42f, -8f), 0.9f, HorizontalAlignment.Left), 40, useStringBuilder: false);
		progressText.Text = "5 / 45";
		progressText.Diffuse = Vector3.One;
		item.addMesh(progressText);
		title = new TextMesh(GameTemplate.StandardFont, new TextDrawProperties(new Vector2(37f, 32f), HorizontalAlignment.Center), 30, useStringBuilder: false);
		title.Text = "Award name description";
		title.Diffuse = GameTemplate.DialogTitleColor;
		item.addMesh(title);
		awardName = new TextMesh(GameTemplate.StandardFont, new TextDrawProperties(new Vector2(37f, -8f), 0.9f, HorizontalAlignment.Center), 40, useStringBuilder: false);
		awardName.Text = "Award unlocked!";
		awardName.Diffuse = Vector3.One;
		item.addMesh(awardName);
		bgImage = new Sized2DRectangleMesh(new Vector2(72f), TextureManager.Textures["Awards/PlayerIndex"]);
		bgImage.Shader = ShaderManager.Shaders["GUIRotate"];
		bgImage.FirstMaterial.AddIntParameter(0);
		bgImage.Offset = new Vector2(-200f, 0f);
		item.addMesh(bgImage);
		image = new Sized2DRectangleMesh(new Vector2(64f), null);
		image.Offset = new Vector2(-200f, 0f);
		item.addMesh(image);
		item.Transform.Translation = new Vector3(0f, (0f - Engine.GUIHeight) * 0.35f, 0f);
		Visible = false;
		item.Transform.Scale = Vector3.Zero;
		addChild(item);
	}

	public override void Notify(AwardsScene.PendingNotification notification)
	{
		Award award = notification.award.Award;
		bool isUnlocked = notification.award.IsUnlocked;
		awardName.Text = award.Name;
		title.Text = award.Name;
		PlayerIndex playerIndex;
		if (!notification.progress.IsDummy)
		{
			Quasar.GameUtils.Player.Player.IsSignedIn(notification.progress.Gamertag, out playerIndex);
		}
		else
		{
			playerIndex = notification.progress.PlayerIndex;
		}
		bgImage.FirstMaterial.SetIntParameter(0, (int)playerIndex);
		if (isUnlocked)
		{
			awardName.Text = LanguageManager.Texts["AWARD_UNLOCKED"];
			progressText.Text = "";
			progressBar.Size = Vector2.Zero;
			progressBar.Alpha = 0f;
			progressBG.Alpha = 0f;
			achievementUnlockedAudio.Start();
		}
		else
		{
			string text = string.Format("{0} / {1}", GameMath.UIntToString(notification.award.Progress, "#,0"), GameMath.UIntToString(award.ProgressNeeded, "#,0"));
			progressText.Text = text;
			progressText.Scale = ((text.Length <= 15) ? 0.9f : ((text.Length > 25) ? 0.5f : 0.6f));
			float num = 168f * notification.award.Percentage;
			progressBar.Size = new Vector2(num, 6f);
			progressBar.Offset = new Vector2(-63f + (-168f + num) * 0.5f, -15f);
			progressBar.Diffuse = GameTemplate.DialogTitleColor;
			progressBar.Alpha = 1f;
			progressBG.Alpha = 1f;
			progressBG.Diffuse = Vector3.One;
			awardName.Text = "";
		}
		image.FirstMaterial.Texture = award.Texture;
		lastNotifyTime = Timer.DefaultTimer.TotalTime;
		scale = 0f;
		Visible = true;
	}

	protected override void DoUpdate()
	{
		if (Visible)
		{
			long num = Timer.DefaultTimer.TotalTime - lastNotifyTime;
			if (num < 150)
			{
				scale = GameMath.Interpolate(0f, 1f, (float)num / 150f);
			}
			else if (num < 4850)
			{
				scale = 1f;
			}
			else if (num < 5000)
			{
				scale = GameMath.Interpolate(1f, 0f, (float)(num - 5000 + 150) / 150f);
			}
			else
			{
				Visible = false;
			}
			item.Transform.Scale = new Vector3(scale);
		}
		base.DoUpdate();
	}

	public override void Dispose()
	{
		if (achievementUnlockedAudio != null)
		{
			achievementUnlockedAudio.Dispose();
		}
		achievementUnlockedAudio = null;
		base.Dispose();
	}
}
