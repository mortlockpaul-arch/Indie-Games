using System;

namespace SynapseGaming.LightingSystem.Editor;

/// <summary>
/// Attribute for enum properties to define dropdownbox specific control options.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class EditorDropDownOptionsAttribute : BaseControlOptionsAttribute
{
	private int HCB;

	private string[] HC_0002;

	internal int Width => HCB;

	internal string[] Values => HC_0002;

	/// <summary>
	/// Creates a new EditorDropDownOptionsAttribute instance.
	/// </summary>
	/// <param name="width">Overrides the control width for this dropdownbox.</param>
	public EditorDropDownOptionsAttribute(int width)
	{
		HCB = width;
	}

	internal EditorDropDownOptionsAttribute(string[] P_0, int P_1)
	{
		HC_0002 = P_0;
		HCB = P_1;
	}
}
