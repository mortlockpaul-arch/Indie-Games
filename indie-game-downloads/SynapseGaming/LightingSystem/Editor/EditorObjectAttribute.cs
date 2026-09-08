using System;

namespace SynapseGaming.LightingSystem.Editor;

/// <summary>
/// Attribute that marks the class as editable inside the SunBurn Editor. 
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
public class EditorObjectAttribute : Attribute
{
	private bool _3A_0018;

	internal bool OnlyMarkedProperties => _3A_0018;

	/// <summary>
	/// Creates a new EditorObjectAttribute instance.
	/// </summary>
	/// <param name="onlymarkedproperties">Defines whether or not the editor will ignore
	/// public properties that do not have an EditorPropertyAttribute.</param>
	public EditorObjectAttribute(bool onlymarkedproperties)
	{
		_3A_0018 = onlymarkedproperties;
	}
}
