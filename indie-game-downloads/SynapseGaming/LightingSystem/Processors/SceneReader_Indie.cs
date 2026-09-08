using System.Text;
using _7;
using Microsoft.Xna.Framework.Content;
using SynapseGaming.LightingSystem.Rendering;

namespace SynapseGaming.LightingSystem.Processors;

/// <summary />
public class SceneReader_Indie : ContentTypeReader<Scene>
{
	/// <summary />
	protected override Scene Read(ContentReader input, Scene instance)
	{
		string text = string.Empty;
		int num = input.ReadInt32();
		if (num > 0)
		{
			byte[] array = input.ReadBytes(num);
			text = Encoding.UTF8.GetString(array, 0, array.Length);
		}
		Scene scene = Scene.Lp(text);
		scene.Lk(input.ReadString());
		scene.FileName = input.ReadString();
		scene.ProjectFile = input.ReadString();
		_7._0018._3_0013(input);
		return scene;
	}
}
