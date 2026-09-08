using Microsoft.Xna.Framework;
using Quasar;
using Quasar.GameUtils.Template;
using Quasar.GameUtils.Template.Controls;
using Quasar.Global;
using Quasar.Language;
using Quasar.Meshes.Text;

namespace AvatarFarmOnline.Template.Controls;

public class PressAToStartItem : RenderItem
{
	private PressAToStart pressAToStart;

	private TextMesh text;

	public PressAToStartItem(PressAToStart pressAToStart)
	{
		text = new TextMesh(GameTemplate.StandardFont, new TextDrawProperties(HorizontalAlignment.Center), 50, useStringBuilder: false);
		text.Text = "PRESS_A_TO_CONTINUE".Translate();
		text.Offset = new Vector2(0f, (0f - Engine.GUIHeight) * 0.35f);
		addMesh(text);
		this.pressAToStart = pressAToStart;
	}

	protected override void DoUpdate()
	{
		Visible = pressAToStart.Visible;
		base.DoUpdate();
	}

	public override void Dispose()
	{
		pressAToStart = null;
		base.Dispose();
	}
}
