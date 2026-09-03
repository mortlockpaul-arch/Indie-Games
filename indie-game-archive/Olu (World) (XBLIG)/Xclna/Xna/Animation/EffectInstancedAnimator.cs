using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xclna.Xna.Animation;

public class EffectInstancedAnimator : ModelAnimator
{
	public EffectInstancedAnimator(Game game, Model model)
		: base(game, model)
	{
	}

	protected unsafe override IList<Effect> CreateEffectList()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		List<Effect> list = new List<Effect>();
		Enumerator enumerator = base.Model.Meshes.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				ModelMesh current = ((Enumerator)(ref enumerator)).Current;
				Enumerator enumerator2 = current.MeshParts.GetEnumerator();
				try
				{
					while (((Enumerator)(ref enumerator2)).MoveNext())
					{
						ModelMeshPart current2 = ((Enumerator)(ref enumerator2)).Current;
						list.Add(current2.Effect.Clone(current2.Effect.GraphicsDevice));
					}
				}
				finally
				{
					((IDisposable)(*(Enumerator*)(&enumerator2))/*cast due to constrained. prefix*/).Dispose();
				}
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
		return list;
	}
}
