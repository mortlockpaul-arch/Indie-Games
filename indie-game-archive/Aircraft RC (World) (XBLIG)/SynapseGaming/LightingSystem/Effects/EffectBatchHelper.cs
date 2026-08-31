using System.Collections.Generic;
using _0003;
using Microsoft.Xna.Framework.Graphics;

namespace SynapseGaming.LightingSystem.Effects;

/// <summary>
/// Helps maximize effect batching by collapsing identical effects in multiple models.
/// </summary>
public class EffectBatchHelper
{
	private Dictionary<string, Effect> HCB = new Dictionary<string, Effect>(16);

	/// <summary>
	/// Maximize effect batching by collapsing identical effects with previously processed effects.
	/// </summary>
	/// <param name="effect"></param>
	/// <param name="disposeunused">Determines if the effects no longer used after collapsing are disposed.
	/// While this removes unused effects from the editor and frees up memory, it also leaves disposed
	/// effects in the XNA content manager (until Unload is called). Be careful when applying this option.</param>
	/// <returns></returns>
	public Effect CollapseEffect(Effect effect, bool disposeunused)
	{
		if (effect == null)
		{
			return effect;
		}
		string text = "";
		if (effect is _0003.B)
		{
			text = (effect as _0003.B).MaterialFile;
		}
		if (string.IsNullOrEmpty(text))
		{
			return effect;
		}
		if (HCB.ContainsKey(text))
		{
			Effect effect2 = HCB[text];
			if (disposeunused && effect != effect2 && !effect.IsDisposed)
			{
				effect.Dispose();
			}
			return effect2;
		}
		HCB.Add(text, effect);
		return effect;
	}

	/// <summary>
	/// Maximize effect batching by collapsing identical effects in this and all previously processed models.
	/// </summary>
	/// <param name="model"></param>
	/// <param name="disposeunused">Determines if the effects no longer used after collapsing are disposed.
	/// While this removes unused effects from the editor and frees up memory, it also leaves disposed
	/// effects in the XNA content manager (until Unload is called). Be careful when applying this option.</param>
	public void CollapseEffects(Model model, bool disposeunused)
	{
		for (int i = 0; i < model.Meshes.Count; i++)
		{
			ModelMesh modelMesh = model.Meshes[i];
			for (int j = 0; j < modelMesh.MeshParts.Count; j++)
			{
				ModelMeshPart modelMeshPart = modelMesh.MeshParts[j];
				modelMeshPart.Effect = CollapseEffect(modelMeshPart.Effect, disposeunused);
			}
		}
	}

	/// <summary>
	/// Remove all processed effects.
	/// </summary>
	public void Clear()
	{
		HCB.Clear();
	}
}
