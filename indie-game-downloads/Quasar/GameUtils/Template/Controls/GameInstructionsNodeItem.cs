using Microsoft.Xna.Framework;
using Quasar.GameUtils.Meshes;
using Quasar.Meshes;
using Quasar.Meshes.Text;
using Quasar.Shaders;
using Quasar.Textures;

namespace Quasar.GameUtils.Template.Controls;

internal class GameInstructionsNodeItem : RenderItem
{
	private const int IMAGE_SIZE = 100;

	private const int WIDTH = 650;

	private Sized2DRectangleMesh image;

	private TextBoxMesh textBox;

	private bool odd;

	public GameInstructionsNodeItem(bool odd)
	{
		this.odd = odd;
		addMesh(new HUDDetailRectangle(new Vector2(650f, 130f)));
		image = new Sized2DRectangleMesh(new Vector2(100f), null);
		image.Shader = ShaderManager.Shaders["HowToPlay"];
		image.FirstMaterial.Textures.Add(TextureManager.Textures["HowToPlay/Alpha"]);
		addMesh(image);
		textBox = new TextBoxMesh(GameTemplate.StandardFont, default(TextBoxDrawProperties), 150, useStringBuilder: false);
		addMesh(textBox);
	}

	public void SetData(GameInstructionsItem item)
	{
		TextBoxDrawProperties drawProperties = new TextBoxDrawProperties(new Vector2(624f, 110f), HorizontalAlignment.Left, VerticalAlignment.Center);
		textBox.Text = item.Text;
		if (item.HasImage)
		{
			image.Offset = new Vector2(265 * (odd ? 1 : (-1)), 0f);
			image.FirstMaterial.Diffuse = Vector3.One;
			image.FirstMaterial.Alpha = 1f;
			image.FirstMaterial.Texture = item.Image;
			drawProperties.Offset = new Vector2(-54 * (odd ? 1 : (-1)), 0f);
			drawProperties.Size = new Vector2(522f, 110f);
		}
		else
		{
			image.FirstMaterial.Diffuse = Vector3.Zero;
			image.FirstMaterial.Alpha = 0f;
		}
		textBox.DrawProperties = drawProperties;
		Visible = true;
	}

	public void Reset()
	{
		Visible = false;
	}
}
