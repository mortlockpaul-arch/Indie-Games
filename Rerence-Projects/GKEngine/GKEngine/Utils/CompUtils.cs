using GKEngine.Cameras;
using GKEngine.Entities;
using Microsoft.Xna.Framework;

namespace GKEngine.Utils;

public static class CompUtils
{
	public static int Entity2D_Depth(Entity2D oEnt1, Entity2D oEnt2)
	{
		if (oEnt1 == null)
		{
			if (oEnt2 == null)
			{
				return 0;
			}
			return -1;
		}
		if (oEnt2 == null)
		{
			return 1;
		}
		return oEnt1.depth.CompareTo(oEnt2.depth);
	}

	public static int Entity3D_Depth_Z(Entity3D oEnt1, Entity3D oEnt2)
	{
		Camera camera = oEnt1.scene.cameras.camera;
		if (oEnt1 == null)
		{
			if (oEnt2 == null)
			{
				return 0;
			}
			return -1;
		}
		if (oEnt2 == null)
		{
			return 1;
		}
		Vector3 vector = Vector3.Transform(oEnt1.position, camera.rotationMatrix);
		Vector3 vector2 = Vector3.Transform(oEnt2.position, camera.rotationMatrix);
		int result = 0;
		if (vector.Z < vector2.Z)
		{
			result = -1;
		}
		else if (vector.Z > vector2.Z)
		{
			result = 1;
		}
		return result;
	}

	public static int Entity3D_DepthSimple(Entity3D oEnt1, Entity3D oEnt2)
	{
		_ = oEnt1.scene.cameras.camera;
		if (oEnt1 == null)
		{
			if (oEnt2 == null)
			{
				return 0;
			}
			return -1;
		}
		if (oEnt2 == null)
		{
			return 1;
		}
		float z = oEnt1.position.Z;
		return oEnt2.position.Z.CompareTo(z);
	}

	public static int Entity3D_Depth(Entity3D oEnt1, Entity3D oEnt2)
	{
		Camera camera = oEnt1.scene.cameras.camera;
		if (oEnt1 == null)
		{
			if (oEnt2 == null)
			{
				return 0;
			}
			return -1;
		}
		if (oEnt2 == null)
		{
			return 1;
		}
		float value = Vector3.Distance(camera.position, oEnt1.position);
		return Vector3.Distance(camera.position, oEnt2.position).CompareTo(value);
	}
}
