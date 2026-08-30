using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Defenders;

internal class Hammer3D
{
	private ModelManager modelManager;

	private VertexPositionTexture[] verts;

	private VertexBuffer vertexBuffer;

	private BasicEffect effect;

	private Texture2D texture;

	private Matrix worldTranslation = Matrix.Identity;

	private Matrix worldRotation = Matrix.Identity;

	public static Camera3D camera3D { get; protected set; }
}
