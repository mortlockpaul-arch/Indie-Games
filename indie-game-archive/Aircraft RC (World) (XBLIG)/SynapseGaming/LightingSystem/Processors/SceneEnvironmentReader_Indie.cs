using System.Text;
using Microsoft.Xna.Framework.Content;
using SynapseGaming.LightingSystem.Core;
using V;

namespace SynapseGaming.LightingSystem.Processors;

/// <summary />
public class SceneEnvironmentReader_Indie : ContentTypeReader<SceneEnvironment>
{
	/// <summary />
	protected override SceneEnvironment Read(ContentReader input, SceneEnvironment instance)
	{
		string text = string.Empty;
		int num = input.ReadInt32();
		if (num > 0)
		{
			byte[] array = input.ReadBytes(num);
			text = Encoding.UTF8.GetString(array, 0, array.Length);
		}
		SceneEnvironment sceneEnvironment = SceneEnvironment._0002F(text);
		sceneEnvironment._0002N(input.ReadString());
		sceneEnvironment.FileName = input.ReadString();
		sceneEnvironment.ProjectFile = input.ReadString();
		V.B.H_0005(input);
		return sceneEnvironment;
	}
}
