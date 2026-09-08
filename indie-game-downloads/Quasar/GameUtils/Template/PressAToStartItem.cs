using Quasar.GameUtils.Template.Controls;
using Quasar.Language;
using Quasar.Meshes.Text;

namespace Quasar.GameUtils.Template;

public class PressAToStartItem : RenderItem
{
	private PressAToStart pressAToStart;

	private TextMesh text;

	public PressAToStartItem(PressAToStart pressAToStart)
	{
		text = new TextMesh(GameTemplate.StandardFont, new TextDrawProperties(HorizontalAlignment.Center), 50, useStringBuilder: false);
		text.Text = "PRESS_A_TO_CONTINUE".Translate();
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
