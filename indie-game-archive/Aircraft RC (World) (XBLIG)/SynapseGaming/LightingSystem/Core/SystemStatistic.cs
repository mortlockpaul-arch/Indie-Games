namespace SynapseGaming.LightingSystem.Core;

/// <summary>
/// Represents a single numeric statistic, which can be rendered on-screen
/// or saved to file using the SystemConsole class.
/// </summary>
public class SystemStatistic
{
	/// <summary>
	/// Current accumulating value being generated over this frame. This is the value
	/// to increment when supplying statistic information. For instance if the statistic
	/// tracks object rendering, then whenever an object is rendered increment the AccumulationValue
	/// by one.
	/// </summary>
	public int AccumulationValue;

	private string HCB = string.Empty;

	private SystemStatisticCategory HC_0002;

	private int HC_0012;

	/// <summary>
	/// Unique display name for the statistic.
	/// </summary>
	public string Name => HCB;

	/// <summary>
	/// Categories the statistic is assigned to.
	/// </summary>
	public SystemStatisticCategory Category => HC_0002;

	/// <summary>
	/// Fully accumulated value generated during the last frame. This is the display value.
	/// </summary>
	public int Value => HC_0012;

	internal SystemStatistic(string P_0, SystemStatisticCategory P_1)
	{
		HCB = P_0;
		HC_0002 = P_1;
	}

	internal void _0002M(string P_0)
	{
		HCB = P_0;
	}

	internal void R()
	{
		HC_0012 = AccumulationValue;
		AccumulationValue = 0;
	}
}
