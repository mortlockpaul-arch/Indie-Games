using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EGEngine;

public struct BotPhysicsPart
{
	public string name;

	public OOBB oobb;

	public Matrix transform;

	public Matrix inverseTransform;

	public ModelMesh mesh;
}
