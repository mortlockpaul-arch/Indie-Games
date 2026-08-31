using System;

namespace SynapseGaming.LightingSystem.Editor;

/// <summary>
/// Attribute for boolean properties to define checkbox specific control options.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class EditorCheckboxOptionsAttribute : BaseControlOptionsAttribute
{
	private bool HCB;

	internal bool IntegerValue => HCB;

	/// <summary>
	/// Creates a new EditorCheckboxOptionsAttribute instance.
	/// </summary>
	/// <param name="integervalue">Defines whether or not the checkbox should
	/// return an integer value (0=false, 1=true), instead of a boolean value.
	/// Useful to allow checkboxes to control enum properties.</param>
	public EditorCheckboxOptionsAttribute(bool integervalue)
	{
		HCB = integervalue;
	}
}
