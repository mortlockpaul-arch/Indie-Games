using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DPSF;

/// <summary>
/// The Default Quad Particle System class
/// </summary>
/// <typeparam name="Particle">The Particle class to use</typeparam>
/// <typeparam name="Vertex">The Vertex Format to use</typeparam>
public class DPSFDefaultQuadParticleSystem<Particle, Vertex> : DPSFDefaultBaseParticleSystem<Particle, Vertex> where Particle : DPSFParticle, new() where Vertex : struct, IDPSFParticleVertex
{
	/// <summary>
	/// Particle System Properties used to initialize a Particle's Properties.
	/// <para>NOTE: These are only applied to the Particle when the InitializeParticleUsingInitialProperties()
	/// function is set as the Particle Initialization Function.</para>
	/// </summary>
	public class CInitialPropertiesForQuad : CInitialProperties
	{
		public Vector3 RotationMin = Vector3.Zero;

		public Vector3 RotationMax = Vector3.Zero;

		public Vector3 RotationalVelocityMin = Vector3.Zero;

		public Vector3 RotationalVelocityMax = Vector3.Zero;

		public Vector3 RotationalAccelerationMin = Vector3.Zero;

		public Vector3 RotationalAccelerationMax = Vector3.Zero;

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

		/// <summary>
		/// If true, the Rotation will be somewhere on the vector joining the Min Rotation to the Max Rotation.
		/// <para>If false, each of the XYZ components will be randomly calculated individually between the Min and Max Rotation XYZ values.</para>
		/// <para>Default value is false.</para>
		/// </summary>
		public bool InterpolateBetweenMinAndMaxRotation;

		/// <summary>
		/// If true, the Rotational Velocity will be somewhere on the vector joining the Min Rotational Velocity to the Max Rotational Velocity.
		/// <para>If false, each of the XYZ components will be randomly calculated individually between the Min and Max Rotational Velocity XYZ values.</para>
		/// <para>Default value is false.</para>
		/// </summary>
		public bool InterpolateBetweenMinAndMaxRotationalVelocity;

		/// <summary>
		/// If true, the Rotational Acceleration will be somewhere on the vector joining the Min Rotational Acceleration to the Max Rotational Acceleration.
		/// <para>If false, each of the XYZ components will be randomly calculated individually between the Min and Max Rotational Acceleration XYZ values.</para>
		/// <para>Default value is false.</para>
		/// </summary>
		public bool InterpolateBetweenMinAndMaxRotationalAcceleration;
	}

	private CInitialPropertiesForQuad mcInitialProperties = new CInitialPropertiesForQuad();

	/// <summary>
	/// Get the Settings used to specify the Initial Properties of a new Particle.
	/// <para>NOTE: These are only applied to the Particle when the InitializeParticleUsingInitialProperties()
	/// function is set as the Particle Initialization Function.</para>
	/// </summary>
	public new CInitialPropertiesForQuad InitialProperties => mcInitialProperties;

	/// <summary>
	/// Get / Set the Position of the Camera.
	/// <para>NOTE: This should be Set (updated) every frame if Billboarding will be used (i.e. Always have the Particles face the Camera).</para>
	/// </summary>
	public Vector3 CameraPosition { get; set; }

	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="cGame">Handle to the Game object being used. Pass in null for this 
	/// parameter if not using a Game object.</param>
	public DPSFDefaultQuadParticleSystem(Game cGame)
		: base(cGame)
	{
	}

	/// <summary>
	/// Function to update the Vertex properties according to the Particle properties
	/// </summary>
	/// <param name="sVertexBuffer">The array containing the Vertices to be drawn</param>
	/// <param name="iIndex">The Index in the array where the Particle's Vertex info should be placed</param>
	/// <param name="Particle">The Particle to copy the information from</param>
	protected virtual void UpdateVertexProperties(ref DefaultQuadParticleVertex[] sVertexBuffer, int iIndex, DPSFParticle Particle)
	{
		DefaultQuadParticle defaultQuadParticle = (DefaultQuadParticle)Particle;
		float num = defaultQuadParticle.Width / 2f;
		float num2 = defaultQuadParticle.Height / 2f;
		Vector3 value = new Vector3(0f - num, 0f - num2, 0f);
		Vector3 value2 = new Vector3(num, 0f - num2, 0f);
		Vector3 value3 = new Vector3(0f - num, num2, 0f);
		Vector3 value4 = new Vector3(num, num2, 0f);
		value = Vector3.Transform(value, defaultQuadParticle.Orientation) + defaultQuadParticle.Position;
		value2 = Vector3.Transform(value2, defaultQuadParticle.Orientation) + defaultQuadParticle.Position;
		value3 = Vector3.Transform(value3, defaultQuadParticle.Orientation) + defaultQuadParticle.Position;
		value4 = Vector3.Transform(value4, defaultQuadParticle.Orientation) + defaultQuadParticle.Position;
		Color colorAsPremultiplied = defaultQuadParticle.ColorAsPremultiplied;
		sVertexBuffer[iIndex].Position = value3;
		sVertexBuffer[iIndex].Color = colorAsPremultiplied;
		sVertexBuffer[iIndex + 1].Position = value;
		sVertexBuffer[iIndex + 1].Color = colorAsPremultiplied;
		sVertexBuffer[iIndex + 2].Position = value4;
		sVertexBuffer[iIndex + 2].Color = colorAsPremultiplied;
		sVertexBuffer[iIndex + 3].Position = value2;
		sVertexBuffer[iIndex + 3].Color = colorAsPremultiplied;
		if (base.GraphicsDevice.GraphicsProfile == GraphicsProfile.HiDef)
		{
			base.IndexBuffer[base.IndexBufferIndex++] = iIndex + 1;
			base.IndexBuffer[base.IndexBufferIndex++] = iIndex + 2;
			base.IndexBuffer[base.IndexBufferIndex++] = iIndex;
			base.IndexBuffer[base.IndexBufferIndex++] = iIndex + 1;
			base.IndexBuffer[base.IndexBufferIndex++] = iIndex + 3;
			base.IndexBuffer[base.IndexBufferIndex++] = iIndex + 2;
		}
		else
		{
			base.IndexBufferReach[base.IndexBufferIndex++] = (short)(iIndex + 1);
			base.IndexBufferReach[base.IndexBufferIndex++] = (short)(iIndex + 2);
			base.IndexBufferReach[base.IndexBufferIndex++] = (short)iIndex;
			base.IndexBufferReach[base.IndexBufferIndex++] = (short)(iIndex + 1);
			base.IndexBufferReach[base.IndexBufferIndex++] = (short)(iIndex + 3);
			base.IndexBufferReach[base.IndexBufferIndex++] = (short)(iIndex + 2);
		}
	}

	/// <summary>
	/// Virtual function to Set the Effect's Parameters before drawing the Particles
	/// </summary>
	protected override void SetEffectParameters()
	{
		if (base.Effect is BasicEffect basicEffect)
		{
			basicEffect.World = base.World;
			basicEffect.View = base.View;
			basicEffect.Projection = base.Projection;
			basicEffect.VertexColorEnabled = true;
			basicEffect.TextureEnabled = false;
			basicEffect.FogEnabled = false;
			basicEffect.LightingEnabled = false;
		}
	}

	/// <summary>
	/// Sets the camera position, so that the particles know how to make themselves face the camera if needed.
	/// </summary>
	/// <param name="cameraPosition">The camera position.</param>
	public override void SetCameraPosition(Vector3 cameraPosition)
	{
		CameraPosition = cameraPosition;
	}

	/// <summary>
	/// Function to Initialize a Default Particle with default settings
	/// </summary>
	/// <param name="Particle">The Particle to be Initialized</param>
	public override void InitializeParticleUsingInitialProperties(DPSFParticle Particle)
	{
		DefaultQuadParticle defaultQuadParticle = (DefaultQuadParticle)Particle;
		InitializeParticleUsingInitialProperties(defaultQuadParticle, mcInitialProperties);
		if (mcInitialProperties.InterpolateBetweenMinAndMaxRotation)
		{
			Vector3 vector = Vector3.Lerp(mcInitialProperties.RotationMin, mcInitialProperties.RotationMax, base.RandomNumber.NextFloat());
			defaultQuadParticle.Orientation = Quaternion.CreateFromYawPitchRoll(vector.Y, vector.X, vector.Z);
		}
		else
		{
			Vector3 vector2 = DPSFHelper.RandomVectorBetweenTwoVectors(mcInitialProperties.RotationMin, mcInitialProperties.RotationMax);
			defaultQuadParticle.Orientation = Quaternion.CreateFromYawPitchRoll(vector2.Y, vector2.X, vector2.Z);
		}
		if (mcInitialProperties.InterpolateBetweenMinAndMaxRotationalVelocity)
		{
			defaultQuadParticle.RotationalVelocity = Vector3.Lerp(mcInitialProperties.RotationalVelocityMin, mcInitialProperties.RotationalVelocityMax, base.RandomNumber.NextFloat());
		}
		else
		{
			defaultQuadParticle.RotationalVelocity = DPSFHelper.RandomVectorBetweenTwoVectors(mcInitialProperties.RotationalVelocityMin, mcInitialProperties.RotationalVelocityMax);
		}
		if (mcInitialProperties.InterpolateBetweenMinAndMaxRotationalAcceleration)
		{
			defaultQuadParticle.RotationalAcceleration = Vector3.Lerp(mcInitialProperties.RotationalAccelerationMin, mcInitialProperties.RotationalAccelerationMax, base.RandomNumber.NextFloat());
		}
		else
		{
			defaultQuadParticle.RotationalAcceleration = DPSFHelper.RandomVectorBetweenTwoVectors(mcInitialProperties.RotationalAccelerationMin, mcInitialProperties.RotationalAccelerationMax);
		}
		defaultQuadParticle.StartWidth = DPSFHelper.RandomNumberBetween((mcInitialProperties.StartSizeMin > 0f) ? mcInitialProperties.StartSizeMin : mcInitialProperties.StartWidthMin, (mcInitialProperties.StartSizeMax > 0f) ? mcInitialProperties.StartSizeMax : mcInitialProperties.StartWidthMax);
		defaultQuadParticle.EndWidth = DPSFHelper.RandomNumberBetween((mcInitialProperties.EndSizeMin > 0f) ? mcInitialProperties.EndSizeMin : mcInitialProperties.EndWidthMin, (mcInitialProperties.EndSizeMax > 0f) ? mcInitialProperties.EndSizeMax : mcInitialProperties.EndWidthMax);
		defaultQuadParticle.StartHeight = DPSFHelper.RandomNumberBetween((mcInitialProperties.StartSizeMin > 0f) ? mcInitialProperties.StartSizeMin : mcInitialProperties.StartHeightMin, (mcInitialProperties.StartSizeMax > 0f) ? mcInitialProperties.StartSizeMax : mcInitialProperties.StartHeightMax);
		defaultQuadParticle.EndHeight = DPSFHelper.RandomNumberBetween((mcInitialProperties.EndSizeMin > 0f) ? mcInitialProperties.EndSizeMin : mcInitialProperties.EndHeightMin, (mcInitialProperties.EndSizeMax > 0f) ? mcInitialProperties.EndSizeMax : mcInitialProperties.EndHeightMax);
		defaultQuadParticle.Width = defaultQuadParticle.StartWidth;
		defaultQuadParticle.Height = defaultQuadParticle.StartHeight;
	}

	/// <summary>
	/// Update a Particle's Rotation according to its Rotational Velocity
	/// </summary>
	/// <param name="cParticle">The Particle to update</param>
	/// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
	protected void UpdateParticleRotationUsingRotationalVelocity(DefaultQuadParticle cParticle, float fElapsedTimeInSeconds)
	{
		if (cParticle.RotationalVelocity != Vector3.Zero)
		{
			cParticle.Orientation.Normalize();
			Quaternion quaternion = new Quaternion(cParticle.RotationalVelocity * (fElapsedTimeInSeconds * 0.5f), 0f);
			cParticle.Orientation += cParticle.Orientation * quaternion;
		}
	}

	/// <summary>
	/// Update a Particle's Rotational Velocity according to its Rotational Acceleration
	/// </summary>
	/// <param name="cParticle">The Particle to update</param>
	/// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
	protected void UpdateParticleRotationalVelocityUsingRotationalAcceleration(DefaultQuadParticle cParticle, float fElapsedTimeInSeconds)
	{
		if (cParticle.RotationalAcceleration != Vector3.Zero)
		{
			cParticle.RotationalVelocity += cParticle.RotationalAcceleration * fElapsedTimeInSeconds;
		}
	}

	/// <summary>
	/// Update a Particle's Rotation and Rotational Velocity according to its Rotational Acceleration
	/// </summary>
	/// <param name="cParticle">The Particle to update</param>
	/// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
	protected void UpdateParticleRotationAndRotationalVelocityUsingRotationalAcceleration(DefaultQuadParticle cParticle, float fElapsedTimeInSeconds)
	{
		UpdateParticleRotationalVelocityUsingRotationalAcceleration(cParticle, fElapsedTimeInSeconds);
		UpdateParticleRotationUsingRotationalVelocity(cParticle, fElapsedTimeInSeconds);
	}

	/// <summary>
	/// Linearly interpolate the Particle's Width between the Start and End Width according
	/// to the Particle's Normalized Lifetime
	/// </summary>
	/// <param name="cParticle">The Particle to update</param>
	/// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
	protected void UpdateParticleWidthUsingLerp(DefaultQuadParticle cParticle, float fElapsedTimeInSeconds)
	{
		cParticle.Width = MathHelper.Lerp(cParticle.StartWidth, cParticle.EndWidth, cParticle.NormalizedElapsedTime);
	}

	/// <summary>
	/// Linearly interpolate the Particle's Height between the Start and End Height according
	/// to the Particle's Normalized Lifetime
	/// </summary>
	/// <param name="cParticle">The Particle to update</param>
	/// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
	protected void UpdateParticleHeightUsingLerp(DefaultQuadParticle cParticle, float fElapsedTimeInSeconds)
	{
		cParticle.Height = MathHelper.Lerp(cParticle.StartHeight, cParticle.EndHeight, cParticle.NormalizedElapsedTime);
	}

	/// <summary>
	/// Linearly interpolate the Particle's Width and Height between the Start and End values according
	/// to the Particle's Normalized Lifetime
	/// </summary>
	/// <param name="cParticle">The Particle to update</param>
	/// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
	protected void UpdateParticleWidthAndHeightUsingLerp(DefaultQuadParticle cParticle, float fElapsedTimeInSeconds)
	{
		cParticle.Width = MathHelper.Lerp(cParticle.StartWidth, cParticle.EndWidth, cParticle.NormalizedElapsedTime);
		cParticle.Height = MathHelper.Lerp(cParticle.StartHeight, cParticle.EndHeight, cParticle.NormalizedElapsedTime);
	}

	protected void UpdateParticleToFaceTheCamera(DefaultQuadParticle cParticle, float fElapsedTimeInSeconds)
	{
		cParticle.Normal = CameraPosition - cParticle.Position;
	}

	/// <summary>
	/// Orientates the Particle to face the camera, but constrains the particle to always be perpendicular to the 
	/// Y-Z plane.
	/// </summary>
	/// <param name="cParticle">The Particle to update.</param>
	/// <param name="fElapsedTimeInSeconds">How long it has been since the last update.</param>
	protected void UpdateParticleToBeConstrainedAroundXAxis(DefaultQuadParticle cParticle, float fElapsedTimeInSeconds)
	{
		Vector3 normal = CameraPosition - cParticle.Position;
		normal.X = 0f;
		cParticle.Normal = normal;
	}

	/// <summary>
	/// Orientates the Particle to face the camera, but constrains the particle to always be perpendicular to the 
	/// X-Z plane (i.e standing straight up).
	/// </summary>
	/// <param name="cParticle">The Particle to update.</param>
	/// <param name="fElapsedTimeInSeconds">How long it has been since the last update.</param>
	protected void UpdateParticleToBeConstrainedAroundYAxis(DefaultQuadParticle cParticle, float fElapsedTimeInSeconds)
	{
		Vector3 normal = CameraPosition - cParticle.Position;
		normal.Y = 0f;
		cParticle.Normal = normal;
	}

	/// <summary>
	/// Orientates the Particle to face the camera, but constrains the particle to always be perpendicular to the 
	/// X-Y plane.
	/// </summary>
	/// <param name="cParticle">The Particle to update.</param>
	/// <param name="fElapsedTimeInSeconds">How long it has been since the last update.</param>
	protected void UpdateParticleToBeConstrainedAroundZAxis(DefaultQuadParticle cParticle, float fElapsedTimeInSeconds)
	{
		Vector3 normal = CameraPosition - cParticle.Position;
		normal.Z = 0f;
		cParticle.Normal = normal;
	}

	/// <summary>
	/// Updates the Particle's DistanceFromCameraSquared property to reflect how far this Particle is from the Camera.
	/// </summary>
	/// <param name="cParticle">The Particle to update.</param>
	/// <param name="fElapsedTimeInSeconds">How long it has been since the last update.</param>
	protected void UpdateParticleDistanceFromCameraSquared(DefaultQuadParticle cParticle, float fElapsedTimeInSeconds)
	{
		cParticle.DistanceFromCameraSquared = Vector3.DistanceSquared(CameraPosition, cParticle.Position);
	}

	/// <summary>
	/// Sorts the particles to draw particles furthest from the camera first, in order to achieve proper depth perspective.
	///
	/// <para>NOTE: This operation is very expensive and should only be used when you are
	/// drawing particles with both opaque and semi-transparent portions, and not using additive blending.</para>
	/// <para>Merge Sort is the sorting algorithm used, as it tends to be best for linked lists.
	/// TODO - WHILE MERGE SORT SHOULD BE USED, DUE TO TIME CONSTRAINTS A (PROBABLY) SLOWER METHOD (QUICK-SORT)
	/// IS BEING USED INSTEAD. THIS FUNCTION NEEDS TO BE UPDATED TO USE MERGE SORT STILL.
	/// THE LINKED LIST MERGE SORT ALGORITHM CAN BE FOUND AT http://www.chiark.greenend.org.uk/~sgtatham/algorithms/listsort.html</para>
	/// </summary>
	/// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
	protected void UpdateParticleSystemToSortParticlesByDistanceFromCamera(float fElapsedTimeInSeconds)
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
				DefaultQuadParticle defaultQuadParticle = (DefaultQuadParticle)(object)Particle1;
				DefaultQuadParticle defaultQuadParticle2 = (DefaultQuadParticle)(object)Particle2;
				return defaultQuadParticle.DistanceFromCameraSquared.CompareTo(defaultQuadParticle2.DistanceFromCameraSquared);
			});
			base.ActiveParticles.Clear();
			for (int num = 0; num < count; num++)
			{
				base.ActiveParticles.AddFirst(list[num]);
			}
		}
	}
}
