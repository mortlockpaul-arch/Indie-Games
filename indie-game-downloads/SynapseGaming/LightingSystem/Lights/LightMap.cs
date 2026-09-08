using System;
using Microsoft.Xna.Framework.Graphics;
using p;

namespace SynapseGaming.LightingSystem.Lights;

/// <summary>
/// Provides access to advanced light mapping data with support for
/// lighting and full material features including bump, specular, and parallax mapping.
/// </summary>
public class LightMap : IDisposable
{
	private Texture2D _3A_0018;

	private Texture2D _3AL;

	private static byte[] _3A_0019;

	/// <summary>
	/// The light map "lighting" texture includes baked-down lighting and shadows.
	/// </summary>
	public Texture2D LightMapColorTexture => _3A_0018;

	/// <summary>
	/// The light map "directional" texture, which adds support for advanced material features including bump, specular, and parallax mapping.
	/// </summary>
	public Texture2D LightMapDirectionalTexture
	{
		get
		{
			return _3AL;
		}
		internal set
		{
			_3AL = texture2D;
		}
	}

	internal static byte[] _3u(int P_0)
	{
		if (_3A_0019 == null || _3A_0019.Length < P_0)
		{
			_3A_0019 = new byte[P_0];
		}
		return _3A_0019;
	}

	/// <summary>
	/// Creates a new LightMap instance using two source textures.
	///
	/// Assigns ownership of the textures to this light map. When the light map
	/// is disposed the textures will also be disposed.
	/// </summary>
	/// <param name="colortexture">The light map "lighting" texture includes
	/// baked-down lighting and shadows.</param>
	/// <param name="directionaltexture">The light map "directional" texture,
	/// which adds support for advanced material features including bump,
	/// specular, and parallax mapping.</param>
	public LightMap(Texture2D colortexture, Texture2D directionaltexture)
	{
		_3A_0018 = colortexture;
		_3AL = directionaltexture;
	}

	/// <summary>
	/// Creates a new LightMap instance.
	/// </summary>
	/// <param name="device"></param>
	/// <param name="colorwidth">Width of the light map "lighting" texture.</param>
	/// <param name="colorheight">Height of the light map "lighting" texture.</param>
	/// <param name="dirwidth">Width of the light map "directional" texture.</param>
	/// <param name="dirheight">Height of the light map "directional" texture.</param>
	/// <param name="mipmap">Determines if the light map textures are mip-mapped.</param>
	/// <param name="format">Determines the surface format of the light map textures.</param>
	public LightMap(GraphicsDevice device, int colorwidth, int colorheight, int dirwidth, int dirheight, bool mipmap, SurfaceFormat format)
	{
		_3A_0018 = new Texture2D(device, colorwidth, colorheight, mipmap, format);
		_3AL = new Texture2D(device, dirwidth, dirheight, mipmap, format);
	}

	/// <summary>
	/// Releases resources allocated by this object.
	/// </summary>
	public void Dispose()
	{
		p._0018._6_0006(ref _3A_0018);
		p._0018._6_0006(ref _3AL);
	}
}
