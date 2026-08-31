using Microsoft.Xna.Framework.Graphics;

namespace DPSF;

/// <summary>
/// Class to hold all of the drawing Settings
/// </summary>
public class RenderProperties
{
	/// <summary>
	/// Get / Set the BlendState to use when drawing the particles.
	/// <para>Default value is BlendState.AlphaBlend.</para>
	/// </summary>
	public BlendState BlendState { get; set; }

	/// <summary>
	/// Get / Set the DepthStencilState to use when drawing the particles.
	/// <para>Default value is DepthStencilState.DepthRead.</para>
	/// </summary>
	public DepthStencilState DepthStencilState { get; set; }

	/// <summary>
	/// Get / Set the RasterizerState to use when drawing the particles.
	/// <para>Default value is RasterizerState.CullCounterClockwise.</para>
	/// </summary>
	public RasterizerState RasterizerState { get; set; }

	/// <summary>
	/// Get / Set the SamplerState to use when drawing the particles.
	/// <para>Default value is SamplerState.LinearClamp.</para>
	/// </summary>
	public SamplerState SamplerState { get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:DPSF.RenderProperties" /> class, setting each property to its default value.
	/// </summary>
	public RenderProperties()
	{
		ResetToDefaults();
	}

	/// <summary>
	/// Resets each of the render properties to their default values.
	/// </summary>
	public void ResetToDefaults()
	{
		BlendState = DPSFHelper.CloneBlendState(BlendState.AlphaBlend);
		DepthStencilState = DPSFHelper.CloneDepthStencilState(DepthStencilState.DepthRead);
		RasterizerState = DPSFHelper.CloneRasterizerState(RasterizerState.CullCounterClockwise);
		SamplerState = DPSFHelper.CloneSamplerState(SamplerState.LinearClamp);
	}
}
