using Microsoft.Xna.Framework;

namespace GKEngine.Entities;

public class Base3DHist : Base3D
{
	public Base3D previous;

	public override float X
	{
		get
		{
			return base.X;
		}
		set
		{
			previous.X = base.X;
			base.X = value;
		}
	}

	public Base3DHist()
	{
		previous = new Base3D();
	}

	public Base3DHist(Vector3 oPosition, Quaternion oRotation, Vector3 oScale)
		: base(oPosition, oRotation, oScale)
	{
		previous = new Base3D(oPosition, oRotation, oScale);
	}

	public Base3DHist(Matrix oMatrix)
		: base(oMatrix)
	{
		previous = new Base3D(oMatrix);
	}
}
