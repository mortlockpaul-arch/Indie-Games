using System;
using F;
using Microsoft.Xna.Framework.Graphics;

namespace SynapseGaming.LightingSystem.Lights;

/// <summary>
/// Provides access to advanced light mapping data with support for
/// lighting and full material features including bump, specular, and parallax mapping.
/// </summary>
public class LightMap : IDisposable
{
	private Texture2D HCB;

	private Texture2D HC_0002;

	private static byte[] HC_0012;

	/// <summary>
	/// The light map "lighting" texture includes baked-down lighting and shadows.
	/// </summary>
	public Texture2D LightMapColorTexture => HCB;

	/// <summary>
	/// The light map "directional" texture, which adds support for advanced material features including bump, specular, and parallax mapping.
	/// </summary>
	public Texture2D LightMapDirectionalTexture
	{
		get
		{
			return HC_0002;
		}
		internal set
		{
			HC_0002 = texture2D;
		}
	}

	internal static byte[] HG(int P_0)
	{
		if (HC_0012 == null || HC_0012.Length < P_0)
		{
			HC_0012 = new byte[P_0];
		}
		return HC_0012;
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
		HCB = colortexture;
		HC_0002 = directionaltexture;
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
		HCB = new Texture2D(device, colorwidth, colorheight, mipmap, format);
		HC_0002 = new Texture2D(device, dirwidth, dirheight, mipmap, format);
	}

	/// <summary>
	/// Releases resources allocated by this object.
	/// </summary>
	public void Dispose()
	{
		F.B._7_0004(ref HCB);
		F.B._7_0004(ref HC_0002);
	}
}
