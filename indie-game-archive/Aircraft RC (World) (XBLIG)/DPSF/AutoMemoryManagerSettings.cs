using Microsoft.Xna.Framework;

namespace DPSF;

/// <summary>
/// Class to hold the Automatic Memory Manager Settings
/// </summary>
public class AutoMemoryManagerSettings
{
	/// <summary>
	/// The Memory Management Mode being used.
	/// <para>NOTE: Default value is AutoMemoryManagerModes.IncreaseAndDecrease.</para>
	/// </summary>
	public AutoMemoryManagerModes MemoryManagementMode = AutoMemoryManagerModes.IncreaseOnly;

	private int miAbsoluteMinNumberOfParticles = 10;

	private float mfReduceAmount = 1.1f;

	private float mfIncreaseAmount = 2f;

	private float mfSecondsMaxNumberOfParticlesMustExistForBeforeReducingSize = 3f;

	/// <summary>
	/// The Absolute Minimum Number Of Particles this Particle System has to have memory allocated for.
	/// The Automatic Memory Manager will never allocate space for fewer Particles than this.
	/// <para>NOTE: This value must be greater than zero.</para>
	/// <para>NOTE: Default value is 10.</para>
	/// </summary>
	public int AbsoluteMinNumberOfParticles
	{
		get
		{
			return miAbsoluteMinNumberOfParticles;
		}
		set
		{
			if (value > 0)
			{
				miAbsoluteMinNumberOfParticles = value;
			}
		}
	}

	/// <summary>
	/// The Automatic Memory Manager keeps track of the Max Particles that were Active in a single
	/// frame over the last X seconds (call this number M). If the Max Number Of Particles is greater
	/// than M, the Automatic Memory Manager can de-allocate unused memory. The Reduce Amount determines
	/// how much more memory than M to allocate. For example, setting the Reduce Amount to 1.0 would set
	/// the Max Number Of Particles to M. Setting the Reduce Amount to 1.1 would set the Max Number Of
	/// Particles to M + 10%. Setting it to 2.0 would set the Max Number Of Particles to M + 100% (i.e. M * 2).
	/// <para>NOTE: This value is clamped to the range 1.0 - 2.0.</para>
	/// <para>NOTE: The Automatic Memory Manager will never reduce the amount of memory to be less than
	/// what is required for the Absolute Min Number Of Particles.</para>
	/// <para>NOTE: Default value is 1.1.</para>
	/// </summary>
	public float ReduceAmount
	{
		get
		{
			return mfReduceAmount;
		}
		set
		{
			mfReduceAmount = MathHelper.Clamp(value, 1f, 2f);
		}
	}

	/// <summary>
	/// The amount the Automatic Memory Manager increases the memory allocated for Particles by.
	/// When adding a new Particle, if we discover that the Number Of Active Particles has reached
	/// the Max Number Of Particles, the Automatic Memory Manager will increase the Max Number Of
	/// Particles by the Increase Amount. For example, if the Increase Amount is set to 2.0, then 
	/// the Max Number Of Particles will be doubled (200%). If it is set to 3.0 it will be tripled 
	/// (300%). If it is set to 0.5, the Max Number Of Particles will be increased to 150%.
	/// <para>NOTE: This value is clamped to the range 1.01 - 10.0 (i.e. 101% - 1000%).</para>
	/// <para>NOTE: The Automatic Memory Manager will never increase the amount of memory to be more than
	/// what is required by the Absolute Max Number Of Particles.</para>
	/// <para>NOTE: Default value is 2.0.</para>
	/// </summary>
	public float IncreaseAmount
	{
		get
		{
			return mfIncreaseAmount;
		}
		set
		{
			mfIncreaseAmount = MathHelper.Clamp(value, 1.01f, 10f);
		}
	}

	/// <summary>
	/// The Automatic Memory Manager keeps track of the Max Particles that were Active in a single
	/// frame over the last X seconds (call this number M). If the Max Number Of Particles is greater
	/// than M, the Automatic Memory Manager can de-allocate unused memory. The Seconds Max Number Of 
	/// Particles Must Exist For Before Reducing Size tells how long M must be unchanged for before
	/// the Automatic Memory Manager can reduce the amount of allocated memory.
	/// <para>NOTE: This value must be greater than zero.</para>
	/// <para>NOTE: Default value is 3.0.</para>
	/// </summary>
	public float SecondsMaxNumberOfParticlesMustExistForBeforeReducingSize
	{
		get
		{
			return mfSecondsMaxNumberOfParticlesMustExistForBeforeReducingSize;
		}
		set
		{
			if (value > 0f)
			{
				mfSecondsMaxNumberOfParticlesMustExistForBeforeReducingSize = value;
			}
		}
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:DPSF.AutoMemoryManagerSettings" /> class.
	/// </summary>
	public AutoMemoryManagerSettings()
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:DPSF.AutoMemoryManagerSettings" /> class, copying all of the settings from the given Settings To Copy.
	/// </summary>
	/// <param name="settingsToCopy">The settings to copy from.</param>
	public AutoMemoryManagerSettings(AutoMemoryManagerSettings settingsToCopy)
	{
		CopyFrom(settingsToCopy);
	}

	/// <summary>
	/// Copies the given Auto Memory Manager Settings into this instance.
	/// </summary>
	/// <param name="settingsToCopy">The settings to copy from.</param>
	public void CopyFrom(AutoMemoryManagerSettings settingsToCopy)
	{
		MemoryManagementMode = settingsToCopy.MemoryManagementMode;
		miAbsoluteMinNumberOfParticles = settingsToCopy.miAbsoluteMinNumberOfParticles;
		mfReduceAmount = settingsToCopy.mfReduceAmount;
		mfIncreaseAmount = settingsToCopy.mfIncreaseAmount;
		mfSecondsMaxNumberOfParticlesMustExistForBeforeReducingSize = settingsToCopy.mfSecondsMaxNumberOfParticlesMustExistForBeforeReducingSize;
	}
}
