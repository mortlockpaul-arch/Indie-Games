using System;

namespace SynapseGaming.LightingSystem.Editor;

/// <summary>
/// Attribute for numeric properties to define numberpad specific control options.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class EditorNumberPadOptionsAttribute : BaseControlOptionsAttribute
{
	private int _3A_0018;

	private double _3AL;

	private double _3A_0019;

	private double _3A3;

	internal int DecimalPlaces => _3A_0018;

	internal double MinValue => _3AL;

	internal double MaxValue => _3A_0019;

	internal double Increment => _3A3;

	/// <summary>
	/// Creates a new EditorNumberPadOptionsAttribute instance.
	/// </summary>
	/// <param name="decimalplaces">The number of decimal places to show in the numberpad control.</param>
	/// <param name="minvalue">The minimum allowed value for the property.</param>
	/// <param name="maxvalue">The maximum allowed value for the property.</param>
	/// <param name="increment">The amount to increase/decrease by when the user cycles through the numberpad.</param>
	public EditorNumberPadOptionsAttribute(int decimalplaces, double minvalue, double maxvalue, double increment)
	{
		_3A_0018 = decimalplaces;
		_3AL = minvalue;
		_3A_0019 = maxvalue;
		_3A3 = increment;
	}
}
