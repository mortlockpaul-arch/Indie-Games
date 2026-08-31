using System;

namespace SynapseGaming.LightingSystem.Editor;

/// <summary>
/// Attribute for numeric properties to define numberpad specific control options.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class EditorNumberPadOptionsAttribute : BaseControlOptionsAttribute
{
	private int HCB;

	private double HC_0002;

	private double HC_0012;

	private double HCH;

	internal int DecimalPlaces => HCB;

	internal double MinValue => HC_0002;

	internal double MaxValue => HC_0012;

	internal double Increment => HCH;

	/// <summary>
	/// Creates a new EditorNumberPadOptionsAttribute instance.
	/// </summary>
	/// <param name="decimalplaces">The number of decimal places to show in the numberpad control.</param>
	/// <param name="minvalue">The minimum allowed value for the property.</param>
	/// <param name="maxvalue">The maximum allowed value for the property.</param>
	/// <param name="increment">The amount to increase/decrease by when the user cycles through the numberpad.</param>
	public EditorNumberPadOptionsAttribute(int decimalplaces, double minvalue, double maxvalue, double increment)
	{
		HCB = decimalplaces;
		HC_0002 = minvalue;
		HC_0012 = maxvalue;
		HCH = increment;
	}
}
