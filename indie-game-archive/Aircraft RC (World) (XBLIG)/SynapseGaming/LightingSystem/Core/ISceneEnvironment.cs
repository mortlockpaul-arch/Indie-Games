using Microsoft.Xna.Framework;
using SynapseGaming.LightingSystem.Editor;

namespace SynapseGaming.LightingSystem.Core;

/// <summary>
/// Interface that provides a base for all scene environment objects.
/// </summary>
[EditorObject(true)]
public interface ISceneEnvironment : IEditorObject, INamedObject
{
	/// <summary>
	/// Maximum world space distance objects are visible.
	/// </summary>
	[EditorProperty(true, Description = "Viewable Distance", HorizontalAlignment = true, MajorGrouping = 4, MinorGrouping = 3, ToolTipText = "")]
	[EditorNumberPadOptions(2, 0.0, 2147483647.0, 2.0)]
	float VisibleDistance { get; set; }

	/// <summary>
	/// Enables scene fog.
	/// </summary>
	[EditorProperty(true, Description = "Fog Enabled", HorizontalAlignment = true, MajorGrouping = 1, MinorGrouping = 1, ToolTipText = "")]
	bool FogEnabled { get; set; }

	/// <summary>
	/// World space distance that fog begins.
	/// </summary>
	[EditorProperty(true, Description = "Fog Start Distance", HorizontalAlignment = true, MajorGrouping = 4, MinorGrouping = 1, ToolTipText = "")]
	[EditorNumberPadOptions(2, 0.0, 2147483647.0, 2.0)]
	float FogStartDistance { get; set; }

	/// <summary>
	/// World space distance that fog fully obscures objects.
	/// </summary>
	[EditorNumberPadOptions(2, 0.0, 2147483647.0, 2.0)]
	[EditorProperty(true, Description = "Fog End Distance", HorizontalAlignment = true, MajorGrouping = 4, MinorGrouping = 2, ToolTipText = "")]
	float FogEndDistance { get; set; }

	/// <summary>
	/// Color applied to scene fog.
	/// </summary>
	[EditorProperty(true, Description = "Fog Color", HorizontalAlignment = true, MajorGrouping = 1, MinorGrouping = 3, ControlType = ControlType.ColorSelection, ToolTipText = "")]
	Vector3 FogColor { get; set; }

	/// <summary>
	/// World space distance that directional shadows begin fading.
	/// </summary>
	[EditorProperty(true, Description = "Shadow Fade Start", HorizontalAlignment = true, MajorGrouping = 5, MinorGrouping = 1, ToolTipText = "")]
	[EditorNumberPadOptions(2, 0.0, 2147483647.0, 2.0)]
	float ShadowFadeStartDistance { get; set; }

	/// <summary>
	/// World space distance that directional shadows completely disappear.
	/// </summary>
	[EditorNumberPadOptions(2, 0.0, 2147483647.0, 2.0)]
	[EditorProperty(true, Description = "Shadow Fade End", HorizontalAlignment = true, MajorGrouping = 5, MinorGrouping = 2, ToolTipText = "")]
	float ShadowFadeEndDistance { get; set; }

	/// <summary>
	/// World space distance used to include shadow casters. This allows including shadows
	/// from objects further away than the shadow fade area, for instance shadows from
	/// distant mountains.
	/// </summary>
	[EditorProperty(true, Description = "Max Caster Distance", HorizontalAlignment = true, MajorGrouping = 5, MinorGrouping = 3, ToolTipText = "")]
	[EditorNumberPadOptions(2, 0.0, 2147483647.0, 2.0)]
	float ShadowCasterDistance { get; set; }

	/// <summary>
	/// Strength of bloom applied to the scene.
	/// </summary>
	[EditorProperty(true, Description = "Bloom Amount", HorizontalAlignment = true, MajorGrouping = 6, MinorGrouping = 1, ToolTipText = "")]
	[EditorNumberPadOptions(2, 0.0, 10000.0, 0.01)]
	float BloomAmount { get; set; }

	/// <summary>
	/// Minimum pixel intensity required for bloom to occur.
	/// </summary>
	[EditorProperty(true, Description = "Bloom Threshold", HorizontalAlignment = true, MajorGrouping = 6, MinorGrouping = 2, ToolTipText = "")]
	[EditorNumberPadOptions(2, 0.0, 100.0, 0.01)]
	float BloomThreshold { get; set; }

	/// <summary>
	/// Enables High Dynamic Range.
	/// </summary>
	[EditorProperty(true, Description = "HDR Enabled", HorizontalAlignment = true, MajorGrouping = 1, MinorGrouping = 2, ToolTipText = "")]
	bool DynamicRangeEnabled { get; set; }

	/// <summary>
	/// Intensity of the scene exposure.
	/// </summary>
	[EditorProperty(true, Description = "Exposure Amount", HorizontalAlignment = true, MajorGrouping = 6, MinorGrouping = 3, ToolTipText = "")]
	[EditorNumberPadOptions(2, 0.0, 100.0, 0.1)]
	float ExposureAmount { get; set; }

	/// <summary>
	/// Intensity of scene colors when using High Dynamic Range.
	/// </summary>
	[EditorNumberPadOptions(2, 0.0, 1.0, 0.05)]
	[EditorProperty(true, Description = "Saturation Amount", HorizontalAlignment = true, MajorGrouping = 8, MinorGrouping = 1, ToolTipText = "")]
	float DynamicRangeSaturationAmount { get; set; }

	/// <summary>
	/// Intensity of scene contrast when using High Dynamic Range.
	/// </summary>
	[EditorProperty(true, Description = "Darken Amount", HorizontalAlignment = true, MajorGrouping = 8, MinorGrouping = 2, ToolTipText = "")]
	[EditorNumberPadOptions(2, 0.0, 0.5, 0.01)]
	float DynamicRangeDarkenAmount { get; set; }

	/// <summary>
	/// Intensity of High Dynamic Range color correction and simulated film exposure effect.
	/// </summary>
	[EditorNumberPadOptions(2, 0.0, 1.0, 0.01)]
	[EditorProperty(true, Description = "Cinematic Amount", HorizontalAlignment = true, MajorGrouping = 8, MinorGrouping = 3, ToolTipText = "")]
	float DynamicRangeCinematicAmount { get; set; }

	/// <summary>
	/// Time required to fully adjust High Dynamic Range to lighting changes.
	/// </summary>
	[EditorProperty(true, Description = "Transition Time", HorizontalAlignment = true, MajorGrouping = 7, MinorGrouping = 3, ToolTipText = "")]
	[EditorNumberPadOptions(2, 0.0, 10000.0, 0.1)]
	float DynamicRangeTransitionTime { get; set; }

	/// <summary>
	/// Maximum intensity increase allowed for High Dynamic Range. Limits intensity
	/// increases, which sets the darkness-level where the scene will remain dark.
	/// </summary>
	[EditorNumberPadOptions(2, 0.0, 10000.0, 0.5)]
	[EditorProperty(true, Description = "HDR Transition Max", HorizontalAlignment = true, MajorGrouping = 7, MinorGrouping = 1, ToolTipText = "")]
	float DynamicRangeTransitionMaxScale { get; set; }

	/// <summary>
	/// Maximum intensity decrease allowed for High Dynamic Range. Limits intensity
	/// decreases, which sets the brightness-level where the scene will remain overly bright.
	/// </summary>
	[EditorNumberPadOptions(2, 0.0, 10000.0, 0.5)]
	[EditorProperty(true, Description = "HDR Transition Min", HorizontalAlignment = true, MajorGrouping = 7, MinorGrouping = 2, ToolTipText = "")]
	float DynamicRangeTransitionMinScale { get; set; }

	/// <summary>
	/// Amount of gravity applied to dynamic collide-able objects in the scene.
	/// </summary>
	[EditorProperty(true, Description = "Gravity", HorizontalAlignment = true, MajorGrouping = 2, MinorGrouping = 3, ToolTipText = "")]
	[EditorNumberPadOptions(4, 0.0, 1000000.0, 0.1)]
	float Gravity { get; set; }
}
