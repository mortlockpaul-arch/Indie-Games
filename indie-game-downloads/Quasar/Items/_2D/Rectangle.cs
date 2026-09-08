using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Quasar.Meshes;
using Quasar.Textures;

namespace Quasar.Items._2D;

public class Rectangle : RenderItem
{
	public Sized2DRectangleMesh RectangleMesh => (Sized2DRectangleMesh)base.Mesh;

	public Rectangle(string texture)
		: this(TextureManager.Textures[texture], Vector2.One)
	{
	}

	public Rectangle(Texture2D texture, Vector2 size)
	{
		Sized2DRectangleMesh m = new Sized2DRectangleMesh(size, texture);
		addMesh(m);
	}

	public Rectangle(Vector2 size, Vector3 color)
	{
		Sized2DRectangleMesh m = new Sized2DRectangleMesh(size, color);
		addMesh(m);
	}

	public Rectangle(Vector2 size, Vector4 color)
	{
		Sized2DRectangleMesh m = new Sized2DRectangleMesh(size, color);
		addMesh(m);
	}
}
