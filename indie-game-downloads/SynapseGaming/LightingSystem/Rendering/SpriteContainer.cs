using System;
using System.Collections.Generic;
using _0003;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Lights;
using X;

namespace SynapseGaming.LightingSystem.Rendering;

/// <summary>
/// Specialized 2D scene object used to store and render sprites using
/// SunBurn's forward and deferred rendering systems and effects.
///
/// Create an instance using SpriteManager.CreateSpriteContainer().
/// </summary>
public class SpriteContainer : SceneObject
{
	private Vector2 _3A_0018 = Vector2.One;

	private Vector2 _3AL = Vector2.Zero;

	private bool _3A_0019;

	private GraphicsDevice _3A3;

	private _0003.t<RenderableMesh> _3A6;

	private _0003.F<X.g> _3AD;

	private int _3A_0017 = -1;

	private X.I _3A_0003;

	private Dictionary<int, X.I> _3Al = new Dictionary<int, X.I>(16);

	internal SpriteContainer(GraphicsDevice P_0, _0003.t<RenderableMesh> P_1, _0003.F<X.g> P_2)
	{
		_3A3 = P_0;
		_3A6 = P_1;
		_3AD = P_2;
		base.StaticLightingType = StaticLightingType.Composite;
	}

	/// <summary>
	/// Prepares the container for new sprites, also clears all existing sprites from the container.
	/// </summary>
	public void Begin()
	{
		if (_3A_0019)
		{
			throw new Exception("Begin already called on this object, make sure all Begin calls have an accompanying End call.");
		}
		_3A_0019 = true;
		_3A3.Indices = null;
		foreach (KeyValuePair<int, X.I> item in _3Al)
		{
			item.Value.U();
		}
		while (base.RenderableMeshes.Count > 0)
		{
			RenderableMesh renderableMesh = base.RenderableMeshes[0];
			_3A6.Free(renderableMesh);
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
		Add(effect, effect.GetHashCode(), ref size, ref position, 0f, ref _3AL, ref _3A_0018, ref _3AL, layerdepth);
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
		Add(effect, effect.GetHashCode(), ref size, ref position, rotation, ref _3AL, ref _3A_0018, ref _3AL, layerdepth);
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
		Add(effect, effect.GetHashCode(), ref size, ref position, rotation, ref origin, ref _3A_0018, ref _3AL, layerdepth);
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
		Add(effect, effect.GetHashCode(), ref size, ref position, 0f, ref _3AL, ref uvsize, ref uvposition, layerdepth);
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
		Add(effect, effect.GetHashCode(), ref size, ref position, rotation, ref _3AL, ref uvsize, ref uvposition, layerdepth);
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
		if (!_3A_0019)
		{
			throw new Exception("Begin must be called before adding sprites to the container.");
		}
		X.I value;
		if (_3A_0017 == effecthashcode && _3A_0003 != null)
		{
			value = _3A_0003;
		}
		else
		{
			if (!_3Al.TryGetValue(effecthashcode, out value))
			{
				value = new X.I(_3A3, _3AD, effect);
				_3Al.Add(effecthashcode, value);
			}
			_3A_0017 = effecthashcode;
			_3A_0003 = value;
		}
		value.V(ref size, ref position, rotation, ref origin, ref uvsize, ref uvposition, layerdepth);
	}

	/// <summary>
	/// Finishes all sprite operations until the next call to Begin().
	/// </summary>
	public void End()
	{
		if (!_3A_0019)
		{
			throw new Exception("Begin must be called before calling End.");
		}
		_3A_0019 = false;
		foreach (KeyValuePair<int, X.I> item in _3Al)
		{
			X.I value = item.Value;
			Effect effect = value.Effect;
			value._67();
			foreach (X.g item2 in value.Buffers)
			{
				RenderableMesh renderableMesh = _3A6.New();
				renderableMesh.Build(this, effect, Matrix.Identity, BoundingSphere.CreateFromBoundingBox(item2.ObjectBoundingBox), item2.ObjectBoundingBox, item2.IndexBuffer, item2.VertexBuffer, 0, PrimitiveType.TriangleList, item2.VertexCount / 4 * 2, 0, item2.VertexCount, 0, detectskinningandlightmapping: false);
				Add(renderableMesh);
			}
		}
	}
}
