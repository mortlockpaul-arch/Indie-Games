using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Lights;
using Z;
using u;

namespace SynapseGaming.LightingSystem.Rendering;

/// <summary>
/// Specialized 2D scene object used to store and render sprites using
/// SunBurn's forward and deferred rendering systems and effects.
///
/// Create an instance using SpriteManager.CreateSpriteContainer().
/// </summary>
public class SpriteContainer : SceneObject
{
	private Vector2 HCB = Vector2.One;

	private Vector2 HC_0002 = Vector2.Zero;

	private bool HC_0012;

	private GraphicsDevice HCH;

	private Z.y<RenderableMesh> HC7;

	private Z._6<u._0011> HC_0001;

	private int HCw = -1;

	private u.K HCZ;

	private Dictionary<int, u.K> HC_000F = new Dictionary<int, u.K>(16);

	internal SpriteContainer(GraphicsDevice P_0, Z.y<RenderableMesh> P_1, Z._6<u._0011> P_2)
	{
		HCH = P_0;
		HC7 = P_1;
		HC_0001 = P_2;
		base.StaticLightingType = StaticLightingType.Composite;
	}

	/// <summary>
	/// Prepares the container for new sprites, also clears all existing sprites from the container.
	/// </summary>
	public void Begin()
	{
		if (HC_0012)
		{
			throw new Exception("Begin already called on this object, make sure all Begin calls have an accompanying End call.");
		}
		HC_0012 = true;
		HCH.Indices = null;
		foreach (KeyValuePair<int, u.K> item in HC_000F)
		{
			item.Value.G();
		}
		while (base.RenderableMeshes.Count > 0)
		{
			RenderableMesh renderableMesh = base.RenderableMeshes[0];
			HC7.Free(renderableMesh);
			Remove(renderableMesh);
		}
	}

	/// <summary>
	/// Adds a sprite to this container. Can only be used between calls to Begin() and End().
	/// </summary>
	/// <param name="effect">Effect applied to the sprite during rendering.</param>
	/// <param name="size">Size of the sprite (in world-space if the container
	/// uses an identity world transform, otherwise in object-space)</param>
	/// <param name="position">Position of the sprite (in world-space if the container
	/// uses an identity world transform, otherwise in object-space)</param>
	/// <param name="layerdepth">Controls both the z-sorting and the height between
	/// sprites, which is critical for proper shadowing. If shadows are too
	/// disconnected form the caster try reducing the depth between the shadow
	/// caster and receiver.</param>
	public void Add(Effect effect, Vector2 size, Vector2 position, float layerdepth)
	{
		Add(effect, effect.GetHashCode(), ref size, ref position, 0f, ref HC_0002, ref HCB, ref HC_0002, layerdepth);
	}

	/// <summary>
	/// Adds a sprite to this container. Can only be used between calls to Begin() and End().
	/// </summary>
	/// <param name="effect">Effect applied to the sprite during rendering.</param>
	/// <param name="size">Size of the sprite (in world-space if the container
	/// uses an identity world transform, otherwise in object-space)</param>
	/// <param name="position">Position of the sprite (in world-space if the container
	/// uses an identity world transform, otherwise in object-space)</param>
	/// <param name="rotation">Rotation of the sprite in radians.</param>
	/// <param name="layerdepth">Controls both the z-sorting and the height between
	/// sprites, which is critical for proper shadowing. If shadows are too
	/// disconnected form the caster try reducing the depth between the shadow
	/// caster and receiver.</param>
	public void Add(Effect effect, Vector2 size, Vector2 position, float rotation, float layerdepth)
	{
		Add(effect, effect.GetHashCode(), ref size, ref position, rotation, ref HC_0002, ref HCB, ref HC_0002, layerdepth);
	}

	/// <summary>
	/// Adds a sprite to this container. Can only be used between calls to Begin() and End().
	/// </summary>
	/// <param name="effect">Effect applied to the sprite during rendering.</param>
	/// <param name="size">Size of the sprite (in world-space if the container
	/// uses an identity world transform, otherwise in object-space)</param>
	/// <param name="position">Position of the sprite (in world-space if the container
	/// uses an identity world transform, otherwise in object-space)</param>
	/// <param name="rotation">Rotation of the sprite in radians.</param>
	/// <param name="origin">Indicates the sprite origin or pivot point (offset from
	/// the sprite center).</param>
	/// <param name="layerdepth">Controls both the z-sorting and the height between
	/// sprites, which is critical for proper shadowing. If shadows are too
	/// disconnected form the caster try reducing the depth between the shadow
	/// caster and receiver.</param>
	public void Add(Effect effect, Vector2 size, Vector2 position, float rotation, Vector2 origin, float layerdepth)
	{
		Add(effect, effect.GetHashCode(), ref size, ref position, rotation, ref origin, ref HCB, ref HC_0002, layerdepth);
	}

	/// <summary>
	/// Adds a sprite to this container. Can only be used between calls to Begin() and End().
	/// </summary>
	/// <param name="effect">Effect applied to the sprite during rendering.</param>
	/// <param name="size">Size of the sprite (in world-space if the container
	/// uses an identity world transform, otherwise in object-space)</param>
	/// <param name="position">Position of the sprite (in world-space if the container
	/// uses an identity world transform, otherwise in object-space)</param>
	/// <param name="uvsize">Indicates the number of times a material will tile
	/// across the sprite.</param>
	/// <param name="uvposition">Indicates the uv offset applied to a material
	/// on the sprite (in uv coordinates, where a single material tile ranges from 0.0f - 1.0f).</param>
	/// <param name="layerdepth">Controls both the z-sorting and the height between
	/// sprites, which is critical for proper shadowing. If shadows are too
	/// disconnected form the caster try reducing the depth between the shadow
	/// caster and receiver.</param>
	public void Add(Effect effect, Vector2 size, Vector2 position, Vector2 uvsize, Vector2 uvposition, float layerdepth)
	{
		Add(effect, effect.GetHashCode(), ref size, ref position, 0f, ref HC_0002, ref uvsize, ref uvposition, layerdepth);
	}

	/// <summary>
	/// Adds a sprite to this container. Can only be used between calls to Begin() and End().
	/// </summary>
	/// <param name="effect">Effect applied to the sprite during rendering.</param>
	/// <param name="size">Size of the sprite (in world-space if the container
	/// uses an identity world transform, otherwise in object-space)</param>
	/// <param name="position">Position of the sprite (in world-space if the container
	/// uses an identity world transform, otherwise in object-space)</param>
	/// <param name="rotation">Rotation of the sprite in radians.</param>
	/// <param name="uvsize">Indicates the number of times a material will tile
	/// across the sprite.</param>
	/// <param name="uvposition">Indicates the uv offset applied to a material
	/// on the sprite (in uv coordinates, where a single material tile ranges from 0.0f - 1.0f).</param>
	/// <param name="layerdepth">Controls both the z-sorting and the height between
	/// sprites, which is critical for proper shadowing. If shadows are too
	/// disconnected form the caster try reducing the depth between the shadow
	/// caster and receiver.</param>
	public void Add(Effect effect, Vector2 size, Vector2 position, float rotation, Vector2 uvsize, Vector2 uvposition, float layerdepth)
	{
		Add(effect, effect.GetHashCode(), ref size, ref position, rotation, ref HC_0002, ref uvsize, ref uvposition, layerdepth);
	}

	/// <summary>
	/// Adds a sprite to this container. Can only be used between calls to Begin() and End().
	/// </summary>
	/// <param name="effect">Effect applied to the sprite during rendering.</param>
	/// <param name="size">Size of the sprite (in world-space if the container
	/// uses an identity world transform, otherwise in object-space)</param>
	/// <param name="position">Position of the sprite (in world-space if the container
	/// uses an identity world transform, otherwise in object-space)</param>
	/// <param name="rotation">Rotation of the sprite in radians.</param>
	/// <param name="origin">Indicates the sprite origin or pivot point (offset from
	/// the sprite center).</param>
	/// <param name="uvsize">Indicates the number of times a material will tile
	/// across the sprite.</param>
	/// <param name="uvposition">Indicates the uv offset applied to a material
	/// on the sprite (in uv coordinates, where a single material tile ranges from 0.0f - 1.0f).</param>
	/// <param name="layerdepth">Controls both the z-sorting and the height between
	/// sprites, which is critical for proper shadowing. If shadows are too
	/// disconnected form the caster try reducing the depth between the shadow
	/// caster and receiver.</param>
	public void Add(Effect effect, Vector2 size, Vector2 position, float rotation, Vector2 origin, Vector2 uvsize, Vector2 uvposition, float layerdepth)
	{
		Add(effect, effect.GetHashCode(), ref size, ref position, rotation, ref origin, ref uvsize, ref uvposition, layerdepth);
	}

	/// <summary>
	/// Adds a sprite to this container. Can only be used between calls to Begin() and End().
	/// </summary>
	/// <param name="effect">Effect applied to the sprite during rendering.</param>
	/// <param name="effecthashcode">Unique hashcode of the effect.</param>
	/// <param name="size">Size of the sprite (in world-space if the container
	/// uses an identity world transform, otherwise in object-space)</param>
	/// <param name="position">Position of the sprite (in world-space if the container
	/// uses an identity world transform, otherwise in object-space)</param>
	/// <param name="rotation">Rotation of the sprite in radians.</param>
	/// <param name="origin">Indicates the sprite origin or pivot point (offset from
	/// the sprite center).</param>
	/// <param name="uvsize">Indicates the number of times a material will tile
	/// across the sprite.</param>
	/// <param name="uvposition">Indicates the uv offset applied to a material
	/// on the sprite (in uv coordinates, where a single material tile ranges from 0.0f - 1.0f).</param>
	/// <param name="layerdepth">Controls both the z-sorting and the height between
	/// sprites, which is critical for proper shadowing. If shadows are too
	/// disconnected form the caster try reducing the depth between the shadow
	/// caster and receiver.</param>
	public void Add(Effect effect, int effecthashcode, Vector2 size, Vector2 position, float rotation, Vector2 origin, Vector2 uvsize, Vector2 uvposition, float layerdepth)
	{
		Add(effect, effecthashcode, ref size, ref position, rotation, ref origin, ref uvsize, ref uvposition, layerdepth);
	}

	/// <summary>
	/// Adds a sprite to this container. Can only be used between calls to Begin() and End().
	/// </summary>
	/// <param name="effect">Effect applied to the sprite during rendering.</param>
	/// <param name="size">Size of the sprite (in world-space if the container
	/// uses an identity world transform, otherwise in object-space)</param>
	/// <param name="position">Position of the sprite (in world-space if the container
	/// uses an identity world transform, otherwise in object-space)</param>
	/// <param name="rotation">Rotation of the sprite in radians.</param>
	/// <param name="origin">Indicates the sprite origin or pivot point (offset from
	/// the sprite center).</param>
	/// <param name="uvsize">Indicates the number of times a material will tile
	/// across the sprite.</param>
	/// <param name="uvposition">Indicates the uv offset applied to a material
	/// on the sprite (in uv coordinates, where a single material tile ranges from 0.0f - 1.0f).</param>
	/// <param name="layerdepth">Controls both the z-sorting and the height between
	/// sprites, which is critical for proper shadowing. If shadows are too
	/// disconnected form the caster try reducing the depth between the shadow
	/// caster and receiver.</param>
	public void Add(Effect effect, ref Vector2 size, ref Vector2 position, float rotation, ref Vector2 origin, ref Vector2 uvsize, ref Vector2 uvposition, float layerdepth)
	{
		Add(effect, effect.GetHashCode(), ref size, ref position, rotation, ref origin, ref uvsize, ref uvposition, layerdepth);
	}

	/// <summary>
	/// Adds a sprite to this container. Can only be used between calls to Begin() and End().
	/// </summary>
	/// <param name="effect">Effect applied to the sprite during rendering.</param>
	/// <param name="effecthashcode">Unique hashcode of the effect.</param>
	/// <param name="size">Size of the sprite (in world-space if the container
	/// uses an identity world transform, otherwise in object-space)</param>
	/// <param name="position">Position of the sprite (in world-space if the container
	/// uses an identity world transform, otherwise in object-space)</param>
	/// <param name="rotation">Rotation of the sprite in radians.</param>
	/// <param name="origin">Indicates the sprite origin or pivot point (offset from
	/// the sprite center).</param>
	/// <param name="uvsize">Indicates the number of times a material will tile
	/// across the sprite.</param>
	/// <param name="uvposition">Indicates the uv offset applied to a material
	/// on the sprite (in uv coordinates, where a single material tile ranges from 0.0f - 1.0f).</param>
	/// <param name="layerdepth">Controls both the z-sorting and the height between
	/// sprites, which is critical for proper shadowing. If shadows are too
	/// disconnected form the caster try reducing the depth between the shadow
	/// caster and receiver.</param>
	public void Add(Effect effect, int effecthashcode, ref Vector2 size, ref Vector2 position, float rotation, ref Vector2 origin, ref Vector2 uvsize, ref Vector2 uvposition, float layerdepth)
	{
		if (!HC_0012)
		{
			throw new Exception("Begin must be called before adding sprites to the container.");
		}
		u.K value;
		if (HCw == effecthashcode && HCZ != null)
		{
			value = HCZ;
		}
		else
		{
			if (!HC_000F.TryGetValue(effecthashcode, out value))
			{
				value = new u.K(HCH, HC_0001, effect);
				HC_000F.Add(effecthashcode, value);
			}
			HCw = effecthashcode;
			HCZ = value;
		}
		value._5(ref size, ref position, rotation, ref origin, ref uvsize, ref uvposition, layerdepth);
	}

	/// <summary>
	/// Finishes all sprite operations until the next call to Begin().
	/// </summary>
	public void End()
	{
		if (!HC_0012)
		{
			throw new Exception("Begin must be called before calling End.");
		}
		HC_0012 = false;
		foreach (KeyValuePair<int, u.K> item in HC_000F)
		{
			u.K value = item.Value;
			Effect effect = value.Effect;
			value._7u();
			foreach (u._0011 item2 in value.Buffers)
			{
				RenderableMesh renderableMesh = HC7.New();
				renderableMesh.Build(this, effect, Matrix.Identity, BoundingSphere.CreateFromBoundingBox(item2.ObjectBoundingBox), item2.ObjectBoundingBox, item2.IndexBuffer, item2.VertexBuffer, 0, PrimitiveType.TriangleList, item2.VertexCount / 4 * 2, 0, item2.VertexCount, 0, detectskinningandlightmapping: false);
				Add(renderableMesh);
			}
		}
	}
}
