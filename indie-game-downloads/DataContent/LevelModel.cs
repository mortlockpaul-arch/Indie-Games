using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace DataContent;

public class LevelModel
{
	[ContentSerializer]
	public string Name;

	[ContentSerializer]
	public object Tag;

	[ContentSerializer]
	public List<eLevelLight> Lights;

	[ContentSerializer]
	public List<eLevelEmitter> Emitters;

	[ContentSerializer]
	public List<eMesh> Meshes;

	private LevelModel()
	{
	}
}
