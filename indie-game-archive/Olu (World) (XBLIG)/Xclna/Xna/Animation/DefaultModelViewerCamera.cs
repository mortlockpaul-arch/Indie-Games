using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xclna.Xna.Animation;

public sealed class DefaultModelViewerCamera : IModelViewerCamera
{
	private Matrix world;

	private Matrix view;

	private Matrix projection;

	private Vector3 cameraPosition;

	private Vector3 up;

	private Vector3 right;

	private float fieldOfView;

	private float nearDistance;

	private float farDistance;

	private float windowWidth;

	private float windowHeight;

	private float centerX;

	private float centerY;

	private float aspectRatio;

	private int initialZoom;

	private float arcRadius;

	private float camOffsetX;

	private float camOffsetY;

	private BoundingSphere sphere;

	private Viewport viewPort;

	private Vector3 modelPos;

	private Model model;

	public Matrix ModelWorld
	{
		get
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			return world;
		}
	}

	public Matrix View
	{
		get
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = up * camOffsetY * sphere.Radius / 3f + right * camOffsetX * sphere.Radius / 3f;
			return Matrix.CreateLookAt(cameraPosition, val, up);
		}
	}

	public Matrix Projection
	{
		get
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			return projection;
		}
	}

	public unsafe DefaultModelViewerCamera(Game game, Model model)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Expected O, but got Unknown
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		camOffsetX = 0f;
		camOffsetY = 0f;
		modelPos = Vector3.One;
		base._002Ector();
		this.model = model;
		sphere = new BoundingSphere(Vector3.Zero, 1f);
		game.IsMouseVisible = true;
		IGraphicsDeviceService val = (IGraphicsDeviceService)game.Services.GetService(typeof(IGraphicsDeviceService));
		viewPort = val.GraphicsDevice.Viewport;
		windowWidth = ((Viewport)(ref viewPort)).Width;
		windowHeight = ((Viewport)(ref viewPort)).Height;
		fieldOfView = (float)Math.PI / 4f;
		nearDistance = 0.1f;
		centerX = windowWidth / 2f;
		centerY = windowHeight / 2f;
		aspectRatio = windowWidth / windowHeight;
		up = Vector3.Up;
		right = Vector3.Right;
		initialZoom = 0;
		this.model = model;
		Enumerator enumerator = model.Meshes.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				ModelMesh current = ((Enumerator)(ref enumerator)).Current;
				sphere = BoundingSphere.CreateMerged(sphere, current.BoundingSphere);
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
		world = Matrix.Identity;
		farDistance = Math.Min(sphere.Radius * 1000f, float.MaxValue);
		projection = Matrix.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearDistance, farDistance);
		cameraPosition = new Vector3(0f, 0f, sphere.Radius * 5f);
		arcRadius = ((Vector3)(ref cameraPosition)).Length() / 2f;
		view = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, up);
	}

	private bool IntersectPoint(int x, int y, out Vector3 intersectionPoint)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		BoundingSphere val = default(BoundingSphere);
		((BoundingSphere)(ref val))._002Ector(default(Vector3), arcRadius);
		Vector3 val2 = default(Vector3);
		((Vector3)(ref val2))._002Ector((float)x, (float)y, 0f);
		val2 = ((Viewport)(ref viewPort)).Unproject(val2, projection, view, Matrix.Identity);
		Vector3 val3 = val2 - cameraPosition;
		((Vector3)(ref val3)).Normalize();
		Ray val4 = default(Ray);
		((Ray)(ref val4))._002Ector(cameraPosition, val3);
		float? num = ((Ray)(ref val4)).Intersects(val);
		if (!num.HasValue)
		{
			intersectionPoint = Vector3.Zero;
			return false;
		}
		intersectionPoint = cameraPosition + val3 * num.Value;
		return true;
	}

	public void Update(GameTime gameTime)
	{
	}
}
