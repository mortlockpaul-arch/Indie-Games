using Microsoft.Xna.Framework;
using Quasar.GUI.Controls;
using Quasar.Meshes.Text;

namespace Quasar.GameUtils.Template;

public class LabelNode : RenderItem
{
	private Label label;

	private TextMesh text;

	public LabelNode(Label label)
	{
		text = new TextMesh(GameTemplate.StandardFont, new TextDrawProperties(HorizontalAlignment.Center), 50, useStringBuilder: false);
		addMesh(text);
		transform.Translation = new Vector3(label.Position, 0f);
		this.label = label;
	}

	protected override void DoUpdate()
	{
		text.Text = label.Text;
		Visible = label.Visible;
		base.DoUpdate();
	}

	public override void Dispose()
	{
		label = null;
		base.Dispose();
	}
}
