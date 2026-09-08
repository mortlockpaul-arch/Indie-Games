using Microsoft.Xna.Framework;
using Quasar.Meshes;

namespace Quasar.GameUtils.Template.Controls;

internal class ImageNode : RenderItem
{
	private Sized2DRectangleMesh mesh;

	public ImageNode(Image image)
	{
		mesh = new Sized2DRectangleMesh(image.Size, image.Texture);
		mesh.Offset = image.Position;
		addMesh(mesh);
		image.OnSizeChanged += image_OnSizeChanged;
		image.OnPositionChanged += image_OnPositionChanged;
	}

	private void image_OnPositionChanged(Image arg1, Vector2 arg2)
	{
		mesh.Offset = arg2;
	}

	private void image_OnSizeChanged(Image arg1, Vector2 arg2)
	{
		mesh.Size = arg2;
	}
}
