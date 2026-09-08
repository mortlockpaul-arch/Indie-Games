using Microsoft.Xna.Framework;
using Quasar.GameUtils.Game;
using Quasar.GameUtils.Template;
using Quasar.Global;
using Quasar.Language;
using Quasar.Meshes;
using Quasar.Meshes.Text;
using Quasar.Scenes;

namespace Quasar.GameUtils.Sections;

public class TrialSection : ExtraSection
{
	private class TrialScene : Scene2D
	{
		private TextMesh trialText;

		private RenderItem ri;

		public TrialScene()
		{
			Layout2D.LayoutData layout = new Layout2D.LayoutData(new Vector2(Engine.GUIWidth * 0.4f - 110f, (0f - Engine.GUIHeight) * 0.425f), new Vector2(220f, 32f));
			Sized2DRectangleMesh m = new Sized2DRectangleMesh(layout, Vector3.Zero)
			{
				Alpha = 0.5f
			};
			trialText = new TextMesh(GameTemplate.StandardFont, new TextDrawProperties(0.75f, HorizontalAlignment.Right), 100, useStringBuilder: true);
			trialText.Offset = new Vector2(Engine.GUIWidth * 0.4f - 10f, (0f - Engine.GUIHeight) * 0.425f + 6f);
			ri = new RenderItem();
			ri.addMesh(m);
			ri.addMesh(trialText);
			Add(ri);
		}

		public override void Update()
		{
			int num = (int)((TrialTime - Timer.DefaultTimer.TotalTime) / 1000);
			if (num > 0 && PlatformInterface.Instance.IsTrial)
			{
				ri.Visible = BaseGame.Instance.CurrentGameSection.Id != 2 && BaseGame.Instance.CurrentGameSection.Id != 3;
			}
			else
			{
				ri.Visible = false;
			}
			if (ri.Visible)
			{
				trialText.StringBuilder.Length = 0;
				trialText.StringBuilder.Append("TRIAL_TIME_LEFT".Translate());
				trialText.StringBuilder.Append(": ");
				int num2 = num / 60;
				int num3 = num % 60;
				trialText.StringBuilder.AppendNumber(num2, 2, AppendNumberOptions.FixedSize);
				trialText.StringBuilder.Append(":");
				trialText.StringBuilder.AppendNumber(num3, 2, AppendNumberOptions.FixedSize);
			}
			base.Update();
		}
	}

	public static int TrialTime = 480000;

	private static TrialSection instance;

	private TrialScene scene;

	public static TrialSection Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new TrialSection();
			}
			return instance;
		}
	}

	private TrialSection()
		: base(Priority.Foreground)
	{
	}

	protected override void initScenes()
	{
		scene = new TrialScene();
		AddScene(scene, isDefault: true);
	}
}
