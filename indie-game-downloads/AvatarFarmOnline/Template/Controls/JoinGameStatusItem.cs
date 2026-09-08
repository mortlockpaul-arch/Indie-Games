using Microsoft.Xna.Framework;
using Quasar;
using Quasar.GameUtils.Template;
using Quasar.Global;
using Quasar.Language;
using Quasar.Meshes.Text;

namespace AvatarFarmOnline.Template.Controls;

internal class JoinGameStatusItem : RenderItem
{
	private TextBoxMesh statusText;

	private AvatarFarmOnline.Template.Controls.JoinGameStatus status;

	public JoinGameStatusItem(AvatarFarmOnline.Template.Controls.JoinGameStatus status)
	{
		this.status = status;
		statusText = new TextBoxMesh(properties: new TextBoxDrawProperties(new Layout2D.LayoutData(new Vector2(0f, (0f - Engine.GUIWidth) * 0.025f), new Vector2(Engine.GUIWidth * 0.3f + GameTemplate.Instance.GetButtonSize(null).X, Engine.GUIHeight * 0.5f - 10f)), 1f, HorizontalAlignment.Center, VerticalAlignment.Center), font: GameTemplate.TitleFont, maxBufferSize: 64, useStringBuilder: false);
		statusText.Diffuse = GameTemplate.TitleColor;
		statusText.Text = "JOINING_GAME".Translate();
		addMesh(statusText);
	}

	protected override void DoUpdate()
	{
		base.DoUpdate();
	}
}
