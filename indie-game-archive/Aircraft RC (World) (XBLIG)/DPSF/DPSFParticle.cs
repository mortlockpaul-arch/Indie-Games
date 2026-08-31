namespace DPSF;

/// <summary>
/// Class used to hold a Particle's properties.
/// This class only holds a Particle's Lifetime information, but may be inherited from
/// in order to specify additional Particle properties, such as position, size, color, etc.
/// </summary>
public class DPSFParticle
{
	private float mfElapsedTime;

	private float mfLastElapsedTime;

	private float mfNormalizedElapsedTime;

	private float mfLastNormalizedElapsedTime;

	private float mfLifetime;

	private bool mbVisible;

	/// <summary>
	/// Get / Set how much Time has Elapsed since this Particle was born.
	/// <para>NOTE: Setting this to be greater than or equal to Lifetime will
	/// cause the Particle to become InActive and be removed from the Particle 
	/// System (if Lifetime is greater than zero).</para>
	/// <para>NOTE: Setting this also sets the Last Elapsed Time to the given value.</para>
	/// </summary>
	public float ElapsedTime
	{
		get
		{
			return mfElapsedTime;
		}
		set
		{
			mfElapsedTime = value;
			mfLastElapsedTime = mfElapsedTime;
			if (mfLifetime > 0f)
			{
				mfNormalizedElapsedTime = mfElapsedTime / mfLifetime;
				mfLastNormalizedElapsedTime = mfNormalizedElapsedTime;
			}
			else
			{
				mfLastNormalizedElapsedTime = 0f;
				mfNormalizedElapsedTime = 0f;
			}
		}
	}

	/// <summary>
	/// Get / Set the Normalized Elapsed Time (0.0 - 1.0) of this Particle (How far through its life it is).
	/// <para>NOTE: Setting this to be greater than or equal to 1.0 will cause the Particle to become InActive 
	/// and be removed from the Particle System (if Lifetime is greater than zero).</para>
	/// <para>NOTE: If the Particle has a Lifetime of zero (is set to live forever), Setting this has no effect,
	/// and Getting this will always return zero.</para>
	/// </summary>
	public float NormalizedElapsedTime
	{
		get
		{
			return mfNormalizedElapsedTime;
		}
		set
		{
			mfNormalizedElapsedTime = value;
			if (mfLifetime > 0f)
			{
				mfElapsedTime = (mfLastElapsedTime = mfNormalizedElapsedTime * mfLifetime);
				mfLastNormalizedElapsedTime = mfNormalizedElapsedTime;
			}
			else
			{
				mfLastNormalizedElapsedTime = 0f;
				mfNormalizedElapsedTime = 0f;
			}
		}
	}

	/// <summary>
	/// Get the Elapsed Time of the Particle at the previous frame
	/// </summary>
	public float LastElapsedTime => mfLastElapsedTime;

	/// <summary>
	/// Get the Normalized Elapsed Time of the Particle at the previous frame
	/// </summary>
	public float LastNormalizedElapsedTime => mfLastNormalizedElapsedTime;

	/// <summary>
	/// Get / Set the Lifetime of the Particle (How long it should live for).
	/// <para>NOTE: Setting this to zero will make the Particle live forever.</para>
	/// <para>NOTE: Negative Lifetimes are reset to zero.</para>
	/// </summary>
	public float Lifetime
	{
		get
		{
			return mfLifetime;
		}
		set
		{
			mfLifetime = value;
			if (mfLifetime > 0f)
			{
				mfNormalizedElapsedTime = mfElapsedTime / mfLifetime;
				mfLastNormalizedElapsedTime = mfNormalizedElapsedTime;
			}
			else
			{
				mfLastNormalizedElapsedTime = 0f;
				mfNormalizedElapsedTime = 0f;
				mfLifetime = 0f;
			}
		}
	}

	/// <summary>
	/// Get / Set if the Particle should be Visible (i.e. be drawn) or not
	/// </summary>
	public bool Visible
	{
		get
		{
			return mbVisible;
		}
		set
		{
			mbVisible = value;
		}
	}

	/// <summary>
	/// Constructor to initialize Particle variables
	/// </summary>
	public DPSFParticle()
	{
		Reset();
	}

	/// <summary>
	/// Function to update the Elapsed Time associated variables of the Particle. This is done
	/// automatically by DPSF when the particle system's Update() function is called, so this
	/// function does not need to be manually called by the user.
	/// </summary>
	/// <param name="fElapsedTimeInSeconds">The amount of time in seconds that 
	/// has passed since this function was last called</param>
	public void UpdateElapsedTimeVariables(float fElapsedTimeInSeconds)
	{
		mfLastElapsedTime = mfElapsedTime;
		mfElapsedTime += fElapsedTimeInSeconds;
		if (mfLifetime > 0f)
		{
			mfLastNormalizedElapsedTime = mfNormalizedElapsedTime;
			mfNormalizedElapsedTime = mfElapsedTime / mfLifetime;
		}
		else
		{
			mfLastNormalizedElapsedTime = 0f;
			mfNormalizedElapsedTime = 0f;
		}
	}

	/// <summary>
	/// Function to tell if a Particle is still Active (alive) or not
	/// </summary>
	/// <returns>Returns true if the Particle is Active (alive), false if it is Inactive (dead)</returns>
	public bool IsActive()
	{
		if (!(mfElapsedTime < mfLifetime))
		{
			return mfLifetime <= 0f;
		}
		return true;
	}

	/// <summary>
	/// Resets the Particles variables to default values
	/// </summary>
	public virtual void Reset()
	{
		mfElapsedTime = (mfLastElapsedTime = 0f);
		mfNormalizedElapsedTime = (mfLastNormalizedElapsedTime = 0f);
		mfLifetime = 0f;
		mbVisible = true;
	}

	/// <summary>
	/// Deep copy the ParticleToCopy's values into this Particle
	/// </summary>
	/// <param name="ParticleToCopy">The Particle whose values should be Copied</param>
	public virtual void CopyFrom(DPSFParticle ParticleToCopy)
	{
		Lifetime = ParticleToCopy.Lifetime;
		Visible = ParticleToCopy.Visible;
		ElapsedTime = ParticleToCopy.ElapsedTime;
	}
}
