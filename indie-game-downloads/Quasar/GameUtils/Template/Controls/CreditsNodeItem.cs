using Microsoft.Xna.Framework;
using Quasar.GameUtils.Meshes;
using Quasar.Meshes;
using Quasar.Meshes.Text;
using Quasar.Shaders;
using Quasar.Textures;

namespace Quasar.GameUtils.Template.Controls;

internal class CreditsNodeItem : RenderItem
{
	private Sized2DRectangleMesh image;

	private TextMesh name;

	private TextBoxMesh functions;

	public CreditsNodeItem()
	{
		addMesh(new HUDDetailRectangle(new Vector2(650f, 130f)));
		image = new Sized2DRectangleMesh(new Vector2(90f), TextureManager.Textures["Credits/noavatar"]);
		image.Shader = ShaderManager.Shaders["HowToPlay"];
		image.FirstMaterial.Textures.Add(TextureManager.Textures["Credits/Alpha"]);
		image.Offset = new Vector2(261f, 0f);
		addMesh(image);
		name = new TextMesh(GameTemplate.StandardFont, new TextDrawProperties(HorizontalAlignment.Left), 40, useStringBuilder: true);
		name.Offset = new Vector2(-300f, 45f);
		name.Diffuse = GameTemplate.DialogTitleColor;
		addMesh(name);
		functions = new TextBoxMesh(GameTemplate.StandardFont, new TextBoxDrawProperties(new Vector2(-50f, -25f), new Vector2(480f, 80f), 0.8f, HorizontalAlignment.Left, VerticalAlignment.Center), 100, useStringBuilder: false);
		addMesh(functions);
	}

	public void SetData(CreditsItem item)
	{
		name.StringBuilder.Length = 0;
		name.StringBuilder.Append(item.Name);
		if (item.HasGamertag)
		{
			name.StringBuilder.Append(" (");
			name.StringBuilder.Append(item.Gamertag);
			name.StringBuilder.Append(")");
		}
		functions.Text = item.Description;
		image.Texture = TextureManager.Textures["Credits/" + item.Image];
		Visible = true;
	}

	public void Reset()
	{
		Visible = false;
	}
}
