using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GKEngine.Entities;

public class Group2D : Entity2D
{
	public Dictionary<string, Entity2D> Objects = new Dictionary<string, Entity2D>();

	public override void Load()
	{
		base.Load();
		foreach (KeyValuePair<string, Entity2D> @object in Objects)
		{
			@object.Value.Load();
		}
	}

	public override void Render(GameTime oGameTime)
	{
		base.Render(oGameTime);
		Entity2D entity2D = new Entity2D();
		foreach (KeyValuePair<string, Entity2D> @object in Objects)
		{
			entity2D.position = @object.Value.position;
			entity2D.rotation = @object.Value.rotation;
			entity2D.size = @object.Value.size;
			@object.Value.position += position;
			@object.Value.rotation += rotation;
			@object.Value.size += size;
			@object.Value.Render(oGameTime);
			@object.Value.position = entity2D.position;
			@object.Value.rotation = entity2D.rotation;
			@object.Value.size = entity2D.size;
		}
		entity2D = null;
	}

	public void Add(string xString, Entity2D oEntity)
	{
		Objects.Add(xString, oEntity);
	}
}
