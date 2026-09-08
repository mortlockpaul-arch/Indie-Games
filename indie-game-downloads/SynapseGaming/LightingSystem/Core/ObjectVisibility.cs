using System;

namespace SynapseGaming.LightingSystem.Core;

/// <summary>
/// Defines how objects are rendered.
///
/// This enumeration is a Flag, which allows combining multiple values using the
/// Logical OR operator (example: "ObjectVisibility.Rendered | ObjectVisibility.CastShadows",
/// both renders objects and casts shadows from them).
/// </summary>
[Flags]
public enum ObjectVisibility
{
	/// <summary>
	/// Object is not rendered.
	/// </summary>
	None = 0,
	/// <summary>
	/// Object is rendered on screen.
	/// </summary>
	Rendered = 1,
	/// <summary>
	/// Object casts shadows.
	/// </summary>
	CastShadows = 2,
	/// <summary>
	/// Object is rendered on screen when the editor is open.
	/// </summary>
	RenderedInEditor = 4,
	/// <summary>
	/// Object is rendered on screen and casts shadows.
	/// </summary>
	RenderedAndCastShadows = Rendered | CastShadows | RenderedInEditor
}
