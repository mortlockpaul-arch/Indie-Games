using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OluXNA;

internal class FinishBox : Enemy
{
	public static Dictionary<int, WaitCond> wCond;

	public ModelWrapper model;

	public ModelWrapper wireModel;

	public Vector3 actualPos;

	public Vector3 vel;

	public float maxSpeed;

	public float accel;

	public Vector3 colGreen;

	public Vector3 colRed;

	public FinishBox()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		state = 0;
		attackCooldown = 5f;
		hitPoints = 8;
		vel = Vector3.Zero;
		maxSpeed = 30f;
		accel = 2f;
		colGreen = new Vector3(0f, 1f, 0f);
		Color red = Color.Red;
		colRed = ((Color)(ref red)).ToVector3();
		if (wCond == null)
		{
			wCond = new Dictionary<int, WaitCond>();
			wCond.Add(0, new WaitCond("Bird01", Beats.Quarter));
			wCond.Add(1, new WaitCond("Bird01", Beats.Eighth));
			wCond.Add(2, new WaitCond("Bird02", Beats.Eighth, Beats.Quarter));
			wCond.Add(3, new WaitCond("Bird03", Beats.Eighth, Beats.Quarter));
			wCond.Add(4, new WaitCond("Bird04", Beats.Eighth, Beats.Quarter));
			wCond.Add(5, new WaitCond("Bird05", Beats.Eighth));
			wCond.Add(6, new WaitCond("Bird06", Beats.Eighth));
			wCond.Add(7, new WaitCond("Bird07", Beats.Eighth));
			wCond.Add(8, new WaitCond("Bird08", Beats.Eighth));
		}
		_eCond = wCond;
	}

	public FinishBox(Dictionary<string, string> attributes, XmlNode node)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		this._002Ector();
		if (attributes.ContainsKey("fill") && attributes["fill"].Equals("wire"))
		{
			fillMode = (FillMode)2;
		}
		LevelLoader.BuildPath(node.SelectSingleNode("paths"), out pathList, BaseGame.Get().level.activeZone);
	}

	public override void draw(GameTime gametime)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Invalid comparison between Unknown and I4
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Invalid comparison between Unknown and I4
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.Get().SwitchEffectTechnique("Textured");
		BaseGame.Get().graphics.GraphicsDevice.RenderState.FillMode = fillMode;
		if ((int)fillMode == 2)
		{
			BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)2;
		}
		BaseGame.Get().matStack.PushMatrix();
		BaseGame.Get().graphics.GraphicsDevice.RenderState.CullMode = (CullMode)1;
		BaseGame.Get().matStack.ApplyMatrix(Matrix.CreateTranslation(getPos()));
		BaseGame.Get().matStack.ApplyMatrix(Matrix.CreateScale(new Vector3(0.1f, 0.1f, -0.1f)));
		int num = -1;
		for (int i = 0; i < ((ReadOnlyCollection<ModelMesh>)(object)model.model.Meshes).Count; i++)
		{
			if (((ReadOnlyCollection<ModelMesh>)(object)model.model.Meshes)[i].Name == "Cube")
			{
				num = i;
			}
		}
		if (num > 0)
		{
			BaseGame.Get().DrawModel(model, num, new Color(((float)hitPoints * colRed + (float)(8 - hitPoints) * colGreen) / 8f));
		}
		if ((int)fillMode == 2)
		{
			BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)3;
		}
		BaseGame.Get().graphics.GraphicsDevice.RenderState.FillMode = (FillMode)3;
		BaseGame.Get().matStack.PopMatrix();
	}

	public override void hit(TargetEffectBase toHit)
	{
		base.hit(toHit);
	}

	public override void act(GameTime gametime)
	{
		base.act(gametime);
	}

	public override void start()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		addTarget(Vector3.Zero, 8, 10);
		model = BaseGame.Get().models.GetModel("Content\\FinishBox\\FinishBox", copyData: false, copyEPC: true);
		BaseGame.SetAllEPCs(model.epc, "xEnableLighting", false);
		BaseGame.SetAllEPCs(model.epc, "DiffuseColor", Vector3.Zero);
		wireModel = BaseGame.Get().models.GetModel("Content\\FinishBox\\FinishBoxWire", copyData: false, copyEPC: true);
		BaseGame.SetAllEPCs(wireModel.epc, "xEnableLighting", false);
		BaseGame.SetAllEPCs(wireModel.epc, "EmissiveColor", (object)new Vector3(1f, 1f, 1f));
		BaseGame.SetAllEPCs(wireModel.epc, "Alpha", 0.5f);
		BaseGame.SetAllEPCs(wireModel.epc, "xGlow", true);
		addCond(new NeverCondition());
		base.start();
		actualPos = pos;
	}

	public override void die()
	{
		BaseGame.Get().MoveToNextZone();
		base.die();
	}

	public override string name()
	{
		return "[PALO_ex|v.1]";
	}

	public override bool Check(int numEnem)
	{
		return wCond[numEnem].Check(BaseGame.Get().curBeat);
	}

	public override void HitSound(int lockNum, float volume)
	{
		if (lockNum <= 8)
		{
			BaseGame.Get().PlayCue(wCond[lockNum].cueName, volume);
		}
	}
}
