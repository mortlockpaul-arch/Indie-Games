using System;

namespace SynapseGaming.LightingSystem.Editor;

/// <summary>
/// Attribute that marks properties as editable inside the SunBurn Editor and 
/// defines UI-behavior.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class EditorPropertyAttribute : Attribute
{
	private bool HCB;

	private string HC_0002;

	private string HC_0012;

	private int HCH;

	private int HC7;

	private bool HC_0001;

	private ControlType HCw;

	private bool HCZ;

	internal bool EditorVisible => HCB;

	/// <summary>
	/// The human-readable description for this property to be displayed in-editor.
	/// </summary>
	public string Description
	{
		get
		{
			return HC_0002;
		}
		set
		{
			HC_0002 = value;
		}
	}

	/// <summary>
	/// The Tooltip information to display when the mouse hovers over the
	/// control for this property.
	/// </summary>
	public string ToolTipText
	{
		get
		{
			return HC_0012;
		}
		set
		{
			HC_0012 = value;
		}
	}

	/// <summary>
	/// The major grouping for organizing properties. Major groups are separated
	/// by dividers in the properties panel.
	/// </summary>
	public int MajorGrouping
	{
		get
		{
			return HCH;
		}
		set
		{
			HCH = value;
			HCZ = true;
		}
	}

	/// <summary>
	/// The minor grouping for organizing properties. This defines the order of 
	/// properties within the same major group.
	/// </summary>
	public int MinorGrouping
	{
		get
		{
			return HC7;
		}
		set
		{
			HC7 = value;
		}
	}

	/// <summary>
	/// Defines the position of the description label for this property. True for the label
	/// to appear next to the control, false for it to appear above the control.
	/// </summary>
	public bool HorizontalAlignment
	{
		get
		{
			return HC_0001;
		}
		set
		{
			HC_0001 = value;
		}
	}

	/// <summary>
	/// Overrides the default control type. Useful to display Vector3 properties
	/// as a color selection box.
	/// Use with caution, will create unexpected results for mismatched datatypes.
	/// </summary>
	public ControlType ControlType
	{
		get
		{
			return HCw;
		}
		set
		{
			HCw = value;
		}
	}

	internal bool PositionSet => HCZ;

	/// <summary>
	/// Creates a new EditorPropertyAttribute instance.
	/// </summary>
	/// <param name="editorvisible">Defines whether or not this property will be
	/// displayed in-editor.</param>
	public EditorPropertyAttribute(bool editorvisible)
	{
		HCB = editorvisible;
		HCw = ControlType.Default;
	}
}
