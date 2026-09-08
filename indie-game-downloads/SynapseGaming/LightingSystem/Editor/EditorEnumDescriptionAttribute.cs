using System;

namespace SynapseGaming.LightingSystem.Editor;

/// <summary>
/// Attribute to give enum values human-readable descriptions to be displayed in-editor.
/// </summary>
[AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public class EditorEnumDescriptionAttribute : Attribute
{
	private string _3A_0018;

	private bool _3AL;

	internal string Description => _3A_0018;

	internal bool Ignore => _3AL;

	/// <summary>
	/// Creates a new EditorEnumDescriptionAttribute instance.
	/// </summary>
	/// <param name="description">The human-readable description to display
	/// in-editor.</param>
	public EditorEnumDescriptionAttribute(string description)
	{
		_3A_0018 = description;
		_3AL = false;
	}

	/// <summary>
	/// Creates a new EditorEnumDescriptionAttribute instance.
	/// </summary>
	/// <param name="ignore">Defines whether or not this enum value will
	/// be hidden in-editor.</param>
	public EditorEnumDescriptionAttribute(bool ignore)
	{
		_3AL = ignore;
	}
}
