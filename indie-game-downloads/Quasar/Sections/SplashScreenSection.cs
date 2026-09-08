using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Quasar.GameUtils.Sections;
using Quasar.Global;
using Quasar.Items._2D;
using Quasar.Scenes;
using Quasar.Shaders;

namespace Quasar.Sections;

public abstract class SplashScreenSection : GameSection
{
	public const long APPEAR_TIME = 1125L;

	public const long STAY_TIME = 1500L;

	public const long DISAPPEAR_TIME = 1125L;

	public static string Shader = "SplashScreen";

	private Texture2D texture;

	private long initTime;

	private float sizeInc = 0.035f;

	private long appearTime = 1125L;

	private long stayTime = 1500L;

	private long disappearTime = 1125L;

	protected Quasar.Items._2D.Rectangle rect;

	protected float progressElapsed;

	protected Texture2D Texture => texture;

	public float SizeIncrement
	{
		get
		{
			return sizeInc;
		}
		set
		{
			sizeInc = value;
		}
	}

	public long AppearTime
	{
		get
		{
			return appearTime;
		}
		set
		{
			appearTime = value;
		}
	}

	public long StayTime
	{
		get
		{
			return stayTime;
		}
		set
		{
			stayTime = value;
		}
	}

	public long DisappearTime
	{
		get
		{
			return disappearTime;
		}
		set
		{
			disappearTime = value;
		}
	}

	public SplashScreenSection(int id, Texture2D texture)
		: base(id)
	{
		this.texture = texture;
	}

	protected override void initScenes()
	{
		Scene2D scene2D = new Scene2D();
		float num = Engine.GUIWidth * 9f / 16f;
		rect = new Quasar.Items._2D.Rectangle(texture, new Vector2(Engine.GUIWidth, num));
		rect.Diffuse = Vector3.Zero;
		rect.Mesh.FirstMaterial.Ambient = Vector3.One;
		rect.Alpha = 1f;
		rect.Mesh.FirstMaterial.SetForcedAlpha(alpha: false);
		rect.Mesh.Shader = ShaderManager.Shaders[Shader];
		scene2D.Add(rect);
		float num2 = (Engine.GUIHeight - num) * 0.5f;
		Quasar.Items._2D.Rectangle rectangle = new Quasar.Items._2D.Rectangle(new Vector2(Engine.GUIWidth, num2), new Vector4(Vector3.Zero, 1f));
		rectangle.RectangleMesh.Offset = new Vector2(0f, (num + num2) * 0.5f);
		scene2D.Add(rectangle);
		rectangle = new Quasar.Items._2D.Rectangle(new Vector2(Engine.GUIWidth, num2), new Vector4(Vector3.Zero, 1f));
		rectangle.RectangleMesh.Offset = new Vector2(0f, (0f - (num + num2)) * 0.5f);
		scene2D.Add(rectangle);
		AddScene(scene2D, isDefault: true);
		initTime = Timer.DefaultTimer.CurrentTotalTime;
	}

	protected override void initRenderProcesses()
	{
		base.initRenderProcesses();
		base.MainRenderPass.MustClearColor = true;
		base.MainRenderPass.BackgroundColor = Color.Black;
	}

	protected abstract void GoToNextSection();

	public override void MainLoop()
	{
		float num = 0f;
		long num2 = Timer.DefaultTimer.TimeSince(initTime);
		if (num2 <= appearTime)
		{
			num = GameMath.Clamp(0f, 1f, GameMath.Interpolate(0f, 1f, (float)num2 / (float)appearTime));
		}
		else if (num2 <= appearTime + stayTime)
		{
			num = 1f;
		}
		else if (num2 <= appearTime + stayTime + disappearTime)
		{
			num = GameMath.Interpolate(1f, 0f, (float)(num2 - appearTime - stayTime) / (float)disappearTime);
		}
		else
		{
			num = 0f;
			GoToNextSection();
		}
		progressElapsed = (float)num2 / (float)(appearTime + stayTime + disappearTime);
		rect.Transform.Scale = new Vector3(1f + progressElapsed * sizeInc, 1f + progressElapsed * sizeInc, 1f);
		rect.Alpha = num;
	}
}
