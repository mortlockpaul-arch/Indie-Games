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

	private string _3A_0018 = string.Empty;

	private SystemStatisticCategory _3AL;

	private int _3A_0019;

	/// <summary>
	/// Unique display name for the statistic.
	/// </summary>
	public string Name => _3A_0018;

	/// <summary>
	/// Categories the statistic is assigned to.
	/// </summary>
	public SystemStatisticCategory Category => _3AL;

	/// <summary>
	/// Fully accumulated value generated during the last frame. This is the display value.
	/// </summary>
	public int Value => _3A_0019;

	internal SystemStatistic(string P_0, SystemStatisticCategory P_1)
	{
		_3A_0018 = P_0;
		_3AL = P_1;
	}

	internal void Li(string P_0)
	{
		_3A_0018 = P_0;
	}

	internal void _0016()
	{
		_3A_0019 = AccumulationValue;
		AccumulationValue = 0;
	}
}
