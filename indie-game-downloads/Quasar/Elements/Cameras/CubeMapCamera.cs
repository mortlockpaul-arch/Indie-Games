using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Quasar.Elements.Cameras;

public class CubeMapCamera : Camera
{
	private Vector3 upDir = Vector3.Zero;

	private Vector3 viewDir = Vector3.Zero;

	public Vector3 UpDir => upDir;

	public Vector3 ViewDir => viewDir;

	public CubeMapCamera(CubeMapFace face)
	{
		projection = Matrix.CreatePerspectiveFieldOfView((float)Math.PI / 2f, 1f, 0.1f, 100f);
		switch (face)
		{
		case CubeMapFace.PositiveX:
			viewDir = Vector3.UnitX;
			upDir = Vector3.UnitY;
			break;
		case CubeMapFace.PositiveY:
			viewDir = Vector3.UnitY;
			upDir = Vector3.UnitZ;
			break;
		case CubeMapFace.PositiveZ:
			viewDir = -Vector3.UnitZ;
			upDir = Vector3.UnitY;
			break;
		case CubeMapFace.NegativeX:
			viewDir = -Vector3.UnitX;
			upDir = Vector3.UnitY;
			break;
		case CubeMapFace.NegativeY:
			viewDir = -Vector3.UnitY;
			upDir = -Vector3.UnitZ;
			break;
		case CubeMapFace.NegativeZ:
			viewDir = Vector3.UnitZ;
			upDir = Vector3.UnitY;
			break;
		}
	}

	protected override void DoUpdate()
	{
		base.Transform.LookAt(base.Transform.Translation, base.Transform.Translation + viewDir, upDir);
		base.DoUpdate();
	}
}
