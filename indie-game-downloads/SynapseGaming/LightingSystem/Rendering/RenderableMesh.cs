using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Effects;

namespace SynapseGaming.LightingSystem.Rendering;

/// <summary>
/// Mesh class used by the built-in renderers that provides
/// properties common to all rendering in XNA / DirectX.
/// </summary>
public class RenderableMesh
{
	/// <summary />
	public class ComparisonIndex
	{
		/// <summary />
		public class BufferComparisonIndex
		{
			internal IndexBuffer _3A_0018;

			internal VertexBuffer _3AL;

			/// <summary />
			public BufferComparisonIndex(IndexBuffer indexbuffer, VertexBuffer vertexbuffer)
			{
				_3A_0018 = indexbuffer;
				_3AL = vertexbuffer;
			}

			/// <summary />
			public override int GetHashCode()
			{
				if (_3A_0018 == null)
				{
					return _3AL.GetHashCode() ^ 1;
				}
				return (_3A_0018.GetHashCode() ^ 1) + (_3AL.GetHashCode() ^ 2);
			}

			/// <summary />
			public override bool Equals(object obj)
			{
				if (!(obj is BufferComparisonIndex bufferComparisonIndex))
				{
					return false;
				}
				if (_3A_0018 == bufferComparisonIndex._3A_0018)
				{
					return _3AL == bufferComparisonIndex._3AL;
				}
				return false;
			}
		}

		internal int _3A_0018;

		internal int _3AL;

		internal int _3A_0019;

		internal int _3A3;

		/// <summary />
		public BufferComparisonIndex BufferIndex = new BufferComparisonIndex(null, null);

		/// <summary />
		public override int GetHashCode()
		{
			if (BufferIndex._3A_0018 == null)
			{
				return (_3A_0018.GetHashCode() ^ 1) + (_3AL.GetHashCode() ^ 2) + (_3A_0019.GetHashCode() ^ 3) + (_3A3.GetHashCode() ^ 4) + (BufferIndex._3AL.GetHashCode() ^ 5);
			}
			return (_3A_0018.GetHashCode() ^ 1) + (_3AL.GetHashCode() ^ 2) + (_3A_0019.GetHashCode() ^ 3) + (_3A3.GetHashCode() ^ 4) + (BufferIndex._3A_0018.GetHashCode() ^ 5) + (BufferIndex._3AL.GetHashCode() ^ 6);
		}

		/// <summary />
		public override bool Equals(object obj)
		{
			if (!(obj is ComparisonIndex comparisonIndex))
			{
				return false;
			}
			if (_3A_0018 == comparisonIndex._3A_0018 && _3AL == comparisonIndex._3AL && _3A_0019 == comparisonIndex._3A_0019 && _3A3 == comparisonIndex._3A3 && BufferIndex._3A_0018 == comparisonIndex.BufferIndex._3A_0018)
			{
				return BufferIndex._3AL == comparisonIndex.BufferIndex._3AL;
			}
			return false;
		}
	}

	private string _3A_0018 = string.Empty;

	internal ISceneObject _3AL;

	internal int _3A_0019;

	internal bool _3A3;

	internal Matrix _3A6;

	internal Matrix _3AD;

	internal BoundingSphere _3A_0017;

	internal BoundingBox _3A_0003;

	internal Matrix _3Al;

	internal Matrix _3At;

	internal Effect _3AF;

	internal Matrix _3Ac;

	internal Matrix _3Ag;

	internal int _3AI;

	internal PrimitiveType _3A8;

	internal CullMode _3AZ = CullMode.CullCounterClockwiseFace;

	/// <summary />
	public ComparisonIndex Index = new ComparisonIndex();

	internal bool _3Ax = true;

	internal int _3Aq;

	internal int _3Ab;

	internal int _3AT;

	internal bool _3Ay;

	internal bool _3A_0015;

	internal bool _3A_0001;

	internal bool _3A7;

	internal bool _3AX;

	internal bool _3A_0010;

	internal TransparencyMode _3A_0016;

	/// <summary>
	/// The mesh's current name.
	/// </summary>
	public string Name
	{
		get
		{
			return _3A_0018;
		}
		set
		{
			_3A_0018 = value;
		}
	}

	/// <summary>
	/// Parent scene object this mesh is contained in.
	/// </summary>
	public ISceneObject SceneObject
	{
		get
		{
			return _3AL;
		}
		set
		{
			_3AL = value;
		}
	}

	/// <summary>
	/// Unique id used to identify the mesh across multiple scene loads / reloads.
	/// </summary>
	public int UniqueId
	{
		get
		{
			return _3A_0019;
		}
		set
		{
			_3A_0019 = value;
		}
	}

	/// <summary>
	/// Indicates the mesh is capable of using light maps.
	/// </summary>
	public bool CanLightMap => _3A3;

	/// <summary>
	/// Effect applied to the mesh during rendering.
	/// </summary>
	public Effect Effect
	{
		get
		{
			return _3AF;
		}
		set
		{
			_3AF = value;
			RemapEffect();
			CalculateMaterialInfo();
		}
	}

	/// <summary>
	/// Complete world space transform of the mesh (from mesh-space to
	/// world-space, ie: includes the mesh's object-space transform).
	/// </summary>
	public Matrix World => _3Ac;

	/// <summary>
	/// Inverse complete world space transform of the mesh (from world-space
	/// to mesh-space, ie: includes the mesh's object-space transform).
	/// </summary>
	public Matrix WorldToMesh => _3Ag;

	/// <summary>
	/// Object space transform of the mesh.
	/// </summary>
	public Matrix MeshToObject
	{
		get
		{
			return _3A6;
		}
		set
		{
			_3A6 = value;
			Matrix.Invert(ref _3A6, out _3AD);
			_66();
		}
	}

	/// <summary>
	/// IndexBuffer that contains the mesh geometry.
	/// </summary>
	public IndexBuffer IndexBuffer => Index.BufferIndex._3A_0018;

	/// <summary>
	/// VertexBuffer that contains the mesh geometry.
	/// </summary>
	public VertexBuffer VertexBuffer => Index.BufferIndex._3AL;

	/// <summary>
	/// Offset in bytes from the beginning of the vertex buffer to start reading data.
	/// </summary>
	public int VertexStreamOffset => Index._3A3;

	/// <summary>
	/// Offset added to each index in the index buffer during rendering.
	/// </summary>
	public int VertexBase
	{
		get
		{
			return Index._3A_0019;
		}
		set
		{
			Index._3A_0019 = value;
		}
	}

	/// <summary>
	/// Number of vertices in the vertex buffer range required to draw the mesh.
	/// For instance, a quad rendering vertices at indices (2, 5, 6, 9) requires
	/// a vertex buffer range of 8 vertices (vertices 2 – 9 inclusive).
	/// </summary>
	public int VertexCount
	{
		get
		{
			return _3AI;
		}
		set
		{
			_3AI = value;
		}
	}

	/// <summary>
	/// Index into the buffer that mesh geometry begins. For indexed meshes this
	/// is the first index in the index buffer. For non-indexed meshes this is
	/// the first vertex in the vertex buffer.
	/// </summary>
	public int ElementStart
	{
		get
		{
			return Index._3AL;
		}
		set
		{
			Index._3AL = value;
		}
	}

	/// <summary>
	/// Primitive format the mesh geometry is stored in.
	/// </summary>
	public PrimitiveType PrimitiveType
	{
		get
		{
			return _3A8;
		}
		set
		{
			_3A8 = value;
		}
	}

	/// <summary>
	/// Number of primitives in the mesh geometry.
	/// </summary>
	public int PrimitiveCount
	{
		get
		{
			return Index._3A_0018;
		}
		set
		{
			Index._3A_0018 = value;
		}
	}

	/// <summary>
	/// Cull mode used to ensure the mesh is rendered correctly.
	/// </summary>
	public CullMode CullMode => _3AZ;

	/// <summary>
	/// Object-space bounding area that completely contains the mesh.
	/// </summary>
	public BoundingSphere MeshBoundingSphere
	{
		get
		{
			return _3A_0017;
		}
		set
		{
			_3A_0017 = value;
		}
	}

	/// <summary>
	/// Object-space bounding area that completely contains the mesh.
	/// </summary>
	public BoundingBox MeshBoundingBox
	{
		get
		{
			return _3A_0003;
		}
		set
		{
			_3A_0003 = value;
		}
	}

	/// <summary>
	/// Creates an empty RenderableMesh instance.
	///
	/// Warning: Build must be called to finish constructing the mesh before
	/// attempting to render it.
	/// </summary>
	public RenderableMesh()
	{
	}

	/// <summary>
	/// Updates the mesh with new effect and geometry data.
	/// </summary>
	/// <param name="sceneobject">Parent scene object.</param>
	/// <param name="mesh">XNA ModelMesh to retrieve information from.</param>
	/// <param name="part">XNA ModelMeshPart to retrieve information from.</param>
	public void Build(ISceneObject sceneobject, ModelMesh mesh, ModelMeshPart part)
	{
		Build(sceneobject, mesh, part, part.Effect);
	}

	/// <summary>
	/// Updates the mesh with new effect and geometry data.
	/// </summary>
	/// <param name="sceneobject">Parent scene object.</param>
	/// <param name="mesh">XNA ModelMesh to retrieve information from.</param>
	/// <param name="part">XNA ModelMeshPart to retrieve information from.</param>
	/// <param name="overrideeffect">Effect applied to the mesh during rendering.</param>
	public void Build(ISceneObject sceneobject, ModelMesh mesh, ModelMeshPart part, Effect overrideeffect)
	{
		_3A_0018 = mesh.Name;
		Matrix identity = Matrix.Identity;
		for (ModelBone modelBone = mesh.ParentBone; modelBone != null; modelBone = modelBone.Parent)
		{
			identity *= modelBone.Transform;
		}
		BoundingSphere sphere;
		BoundingBox result;
		if (mesh.Tag is IBoundingVolume)
		{
			IBoundingVolume boundingVolume = mesh.Tag as IBoundingVolume;
			sphere = boundingVolume.BoundingSphere;
			result = boundingVolume.BoundingBox;
		}
		else
		{
			sphere = mesh.BoundingSphere;
			BoundingBox.CreateFromSphere(ref sphere, out result);
		}
		Build(sceneobject, overrideeffect, identity, sphere, result, part.IndexBuffer, part.VertexBuffer, part.StartIndex, PrimitiveType.TriangleList, part.PrimitiveCount, part.VertexOffset, part.NumVertices, 0, detectskinningandlightmapping: true);
	}

	/// <summary>
	/// Updates the mesh with new effect and geometry data.
	/// </summary>
	/// <param name="sceneobject">Parent scene object.</param>
	/// <param name="effect">Effect applied to the mesh during rendering.</param>
	/// <param name="indexbuffer">IndexBuffer that contains the mesh geometry.</param>
	/// <param name="vertexbuffer">VertexBuffer that contains the mesh geometry.</param>
	/// <param name="elementstart">Index into the buffer that mesh geometry begins. For indexed meshes this
	/// is the first index in the index buffer. For non-indexed meshes this is
	/// the first vertex in the vertex buffer.</param>
	/// <param name="primitivetype">Primitive format the mesh geometry is stored in.</param>
	/// <param name="primitivecount">Number of primitives in the mesh geometry.</param>
	/// <param name="vertexbase">Offset added to each index in the index buffer during rendering.</param>
	/// <param name="vertexcount">Number of vertices in the vertex buffer range required to
	/// draw the mesh.  For instance, a quad rendering vertices at indices (2, 5, 6, 9) requires
	/// a vertex buffer range of 8 vertices (vertices 2 – 9 inclusive).</param>
	/// <param name="vertexstreamoffset">Offset in bytes from the beginning of the vertex
	/// buffer to start reading data.</param>
	/// <param name="objectspace">Mesh object-space matrix.</param>
	/// <param name="meshboundingsphere">Smallest mesh space bounding sphere that
	/// completely encloses the object.</param>
	/// <param name="meshboundingbox">Smallest mesh space bounding box that
	/// completely encloses the object.</param>
	/// <param name="detectskinningandlightmapping">Indicates if the mesh should test for skinning
	/// and light mapping support. Only necessary if the provided effect supports these features
	/// and the game will use them. Testing for the features allocates memory.</param>
	public void Build(ISceneObject sceneobject, Effect effect, Matrix objectspace, BoundingSphere meshboundingsphere, BoundingBox meshboundingbox, IndexBuffer indexbuffer, VertexBuffer vertexbuffer, int elementstart, PrimitiveType primitivetype, int primitivecount, int vertexbase, int vertexcount, int vertexstreamoffset, bool detectskinningandlightmapping)
	{
		bool flag = false;
		bool flag2 = false;
		_3A3 = false;
		if (detectskinningandlightmapping)
		{
			VertexElement[] vertexElements = vertexbuffer.VertexDeclaration.GetVertexElements();
			for (int i = 0; i < vertexElements.Length; i++)
			{
				VertexElement vertexElement = vertexElements[i];
				switch (vertexElement.VertexElementUsage)
				{
				case VertexElementUsage.BlendWeight:
					flag = true;
					break;
				case VertexElementUsage.BlendIndices:
					flag2 = true;
					break;
				case VertexElementUsage.TextureCoordinate:
					if (vertexElement.UsageIndex == 1)
					{
						_3A3 = true;
					}
					break;
				}
			}
		}
		_3AL = sceneobject;
		_3AF = effect;
		_3A6 = objectspace;
		Matrix.Invert(ref _3A6, out _3AD);
		_3A_0017 = meshboundingsphere;
		_3A_0003 = meshboundingbox;
		Index.BufferIndex._3A_0018 = indexbuffer;
		Index._3AL = elementstart;
		_3A8 = primitivetype;
		Index._3A_0018 = primitivecount;
		Index._3A_0019 = vertexbase;
		Index.BufferIndex._3AL = vertexbuffer;
		_3AI = vertexcount;
		Index._3A3 = vertexstreamoffset;
		if (!(_3AF is IRenderableEffect) && !(_3AF is IEffectMatrices))
		{
			throw new ArgumentException("Only effects derived from IRenderableEffect and IEffectMatrices are supported by built-in renderers.");
		}
		if (_3AF is ISkinnedEffect { Skinned: not false } && (!flag || !flag2))
		{
			throw new ArgumentException("Effects that implement skinning require object vertex buffers to supply both blending weight and indices in the vertex stream.");
		}
		if (Index.BufferIndex._3A_0018 != null)
		{
			_3Ab = CoreHelper.GetHashCode(Index.BufferIndex._3A_0018.GetHashCode(), Index.BufferIndex._3AL.GetHashCode(), Index._3A3);
		}
		else
		{
			_3Ab = CoreHelper.GetHashCode(Index.BufferIndex._3AL.GetHashCode(), Index._3A3);
		}
		Matrix world = Matrix.Identity;
		SetWorldAndWorldToObject(ref world, ref world);
		CalculateMaterialInfo();
	}

	/// <summary>
	/// Recalculates the mesh batching information. This may become necessary
	/// if the mesh effect changes from a non-transparent mode to transparent.
	/// </summary>
	public void CalculateMaterialInfo()
	{
		if (_3AF != null)
		{
			_3Aq = _3AF.GetHashCode();
		}
		else
		{
			_3Aq = 0;
		}
		EffectTypeCaster effectTypeCaster = OptimizationSystem.EffectTypeCasters.Get(_3AF);
		IRenderableEffect renderableEffect = effectTypeCaster.RenderableEffect;
		ISkinnedEffect skinnedEffect = effectTypeCaster.SkinnedEffect;
		IShadowGenerateEffect shadowGenerateEffect = effectTypeCaster.ShadowGenerateEffect;
		ITransparentEffect transparentEffect = effectTypeCaster.TransparentEffect;
		ITerrainEffect terrainEffect = effectTypeCaster.TerrainEffect;
		if (transparentEffect != null)
		{
			_3A_0016 = transparentEffect.TransparencyMode;
		}
		else
		{
			_3A_0016 = TransparencyMode.None;
		}
		_3A_0015 = skinnedEffect?.Skinned ?? false;
		_3A_0001 = _3A_0016 != TransparencyMode.None;
		_3A7 = renderableEffect?.DoubleSided ?? false;
		_3AX = shadowGenerateEffect?.SupportsShadowGeneration ?? false;
		_3A_0010 = terrainEffect != null;
		if (terrainEffect != null)
		{
			_63(terrainEffect);
		}
	}

	private void _63(ITerrainEffect P_0)
	{
		if (!(P_0 is BaseTerrainEffect baseTerrainEffect))
		{
			return;
		}
		baseTerrainEffect._0019_0006(CalculateMaterialInfo);
		baseTerrainEffect._0019d(CalculateMaterialInfo);
		float tileWidth = baseTerrainEffect.GetTileWidth();
		float num = tileWidth * 0.5f;
		float num2 = (float)Math.Ceiling((float)baseTerrainEffect.TileRepeatCount * 0.5f);
		float num3 = num2 * (0f - tileWidth) + num;
		float num4 = (float)baseTerrainEffect.TileRepeatCount * tileWidth + num3;
		float heightScale = baseTerrainEffect.HeightScale;
		_3A_0003 = new BoundingBox(new Vector3(num3, num3, 0f), new Vector3(num4, num4, heightScale));
		BoundingSphere.CreateFromBoundingBox(ref _3A_0003, out _3A_0017);
		if (_3AL is SceneObject sceneObject)
		{
			sceneObject.CalculateBounds();
			if (sceneObject.ContainingManagers.GetItem(SceneInterface.ObjectManagerType) is IObjectManager objectManager)
			{
				objectManager.Move(sceneObject);
			}
		}
	}

	/// <summary>
	/// Should be called by custom renderers when receiving a ReplaceEffect event from
	/// the editor. Replaces the current effect with an editor assigned effect.
	/// </summary>
	public void RemapEffect()
	{
	}

	/// <summary>
	/// Sets both the world and inverse world matrices.  Used to improve
	/// performance when the world matrix is set, by providing a cached
	/// or precalculated inverse matrix with the world matrix.
	///
	/// Note: the matrix should only contain the objectToWorld (not the meshToWorld)
	/// transform. The mesh specific meshToObject transform is applied using the
	/// MeshToObject property.
	/// </summary>
	/// <param name="world">World space transform of the object.</param>
	/// <param name="worldtoobject">Inverse world space transform of the object.</param>
	public void SetWorldAndWorldToObject(Matrix world, Matrix worldtoobject)
	{
		SetWorldAndWorldToObject(ref world, ref worldtoobject);
	}

	/// <summary>
	/// Sets both the world and inverse world matrices.  Used to improve
	/// performance when the world matrix is set, by providing a cached
	/// or precalculated inverse matrix with the world matrix.
	///
	/// Note: the matrix should only contain the objectToWorld (not the meshToWorld)
	/// transform. The mesh specific meshToObject transform is applied using the
	/// MeshToObject property.
	/// </summary>
	/// <param name="world">World space transform of the object.</param>
	/// <param name="worldtoobject">Inverse world space transform of the object.</param>
	public void SetWorldAndWorldToObject(ref Matrix world, ref Matrix worldtoobject)
	{
		_3Al = world;
		_3At = worldtoobject;
		_66();
	}

	private void _66()
	{
		Matrix.Multiply(ref _3A6, ref _3Al, out _3Ac);
		Matrix.Multiply(ref _3At, ref _3AD, out _3Ag);
		if ((double)_3Ac.Determinant() >= 0.0)
		{
			_3AZ = CullMode.CullCounterClockwiseFace;
		}
		else
		{
			_3AZ = CullMode.CullClockwiseFace;
		}
	}

	/// <summary>
	/// Clones the object.
	/// </summary>
	/// <returns></returns>
	public virtual RenderableMesh Clone()
	{
		RenderableMesh renderableMesh = new RenderableMesh();
		renderableMesh.Build(_3AL, _3AF, _3A6, _3A_0017, _3A_0003, Index.BufferIndex._3A_0018, Index.BufferIndex._3AL, Index._3AL, _3A8, Index._3A_0018, Index._3A_0019, _3AI, Index._3A3, detectskinningandlightmapping: true);
		return renderableMesh;
	}
}
