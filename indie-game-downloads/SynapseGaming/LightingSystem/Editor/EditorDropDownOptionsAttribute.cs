using System;

namespace SynapseGaming.LightingSystem.Editor;

/// <summary>
/// Attribute for enum properties to define dropdownbox specific control options.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class EditorDropDownOptionsAttribute : BaseControlOptionsAttribute
{
	private int _3A_0018;

	private string[] _3AL;

	internal int Width => _3A_0018;

	internal string[] Values => _3AL;

	/// <summary>
	/// Creates a new EditorDropDownOptionsAttribute instance.
	/// </summary>
	/// <param name="width">Overrides the control width for this dropdownbox.</param>
	public EditorDropDownOptionsAttribute(int width)
	{
		_3A_0018 = width;
	}

	internal EditorDropDownOptionsAttribute(string[] P_0, int P_1)
	{
		_3AL = P_0;
		_3A_0018 = P_1;
	}
}
