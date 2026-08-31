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
	private string HCB = string.Empty;

	internal ISceneObject HC_0002;

	internal int HC_0012;

	internal bool HCH;

	internal Matrix HC7;

	internal Matrix HC_0001;

	internal BoundingSphere HCw;

	internal BoundingBox HCZ;

	internal Matrix HC_000F;

	internal Matrix HCy;

	internal Effect HC6;

	internal Matrix HCD;

	internal Matrix HC_0011;

	internal IndexBuffer HCK;

	internal VertexBuffer HC_0003;

	internal int HCk;

	internal int HCs;

	internal int HC_0013;

	internal int HCX;

	internal PrimitiveType HCz;

	internal int HCA;

	internal CullMode HCc = CullMode.CullCounterClockwiseFace;

	internal bool HCY = true;

	internal int HCV;

	internal int HCu;

	internal int HCq;

	internal bool HCR;

	internal bool HCN;

	internal bool HCF;

	internal bool HCf;

	internal bool HCG;

	internal bool HC_0010;

	internal TransparencyMode HC_0014;

	/// <summary>
	/// The mesh's current name.
	/// </summary>
	public string Name
	{
		get
		{
			return HCB;
		}
		set
		{
			HCB = value;
		}
	}

	/// <summary>
	/// Parent scene object this mesh is contained in.
	/// </summary>
	public ISceneObject SceneObject
	{
		get
		{
			return HC_0002;
		}
		set
		{
			HC_0002 = value;
		}
	}

	/// <summary>
	/// Unique id used to identify the mesh across multiple scene loads / reloads.
	/// </summary>
	public int UniqueId
	{
		get
		{
			return HC_0012;
		}
		set
		{
			HC_0012 = value;
		}
	}

	/// <summary>
	/// Indicates the mesh is capable of using light maps.
	/// </summary>
	public bool CanLightMap => HCH;

	/// <summary>
	/// Effect applied to the mesh during rendering.
	/// </summary>
	public Effect Effect
	{
		get
		{
			return HC6;
		}
		set
		{
			HC6 = value;
			RemapEffect();
			CalculateMaterialInfo();
		}
	}

	/// <summary>
	/// Complete world space transform of the mesh (from mesh-space to
	/// world-space, ie: includes the mesh's object-space transform).
	/// </summary>
	public Matrix World => HCD;

	/// <summary>
	/// Inverse complete world space transform of the mesh (from world-space
	/// to mesh-space, ie: includes the mesh's object-space transform).
	/// </summary>
	public Matrix WorldToMesh => HC_0011;

	/// <summary>
	/// Object space transform of the mesh.
	/// </summary>
	public Matrix MeshToObject
	{
		get
		{
			return HC7;
		}
		set
		{
			HC7 = value;
			Matrix.Invert(ref HC7, out HC_0001);
			_77();
		}
	}

	/// <summary>
	/// IndexBuffer that contains the mesh geometry.
	/// </summary>
	public IndexBuffer IndexBuffer => HCK;

	/// <summary>
	/// VertexBuffer that contains the mesh geometry.
	/// </summary>
	public VertexBuffer VertexBuffer => HC_0003;

	/// <summary>
	/// Offset in bytes from the beginning of the vertex buffer to start reading data.
	/// </summary>
	public int VertexStreamOffset => HCk;

	/// <summary>
	/// Offset added to each index in the index buffer during rendering.
	/// </summary>
	public int VertexBase
	{
		get
		{
			return HCs;
		}
		set
		{
			HCs = value;
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
			return HC_0013;
		}
		set
		{
			HC_0013 = value;
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
			return HCX;
		}
		set
		{
			HCX = value;
		}
	}

	/// <summary>
	/// Primitive format the mesh geometry is stored in.
	/// </summary>
	public PrimitiveType PrimitiveType
	{
		get
		{
			return HCz;
		}
		set
		{
			HCz = value;
		}
	}

	/// <summary>
	/// Number of primitives in the mesh geometry.
	/// </summary>
	public int PrimitiveCount
	{
		get
		{
			return HCA;
		}
		set
		{
			HCA = value;
		}
	}

	/// <summary>
	/// Cull mode used to ensure the mesh is rendered correctly.
	/// </summary>
	public CullMode CullMode => HCc;

	/// <summary>
	/// Object-space bounding area that completely contains the mesh.
	/// </summary>
	public BoundingSphere MeshBoundingSphere
	{
		get
		{
			return HCw;
		}
		set
		{
			HCw = value;
		}
	}

	/// <summary>
	/// Object-space bounding area that completely contains the mesh.
	/// </summary>
	public BoundingBox MeshBoundingBox
	{
		get
		{
			return HCZ;
		}
		set
		{
			HCZ = value;
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
		HCB = mesh.Name;
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
		HCH = false;
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
						HCH = true;
					}
					break;
				}
			}
		}
		HC_0002 = sceneobject;
		HC6 = effect;
		HC7 = objectspace;
		Matrix.Invert(ref HC7, out HC_0001);
		HCw = meshboundingsphere;
		HCZ = meshboundingbox;
		HCK = indexbuffer;
		HCX = elementstart;
		HCz = primitivetype;
		HCA = primitivecount;
		HCs = vertexbase;
		HC_0003 = vertexbuffer;
		HC_0013 = vertexcount;
		HCk = vertexstreamoffset;
		if (!(HC6 is IRenderableEffect) && !(HC6 is IEffectMatrices))
		{
			throw new ArgumentException("Only effects derived from IRenderableEffect and IEffectMatrices are supported by built-in renderers.");
		}
		if (HC6 is ISkinnedEffect { Skinned: not false } && (!flag || !flag2))
		{
			throw new ArgumentException("Effects that implement skinning require object vertex buffers to supply both blending weight and indices in the vertex stream.");
		}
		if (HCK != null)
		{
			HCu = CoreHelper.GetHashCode(HCK.GetHashCode(), HC_0003.GetHashCode(), HCk);
		}
		else
		{
			HCu = CoreHelper.GetHashCode(HC_0003.GetHashCode(), HCk);
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
		if (HC6 != null)
		{
			HCV = HC6.GetHashCode();
		}
		else
		{
			HCV = 0;
		}
		EffectTypeCaster effectTypeCaster = OptimizationSystem.EffectTypeCasters.Get(HC6);
		IRenderableEffect renderableEffect = effectTypeCaster.RenderableEffect;
		ISkinnedEffect skinnedEffect = effectTypeCaster.SkinnedEffect;
		IShadowGenerateEffect shadowGenerateEffect = effectTypeCaster.ShadowGenerateEffect;
		ITransparentEffect transparentEffect = effectTypeCaster.TransparentEffect;
		ITerrainEffect terrainEffect = effectTypeCaster.TerrainEffect;
		if (transparentEffect != null)
		{
			HC_0014 = transparentEffect.TransparencyMode;
		}
		else
		{
			HC_0014 = TransparencyMode.None;
		}
		HCN = skinnedEffect?.Skinned ?? false;
		HCF = HC_0014 != TransparencyMode.None;
		HCf = renderableEffect?.DoubleSided ?? false;
		HCG = shadowGenerateEffect?.SupportsShadowGeneration ?? false;
		HC_0010 = terrainEffect != null;
		if (terrainEffect != null)
		{
			_7H(terrainEffect);
		}
	}

	private void _7H(ITerrainEffect P_0)
	{
		if (!(P_0 is BaseTerrainEffect baseTerrainEffect))
		{
			return;
		}
		baseTerrainEffect._0012_0004(CalculateMaterialInfo);
		baseTerrainEffect._00129(CalculateMaterialInfo);
		float tileWidth = baseTerrainEffect.GetTileWidth();
		float num = tileWidth * 0.5f;
		float num2 = (float)Math.Ceiling((float)baseTerrainEffect.TileRepeatCount * 0.5f);
		float num3 = num2 * (0f - tileWidth) + num;
		float x = (float)baseTerrainEffect.TileRepeatCount * tileWidth + num3;
		float heightScale = baseTerrainEffect.HeightScale;
		HCZ = new BoundingBox(new Vector3(num3, num3, 0f), new Vector3(x, x, heightScale));
		BoundingSphere.CreateFromBoundingBox(ref HCZ, out HCw);
		if (HC_0002 is SceneObject sceneObject)
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
		HC_000F = world;
		HCy = worldtoobject;
		_77();
	}

	private void _77()
	{
		Matrix.Multiply(ref HC7, ref HC_000F, out HCD);
		Matrix.Multiply(ref HCy, ref HC_0001, out HC_0011);
		if ((double)HCD.Determinant() >= 0.0)
		{
			HCc = CullMode.CullCounterClockwiseFace;
		}
		else
		{
			HCc = CullMode.CullClockwiseFace;
		}
	}

	/// <summary>
	/// Clones the object.
	/// </summary>
	/// <returns></returns>
	public virtual RenderableMesh Clone()
	{
		RenderableMesh renderableMesh = new RenderableMesh();
		renderableMesh.Build(HC_0002, HC6, HC7, HCw, HCZ, HCK, HC_0003, HCX, HCz, HCA, HCs, HC_0013, HCk, detectskinningandlightmapping: true);
		return renderableMesh;
	}
}
