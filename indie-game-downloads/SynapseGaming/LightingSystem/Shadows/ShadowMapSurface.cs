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
	private bool _3A_0018 = true;

	private Matrix _3AL = Matrix.Identity;

	private Matrix _3A_0019 = Matrix.Identity;

	private bool _3A3 = true;

	private BoundingFrustum _3A6 = new BoundingFrustum(Matrix.Identity);

	private Viewport _3AD = default(Viewport);

	private float _3A_0017 = 1f;

	private Rectangle _3A_0003 = default(Rectangle);

	/// <summary>
	/// View transform used to project the scene into the
	/// surface and the surface onto the scene.
	/// </summary>
	public Matrix WorldToSurfaceView
	{
		get
		{
			return _3AL;
		}
		set
		{
			_3AL = value;
			_3A3 = true;
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
			return _3A_0019;
		}
		set
		{
			_3A_0019 = value;
			_3A3 = true;
		}
	}

	/// <summary>
	/// The surface projection frustum.
	/// </summary>
	public BoundingFrustum Frustum
	{
		get
		{
			if (_3A3)
			{
				_3A6.Matrix = _3AL * _3A_0019;
				_3A3 = false;
			}
			return _3A6;
		}
	}

	/// <summary>
	/// Viewport used when rendering to the surface render target location.
	/// </summary>
	public Viewport Viewport => _3AD;

	/// <summary>
	/// Level-of-detail applied to the surface.
	/// </summary>
	public float LevelOfDetail
	{
		get
		{
			return _3A_0017;
		}
		set
		{
			_3A_0017 = value;
		}
	}

	/// <summary>
	/// The surface location in the render target.
	/// </summary>
	public Rectangle RenderTargetLocation
	{
		get
		{
			return _3A_0003;
		}
		set
		{
			_3A_0003 = value;
			_3AD.X = _3A_0003.X;
			_3AD.Y = _3A_0003.Y;
			_3AD.Width = _3A_0003.Width;
			_3AD.Height = _3A_0003.Height;
			_3AD.MinDepth = 0f;
			_3AD.MaxDepth = 1f;
		}
	}

	/// <summary>
	/// Determines if the shadow map contents should be generated for this face.
	/// </summary>
	public bool Enabled
	{
		get
		{
			return _3A_0018;
		}
		set
		{
			_3A_0018 = value;
		}
	}

	/// <summary>
	/// Creates a new ShadowMapSurface instance.
	/// </summary>
	public ShadowMapSurface()
	{
	}

	internal Rectangle _6G(int P_0)
	{
		return new Rectangle(_3A_0003.X + P_0, _3A_0003.Y + P_0, _3A_0003.Width - P_0 * 2, _3A_0003.Height - P_0 * 2);
	}

	internal void _6n(Vector3 P_0)
	{
		_3AL.Translation = P_0;
	}
}
