using System;
using Microsoft.Xna.Framework;

namespace EGEngine;

public class DataQueue
{
	public const int MAX_DATA_QUEUE = 2;

	public const int DATA_QUEUE_EMPTY = 0;

	public const int DATA_QUEUE_FULL = 1;

	public const int DATA_QUEUE_UPDATING = 2;

	public int status;

	public Vector3 eyePosition;

	public Vector3 cameraPos;

	public Vector3 cameralookAt;

	public Vector3 cameraUp;

	public Vector3 cameraDirN;

	public Vector3 cameraAngles;

	public Vector3 cameraEyePos;

	public Vector4 lightPos;

	public Vector4 lightDir;

	public Vector3 lightEyePos;

	public BoundingFrustum frustum;

	public Matrix world;

	public Matrix view;

	public Matrix projection;

	public Matrix viewProj;

	public Matrix invViewProj;

	public Matrix textureProj;

	public Matrix reflection;

	public Matrix lightView;

	public Matrix lightProj;

	public Matrix[] lightView2;

	public Matrix[] lightProj2;

	public Matrix weaponProjection;

	public float gameTime;

	public DataQueue()
	{
		status = 0;
		cameraPos = Vector3.Zero;
		cameralookAt = Vector3.UnitX;
		cameraUp = Vector3.UnitY;
		projection = Matrix.CreatePerspectiveFieldOfView((float)Math.PI / 4f, 1.7777778f, 1f, PlayerBase.FarZPlane);
		weaponProjection = Matrix.CreatePerspectiveFieldOfView((float)Math.PI / 4f, 1.7777778f, 1f, PlayerBase.FarZPlane);
	}
}
