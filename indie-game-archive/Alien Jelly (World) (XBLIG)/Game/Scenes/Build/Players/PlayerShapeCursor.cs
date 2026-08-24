using GKEngine;
using GKEngine.Cameras;
using GKEngine.Entities;
using Game.Atoms;
using Game.Grids;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Scenes.Build.Players;

public class PlayerShapeCursor : Entity3D
{
	public Player player;

	public AtomPainter painter;

	public new BuildScene scene;

	public bool able = true;

	public GridPoint point;

	private Effect effect;

	private EffectParameter effectParamWorldIT;

	private EffectParameter effectParamWorldVP;

	private EffectParameter effectParamWorld;

	private EffectParameter effectParamViewI;

	private EffectParameter effectParamAble;

	public PlayerShapeCursor(Player oPlayer, AtomPainter oBrowser)
	{
		player = oPlayer;
		scene = player.universe.scene;
		painter = oBrowser;
		point = new GridPoint(this);
		scene.RenderStacks_FromName(GameMain.RENDERSTACK_UI).Add(guid.value, this);
		visible = false;
	}

	public override void Load()
	{
		base.Load();
		effect = GameEngine.SceneContent.Load<Effect>("Content/Effects/AtomShape");
		effectParamWorldIT = effect.Parameters["WorldIT"];
		effectParamWorldVP = effect.Parameters["WorldVP"];
		effectParamWorld = effect.Parameters["World"];
		effectParamViewI = effect.Parameters["ViewI"];
		effectParamAble = effect.Parameters["able"];
		visible = true;
	}

	public void Update(GameTime oGameTime)
	{
		if (visible)
		{
			point.FromPosition(player.position);
		}
	}

	public override void Render(GameTime oGameTime)
	{
		if (visible)
		{
			Camera camera = scene.cameras.camera;
			effectParamWorldIT.SetValue(Matrix.Transpose(Matrix.Invert(matrix)));
			effectParamWorldVP.SetValue(Matrix.Multiply(Matrix.Multiply(matrix, camera.view), camera.projection));
			effectParamWorld.SetValue(matrix);
			effectParamViewI.SetValue(Matrix.Invert(camera.view));
			effectParamAble.SetValue(able);
			painter.cursor.RenderEffect(ref effect);
		}
	}
}
