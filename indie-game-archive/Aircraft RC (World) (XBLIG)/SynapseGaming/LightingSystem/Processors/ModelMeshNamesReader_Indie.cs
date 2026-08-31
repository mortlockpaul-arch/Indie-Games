using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using V;

namespace SynapseGaming.LightingSystem.Processors;

/// <summary />
public class ModelMeshNamesReader_Indie : ContentTypeReader<ModelMeshNames>
{
	/// <summary />
	protected override ModelMeshNames Read(ContentReader input, ModelMeshNames instance)
	{
		ModelMeshNames modelMeshNames = new ModelMeshNames();
		modelMeshNames.MeshNames = input.ReadObject<List<string>>();
		V.B.H_0005(input);
		if (input.ReadInt32() != 1234)
		{
			throw new Exception("Error loading asset.");
		}
		return modelMeshNames;
	}
}
