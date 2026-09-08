using System;

namespace SynapseGaming.LightingSystem.Core;

/// <summary>
/// Statistic categories used when rendering. Data is always captured even when not rendered.
///
/// This enumeration is a Flag, which allows combining multiple values using the
/// Logical OR operator (example: "RenderCategories.Rendering | RenderCategories.Lighting",
/// renders both rendering and lighting statistics).
/// </summary>
[Flags]
public enum SystemStatisticCategory
{
	/// <summary>
	/// Only renders the frame rate and total poly count.
	/// </summary>
	None = 0,
	/// <summary>
	/// Renders all rendering related statistics.
	/// </summary>
	Rendering = 1,
	/// <summary>
	/// Renders all lighting related statistics.
	/// </summary>
	Lighting = 2,
	/// <summary>
	/// Renders all shadowing related statistics.
	/// </summary>
	Shadowing = 4,
	/// <summary>
	/// Renders all scenegraph related statistics.
	/// </summary>
	SceneGraph = 8,
	/// <summary>
	/// Renders all collision related statistics.
	/// </summary>
	Collision = 0x10,
	/// <summary>
	///
	/// </summary>
	Performance = 0x20,
	/// <summary>
	/// Category used for storing and rendering user defined statistics.
	/// </summary>
	UserDefined1 = 0x10000,
	/// <summary>
	/// Category used for storing and rendering user defined statistics.
	/// </summary>
	UserDefined2 = 0x20000,
	/// <summary>
	/// Category used for storing and rendering user defined statistics.
	/// </summary>
	UserDefined3 = 0x40000,
	/// <summary>
	/// Category used for storing and rendering user defined statistics.
	/// </summary>
	UserDefined4 = 0x80000,
	/// <summary>
	/// Category used for storing and rendering user defined statistics.
	/// </summary>
	UserDefined5 = 0x100000,
	/// <summary>
	/// Category used for storing and rendering user defined statistics.
	/// </summary>
	UserDefined6 = 0x200000,
	/// <summary>
	/// Category used for storing and rendering user defined statistics.
	/// </summary>
	UserDefined7 = 0x400000,
	/// <summary>
	/// Category used for storing and rendering user defined statistics.
	/// </summary>
	UserDefined8 = 0x800000,
	/// <summary>
	/// Renders all statistics.
	/// </summary>
	All = 0x3FFFFFFF
}
