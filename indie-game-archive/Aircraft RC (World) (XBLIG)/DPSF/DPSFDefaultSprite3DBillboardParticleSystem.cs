using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DPSF;

/// <summary>
/// The Default Sprite 3D Billboard Particle System class.
/// This class just inherits from the Default Sprite Particle System class and overrides the DrawSprite()
/// function to draw the sprites as Billboards in 3D space.
/// </summary>
/// <typeparam name="Particle">The Particle class to use.</typeparam>
/// <typeparam name="Vertex">The Vertex Format to use.</typeparam>
public class DPSFDefaultSprite3DBillboardParticleSystem<Particle, Vertex> : DPSFDefaultSpriteParticleSystem<Particle, Vertex> where Particle : DPSFParticle, new() where Vertex : struct, IDPSFParticleVertex
{
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
	public DPSFDefaultSprite3DBillboardParticleSystem(Game cGame)
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
		DefaultSprite3DBillboardParticle defaultSprite3DBillboardParticle = (DefaultSprite3DBillboardParticle)Particle;
		Vector3 vector = Vector3.Transform(defaultSprite3DBillboardParticle.Position, base.View);
		Vector2 position = new Vector2(vector.X, vector.Y);
		Rectangle value = new Rectangle(0, 0, base.Texture.Width, base.Texture.Height);
		cSpriteBatch.Draw(scale: new Vector2(defaultSprite3DBillboardParticle.Width / (float)value.Width, (0f - defaultSprite3DBillboardParticle.Height) / (float)value.Height), origin: new Vector2(value.Width / 2, value.Height / 2), texture: base.Texture, position: position, sourceRectangle: value, color: defaultSprite3DBillboardParticle.ColorAsPremultiplied, rotation: defaultSprite3DBillboardParticle.Rotation, effects: defaultSprite3DBillboardParticle.FlipMode, layerDepth: vector.Z);
	}

	/// <summary>
	/// Function to setup the Render Properties (i.e. BlendState, DepthStencilState, RasterizerState, and SamplerState)
	/// which will be applied to the Graphics Device before drawing the Particle System's Particles.
	/// <para>This function is called when initializing the particle system.</para>
	/// </summary>
	protected override void InitializeRenderProperties()
	{
		base.Effect = new AlphaTestEffect(base.GraphicsDevice);
		base.RenderProperties.RasterizerState = DPSFHelper.CloneRasterizerState(RasterizerState.CullNone);
	}

	/// <summary>
	/// Function to set the Shader's global variables before drawing
	/// </summary>
	protected override void SetEffectParameters()
	{
		if (base.Effect is AlphaTestEffect alphaTestEffect)
		{
			alphaTestEffect.World = base.World;
			alphaTestEffect.View = Matrix.Identity;
			alphaTestEffect.Projection = base.Projection;
			alphaTestEffect.VertexColorEnabled = true;
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
	/// Updates the Particle's DistanceFromCameraSquared property to reflect how far this Particle is from the Camera.
	/// </summary>
	/// <param name="cParticle">The Particle to update.</param>
	/// <param name="fElapsedTimeInSeconds">How long it has been since the last update.</param>
	protected void UpdateParticleDistanceFromCameraSquared(DefaultSprite3DBillboardParticle cParticle, float fElapsedTimeInSeconds)
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
				DefaultSprite3DBillboardParticle defaultSprite3DBillboardParticle = (DefaultSprite3DBillboardParticle)(object)Particle1;
				DefaultSprite3DBillboardParticle defaultSprite3DBillboardParticle2 = (DefaultSprite3DBillboardParticle)(object)Particle2;
				return defaultSprite3DBillboardParticle.DistanceFromCameraSquared.CompareTo(defaultSprite3DBillboardParticle2.DistanceFromCameraSquared);
			});
			base.ActiveParticles.Clear();
			for (int num = 0; num < count; num++)
			{
				base.ActiveParticles.AddFirst(list[num]);
			}
		}
	}
}
