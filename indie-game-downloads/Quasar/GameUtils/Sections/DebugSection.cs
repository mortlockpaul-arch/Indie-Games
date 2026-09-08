using System.Text;
using Microsoft.Xna.Framework;
using Quasar.GameUtils.Template;
using Quasar.Global;
using Quasar.Meshes.Text;
using Quasar.Scenes;

namespace Quasar.GameUtils.Sections;

public class DebugSection : ExtraSection
{
	private class DebugScene : Scene2D
	{
		private TextBoxMesh debugText;

		public TextBoxMesh DebugText => debugText;

		public DebugScene()
		{
			debugText = new TextBoxMesh(GameTemplate.StandardFont, new TextBoxDrawProperties(new Vector2(Engine.GUIWidth, Engine.GUIHeight * 0.5f), 0.75f, HorizontalAlignment.Left, VerticalAlignment.Top), 512, useStringBuilder: true);
			debugText.Offset = new Vector2(0f, 0f);
			Add(new RenderItem(debugText));
		}

		public override void Render()
		{
			base.Render();
			debugText.StringBuilder.Length = 0;
		}
	}

	private static DebugSection instance;

	private DebugScene scene;

	public static DebugSection Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new DebugSection();
			}
			return instance;
		}
	}

	public StringBuilder DebugStringBuilder => scene.DebugText.StringBuilder;

	private DebugSection()
		: base(Priority.Foreground)
	{
	}

	protected override void initScenes()
	{
		scene = new DebugScene();
		AddScene(scene, isDefault: true);
	}
}
