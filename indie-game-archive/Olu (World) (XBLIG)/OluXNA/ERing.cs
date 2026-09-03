using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OluXNA;

internal class ERing : Enemy
{
	private List<Model> shell;

	private OluModel jelly;

	private bool shellIntact;

	public ERing()
	{
		shell = new List<Model>();
	}

	public ERing(Dictionary<string, string> attributes, XmlNode node)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		this._002Ector();
		if (attributes.ContainsKey("fill") && attributes["fill"].Equals("wire"))
		{
			fillMode = (FillMode)2;
		}
		LevelLoader.BuildPath(node.SelectSingleNode("paths"), out pathList, BaseGame.Get().level.activeZone);
	}

	public override string name()
	{
		return "[ring]";
	}

	public override void draw(GameTime gametime)
	{
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.Get().fogEffect.CurrentTechnique = BaseGame.Get().fogEffect.Techniques["Textured"];
		BaseGame.Get().fogEffect.CommitChanges();
		BaseGame.Get().matStack.PushMatrix();
		BaseGame.Get().graphics.GraphicsDevice.RenderState.CullMode = (CullMode)1;
		BaseGame.Get().matStack.ApplyMatrix(Matrix.CreateTranslation(getPos()));
		BaseGame.Get().matStack.ApplyMatrix(Matrix.CreateScale(0.2f));
		BaseGame.Get().matStack.PushMatrix();
		BaseGame.Get().matStack.ApplyMatrix(Matrix.CreateScale(10f));
		jelly.drawModel(1);
		BaseGame.Get().matStack.PopMatrix();
		BaseGame.Get().graphics.GraphicsDevice.RenderState.CullMode = (CullMode)3;
		BaseGame.Get().matStack.PopMatrix();
	}

	public override Enemy attack()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		Enemy enemy = new Enemy();
		enemy = new BulletA(getPos());
		enemy.start();
		return enemy;
	}

	public override void hit(TargetEffectBase toHit)
	{
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.Get().PlayCue("kick");
		int num = -1;
		for (int i = 0; i < targets.Count; i++)
		{
			if (targets[i] == toHit.eTarget)
			{
				num = i;
				break;
			}
		}
		if (num >= 0)
		{
			targets[num].hp--;
			targets[num].selected--;
			if (targets[num].hp <= 0)
			{
				targets.RemoveAt(num);
			}
			if (shellIntact)
			{
				shell.RemoveAt(num);
			}
		}
		hitPoints--;
		if (hitPoints <= 0)
		{
			if (shellIntact)
			{
				addTarget(new Vector3(0f, 0f, 0f), 2, 20);
				hitPoints += 2;
				shellIntact = false;
			}
			else
			{
				die();
			}
		}
	}

	public override void act(GameTime gametime)
	{
		base.act(gametime);
	}

	public override void start()
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		state = 0;
		attackCooldown = 0f;
		hitPoints = 3;
		shellIntact = true;
		shell.Add(BaseGame.Get().content.Load<Model>("Content\\ovalA"));
		addTarget(new Vector3(-7f, 7f, 0f), 1, 10);
		shell.Add(BaseGame.Get().content.Load<Model>("Content\\ovalB"));
		addTarget(new Vector3(-3.6f, -8.6f, 0f), 1, 10);
		shell.Add(BaseGame.Get().content.Load<Model>("Content\\ovalC"));
		addTarget(new Vector3(8.6f, 3.6f, 0f), 1, 10);
		jelly = new OluModel("Content\\ovalD.obj");
		jelly.GenerateSimpleFaceEffects(2, 0.5f, 0.3f, 0.4f, 0.4f, Color.White);
		addCond(new TimeCondition(5.0));
	}
}
