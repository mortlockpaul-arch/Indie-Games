using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace DPSF;

/// <summary>
/// The Base Particle System class that the Default Particle System classes inherit from
/// </summary>
/// <typeparam name="Particle">The Particle class to use</typeparam>
/// <typeparam name="Vertex">The Vertex Format to use</typeparam>
public abstract class DPSFDefaultBaseParticleSystem<Particle, Vertex> : DPSF<Particle, Vertex> where Particle : DPSFParticle, new() where Vertex : struct, IDPSFParticleVertex
{
	/// <summary>
	/// Particle System Properties used to initialize a Particle's Properties.
	/// <para>NOTE: These are only applied to the Particle when the InitializeParticleUsingInitialProperties()
	/// function is set as the Particle Initialization Function.</para>
	/// </summary>
	public class CInitialProperties
	{
		public float LifetimeMin = 1f;

		public float LifetimeMax = 1f;

		public Vector3 PositionMin = Vector3.Zero;

		public Vector3 PositionMax = Vector3.Zero;

		public Vector3 VelocityMin = Vector3.Zero;

		public Vector3 VelocityMax = Vector3.Zero;

		public Vector3 AccelerationMin = Vector3.Zero;

		public Vector3 AccelerationMax = Vector3.Zero;

		public Vector3 ExternalForceMin = Vector3.Zero;

		public Vector3 ExternalForceMax = Vector3.Zero;

		public float FrictionMin;

		public float FrictionMax;

		public Color StartColorMin = Color.White;

		public Color StartColorMax = Color.White;

		public Color EndColorMin = Color.White;

		public Color EndColorMax = Color.White;

		/// <summary>
		/// If true the Position will be somewhere on the vector joining the Min Position to the Max Position.
		/// <para>If false each of the XYZ components will be randomly calculated individually between the Min and Max Position XYZ values.</para>
		/// <para>Default value is false.</para>
		/// </summary>
		public bool InterpolateBetweenMinAndMaxPosition;

		/// <summary>
		/// If true the Velocity will be somewhere on the vector joining the Min Velocity to the Max Velocity.
		/// <para>If false each of the XYZ components will be randomly calculated individually between the Min and Max Velocity XYZ values.</para>
		/// <para>Default value is false.</para>
		/// </summary>
		public bool InterpolateBetweenMinAndMaxVelocity;

		/// <summary>
		/// If true the Acceleration will be somewhere on the vector joining the Min Acceleration to the Max Acceleration.
		/// <para>If false each of the XYZ components will be randomly calculated individually between the Min and Max Acceleration XYZ values.</para>
		/// <para>Default value is false.</para>
		/// </summary>
		public bool InterpolateBetweenMinAndMaxAcceleration;

		/// <summary>
		/// If true the External Force will be somewhere on the vector joining the Min External Force to the Max External Force.
		/// <para>If false each of the XYZ components will be randomly calculated individually between the Min and Max External Force XYZ values.</para>
		/// <para>Default value is false.</para>
		/// </summary>
		public bool InterpolateBetweenMinAndMaxExternalForce;

		/// <summary>
		/// If true a Lerp'd value between the Min and Max Colors will be randomly chosen.
		/// <para>If false the RGBA component values will be randomly chosen individually between the Min and Max Color RGBA values.</para>
		/// <para>Default value is false.</para>
		/// </summary>
		public bool InterpolateBetweenMinAndMaxColors;

		/// <summary>
		/// If true the Emitter's Position will be added to the Particle's starting Position. For example, if the Particle is given
		/// an initial position of zero it will be placed wherever the Emitter currently is.
		/// <para>Default value is true.</para>
		/// </summary>
		public bool PositionIsAffectedByEmittersPosition = true;

		/// <summary>
		/// If true the Particle's Velocity direction will be adjusted according to the Emitter's Orientation. For example, if the
		/// Emitter is orientated to face backwards, the Particle's Velocity direction will be reversed.
		/// <para>Default value is true.</para>
		/// </summary>
		public bool VelocityIsAffectedByEmittersOrientation = true;
	}

	private CInitialProperties mcInitialProperties = new CInitialProperties();

	private string msName = "Default";

	/// <summary>
	/// A list of Magnets that should affect this Particle System's Particles.
	/// <para>NOTE: You must add a UpdateParticleXAccordingToMagnets function to the Particle
	/// Events in order for these Magnets to affect the Particles.</para>
	/// </summary>
	public LinkedList<DefaultParticleSystemMagnet> MagnetList = new LinkedList<DefaultParticleSystemMagnet>();

	/// <summary>
	/// Get the Settings used to specify the Initial Properties of a new Particle.
	/// <para>NOTE: These are only applied to the Particle when the InitializeParticleUsingInitialProperties()
	/// function is set as the Particle Initialization Function.</para>
	/// </summary>
	public CInitialProperties InitialProperties => mcInitialProperties;

	/// <summary>
	/// The Name of the Particle System
	/// </summary>
	public string Name
	{
		get
		{
			return msName;
		}
		set
		{
			msName = value;
		}
	}

	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="cGame">Handle to the Game object being used. Pass in null for this 
	/// parameter if not using a Game object.</param>
	public DPSFDefaultBaseParticleSystem(Game cGame)
		: base(cGame)
	{
	}

	/// <summary>
	/// Function to Initialize a Default Particle with the Initial Settings
	/// </summary>
	/// <param name="Particle">The Particle to be Initialized</param>
	public virtual void InitializeParticleUsingInitialProperties(DPSFParticle Particle)
	{
		InitializeParticleUsingInitialProperties(Particle, mcInitialProperties);
	}

	/// <summary>
	/// Function to Initialize a Default Particle with the Initial Settings
	/// </summary>
	/// <param name="Particle">The Particle to be Initialized</param>
	/// <param name="cInitialProperties">The Initial Settings to use to Initialize the Particle</param>
	public void InitializeParticleUsingInitialProperties(DPSFParticle Particle, CInitialProperties cInitialProperties)
	{
		DPSFDefaultBaseParticle dPSFDefaultBaseParticle = (DPSFDefaultBaseParticle)Particle;
		dPSFDefaultBaseParticle.Lifetime = DPSFHelper.RandomNumberBetween(cInitialProperties.LifetimeMin, cInitialProperties.LifetimeMax);
		if (cInitialProperties.InterpolateBetweenMinAndMaxPosition)
		{
			dPSFDefaultBaseParticle.Position = Vector3.Lerp(cInitialProperties.PositionMin, cInitialProperties.PositionMax, base.RandomNumber.NextFloat());
		}
		else
		{
			dPSFDefaultBaseParticle.Position = DPSFHelper.RandomVectorBetweenTwoVectors(cInitialProperties.PositionMin, cInitialProperties.PositionMax);
		}
		if (cInitialProperties.VelocityIsAffectedByEmittersOrientation)
		{
			dPSFDefaultBaseParticle.Position = Vector3.Transform(dPSFDefaultBaseParticle.Position, base.Emitter.OrientationData.Orientation);
		}
		if (cInitialProperties.PositionIsAffectedByEmittersPosition)
		{
			dPSFDefaultBaseParticle.Position += base.Emitter.PositionData.Position;
		}
		if (cInitialProperties.InterpolateBetweenMinAndMaxVelocity)
		{
			dPSFDefaultBaseParticle.Velocity = Vector3.Lerp(cInitialProperties.VelocityMin, cInitialProperties.VelocityMax, base.RandomNumber.NextFloat());
		}
		else
		{
			dPSFDefaultBaseParticle.Velocity = DPSFHelper.RandomVectorBetweenTwoVectors(cInitialProperties.VelocityMin, cInitialProperties.VelocityMax);
		}
		dPSFDefaultBaseParticle.Velocity = Vector3.Transform(dPSFDefaultBaseParticle.Velocity, base.Emitter.OrientationData.Orientation);
		if (cInitialProperties.InterpolateBetweenMinAndMaxAcceleration)
		{
			dPSFDefaultBaseParticle.Acceleration = Vector3.Lerp(cInitialProperties.AccelerationMin, cInitialProperties.AccelerationMax, base.RandomNumber.NextFloat());
		}
		else
		{
			dPSFDefaultBaseParticle.Acceleration = DPSFHelper.RandomVectorBetweenTwoVectors(cInitialProperties.AccelerationMin, cInitialProperties.AccelerationMax);
		}
		if (cInitialProperties.InterpolateBetweenMinAndMaxExternalForce)
		{
			dPSFDefaultBaseParticle.ExternalForce = Vector3.Lerp(cInitialProperties.ExternalForceMin, cInitialProperties.ExternalForceMax, base.RandomNumber.NextFloat());
		}
		else
		{
			dPSFDefaultBaseParticle.ExternalForce = DPSFHelper.RandomVectorBetweenTwoVectors(cInitialProperties.ExternalForceMin, cInitialProperties.ExternalForceMax);
		}
		dPSFDefaultBaseParticle.Friction = DPSFHelper.RandomNumberBetween(cInitialProperties.FrictionMin, cInitialProperties.FrictionMax);
		if (cInitialProperties.InterpolateBetweenMinAndMaxColors)
		{
			dPSFDefaultBaseParticle.StartColor = DPSFHelper.LerpColor(cInitialProperties.StartColorMin, cInitialProperties.StartColorMax, base.RandomNumber.NextFloat());
			dPSFDefaultBaseParticle.EndColor = DPSFHelper.LerpColor(cInitialProperties.EndColorMin, cInitialProperties.EndColorMax, base.RandomNumber.NextFloat());
		}
		else
		{
			dPSFDefaultBaseParticle.StartColor = DPSFHelper.LerpColor(cInitialProperties.StartColorMin, cInitialProperties.StartColorMax, base.RandomNumber.NextFloat(), base.RandomNumber.NextFloat(), base.RandomNumber.NextFloat(), base.RandomNumber.NextFloat());
			dPSFDefaultBaseParticle.EndColor = DPSFHelper.LerpColor(cInitialProperties.EndColorMin, cInitialProperties.EndColorMax, base.RandomNumber.NextFloat(), base.RandomNumber.NextFloat(), base.RandomNumber.NextFloat(), base.RandomNumber.NextFloat());
		}
		dPSFDefaultBaseParticle.Color = dPSFDefaultBaseParticle.StartColor;
	}

	/// <summary>
	/// Update a Particle's Position according to its Velocity
	/// </summary>
	/// <param name="cParticle">The Particle to update</param>
	/// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
	protected void UpdateParticlePositionUsingVelocity(DPSFDefaultBaseParticle cParticle, float fElapsedTimeInSeconds)
	{
		cParticle.Position += cParticle.Velocity * fElapsedTimeInSeconds;
	}

	/// <summary>
	/// Update a Particle's Velocity according to its Acceleration
	/// </summary>
	/// <param name="cParticle">The Particle to update</param>
	/// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
	protected void UpdateParticleVelocityUsingAcceleration(DPSFDefaultBaseParticle cParticle, float fElapsedTimeInSeconds)
	{
		cParticle.Velocity += cParticle.Acceleration * fElapsedTimeInSeconds;
	}

	/// <summary>
	/// Updates a Particle's Velocity according to its Acceleration, and then the Position according
	/// to the new Velocity
	/// </summary>
	/// <param name="cParticle">The Particle to update</param>
	/// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
	protected void UpdateParticlePositionAndVelocityUsingAcceleration(DPSFDefaultBaseParticle cParticle, float fElapsedTimeInSeconds)
	{
		cParticle.Velocity += cParticle.Acceleration * fElapsedTimeInSeconds;
		cParticle.Position += cParticle.Velocity * fElapsedTimeInSeconds;
	}

	/// <summary>
	/// Applies the External Force to the Particle's Position
	/// </summary>
	/// <param name="cParticle">The Particle to update</param>
	/// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
	protected void UpdateParticlePositionUsingExternalForce(DPSFDefaultBaseParticle cParticle, float fElapsedTimeInSeconds)
	{
		cParticle.Position += cParticle.ExternalForce * fElapsedTimeInSeconds;
	}

	/// <summary>
	/// Applies the External Force to the Particle's Velocity
	/// </summary>
	/// <param name="cParticle">The Particle to update</param>
	/// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
	protected void UpdateParticleVelocityUsingExternalForce(DPSFDefaultBaseParticle cParticle, float fElapsedTimeInSeconds)
	{
		cParticle.Velocity += cParticle.ExternalForce * fElapsedTimeInSeconds;
	}

	/// <summary>
	/// Applies the Particle's Friction to the its Velocity to slow the Particle down to a stop
	/// </summary>
	/// <param name="cParticle">The Particle to update</param>
	/// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
	protected void UpdateParticleVelocityUsingFriction(DPSFDefaultBaseParticle cParticle, float fElapsedTimeInSeconds)
	{
		if (cParticle.Velocity != Vector3.Zero && cParticle.Friction != 0f)
		{
			Vector3 velocity = cParticle.Velocity;
			float num = velocity.Length();
			num -= cParticle.Friction * fElapsedTimeInSeconds;
			if (num <= 0f)
			{
				velocity = Vector3.Zero;
			}
			else
			{
				velocity.Normalize();
				velocity *= num;
			}
			cParticle.Velocity = velocity;
		}
	}

	/// <summary>
	/// Linearly interpolates the Particles Color between it's Start Color and End Color based on the 
	/// Particle's Normalized Elapsed Time.
	/// </summary>
	/// <param name="cParticle">The Particle to update</param>
	/// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
	protected void UpdateParticleColorUsingLerp(DPSFDefaultBaseParticle cParticle, float fElapsedTimeInSeconds)
	{
		cParticle.Color = DPSFHelper.LerpColor(cParticle.StartColor, cParticle.EndColor, cParticle.NormalizedElapsedTime);
	}

	/// <summary>
	/// Linearly interpolates the Particles Transparency to fade out based on the Particle's Normalized Elapsed Time.
	/// <para>If you are also updating the Particle Color using an EveryTime Event, be sure to set the ExecutionOrder of the 
	/// event calling this function to be greater than that one, so that this function is called AFTER the color update function.</para>
	/// </summary>
	/// <param name="cParticle">The Particle to update</param>
	/// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
	protected void UpdateParticleTransparencyToFadeOutUsingLerp(DPSFDefaultBaseParticle cParticle, float fElapsedTimeInSeconds)
	{
		byte a = (byte)(255f - cParticle.NormalizedElapsedTime * 255f);
		cParticle.Color = new Color(cParticle.Color.R, cParticle.Color.G, cParticle.Color.B, a);
	}

	/// <summary>
	/// Linearly interpolates the Particles Transparency to fade in based on the Particle's Normalized Elapsed Time.
	/// <para>If you are also updating the Particle Color using an EveryTime Event, be sure to set the ExecutionOrder of the 
	/// event calling this function to be greater than that one, so that this function is called AFTER the color update function.</para>
	/// </summary>
	/// <param name="cParticle">The Particle to update</param>
	/// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
	protected void UpdateParticleTransparencyToFadeInUsingLerp(DPSFDefaultBaseParticle cParticle, float fElapsedTimeInSeconds)
	{
		byte a = (byte)(cParticle.NormalizedElapsedTime * 255f);
		cParticle.Color = new Color(cParticle.Color.R, cParticle.Color.G, cParticle.Color.B, a);
	}

	/// <summary>
	/// Quickly fades particle in when born and slowly fades it out as it gets closer to death.
	/// <para>If you are also updating the Particle Color using an EveryTime Event, be sure to set the ExecutionOrder of the 
	/// event calling this function to be greater than that one, so that this function is called AFTER the color update function.</para>
	/// </summary>
	/// <param name="cParticle">The Particle to update</param>
	/// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
	protected void UpdateParticleTransparencyWithQuickFadeInAndSlowFadeOut(DPSFDefaultBaseParticle cParticle, float fElapsedTimeInSeconds)
	{
		byte a = DPSFHelper.FadeInQuicklyAndFadeOutSlowlyBasedOnLifetime(cParticle.NormalizedElapsedTime);
		cParticle.Color = new Color(cParticle.Color.R, cParticle.Color.G, cParticle.Color.B, a);
	}

	/// <summary>
	/// Quickly fades particle in when born and quickly fades it out as it approaches its death.
	/// <para>If you are also updating the Particle Color using an EveryTime Event, be sure to set the ExecutionOrder of the 
	/// event calling this function to be greater than that one, so that this function is called AFTER the color update function.</para>
	/// </summary>
	/// <param name="cParticle">The Particle to update</param>
	/// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
	protected void UpdateParticleTransparencyWithQuickFadeInAndQuickFadeOut(DPSFDefaultBaseParticle cParticle, float fElapsedTimeInSeconds)
	{
		byte a = DPSFHelper.FadeInQuicklyAndFadeOutQuicklyBasedOnLifetime(cParticle.NormalizedElapsedTime);
		cParticle.Color = new Color(cParticle.Color.R, cParticle.Color.G, cParticle.Color.B, a);
	}

	/// <summary>
	/// Calculates how much affect each of the Particle System's Magnets should have on 
	/// this Particle and updates the Particle's Position accordingly.
	/// </summary>
	/// <param name="cParticle">The Particle to update</param>
	/// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
	protected void UpdateParticlePositionAccordingToMagnets(DPSFDefaultBaseParticle cParticle, float fElapsedTimeInSeconds)
	{
		DefaultParticleSystemMagnet defaultParticleSystemMagnet = null;
		for (LinkedListNode<DefaultParticleSystemMagnet> linkedListNode = MagnetList.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
		{
			defaultParticleSystemMagnet = linkedListNode.Value;
			if (defaultParticleSystemMagnet.Mode != DefaultParticleSystemMagnet.MagnetModes.Other)
			{
				cParticle.Position += CalculateForceMagnetShouldExertOnParticle(defaultParticleSystemMagnet, cParticle) * fElapsedTimeInSeconds;
			}
		}
	}

	/// <summary>
	/// Calculates how much affect each of the Particle System's Magnets should have on 
	/// this Particle and updates the Particle's Velocity accordingly.
	/// </summary>
	/// <param name="cParticle">The Particle to update</param>
	/// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
	protected void UpdateParticleVelocityAccordingToMagnets(DPSFDefaultBaseParticle cParticle, float fElapsedTimeInSeconds)
	{
		DefaultParticleSystemMagnet defaultParticleSystemMagnet = null;
		for (LinkedListNode<DefaultParticleSystemMagnet> linkedListNode = MagnetList.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
		{
			defaultParticleSystemMagnet = linkedListNode.Value;
			if (defaultParticleSystemMagnet.Mode != DefaultParticleSystemMagnet.MagnetModes.Other)
			{
				cParticle.Velocity += CalculateForceMagnetShouldExertOnParticle(defaultParticleSystemMagnet, cParticle) * fElapsedTimeInSeconds;
			}
		}
	}

	/// <summary>
	/// Returns the vector force that a Magnet should exert on a Particle
	/// </summary>
	/// <param name="cMagnet">The Magnet affecting the Particle</param>
	/// <param name="cParticle">The Particle being affected by the Magnet</param>
	/// <returns>Returns the vector force that a Magnet should exert on a Particle</returns>
	protected Vector3 CalculateForceMagnetShouldExertOnParticle(DefaultParticleSystemMagnet cMagnet, DPSFDefaultBaseParticle cParticle)
	{
		Vector3 result = Vector3.Zero;
		Vector3 vector;
		if (cMagnet.MagnetType == DefaultParticleSystemMagnet.MagnetTypes.PointMagnet)
		{
			MagnetPoint magnetPoint = (MagnetPoint)cMagnet;
			vector = magnetPoint.PositionData.Position - cParticle.Position;
		}
		else if (cMagnet.MagnetType == DefaultParticleSystemMagnet.MagnetTypes.LineMagnet)
		{
			MagnetLine magnetLine = (MagnetLine)cMagnet;
			Vector3 positionOnLine = magnetLine.PositionOnLine;
			Vector3 vector2 = magnetLine.PositionOnLine + magnetLine.Direction;
			float num = cParticle.Position.X - positionOnLine.X;
			float num2 = cParticle.Position.Y - positionOnLine.Y;
			float num3 = cParticle.Position.Z - positionOnLine.Z;
			float num4 = vector2.X - positionOnLine.X;
			float num5 = vector2.Y - positionOnLine.Y;
			float num6 = vector2.Z - positionOnLine.Z;
			float num7 = num * num4 + num2 * num5 + num3 * num6;
			vector = new Vector3
			{
				X = positionOnLine.X + num7 * num4,
				Y = positionOnLine.Y + num7 * num5,
				Z = positionOnLine.Z + num7 * num6
			} - cParticle.Position;
		}
		else if (cMagnet.MagnetType == DefaultParticleSystemMagnet.MagnetTypes.LineSegmentMagnet)
		{
			MagnetLineSegment magnetLineSegment = (MagnetLineSegment)cMagnet;
			Vector3 endPoint = magnetLineSegment.EndPoint1;
			Vector3 endPoint2 = magnetLineSegment.EndPoint2;
			float num8 = cParticle.Position.X - endPoint.X;
			float num9 = cParticle.Position.Y - endPoint.Y;
			float num10 = cParticle.Position.Z - endPoint.Z;
			float num11 = endPoint2.X - endPoint.X;
			float num12 = endPoint2.Y - endPoint.Y;
			float num13 = endPoint2.Z - endPoint.Z;
			float num14 = num8 * num11 + num9 * num12 + num10 * num13;
			float num15 = num11 * num11 + num12 * num12 + num13 * num13;
			float num16 = num14 / num15;
			Vector3 vector3 = default(Vector3);
			if (num16 < 0f)
			{
				vector3 = endPoint;
			}
			else if (num16 > 1f)
			{
				vector3 = endPoint2;
			}
			else
			{
				vector3.X = endPoint.X + num16 * (endPoint2.X - endPoint.X);
				vector3.Y = endPoint.Y + num16 * (endPoint2.Y - endPoint.Y);
				vector3.Z = endPoint.Z + num16 * (endPoint2.Z - endPoint.Z);
			}
			vector = vector3 - cParticle.Position;
		}
		else
		{
			if (cMagnet.MagnetType != DefaultParticleSystemMagnet.MagnetTypes.PlaneMagnet)
			{
				return Vector3.Zero;
			}
			MagnetPlane magnetPlane = (MagnetPlane)cMagnet;
			float num17 = Vector3.Dot(cParticle.Position - magnetPlane.PositionOnPlane, magnetPlane.Normal);
			Vector3 vector4 = cParticle.Position + -magnetPlane.Normal * num17;
			vector = vector4 - cParticle.Position;
		}
		if (cMagnet.Mode == DefaultParticleSystemMagnet.MagnetModes.Repel)
		{
			vector *= -1f;
		}
		if (vector == Vector3.Zero && cMagnet.Mode == DefaultParticleSystemMagnet.MagnetModes.Repel)
		{
			vector = DPSFHelper.RandomNormalizedVector() * 1E-05f;
		}
		float num18 = vector.Length();
		if (num18 >= cMagnet.MinDistance && num18 <= cMagnet.MaxDistance)
		{
			if (vector != Vector3.Zero)
			{
				vector.Normalize();
			}
			float num19 = 0f;
			num19 = ((cMagnet.MaxDistance == cMagnet.MinDistance) ? 1f : ((num18 - cMagnet.MinDistance) / (cMagnet.MaxDistance - cMagnet.MinDistance)));
			float num20 = 0f;
			num20 = cMagnet.DistanceFunction switch
			{
				DefaultParticleSystemMagnet.DistanceFunctions.Linear => MathHelper.Lerp(0f, cMagnet.MaxForce, num19), 
				DefaultParticleSystemMagnet.DistanceFunctions.Squared => MathHelper.Lerp(0f, cMagnet.MaxForce, num19 * num19), 
				DefaultParticleSystemMagnet.DistanceFunctions.Cubed => MathHelper.Lerp(0f, cMagnet.MaxForce, num19 * num19 * num19), 
				DefaultParticleSystemMagnet.DistanceFunctions.LinearInverse => MathHelper.Lerp(cMagnet.MaxForce, 0f, num19), 
				DefaultParticleSystemMagnet.DistanceFunctions.SquaredInverse => MathHelper.Lerp(cMagnet.MaxForce, 0f, num19 * num19), 
				DefaultParticleSystemMagnet.DistanceFunctions.CubedInverse => MathHelper.Lerp(cMagnet.MaxForce, 0f, num19 * num19 * num19), 
				_ => cMagnet.MaxForce, 
			};
			result = vector * (num20 * cMagnet.MaxForce);
		}
		return result;
	}

	/// <summary>
	/// Sets the Emitter to Emit Particles Automatically
	/// </summary>
	/// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
	protected void UpdateParticleSystemEmitParticlesAutomaticallyOn(float fElapsedTimeInSeconds)
	{
		base.Emitter.EmitParticlesAutomatically = true;
	}

	/// <summary>
	/// Sets the Emitter to not Emit Particles Automatically
	/// </summary>
	/// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
	protected void UpdateParticleSystemEmitParticlesAutomaticallyOff(float fElapsedTimeInSeconds)
	{
		base.Emitter.EmitParticlesAutomatically = false;
	}

	/// <summary>
	/// Enables the Emitter
	/// </summary>
	/// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
	protected void UpdateParticleSystemEnableEmitter(float fElapsedTimeInSeconds)
	{
		base.Emitter.Enabled = true;
	}

	/// <summary>
	/// Disables the Emitter
	/// </summary>
	/// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
	protected void UpdateParticleSystemDisableEmitter(float fElapsedTimeInSeconds)
	{
		base.Emitter.Enabled = false;
	}
}
