using System;
using System.Collections.Generic;
using _7;
using Microsoft.Xna.Framework.Content;

namespace SynapseGaming.LightingSystem.Processors;

/// <summary />
public class ModelMeshNamesReader_Indie : ContentTypeReader<ModelMeshNames>
{
	/// <summary />
	protected override ModelMeshNames Read(ContentReader input, ModelMeshNames instance)
	{
		ModelMeshNames modelMeshNames = new ModelMeshNames();
		modelMeshNames.MeshNames = input.ReadObject<List<string>>();
		_7._0018._3_0013(input);
		if (input.ReadInt32() != 1234)
		{
			throw new Exception("Error loading asset.");
		}
		return modelMeshNames;
	}
}
