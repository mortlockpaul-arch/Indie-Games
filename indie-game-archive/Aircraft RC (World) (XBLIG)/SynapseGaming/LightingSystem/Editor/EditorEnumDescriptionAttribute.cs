using System;

namespace SynapseGaming.LightingSystem.Editor;

/// <summary>
/// Attribute to give enum values human-readable descriptions to be displayed in-editor.
/// </summary>
[AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public class EditorEnumDescriptionAttribute : Attribute
{
	private string HCB;

	private bool HC_0002;

	internal string Description => HCB;

	internal bool Ignore => HC_0002;

	/// <summary>
	/// Creates a new EditorEnumDescriptionAttribute instance.
	/// </summary>
	/// <param name="description">The human-readable description to display
	/// in-editor.</param>
	public EditorEnumDescriptionAttribute(string description)
	{
		HCB = description;
		HC_0002 = false;
	}

	/// <summary>
	/// Creates a new EditorEnumDescriptionAttribute instance.
	/// </summary>
	/// <param name="ignore">Defines whether or not this enum value will
	/// be hidden in-editor.</param>
	public EditorEnumDescriptionAttribute(bool ignore)
	{
		HC_0002 = ignore;
	}
}
