using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace SynapseGaming.LightingSystem.Core;

/// <summary>
/// Provides information on the device capabilities supported by the current hardware
/// and if those capabilities are allowed by the current configuration.  This allows
/// users to select various system specific configurations and for developers to test
/// a wide variety of configurations on a single machine.
///
/// Check a capability property to see if it's supported and allowed.  Setting a
/// capability property to true will allow it only if the current hardware supports it.
/// </summary>
public class GraphicsDeviceSupport
{
	private int HCB;

	private int HC_0002;

	private Dictionary<SurfaceFormat, bool> HC_0012 = new Dictionary<SurfaceFormat, bool>(16);

	/// <summary>
	/// The maximum texture size supported by the hardware.
	/// </summary>
	public int MaxTextureSize => HCB;

	/// <summary>
	/// The maximum anisotropy value supported by the hardware.
	/// </summary>
	public int MaxAnisotropy => HC_0002;

	/// <summary>
	/// List of surface formats and related hardware support.
	/// </summary>
	public Dictionary<SurfaceFormat, bool> SurfaceFormat => HC_0012;

	internal static bool _00026(bool P_0, bool P_1)
	{
		if (!P_0)
		{
			return false;
		}
		return P_1;
	}

	internal GraphicsDeviceSupport(GraphicsDevice P_0)
	{
		if (P_0.GraphicsProfile == GraphicsProfile.Reach)
		{
			HCB = 2048;
			HC_0002 = 4;
		}
		else
		{
			HCB = 4096;
			HC_0002 = 4;
		}
		DepthFormat depthStencilFormat = P_0.PresentationParameters.DepthStencilFormat;
		int multiSampleCount = P_0.PresentationParameters.MultiSampleCount;
		for (int i = 0; i < 19; i++)
		{
			SurfaceFormat surfaceFormat = (SurfaceFormat)i;
			bool flag = GraphicsAdapter.DefaultAdapter.QueryRenderTargetFormat(P_0.GraphicsProfile, surfaceFormat, depthStencilFormat, multiSampleCount, out var selectedFormat, out var _, out var _);
			HC_0012.Add(surfaceFormat, flag && surfaceFormat == selectedFormat);
		}
	}

	/// <summary>
	/// Finds the first supported surface format in a list of requested formats. Always sort the
	/// requested format list in order of preference to ensure the supported and returned format is the best possible match.
	/// </summary>
	/// <param name="requestedformats">List of requested formats.</param>
	/// <returns></returns>
	public SurfaceFormat FindSupportedFormat(SurfaceFormat[] requestedformats)
	{
		foreach (SurfaceFormat surfaceFormat in requestedformats)
		{
			if (SurfaceFormat[surfaceFormat])
			{
				return surfaceFormat;
			}
		}
		return Microsoft.Xna.Framework.Graphics.SurfaceFormat.Color;
	}
}
