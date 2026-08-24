using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AircraftRC;

public class Skybox
{
	private Effect efectc;

	private Model Face;

	private Model Droite;

	private Model Arriere;

	private Model Gauche;

	private Model H;

	private Texture2D face;

	private Texture2D droite;

	private Texture2D arriere;

	private Texture2D gauche;

	private Texture2D h;

	public void Load(CustomPhysicsGame game)
	{
		efectc = game.Content.Load<Effect>("Effects/Ciel");
		Face = game.Content.Load<Model>("Models/ciel/Face");
		Droite = game.Content.Load<Model>("Models/ciel/Droite");
		Arriere = game.Content.Load<Model>("Models/ciel/Arriere");
		Gauche = game.Content.Load<Model>("Models/ciel/Gauche");
		H = game.Content.Load<Model>("Models/ciel/H");
		face = game.Content.Load<Texture2D>("Textures/180");
		gauche = game.Content.Load<Texture2D>("Textures/90");
		arriere = game.Content.Load<Texture2D>("Textures/0");
		droite = game.Content.Load<Texture2D>("Textures/270");
		h = game.Content.Load<Texture2D>("Textures/Ht");
	}

	public void Draw(Matrix world, CustomPhysicsGame game, TerrainP terrain)
	{
		foreach (ModelMesh mesh in Face.Meshes)
		{
			foreach (ModelMeshPart meshPart in mesh.MeshParts)
			{
				meshPart.Effect = efectc;
				efectc.Parameters["World"].SetValue(world);
				efectc.Parameters["View"].SetValue(terrain.sceneStateScene.View);
				efectc.Parameters["Projection"].SetValue(terrain.sceneStateScene.Projection);
				efectc.Parameters["ModelTexture"].SetValue(face);
			}
			mesh.Draw();
		}
		foreach (ModelMesh mesh2 in Droite.Meshes)
		{
			foreach (ModelMeshPart meshPart2 in mesh2.MeshParts)
			{
				meshPart2.Effect = efectc;
				efectc.Parameters["World"].SetValue(world);
				efectc.Parameters["View"].SetValue(terrain.sceneStateScene.View);
				efectc.Parameters["Projection"].SetValue(terrain.sceneStateScene.Projection);
				efectc.Parameters["ModelTexture"].SetValue(droite);
			}
			mesh2.Draw();
		}
		foreach (ModelMesh mesh3 in Arriere.Meshes)
		{
			foreach (ModelMeshPart meshPart3 in mesh3.MeshParts)
			{
				meshPart3.Effect = efectc;
				efectc.Parameters["World"].SetValue(world);
				efectc.Parameters["View"].SetValue(terrain.sceneStateScene.View);
				efectc.Parameters["Projection"].SetValue(terrain.sceneStateScene.Projection);
				efectc.Parameters["ModelTexture"].SetValue(arriere);
			}
			mesh3.Draw();
		}
		foreach (ModelMesh mesh4 in Gauche.Meshes)
		{
			foreach (ModelMeshPart meshPart4 in mesh4.MeshParts)
			{
				meshPart4.Effect = efectc;
				efectc.Parameters["World"].SetValue(world);
				efectc.Parameters["View"].SetValue(terrain.sceneStateScene.View);
				efectc.Parameters["Projection"].SetValue(terrain.sceneStateScene.Projection);
				efectc.Parameters["ModelTexture"].SetValue(gauche);
			}
			mesh4.Draw();
		}
		foreach (ModelMesh mesh5 in H.Meshes)
		{
			foreach (ModelMeshPart meshPart5 in mesh5.MeshParts)
			{
				meshPart5.Effect = efectc;
				efectc.Parameters["World"].SetValue(world);
				efectc.Parameters["View"].SetValue(terrain.sceneStateScene.View);
				efectc.Parameters["Projection"].SetValue(terrain.sceneStateScene.Projection);
				efectc.Parameters["ModelTexture"].SetValue(h);
			}
			mesh5.Draw();
		}
		game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
	}
}
