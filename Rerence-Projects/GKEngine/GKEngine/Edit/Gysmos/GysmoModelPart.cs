using GKEngine.Entities;
using Microsoft.Xna.Framework;

namespace GKEngine.Edit.Gysmos;

public class GysmoModelPart : ModelPart
{
	public MeshData collision;

	public Matrix transform;

	public GysmoModel model;

	public override void Dispose()
	{
		collision = null;
		base.Dispose();
	}
}
