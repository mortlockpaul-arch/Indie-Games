using Microsoft.Xna.Framework;
using Quasar.GUI.Controls;
using Quasar.Meshes.Text;

namespace Quasar.GameUtils.Template;

internal class MessageItem : RenderItem
{
	private const float SIZE_X = 400f;

	private const float BORDER = 32f;

	private TextBoxMesh text;

	private Message message;

	public MessageItem(Message message)
	{
		this.message = message;
		text = new TextBoxMesh(BitmapFontManager.Fonts["GUI"], new TextBoxDrawProperties(Vector2.Zero, new Vector2(400f, 100f), 1f, HorizontalAlignment.Center, VerticalAlignment.Center));
		addMesh(text);
		transform.Translation = new Vector3(0f, 0f, 0f);
	}

	protected override void DoUpdate()
	{
		text.Text = message.Text;
		base.DoUpdate();
	}
}
