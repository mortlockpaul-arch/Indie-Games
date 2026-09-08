using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Quasar.Global;
using Quasar.Meshes.Generic;
using Quasar.Particles;
using Quasar.Shaders;

namespace Quasar.Meshes;

public class ParticleMesh : UserIndexVertexMesh<BillboardVertex>
{
	public struct Particle(Vector3 position, Vector4 color, float size, float rotation)
	{
		public Vector3 Position = position;

		public Vector3 Color = GameMath.ToVector3(color);

		public float Alpha = color.W;

		public float Size = size;

		public float Rotation = rotation;

		public float ParticleLife = 0f;

		public int Tile = 0;
	}

	public const string PARTICLE_SHADER = "Particles/BillboardParticleAdditive";

	public const string PARTICLE_ALPHA_SHADER = "Particles/BillboardParticleAlpha";

	public const int MAX_PARTICLES = 4096;

	private int particleNumber;

	private Particle[] particleData;

	private BoundingSphere sphere;

	private List<Vector3> spherePoints = new List<Vector3>();

	public int ParticleNumber
	{
		get
		{
			return particleNumber;
		}
		set
		{
			particleNumber = Math.Min(ParticleData.Length, value);
			base.PrimitiveCount = particleNumber * 2;
		}
	}

	public int MaxParticles => ParticleData.Length;

	public Particle[] ParticleData => particleData;

	public bool WorldTransform
	{
		set
		{
			base.FirstMaterial.SetIntParameter(0, value ? 1 : 0);
		}
	}

	public int TileCols
	{
		set
		{
			base.FirstMaterial.SetIntParameter(1, value);
		}
	}

	public int TileRows
	{
		set
		{
			base.FirstMaterial.SetIntParameter(2, value);
		}
	}

	public float GlobalScale
	{
		set
		{
			base.FirstMaterial.SetFloatParameter(0, value);
		}
	}

	private void UpdateSphere()
	{
		spherePoints.Capacity = particleNumber;
		spherePoints.Clear();
		for (int i = 0; i < particleNumber; i++)
		{
			spherePoints.Add(particleData[i].Position);
		}
		sphere = BoundingSphere.CreateFromPoints(spherePoints);
	}

	public ParticleMesh(Texture source, Shader shader)
		: this(source, 4096, shader)
	{
	}

	public ParticleMesh(Texture source)
		: this(source, 4096, ShaderManager.Shaders["Particles/BillboardParticleAdditive"])
	{
	}

	public ParticleMesh(Texture source, int maxParticles)
		: this(source, maxParticles, ShaderManager.Shaders["Particles/BillboardParticleAdditive"])
	{
	}

	private new void initMesh(int maxParticles)
	{
		initMesh(maxParticles * 4, maxParticles * 6, useLongIndices: false);
		for (short num = 0; num < maxParticles; num++)
		{
			verticesBuffer[num * 4].UV = new Vector2(0f, 0f);
			verticesBuffer[num * 4 + 1].UV = new Vector2(1f, 0f);
			verticesBuffer[num * 4 + 2].UV = new Vector2(0f, 1f);
			verticesBuffer[num * 4 + 3].UV = new Vector2(1f, 1f);
			indicesBuffer[num * 6] = (short)(num * 4);
			indicesBuffer[num * 6 + 1] = (short)(num * 4 + 1);
			indicesBuffer[num * 6 + 2] = (short)(num * 4 + 2);
			indicesBuffer[num * 6 + 3] = (short)(num * 4 + 2);
			indicesBuffer[num * 6 + 4] = (short)(num * 4 + 1);
			indicesBuffer[num * 6 + 5] = (short)(num * 4 + 3);
		}
	}

	public ParticleMesh(Texture source, int maxParticles, Shader shader)
	{
		DebugColor = Color.LightGray;
		particleData = new Particle[maxParticles];
		hasSimpleLayout = true;
		initMesh(maxParticles);
		base.shader = shader;
		Material material = new Material();
		material.Texture = source;
		material.AddIntParameter(0);
		material.AddIntParameter(1);
		material.AddIntParameter(1);
		material.AddFloatParameter(1f);
		material.SetForcedAlpha(alpha: true);
		material.RenderPriority = Material.Priority.Low;
		materials.Add(material);
	}

	public override void Render(Transform motion)
	{
		for (int i = 0; i < particleNumber; i++)
		{
			setParticle(ref particleData[i], ref verticesBuffer[i * 4], ref verticesBuffer[i * 4 + 1], ref verticesBuffer[i * 4 + 2], ref verticesBuffer[i * 4 + 3]);
		}
		base.Render(motion);
	}

	private void setParticle(ref Particle particle, ref BillboardVertex v1, ref BillboardVertex v2, ref BillboardVertex v3, ref BillboardVertex v4)
	{
		v1.Position = (v2.Position = (v3.Position = (v4.Position = particle.Position)));
		v1.Color = (v2.Color = (v3.Color = (v4.Color = particle.Color)));
		v1.Alpha = (v2.Alpha = (v3.Alpha = (v4.Alpha = particle.Alpha)));
		v2.ParticleData.X = (v3.ParticleData.X = (v4.ParticleData.X = (v1.ParticleData.X = particle.Size)));
		v1.ParticleData.Y = (v2.ParticleData.Y = (v3.ParticleData.Y = (v4.ParticleData.Y = particle.Rotation)));
		v1.ParticleData.Z = (v2.ParticleData.Z = (v3.ParticleData.Z = (v4.ParticleData.Z = particle.ParticleLife)));
		v1.Tile = (v2.Tile = (v3.Tile = (v4.Tile = particle.Tile)));
	}

	public override void Dispose()
	{
		particleData = null;
		base.Dispose();
	}
}
