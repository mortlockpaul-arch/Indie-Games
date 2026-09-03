using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xclna.Xna.Animation;

public class ModelViewer : DrawableGameComponent
{
	private List<Effect> effects = new List<Effect>();

	private ModelAnimator animator;

	private Model model;

	private IModelViewerCamera cam;

	public ModelAnimator Animator => animator;

	public IModelViewerCamera Camera
	{
		get
		{
			return cam;
		}
		set
		{
			if (value == null)
			{
				throw new ArgumentNullException("Camera can not be null.");
			}
			cam = value;
		}
	}

	public ModelViewer(Game game, Model model)
		: base(game)
	{
		cam = new DefaultModelViewerCamera(game, model);
		((GameComponent)this).UpdateOrder = 3;
		Add(model);
		((Collection<IGameComponent>)(object)game.Components).Add((IGameComponent)(object)this);
	}

	private void Add(Model model)
	{
		this.model = model;
		animator = new ModelAnimator(((GameComponent)this).Game, model);
		((GameComponent)animator).Enabled = true;
		((DrawableGameComponent)animator).Visible = true;
		InitializeEffects(model);
	}

	private unsafe void InitializeEffects(Model model)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Expected O, but got Unknown
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		Enumerator enumerator = model.Meshes.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				ModelMesh current = ((Enumerator)(ref enumerator)).Current;
				Enumerator enumerator2 = current.Effects.GetEnumerator();
				try
				{
					while (((Enumerator)(ref enumerator2)).MoveNext())
					{
						Effect current2 = ((Enumerator)(ref enumerator2)).Current;
						effects.Add(current2);
						current2.Parameters["View"].SetValue(cam.View);
						EffectParameter obj = current2.Parameters["EyePosition"];
						Matrix val = Matrix.Invert(cam.View);
						obj.SetValue(((Matrix)(ref val)).Translation);
						current2.Parameters["Projection"].SetValue(cam.Projection);
						current2.Parameters["World"].SetValue(cam.ModelWorld);
						if (current2 is BasicPaletteEffect)
						{
							BasicPaletteEffect basicPaletteEffect = (BasicPaletteEffect)(object)current2;
							basicPaletteEffect.EnableDefaultLighting();
							basicPaletteEffect.DirectionalLight0.Direction = new Vector3(0f, 0f, -1f);
						}
						else if (current2 is BasicEffect)
						{
							BasicEffect val2 = (BasicEffect)current2;
							val2.EnableDefaultLighting();
							val2.DirectionalLight0.Direction = new Vector3(0f, 0f, -1f);
							val2.DirectionalLight1.Enabled = false;
							val2.DirectionalLight2.Enabled = false;
							Color black = Color.Black;
							val2.AmbientLightColor = ((Color)(ref black)).ToVector3();
							black = Color.Black;
							val2.EmissiveColor = ((Color)(ref black)).ToVector3();
						}
					}
				}
				finally
				{
					((IDisposable)(*(Enumerator*)(&enumerator2))/*cast due to constrained. prefix*/).Dispose();
				}
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
	}

	public override void Update(GameTime gameTime)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		if (cam != null)
		{
			cam.Update(gameTime);
			animator.World = cam.ModelWorld;
			foreach (Effect effect in effects)
			{
				effect.Parameters["View"].SetValue(cam.View);
				EffectParameter obj = effect.Parameters["EyePosition"];
				Matrix val = Matrix.Invert(cam.View);
				obj.SetValue(((Matrix)(ref val)).Translation);
				effect.Parameters["Projection"].SetValue(cam.Projection);
			}
		}
		((GameComponent)this).Update(gameTime);
	}
}
