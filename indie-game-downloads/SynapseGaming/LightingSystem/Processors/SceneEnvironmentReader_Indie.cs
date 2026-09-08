using System.Text;
using _7;
using Microsoft.Xna.Framework.Content;
using SynapseGaming.LightingSystem.Core;

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
		SceneEnvironment sceneEnvironment = SceneEnvironment.Lp(text);
		sceneEnvironment.Lk(input.ReadString());
		sceneEnvironment.FileName = input.ReadString();
		sceneEnvironment.ProjectFile = input.ReadString();
		_7._0018._3_0013(input);
		return sceneEnvironment;
	}
}
