using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SynapseGaming.LightingSystem.Shadows;

/// <summary>
/// Class that represents one surface in a shadow map, which can be
/// used for multi-part rendering and level-of-detail. The surface
/// contains its own section within a render target.
/// </summary>
public class ShadowMapSurface
{
	private bool HCB = true;

	private Matrix HC_0002 = Matrix.Identity;

	private Matrix HC_0012 = Matrix.Identity;

	private bool HCH = true;

	private BoundingFrustum HC7 = new BoundingFrustum(Matrix.Identity);

	private Viewport HC_0001 = default(Viewport);

	private float HCw = 1f;

	private Rectangle HCZ = default(Rectangle);

	/// <summary>
	/// View transform used to project the scene into the
	/// surface and the surface onto the scene.
	/// </summary>
	public Matrix WorldToSurfaceView
	{
		get
		{
			return HC_0002;
		}
		set
		{
			HC_0002 = value;
			HCH = true;
		}
	}

	/// <summary>
	/// Projection transform used to project the scene into
	/// the surface and the surface onto the scene.
	/// </summary>
	public Matrix Projection
	{
		get
		{
			return HC_0012;
		}
		set
		{
			HC_0012 = value;
			HCH = true;
		}
	}

	/// <summary>
	/// The surface projection frustum.
	/// </summary>
	public BoundingFrustum Frustum
	{
		get
		{
			if (HCH)
			{
				HC7.Matrix = HC_0002 * HC_0012;
				HCH = false;
			}
			return HC7;
		}
	}

	/// <summary>
	/// Viewport used when rendering to the surface render target location.
	/// </summary>
	public Viewport Viewport => HC_0001;

	/// <summary>
	/// Level-of-detail applied to the surface.
	/// </summary>
	public float LevelOfDetail
	{
		get
		{
			return HCw;
		}
		set
		{
			HCw = value;
		}
	}

	/// <summary>
	/// The surface location in the render target.
	/// </summary>
	public Rectangle RenderTargetLocation
	{
		get
		{
			return HCZ;
		}
		set
		{
			HCZ = value;
			HC_0001.X = HCZ.X;
			HC_0001.Y = HCZ.Y;
			HC_0001.Width = HCZ.Width;
			HC_0001.Height = HCZ.Height;
			HC_0001.MinDepth = 0f;
			HC_0001.MaxDepth = 1f;
		}
	}

	/// <summary>
	/// Determines if the shadow map contents should be generated for this face.
	/// </summary>
	public bool Enabled
	{
		get
		{
			return HCB;
		}
		set
		{
			HCB = value;
		}
	}

	/// <summary>
	/// Creates a new ShadowMapSurface instance.
	/// </summary>
	public ShadowMapSurface()
	{
	}

	internal Rectangle _7_0015(int P_0)
	{
		return new Rectangle(HCZ.X + P_0, HCZ.Y + P_0, HCZ.Width - P_0 * 2, HCZ.Height - P_0 * 2);
	}

	internal void _7U(Vector3 P_0)
	{
		HC_0002.Translation = P_0;
	}
}
