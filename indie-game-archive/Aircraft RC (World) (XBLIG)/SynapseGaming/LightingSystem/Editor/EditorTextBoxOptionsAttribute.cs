using System;

namespace SynapseGaming.LightingSystem.Editor;

/// <summary>
/// Attribute for string properties to define textbox specific control options.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class EditorTextBoxOptionsAttribute : BaseControlOptionsAttribute
{
	private int HCB;

	private bool HC_0002;

	/// <summary>
	/// Overrides the control width for this textbox.
	/// </summary>
	public int Width
	{
		get
		{
			return HCB;
		}
		set
		{
			HCB = value;
		}
	}

	internal bool ErrorTextBox => HC_0002;

	/// <summary>
	/// Creates a new EditorTextBoxOptionsAttribute instance.
	/// </summary>
	/// <param name="errortextbox">Defines whether or not this textbox will
	/// be an error textbox. Error textboxes are readonly and display their value
	/// in red text.</param>
	public EditorTextBoxOptionsAttribute(bool errortextbox)
	{
		HC_0002 = errortextbox;
	}
}
