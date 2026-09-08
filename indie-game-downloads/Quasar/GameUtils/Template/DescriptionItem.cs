using Microsoft.Xna.Framework;
using Quasar.GUI.Controls;
using Quasar.Global;
using Quasar.Meshes;
using Quasar.Meshes.Text;
using Quasar.Textures;

namespace Quasar.GameUtils.Template;

internal class DescriptionItem : RenderItem
{
	private const float SIZE_X = 400f;

	private const float BORDER = 32f;

	private Description description;

	private TextBoxMesh text;

	private BorderedRectangle rectangle;

	public DescriptionItem(Description description)
	{
		this.description = description;
		rectangle = new BorderedRectangle(TextureManager.Textures["GUI/GroupBG"], new Vector2(400f, 100f), new float[4] { 32f, 32f, 32f, 32f });
		addMesh(rectangle);
		text = new TextBoxMesh(BitmapFontManager.Fonts["GUI"], new TextBoxDrawProperties(Vector2.Zero, new Vector2(400f, 100f), 1f, HorizontalAlignment.Left, VerticalAlignment.Top));
		addMesh(text);
		transform.Translation = new Vector3(Engine.GUIWidth * 0.25f, Engine.GUIHeight * 0.25f, 0f);
	}

	protected override void DoUpdate()
	{
		text.Text = description.Text;
		base.DoUpdate();
	}
}
