using System;

namespace SynapseGaming.LightingSystem.Core;

/// <summary>
/// Defines the types of objects that should be returned in a Find() query.
///
/// This enumeration is a Flag, which allows combining multiple values using the
/// Logical OR operator (example: "ObjectFilter.Dynamic | ObjectFilter.Enabled",
/// finds objects that are both dynamic and enabled).
/// </summary>
[Flags]
public enum ObjectFilter
{
	/// <summary>
	/// Used to include objects of type ObjectType.Dynamic in the query results.
	/// </summary>
	Dynamic = 1,
	/// <summary>
	/// Used to include objects of type ObjectType.Static in the query results.
	/// </summary>
	Static = 2,
	/// <summary>
	/// Used to include objects that are enabled in the query results.
	///
	/// If the objects being queried do not support enabling / disabling they will be included
	/// in the query results regardless of whether this flag is used.
	/// </summary>
	Enabled = 4,
	/// <summary>
	/// Used to include objects that are disabled in the query results.
	///
	/// If the objects being queried do not support enabling / disabling they will be included
	/// in the query results regardless of whether this flag is used.
	/// </summary>
	Disabled = 8,
	/// <summary>
	/// Used to include objects of type ObjectType.Dynamic and ObjectType.Static in the query results.
	/// </summary>
	DynamicAndStatic = Dynamic | Static,
	/// <summary>
	/// Used to include both objects that are enabled and disabled in the query results.
	/// </summary>
	EnabledAndDisabled = Enabled | Disabled,
	/// <summary>
	/// Used to include enabled objects of type ObjectType.Dynamic and ObjectType.Static in the query results.
	/// </summary>
	EnabledDynamicAndStatic = DynamicAndStatic | Enabled,
	/// <summary>
	/// Used to include all objects in the query results.
	///
	/// Note: this includes disabled objects, which in most cases should not be used in normal rendering.
	/// </summary>
	All = 0xFFFF
}
