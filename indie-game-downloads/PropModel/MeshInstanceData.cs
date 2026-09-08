using Microsoft.Xna.Framework;

namespace PropModel;

public class MeshInstanceData
{
	public string Name;

	public bool[] InFrustum = new bool[2];

	public float[] DistanceSqr = new float[2];

	public int ReferenceId;

	public Matrix matWorld;
}
