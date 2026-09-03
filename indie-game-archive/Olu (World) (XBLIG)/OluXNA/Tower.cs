using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Xclna.Xna.Animation;

namespace OluXNA;

internal class Tower : Enemy
{
	public static Dictionary<int, WaitCond> wCond;

	public static Dictionary<ModelBone, int> tBones;

	public static ModelWrapper s_model;

	public ModelWrapper model;

	public List<int>[] wireModel;

	public Vector3 vel;

	public Vector3 up;

	public float maxSpeed;

	public float accel;

	public ModelOluAnimator anim;

	public AnimationController walkAnim1;

	public AnimationController walkAnim2;

	public AnimationController fallAnim;

	public PlaneDetachColl pdColl;

	public static PlaneEffect pE;

	public List<RippleEffect> rE;

	public int curMesh;

	public int curIndex;

	public float pCooldown;

	public float pMax;

	public float size;

	public bool looking;

	public bool dying;

	public float blendAmount;

	public float floor;

	public Tower()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		pMax = 0.05f;
		base._002Ector();
		state = 0;
		attackCooldown = 5f;
		hitPoints = 330;
		vel = Vector3.Forward;
		up = Vector3.Up;
		maxSpeed = 40f;
		accel = 2f;
		if (wCond == null)
		{
			wCond = new Dictionary<int, WaitCond>();
			wCond.Add(0, new WaitCond("woodblock", Beats.Eighth));
			wCond.Add(1, new WaitCond("woodblock", Beats.Eighth));
			wCond.Add(2, new WaitCond("woodblock", Beats.Eighth));
			wCond.Add(3, new WaitCond("woodblock", Beats.Eighth));
			wCond.Add(4, new WaitCond("woodblock", Beats.Eighth));
			wCond.Add(5, new WaitCond("woodblock", Beats.Eighth));
			wCond.Add(6, new WaitCond("woodblock", Beats.Eighth));
			wCond.Add(7, new WaitCond("woodblock", Beats.Eighth));
			wCond.Add(8, new WaitCond("woodblock", Beats.Eighth));
		}
		_eCond = wCond;
		rE = new List<RippleEffect>();
		looking = (dying = false);
		blendAmount = 0f;
	}

	public static void LoadModel()
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		s_model = BaseGame.Get().models.GetModel("Content\\Tower\\Tower", copyData: true, copyEPC: false);
		BaseGame.SetAllEPCs(s_model.epc, "xEnableLighting", true);
		BaseGame.SetAllEPCs(s_model.epc, "DirLight0Direction", (object)new Vector3(-0.5f, -0.5f, -1f));
		BaseGame.SetAllEPCs(s_model.epc, "TextureMix", BaseGame.T_MUL);
		tBones = new Dictionary<ModelBone, int>();
		for (int i = 0; i < ((ReadOnlyCollection<ModelBone>)(object)s_model.model.Bones).Count; i++)
		{
			if (!tBones.ContainsKey(((ReadOnlyCollection<ModelBone>)(object)s_model.model.Bones)[i]))
			{
				tBones.Add(((ReadOnlyCollection<ModelBone>)(object)s_model.model.Bones)[i], i);
			}
		}
		Random random = new Random();
		pE = new PlaneEffect();
		for (int j = 0; j < 4; j++)
		{
			TreeNode treeNode = new TreeNode((float)random.NextDouble(), 0f, 0f, 1, 0.12f, 0.04f, 0.12f, 0.04f);
			treeNode.branchTree = false;
			treeNode.setColor(Color.Red);
			pE.addNode(treeNode);
		}
		ref Vector3 reference = ref pE.cornerNodes[0];
		reference = new Vector3(-0.5f, 0f, 0.5f);
		ref Vector3 reference2 = ref pE.cornerNodes[1];
		reference2 = new Vector3(0.5f, 0f, 0.5f);
		ref Vector3 reference3 = ref pE.cornerNodes[2];
		reference3 = new Vector3(-0.5f, 0f, -0.5f);
		ref Vector3 reference4 = ref pE.cornerNodes[3];
		reference4 = new Vector3(0.5f, 0f, -0.5f);
		pE.iteratePlane();
		pE.FinalizeEffect();
	}

	public Tower(Dictionary<string, string> attributes, XmlNode node)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		this._002Ector();
		if (attributes.ContainsKey("fill") && attributes["fill"].Equals("wire"))
		{
			fillMode = (FillMode)2;
		}
		LevelLoader.BuildPath(node.SelectSingleNode("paths"), out pathList, BaseGame.Get().level.activeZone);
		size = LevelLoader.GetFloatFromAtt(attributes, "size", 25f);
	}

	public override Matrix Transformation()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		return Matrix.CreateRotationY(MathHelper.ToRadians(180f)) * Matrix.CreateScale(size) * Matrix.CreateTranslation(getPos());
	}

	public override void draw(GameTime gametime)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.Get().SwitchEffectTechnique("Textured");
		BaseGame.Get().matStack.PushMatrix();
		BaseGame.Get().matStack.ApplyMatrix(Transformation());
		BaseGame.Get().graphics.GraphicsDevice.RenderState.CullMode = (CullMode)1;
		BaseGame.Get().graphics.GraphicsDevice.RenderState.FillMode = (FillMode)3;
		BaseGame.Get().DrawModel(ref model);
		BaseGame.Get().graphics.GraphicsDevice.RenderState.FillMode = (FillMode)2;
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)2;
		if (dying)
		{
			BaseGame.Get().SwitchEffectTechnique("Water");
			BaseGame.Get().fogEffect.Parameters["WaterHeight"].SetValue(floor);
		}
		BaseGame.Get().DrawModel(ref model, clearEpc: false, disableAnim: false, ref wireModel);
		BaseGame.Get().matStack.PopMatrix();
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)3;
		pdColl.draw(gametime);
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)2;
		BaseGame.Get().graphics.GraphicsDevice.VertexDeclaration = BaseGame.Get().VertDec;
		for (int num = rE.Count - 1; num >= 0; num--)
		{
			if (!rE[num].done)
			{
				BaseGame.Get().matStack.PushMatrix();
				rE[num].Draw(gametime);
				BaseGame.Get().matStack.ApplyMatrix(Matrix.CreateScale(120f) * Matrix.CreateTranslation(rE[num].pos));
				BaseGame.Get().fogEffect.Begin();
				BaseGame.Get().fogEffect.CurrentTechnique.Passes[0].Begin();
				pE.draw();
				BaseGame.Get().fogEffect.CurrentTechnique.Passes[0].End();
				BaseGame.Get().fogEffect.End();
				BaseGame.Get().matStack.PopMatrix();
			}
		}
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)3;
		BaseGame.Get().SwitchEffectTechnique("Textured");
	}

	public override void hit(TargetEffectBase toHit)
	{
		SplitList(((FaceTarget)toHit.eTarget).meshNum, ((FaceTarget)toHit.eTarget).indexNum);
		pdColl.AddPlane(ref model, ((FaceTarget)toHit.eTarget).meshNum, ((FaceTarget)toHit.eTarget).indexNum, this, ref rE);
		if (hitPoints <= 2 || dying)
		{
			hitPoints = 3;
		}
		base.hit(toHit);
	}

	public override void die()
	{
		base.die();
	}

	public override void act(GameTime gametime)
	{
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		base.act(gametime);
		if (!exists)
		{
			return;
		}
		if (!dying)
		{
			BaseGame.RunController(anim, walkAnim1, walkAnim2, blendAmount);
		}
		else
		{
			BaseGame.RunController(anim, looking ? walkAnim2 : walkAnim1, fallAnim, blendAmount);
		}
		((GameComponent)anim).Update(gametime);
		((GameComponent)walkAnim1).Update(gametime);
		((GameComponent)walkAnim2).Update(gametime);
		((GameComponent)fallAnim).Update(gametime);
		if (((!dying && looking && blendAmount > 0f) || dying) && blendAmount < 1f)
		{
			blendAmount += (float)gametime.ElapsedGameTime.TotalSeconds * (dying ? 0.2f : 1f);
		}
		else if (!dying && !looking && blendAmount > 0f)
		{
			blendAmount -= (float)gametime.ElapsedGameTime.TotalSeconds;
		}
		if (!dying)
		{
			if (!looking && (float)hitPoints < 200f && BaseGame.Get().r.NextDouble() < 0.014999999664723873)
			{
				looking = true;
			}
			else if (looking && BaseGame.Get().r.NextDouble() < 0.001500000013038516)
			{
				looking = false;
			}
			if (((looking && blendAmount >= 1f) || (!looking && blendAmount >= 0f)) && hitPoints <= 4)
			{
				blendAmount = 0f;
				dying = true;
				BaseGame.Get().actualEnem--;
			}
		}
		pdColl.act(gametime);
		for (int num = rE.Count - 1; num >= 0; num--)
		{
			rE[num].Update(gametime);
			if (rE[num].done)
			{
				rE.RemoveAt(num);
			}
		}
		if (dying)
		{
			for (int num2 = targets.Count - 1; num2 >= 0; num2--)
			{
				if (targets[num2].absolutePos().Y < floor)
				{
					SplitList(((FaceTarget)targets[num2]).meshNum, ((FaceTarget)targets[num2]).indexNum);
					pdColl.AddPlane(ref model, ((FaceTarget)targets[num2]).meshNum, ((FaceTarget)targets[num2]).indexNum, this, ref rE);
					targets.RemoveAt(num2);
				}
			}
		}
		pCooldown -= (float)gametime.ElapsedGameTime.TotalSeconds;
		if (pCooldown <= 0f)
		{
			pCooldown += pMax;
		}
		if (dying && blendAmount >= 1f && pdColl.fx.Count == 0 && rE.Count == 0)
		{
			die();
		}
	}

	public override void start()
	{
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		addCond(new NeverCondition());
		model = new ModelWrapper(s_model, copyEPC: true);
		anim = new ModelOluAnimator(BaseGame.Get().CoreGame, model, BaseGame.GetFogEffect());
		walkAnim1 = new AnimationController(BaseGame.Get().CoreGame, anim.Animations["walk1"], component: false);
		walkAnim2 = new AnimationController(BaseGame.Get().CoreGame, anim.Animations["walk2"], component: false);
		fallAnim = new AnimationController(BaseGame.Get().CoreGame, anim.Animations["fall"], component: false);
		BaseGame.RunController(anim, walkAnim1, walkAnim2, blendAmount);
		model.ResetIndicesToDraw();
		((GameComponent)anim).Update(BaseGame.Get().emptytime);
		((GameComponent)walkAnim1).Update(BaseGame.Get().emptytime);
		((GameComponent)walkAnim2).Update(BaseGame.Get().emptytime);
		((GameComponent)fallAnim).Update(BaseGame.Get().emptytime);
		((GameComponent)walkAnim1).Enabled = true;
		((GameComponent)walkAnim2).Enabled = true;
		((GameComponent)fallAnim).Enabled = true;
		wireModel = new List<int>[((ReadOnlyCollection<ModelMesh>)(object)s_model.model.Meshes).Count];
		for (int i = 0; i < ((ReadOnlyCollection<ModelMesh>)(object)s_model.model.Meshes).Count; i++)
		{
			wireModel[i] = new List<int>();
		}
		SetupTiles();
		pdColl = new PlaneDetachColl(ref model);
		floor = Vector3.Transform(Vector3.Zero, Transformation()).Y - 10f;
		base.start();
	}

	public override string name()
	{
		return "[tower]";
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

	private void SetupTiles()
	{
		bool flag = false;
		while (curIndex < model.indices[curMesh].Length - 1 && !flag)
		{
			addTarget(1, 10, ref model, curMesh, curIndex);
			curIndex += 3;
		}
		if (!flag)
		{
			curMesh = -1;
		}
	}

	private void SplitList(int _mesh, int _index)
	{
		bool flag = false;
		for (int i = 0; i < model.indicesToDraw[_mesh].Count - 1; i += 2)
		{
			if (flag)
			{
				break;
			}
			if (_index < model.indicesToDraw[_mesh][i] || _index > model.indicesToDraw[_mesh][i + 1])
			{
				continue;
			}
			flag = true;
			if (_index == model.indicesToDraw[_mesh][i])
			{
				if (_index == model.indicesToDraw[_mesh][i + 1] - 2)
				{
					model.indicesToDraw[_mesh].RemoveAt(i);
					model.indicesToDraw[_mesh].RemoveAt(i);
				}
				else
				{
					model.indicesToDraw[_mesh][i] = _index + 3;
				}
			}
			else if (_index == model.indicesToDraw[_mesh][i + 1] - 2)
			{
				model.indicesToDraw[_mesh][i + 1] = _index - 1;
			}
			else
			{
				model.indicesToDraw[_mesh].Insert(i + 1, _index + 3);
				model.indicesToDraw[_mesh].Insert(i + 1, _index - 1);
			}
		}
		flag = false;
		for (int j = 0; j < wireModel[_mesh].Count - 1; j += 2)
		{
			if (flag)
			{
				break;
			}
			if (_index > wireModel[_mesh][j + 1] + 3)
			{
				continue;
			}
			flag = true;
			if (_index + 3 == wireModel[_mesh][j])
			{
				wireModel[_mesh][j] = _index;
			}
			else if (_index < wireModel[_mesh][j])
			{
				wireModel[_mesh].Insert(j, _index + 2);
				wireModel[_mesh].Insert(j, _index);
			}
			else if (_index == wireModel[_mesh][j + 1] + 1)
			{
				if (j < wireModel[_mesh].Count - 2 && _index + 3 == wireModel[_mesh][j + 2])
				{
					wireModel[_mesh].RemoveAt(j + 1);
					wireModel[_mesh].RemoveAt(j + 1);
				}
				else
				{
					wireModel[_mesh][j + 1] = _index + 2;
				}
			}
		}
		if (!flag)
		{
			wireModel[_mesh].Add(_index);
			wireModel[_mesh].Add(_index + 2);
		}
	}
}
