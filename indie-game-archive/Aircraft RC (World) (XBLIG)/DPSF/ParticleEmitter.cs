using System;

namespace DPSF;

/// <summary>
/// Class used to automatically create new Particles in a Particle System
/// </summary>
public class ParticleEmitter
{
	private Position3D mcPositionData;

	private Orientation3D mcOrientationData;

	private PivotPoint3D mcPivotPointData;

	private bool mbEnabled = true;

	private bool mbEmitParticlesAutomatically = true;

	private int miBurstNumberOfParticles;

	private float mfBurstTimeInSeconds;

	private float mfParticlesPerSecond;

	private float mfSecondsPerParticle;

	private float mfTimeElapsedSinceGeneratingLastParticle;

	/// <summary>
	/// Get / Set if the Emitter is able to Emit Particles or not.
	/// <para>NOTE: If this is false, not even Bursts will Emit Particles.</para>
	/// <para>NOTE: The Position, Orientation, and Pivot Data will still be updated when this is false.</para>
	/// </summary>
	public bool Enabled
	{
		get
		{
			return mbEnabled;
		}
		set
		{
			mbEnabled = value;
		}
	}

	/// <summary>
	/// Get the Position Data (Position, Velocity, and Acceleration)
	/// </summary>
	public Position3D PositionData => mcPositionData;

	/// <summary>
	/// Get the Orientation Data (Orientation, Rotational Velocity, and Rotational Acceleration)
	/// </summary>
	public Orientation3D OrientationData => mcOrientationData;

	/// <summary>
	/// Get the Pivot Point Data (Pivot Point, Pivot Rotational Velocity, and Pivot Rotational Acceleration)
	/// </summary>
	public PivotPoint3D PivotPointData => mcPivotPointData;

	/// <summary>
	/// Get / Set if the Emitter should Emit Particles Automatically or not.
	/// <para>NOTE: Particles will only be emitted if the Emitter is Enabled.</para>
	/// </summary>
	public bool EmitParticlesAutomatically
	{
		get
		{
			return mbEmitParticlesAutomatically;
		}
		set
		{
			mbEmitParticlesAutomatically = value;
		}
	}

	/// <summary>
	/// Get / Set how many Particles should be emitted per Second
	/// </summary>
	public float ParticlesPerSecond
	{
		get
		{
			return mfParticlesPerSecond;
		}
		set
		{
			mfParticlesPerSecond = value;
			if (mfParticlesPerSecond == 0f)
			{
				mfSecondsPerParticle = 0f;
			}
			else
			{
				mfSecondsPerParticle = 1f / mfParticlesPerSecond;
			}
		}
	}

	/// <summary>
	/// Get / Set how many Particles the Emitter should Burst. The Emitter will emit
	/// Particles, at the speed corresponding to its Particles Per Second rate, until this amount 
	/// of Particles have been emitted.
	/// <para>NOTE: Bursts are only processed when the Emit Particles Automatically property is false.</para>
	/// <para>NOTE: Bursts will only emit Particles if the Emitter is Enabled.</para>
	/// <para>NOTE: This will be set to zero if a negative value is specified.</para>
	/// <para>NOTE: This will fire the BurstComplete event when it reaches (or is set to) zero.</para>
	/// </summary>
	public int BurstParticles
	{
		get
		{
			return miBurstNumberOfParticles;
		}
		set
		{
			if (value <= 0)
			{
				miBurstNumberOfParticles = 0;
				if (BurstComplete != null)
				{
					BurstComplete(this, null);
				}
			}
			else
			{
				miBurstNumberOfParticles = value;
			}
		}
	}

	/// <summary>
	/// Get / Set how long the Emitter should Burst for (in seconds). The Emitter will emit
	/// Particles, at the speed corresponding to its Particles Per Second rate, until this amount 
	/// of time in seconds has elapsed.
	/// <para>NOTE: Bursts are only processed when the Emit Particles Automatically property is false.</para>
	/// <para>NOTE: Bursts will only emit Particles if the Emitter is Enabled.</para>
	/// <para>NOTE: This will be set to zero if a negative value is specified.</para>
	/// <para>NOTE: This will fire the BurstComplete event when it reaches (or is set to) zero.</para>
	/// </summary>
	public float BurstTime
	{
		get
		{
			return mfBurstTimeInSeconds;
		}
		set
		{
			if (value <= 0f)
			{
				mfBurstTimeInSeconds = 0f;
				if (BurstComplete != null)
				{
					BurstComplete(this, null);
				}
			}
			else
			{
				mfBurstTimeInSeconds = value;
			}
		}
	}

	/// <summary>
	/// Raised when a Burst property reaches (or is set to) zero 
	/// </summary>
	public event EventHandler BurstComplete;

	/// <summary>
	/// Initializes a new instance of the <see cref="T:DPSF.ParticleEmitter" /> class.
	/// </summary>
	public ParticleEmitter()
	{
		mcPositionData = new Position3D();
		mcOrientationData = new Orientation3D();
		mcPivotPointData = new PivotPoint3D(mcPositionData, mcOrientationData);
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:DPSF.ParticleEmitter" /> class.
	/// </summary>
	/// <param name="emitterToCopy">The emitter to copy from.</param>
	public ParticleEmitter(ParticleEmitter emitterToCopy)
		: this()
	{
		CopyFrom(emitterToCopy);
	}

	/// <summary>
	/// Copies the given Emitter's values into this instance.
	/// </summary>
	/// <param name="emitterToCopy">The emitter to copy from.</param>
	public void CopyFrom(ParticleEmitter emitterToCopy)
	{
		mcPositionData.CopyFrom(emitterToCopy.mcPositionData);
		mcOrientationData.CopyFrom(emitterToCopy.mcOrientationData);
		mcPivotPointData = new PivotPoint3D(mcPositionData, mcOrientationData);
		mbEnabled = emitterToCopy.mbEnabled;
		mbEmitParticlesAutomatically = emitterToCopy.mbEmitParticlesAutomatically;
		miBurstNumberOfParticles = emitterToCopy.miBurstNumberOfParticles;
		mfBurstTimeInSeconds = emitterToCopy.mfBurstTimeInSeconds;
		mfParticlesPerSecond = emitterToCopy.mfParticlesPerSecond;
		mfSecondsPerParticle = emitterToCopy.mfSecondsPerParticle;
		mfTimeElapsedSinceGeneratingLastParticle = emitterToCopy.mfTimeElapsedSinceGeneratingLastParticle;
		BurstComplete = emitterToCopy.BurstComplete;
	}

	/// <summary>
	/// Updates the Emitter's Position and Orientation according to its 
	/// Velocities and Accelerations, and returns how many Particles should 
	/// be emitted this frame.
	/// </summary>
	/// <param name="fElapsedTimeInSeconds">How long (in seconds) it has been 
	/// since this function was called</param>
	/// <returns>Returns the number of Particles that should be emitted</returns>
	public int UpdateAndGetNumberOfParticlesToEmit(float fElapsedTimeInSeconds)
	{
		PositionData.Update(fElapsedTimeInSeconds);
		OrientationData.Update(fElapsedTimeInSeconds);
		PivotPointData.Update(fElapsedTimeInSeconds);
		int num = 0;
		if (mfParticlesPerSecond > 0f && mbEnabled)
		{
			if (mbEmitParticlesAutomatically)
			{
				num = CalculateHowManyParticlesToEmit(fElapsedTimeInSeconds);
			}
			else if (mfBurstTimeInSeconds > 0f)
			{
				float num2 = fElapsedTimeInSeconds;
				if (num2 > mfBurstTimeInSeconds)
				{
					num2 = mfBurstTimeInSeconds;
				}
				num = CalculateHowManyParticlesToEmit(num2);
				BurstTime -= num2;
			}
			else if (miBurstNumberOfParticles > 0)
			{
				num = CalculateHowManyParticlesToEmit(fElapsedTimeInSeconds);
				if (num > miBurstNumberOfParticles)
				{
					num = miBurstNumberOfParticles;
				}
				BurstParticles -= num;
			}
		}
		return num;
	}

	/// <summary>
	/// Calculates how many Particles should be emitted based on the amount of Time Elapsed
	/// </summary>
	/// <param name="fElapsedTimeInSeconds">How much Time has Elapsed (in seconds) since the last Update</param>
	/// <returns>Returns how many Particles should be emitted</returns>
	private int CalculateHowManyParticlesToEmit(float fElapsedTimeInSeconds)
	{
		int num = 0;
		mfTimeElapsedSinceGeneratingLastParticle += fElapsedTimeInSeconds;
		float num2 = mfTimeElapsedSinceGeneratingLastParticle / mfSecondsPerParticle;
		num = (int)Math.Floor(num2);
		mfTimeElapsedSinceGeneratingLastParticle = (num2 - (float)num) * mfSecondsPerParticle;
		return num;
	}
}
