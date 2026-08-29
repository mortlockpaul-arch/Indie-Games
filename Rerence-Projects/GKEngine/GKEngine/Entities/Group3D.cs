using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GKEngine.Entities;

public class Group3D(GameEngine oGameEngine) : Entity3D
{
	public Dictionary<string, Entity3D> Objects = new Dictionary<string, Entity3D>();

	public override void Load()
	{
		base.Load();
		foreach (KeyValuePair<string, Entity3D> @object in Objects)
		{
			@object.Value.Load();
		}
	}

	public override void Render(GameTime oGameTime)
	{
		base.Render(oGameTime);
		Entity3D entity3D = new Entity3D();
		foreach (KeyValuePair<string, Entity3D> @object in Objects)
		{
			entity3D.position = @object.Value.position;
			entity3D.rotation = @object.Value.rotation;
			@object.Value.position += position;
			@object.Value.rotation += rotation;
			@object.Value.Render(oGameTime);
			@object.Value.position = entity3D.position;
			@object.Value.rotation = entity3D.rotation;
		}
		entity3D = null;
	}

	public void Add(string xString, Entity3D oEntity)
	{
		Objects.Add(xString, oEntity);
	}
}
