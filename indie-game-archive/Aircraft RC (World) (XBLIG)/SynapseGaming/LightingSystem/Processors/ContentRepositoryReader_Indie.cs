using System;
using System.IO;
using Microsoft.Xna.Framework.Content;
using SynapseGaming.LightingSystem.Core;
using V;

namespace SynapseGaming.LightingSystem.Processors;

/// <summary />
public class ContentRepositoryReader_Indie : ContentTypeReader<ContentRepository>
{
	private string H4(ContentReader P_0, string P_1)
	{
		if (string.IsNullOrEmpty(P_1))
		{
			return "";
		}
		int num = P_0.AssetName.LastIndexOfAny(new char[3]
		{
			'\\',
			'/',
			Path.DirectorySeparatorChar
		});
		string path = "";
		if (num != -1)
		{
			path = P_0.AssetName.Substring(0, num);
		}
		return Path.Combine(path, P_1);
	}

	/// <summary />
	protected override ContentRepository Read(ContentReader input, ContentRepository instance)
	{
		ContentRepository contentRepository = new ContentRepository(input.ReadString(), input.ContentManager);
		contentRepository.XnbContentManagerFileName = input.AssetName;
		contentRepository.FileName = input.ReadString();
		contentRepository.ProjectFile = input.ReadString();
		contentRepository.ProcessorRenderingType = (ProcessorRenderingType)input.ReadInt32();
		int num = 0;
		num = input.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			string text = input.ReadString();
			string text2 = H4(input, input.ReadString());
			ModelMeshNames modelMeshNames = input.ReadExternalReference<ModelMeshNames>();
			ContentRepository.ModelData modelData = new ContentRepository.ModelData(modelMeshNames.MeshNames);
			modelData.m(input);
			contentRepository.t(text, text2, modelData);
		}
		num = input.ReadInt32();
		for (int j = 0; j < num; j++)
		{
			string text3 = input.ReadString();
			string text4 = H4(input, input.ReadString());
			ContentRepository.SoundEffectData soundEffectData = new ContentRepository.SoundEffectData();
			soundEffectData.m(input);
			contentRepository.Q(text3, text4, soundEffectData);
		}
		num = input.ReadInt32();
		for (int k = 0; k < num; k++)
		{
			contentRepository.v(input.ReadString(), H4(input, input.ReadString()));
		}
		num = input.ReadInt32();
		for (int l = 0; l < num; l++)
		{
			contentRepository._2(input.ReadString(), H4(input, input.ReadString()));
		}
		num = input.ReadInt32();
		for (int m = 0; m < num; m++)
		{
			string text5 = input.ReadString();
			PrefabObjectCategory prefabObjectCategory = (PrefabObjectCategory)input.ReadInt32();
			string text6 = input.ReadString();
			contentRepository._0005(text5, new PrefabObjectGenerator(text6, prefabObjectCategory));
		}
		V.B.H_0005(input);
		if (input.ReadInt32() != 1234)
		{
			throw new Exception("Error loading asset.");
		}
		return contentRepository;
	}
}
