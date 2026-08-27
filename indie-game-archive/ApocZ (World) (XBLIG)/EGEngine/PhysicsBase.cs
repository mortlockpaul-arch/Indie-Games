using DataContent;
using Microsoft.Xna.Framework;

namespace EGEngine;

public class PhysicsBase
{
	public virtual void LoadContent()
	{
	}

	public virtual void LoadRagDolls()
	{
	}

	public virtual void Initialize()
	{
	}

	public virtual void Update()
	{
	}

	public virtual void Draw(ref Matrix view, ref Matrix projection)
	{
	}

	public static void Add_OOBB_BEPUPhysics(eOOBB physics)
	{
	}

	public virtual void LoadBEPUSpace()
	{
	}

	public static void ExtractRTS(ref Matrix inMat, out Matrix outMat)
	{
		outMat = Matrix.Identity;
		Vector4 vector = new Vector4(inMat.M11, inMat.M12, inMat.M13, inMat.M14);
		Vector4 vector2 = new Vector4(inMat.M21, inMat.M22, inMat.M23, inMat.M24);
		Vector4 vector3 = new Vector4(inMat.M31, inMat.M32, inMat.M33, inMat.M34);
		outMat.M14 = vector.Length();
		outMat.M24 = vector2.Length();
		outMat.M34 = vector3.Length();
		vector.Normalize();
		vector2.Normalize();
		vector3.Normalize();
		outMat.M11 = vector.X;
		outMat.M12 = vector.Y;
		outMat.M13 = vector.Z;
		outMat.M21 = vector2.X;
		outMat.M22 = vector2.Y;
		outMat.M23 = vector2.Z;
		outMat.M31 = vector3.X;
		outMat.M32 = vector3.Y;
		outMat.M33 = vector3.Z;
		outMat.M41 = inMat.M41;
		outMat.M42 = inMat.M42;
		outMat.M43 = inMat.M43;
		outMat.M44 = inMat.M44;
	}
}
