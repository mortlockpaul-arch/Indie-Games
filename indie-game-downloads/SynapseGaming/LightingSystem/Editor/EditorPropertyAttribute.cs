using System;

namespace SynapseGaming.LightingSystem.Editor;

/// <summary>
/// Attribute that marks properties as editable inside the SunBurn Editor and 
/// defines UI-behavior.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class EditorPropertyAttribute : Attribute
{
	private bool _3A_0018;

	private string _3AL;

	private string _3A_0019;

	private int _3A3;

	private int _3A6;

	private bool _3AD;

	private ControlType _3A_0017;

	private bool _3A_0003;

	internal bool EditorVisible => _3A_0018;

	/// <summary>
	/// The human-readable description for this property to be displayed in-editor.
	/// </summary>
	public string Description
	{
		get
		{
			return _3AL;
		}
		set
		{
			_3AL = value;
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
			return _3A_0019;
		}
		set
		{
			_3A_0019 = value;
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
			return _3A3;
		}
		set
		{
			_3A3 = value;
			_3A_0003 = true;
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
			return _3A6;
		}
		set
		{
			_3A6 = value;
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
			return _3AD;
		}
		set
		{
			_3AD = value;
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
			return _3A_0017;
		}
		set
		{
			_3A_0017 = value;
		}
	}

	internal bool PositionSet => _3A_0003;

	/// <summary>
	/// Creates a new EditorPropertyAttribute instance.
	/// </summary>
	/// <param name="editorvisible">Defines whether or not this property will be
	/// displayed in-editor.</param>
	public EditorPropertyAttribute(bool editorvisible)
	{
		_3A_0018 = editorvisible;
		_3A_0017 = ControlType.Default;
	}
}
