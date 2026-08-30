using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Defenders;

public class ModelManager(Game game) : DrawableGameComponent(game)
{
	private List<BasicModel> models = new List<BasicModel>();

	protected override void LoadContent()
	{
		models.Add(new BasicModel(base.Game.Content.Load<Model>("Models\\ship")));
		base.LoadContent();
	}

	public override void Initialize()
	{
		base.Initialize();
	}

	public override void Update(GameTime gameTime)
	{
		for (int i = 0; i < models.Count; i++)
		{
			models[i].Update();
		}
		base.Update(gameTime);
	}

	public override void Draw(GameTime gameTime)
	{
	}
}
