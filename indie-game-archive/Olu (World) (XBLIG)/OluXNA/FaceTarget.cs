using Microsoft.Xna.Framework;

namespace OluXNA;

internal class FaceTarget : Target
{
	public ModelWrapper model;

	public int meshNum;

	public int indexNum;

	public int bossPart;

	public Matrix modMatrix;

	public FaceTarget()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		modMatrix = Matrix.Identity;
		base._002Ector();
		pos = Vector3.Zero;
		selected = 0;
		hp = 1;
		score = 10;
		meshNum = 0;
		indexNum = 0;
	}

	public FaceTarget(FaceTarget other)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		modMatrix = Matrix.Identity;
		base._002Ector(other);
		model = other.model;
		meshNum = other.meshNum;
		indexNum = other.indexNum;
	}

	public override Vector3 absolutePos()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		return (GetVertexPos(0) + GetVertexPos(1) + GetVertexPos(2)) / 3f;
	}

	public Vector3 GetVertexPos(int iOffset)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		return BaseGame.GetVertexPos(ref model, meshNum, indexNum + iOffset, ref enem, modMatrix);
	}
}
