using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DPSF;

/// <summary>
/// The Default Sprite Particle System class
/// </summary>
/// <typeparam name="Particle">The Particle class to use</typeparam>
/// <typeparam name="Vertex">The Vertex Format to use</typeparam>
public class DPSFDefaultSpriteParticleSystem<Particle, Vertex> : DPSFDefaultBaseParticleSystem<Particle, Vertex> where Particle : DPSFParticle, new() where Vertex : struct, IDPSFParticleVertex
{
	/// <summary>
	/// Particle System Properties used to initialize a Particle's Properties.
	/// <para>NOTE: These are only applied to the Particle when the InitializeParticleUsingInitialProperties()
	/// function is set as the Particle Initialization Function.</para>
	/// </summary>
	public class CInitialPropertiesForSprite : CInitialProperties
	{
		public float RotationMin;

		public float RotationMax;

		public float RotationalVelocityMin;

		public float RotationalVelocityMax;

		public float RotationalAccelerationMin;

		public float RotationalAccelerationMax;

		public float StartWidthMin = 10f;

		public float StartWidthMax = 10f;

		public float StartHeightMin = 10f;

		public float StartHeightMax = 10f;

		public float EndWidthMin = 10f;

		public float EndWidthMax = 10f;

		public float EndHeightMin = 10f;

		public float EndHeightMax = 10f;

		/// <summary>
		/// The Min Start Size for the particle's StartWidth and StartHeight properties.
		/// <para>NOTE: If this is greater than zero, this will be used instead of 
		/// the StartWidthMin and StartHeightMin properties.</para>
		/// </summary>
		public float StartSizeMin;

		/// <summary>
		/// The Max Start Size for the particle's StartWidth and StartHeight properties.
		/// <para>NOTE: If this is greater than zero, this will be used instead of 
		/// the StartWidthMax and StartHeightMax properties.</para>
		/// </summary>
		public float StartSizeMax;

		/// <summary>
		/// The Min End Size for the particle's EndWidth and EndHeight properties.
		/// <para>NOTE: If this is greater than zero, this will be used instead of 
		/// the EndWidthMin and EndHeightMin properties.</para>
		/// </summary>
		public float EndSizeMin;

		/// <summary>
		/// The Max End Size for the particle's EndWidth and EndHeight properties.
		/// <para>NOTE: If this is greater than zero, this will be used instead of 
		/// the EndWidthMax and EndHeightMax properties.</para>
		/// </summary>
		public float EndSizeMax;
	}

	private CInitialPropertiesForSprite mcInitialProperties = new CInitialPropertiesForSprite();

	/// <summary>
	/// Get the Settings used to specify the Initial Properties of a new Particle.
	/// <para>NOTE: These are only applied to the Particle when the InitializeParticleUsingInitialProperties()
	/// function is set as the Particle Initialization Function.</para>
	/// </summary>
	public new CInitialPropertiesForSprite InitialProperties => mcInitialProperties;

	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="cGame">Handle to the Game object being used. Pass in null for this 
	/// parameter if not using a Game object.</param>
	public DPSFDefaultSpriteParticleSystem(Game cGame)
		: base(cGame)
	{
	}

	/// <summary>
	/// Function to draw a Sprite Particle. This function should be used to draw the given
	/// Particle with the provided SpriteBatch.
	/// </summary>
	/// <param name="Particle">The Particle Sprite to Draw</param>
	/// <param name="cSpriteBatch">The SpriteBatch to use to doing the Drawing</param>
	protected override void DrawSprite(DPSFParticle Particle, SpriteBatch cSpriteBatch)
	{
		DefaultSpriteParticle defaultSpriteParticle = (DefaultSpriteParticle)Particle;
		Rectangle destinationRectangle = new Rectangle((int)defaultSpriteParticle.Position.X, (int)defaultSpriteParticle.Position.Y, (int)defaultSpriteParticle.Width, (int)defaultSpriteParticle.Height);
		float layerDepth = MathHelper.Clamp(defaultSpriteParticle.Position.Z, 0f, 1f);
		Rectangle value = new Rectangle(0, 0, base.Texture.Width, base.Texture.Height);
		cSpriteBatch.Draw(origin: new Vector2(value.Width / 2, value.Height / 2), texture: base.Texture, destinationRectangle: destinationRectangle, sourceRectangle: value, color: defaultSpriteParticle.Color, rotation: defaultSpriteParticle.Rotation, effects: defaultSpriteParticle.FlipMode, layerDepth: layerDepth);
	}

	/// <summary>
	/// Function to Initialize a Default Particle with default Properties
	/// </summary>
	/// <param name="Particle">The Particle to be Initialized</param>
	public override void InitializeParticleUsingInitialProperties(DPSFParticle Particle)
	{
		DefaultSpriteParticle defaultSpriteParticle = (DefaultSpriteParticle)Particle;
		InitializeParticleUsingInitialProperties(defaultSpriteParticle, mcInitialProperties);
		defaultSpriteParticle.Rotation = DPSFHelper.RandomNumberBetween(mcInitialProperties.RotationMin, mcInitialProperties.RotationMax);
		defaultSpriteParticle.RotationalVelocity = DPSFHelper.RandomNumberBetween(mcInitialProperties.RotationalVelocityMin, mcInitialProperties.RotationalVelocityMax);
		defaultSpriteParticle.RotationalAcceleration = DPSFHelper.RandomNumberBetween(mcInitialProperties.RotationalAccelerationMin, mcInitialProperties.RotationalAccelerationMax);
		defaultSpriteParticle.StartWidth = DPSFHelper.RandomNumberBetween((mcInitialProperties.StartSizeMin > 0f) ? mcInitialProperties.StartSizeMin : mcInitialProperties.StartWidthMin, (mcInitialProperties.StartSizeMax > 0f) ? mcInitialProperties.StartSizeMax : mcInitialProperties.StartWidthMax);
		defaultSpriteParticle.EndWidth = DPSFHelper.RandomNumberBetween((mcInitialProperties.EndSizeMin > 0f) ? mcInitialProperties.EndSizeMin : mcInitialProperties.EndWidthMin, (mcInitialProperties.EndSizeMax > 0f) ? mcInitialProperties.EndSizeMax : mcInitialProperties.EndWidthMax);
		defaultSpriteParticle.StartHeight = DPSFHelper.RandomNumberBetween((mcInitialProperties.StartSizeMin > 0f) ? mcInitialProperties.StartSizeMin : mcInitialProperties.StartHeightMin, (mcInitialProperties.StartSizeMax > 0f) ? mcInitialProperties.StartSizeMax : mcInitialProperties.StartHeightMax);
		defaultSpriteParticle.EndHeight = DPSFHelper.RandomNumberBetween((mcInitialProperties.EndSizeMin > 0f) ? mcInitialProperties.EndSizeMin : mcInitialProperties.EndHeightMin, (mcInitialProperties.EndSizeMax > 0f) ? mcInitialProperties.EndSizeMax : mcInitialProperties.EndHeightMax);
		defaultSpriteParticle.Width = defaultSpriteParticle.StartWidth;
		defaultSpriteParticle.Height = defaultSpriteParticle.StartHeight;
	}

	/// <summary>
	/// Update a Particle's Rotation according to its Rotational Velocity
	/// </summary>
	/// <param name="cParticle">The Particle to update</param>
	/// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
	protected void UpdateParticleRotationUsingRotationalVelocity(DefaultSpriteParticle cParticle, float fElapsedTimeInSeconds)
	{
		cParticle.Rotation += cParticle.RotationalVelocity * fElapsedTimeInSeconds;
	}

	/// <summary>
	/// Update a Particle's Rotational Velocity according to its Rotational Acceleration
	/// </summary>
	/// <param name="cParticle">The Particle to update</param>
	/// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
	protected void UpdateParticleRotationalVelocityUsingRotationalAcceleration(DefaultSpriteParticle cParticle, float fElapsedTimeInSeconds)
	{
		cParticle.RotationalVelocity += cParticle.RotationalAcceleration * fElapsedTimeInSeconds;
	}

	/// <summary>
	/// Update a Particle's Rotation and Rotational Velocity according to its Rotational Acceleration
	/// </summary>
	/// <param name="cParticle">The Particle to update</param>
	/// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
	protected void UpdateParticleRotationAndRotationalVelocityUsingRotationalAcceleration(DefaultSpriteParticle cParticle, float fElapsedTimeInSeconds)
	{
		cParticle.RotationalVelocity += cParticle.RotationalAcceleration * fElapsedTimeInSeconds;
		cParticle.Rotation += cParticle.RotationalVelocity * fElapsedTimeInSeconds;
	}

	/// <summary>
	/// Linearly interpolate the Particle's Width between the Start and End Width according
	/// to the Particle's Normalized Lifetime
	/// </summary>
	/// <param name="cParticle">The Particle to update</param>
	/// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
	protected void UpdateParticleWidthUsingLerp(DefaultSpriteParticle cParticle, float fElapsedTimeInSeconds)
	{
		cParticle.Width = MathHelper.Lerp(cParticle.StartWidth, cParticle.EndWidth, cParticle.NormalizedElapsedTime);
	}

	/// <summary>
	/// Linearly interpolate the Particle's Height between the Start and End Height according
	/// to the Particle's Normalized Lifetime
	/// </summary>
	/// <param name="cParticle">The Particle to update</param>
	/// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
	protected void UpdateParticleHeightUsingLerp(DefaultSpriteParticle cParticle, float fElapsedTimeInSeconds)
	{
		cParticle.Height = MathHelper.Lerp(cParticle.StartHeight, cParticle.EndHeight, cParticle.NormalizedElapsedTime);
	}

	/// <summary>
	/// Linearly interpolate the Particle's Width and Height between the Start and End values according
	/// to the Particle's Normalized Lifetime
	/// </summary>
	/// <param name="cParticle">The Particle to update</param>
	/// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
	protected void UpdateParticleWidthAndHeightUsingLerp(DefaultSpriteParticle cParticle, float fElapsedTimeInSeconds)
	{
		cParticle.Width = MathHelper.Lerp(cParticle.StartWidth, cParticle.EndWidth, cParticle.NormalizedElapsedTime);
		cParticle.Height = MathHelper.Lerp(cParticle.StartHeight, cParticle.EndHeight, cParticle.NormalizedElapsedTime);
	}

	/// <summary>
	/// Linearly interpolate the Particle's Position.Z value from 1.0 (back) to
	/// 0.0 (front) according to the Particle's Normalized Lifetime
	/// </summary>
	/// <param name="cParticle">The Particle to update</param>
	/// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
	protected void UpdateParticleDepthFromBackToFrontUsingLerp(DefaultSpriteParticle cParticle, float fElapsedTimeInSeconds)
	{
		cParticle.Position.Z = 1f - cParticle.NormalizedElapsedTime;
	}

	/// <summary>
	/// Linearly interpolate the Particle's Position.Z value from 0.0 (front) to
	/// 1.0 (back) according to the Particle's Normalized Lifetime
	/// </summary>
	/// <param name="cParticle">The Particle to update</param>
	/// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
	protected void UpdateParticleDepthFromFrontToBackUsingLerp(DefaultSpriteParticle cParticle, float fElapsedTimeInSeconds)
	{
		cParticle.Position.Z = cParticle.NormalizedElapsedTime;
	}

	/// <summary>
	/// Sorts the Particle System's Active Particles so that the Particles at the back
	/// (i.e. Position.Z = 1.0) are drawn before the Particles at the front (i.e. 
	/// Position.Z = 0.0).
	/// <para>NOTE: This operation is very expensive and should only be used when you are
	/// using a Shader (i.e. Effect and Technique).</para>
	/// <para>If you are not using a Shader and want the Particles sorted by Depth, use SpriteSortMode.BackToFront.</para>
	/// <para>Merge Sort is the sorting algorithm used, as it tends to be best for linked lists.
	/// TODO - WHILE MERGE SORT SHOULD BE USED, DUE TO TIME CONSTRAINTS A (PROBABLY) SLOWER METHOD (QUICK-SORT)
	/// IS BEING USED INSTEAD. THIS FUNCTION NEEDS TO BE UPDATED TO USE MERGE SORT STILL.
	/// THE LINKED LIST MERGE SORT ALGORITHM CAN BE FOUND AT http://www.chiark.greenend.org.uk/~sgtatham/algorithms/listsort.html</para>
	/// </summary>
	/// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
	protected void UpdateParticleSystemToSortParticlesByDepth(float fElapsedTimeInSeconds)
	{
		int count = base.ActiveParticles.Count;
		if (count > 1)
		{
			List<Particle> list = new List<Particle>(count);
			for (LinkedListNode<Particle> linkedListNode = base.ActiveParticles.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				list.Add(linkedListNode.Value);
			}
			list.Sort(delegate(Particle Particle1, Particle Particle2)
			{
				DPSFDefaultBaseParticle dPSFDefaultBaseParticle = (DPSFDefaultBaseParticle)(object)Particle1;
				DPSFDefaultBaseParticle dPSFDefaultBaseParticle2 = (DPSFDefaultBaseParticle)(object)Particle2;
				return dPSFDefaultBaseParticle.Position.Z.CompareTo(dPSFDefaultBaseParticle2.Position.Z);
			});
			base.ActiveParticles.Clear();
			for (int num = 0; num < count; num++)
			{
				base.ActiveParticles.AddFirst(list[num]);
			}
		}
	}
}
