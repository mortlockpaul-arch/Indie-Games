using Microsoft.Xna.Framework;
using Quasar.GUI;
using Quasar.Global;
using Quasar.Items._2D;
using Quasar.Meshes.Text;

namespace Quasar.GameUtils.Template;

public class LayoutTitle : BasicText
{
	private Layout layout;

	public LayoutTitle(Layout layout)
		: base(GameTemplate.TitleFont, new TextDrawProperties(HorizontalAlignment.Center), 50)
	{
		Transform.Translation = new Vector3(0f, Engine.GUIHeight * 0.4f, 0f);
		this.layout = layout;
		Text = layout.Title;
		Diffuse = GameTemplate.TitleColor;
	}

	protected override void DoUpdate()
	{
		Text = layout.Title;
		base.DoUpdate();
	}
}
