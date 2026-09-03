using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OluXNA;

internal class BackgroundElement : IDrawable
{
	public PathList pathList;

	public Vector3 pos;

	public TransformSet transforms;

	private int _drawMode;

	private OluModel model;

	private ModelWrapper mBE;

	private bool normalDraw;

	private bool wireDraw;

	private bool glow;

	private bool endTransform;

	private EffectHelper elementEffect;

	private string modelName;

	private Color modelColor;

	private Vector4 texMode;

	public int drawMode => _drawMode;

	public BackgroundElement()
	{
	}

	public BackgroundElement(Dictionary<string, string> attributes, XmlNode node)
	{
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		if (attributes.ContainsKey("mode"))
		{
			_drawMode = LevelLoader.GetIntFromAtt(attributes, "mode", 0);
		}
		if (attributes.ContainsKey("glow"))
		{
			glow = LevelLoader.GetBoolFromAtt(attributes, "glow", defVal: false);
		}
		normalDraw = false;
		wireDraw = false;
		endTransform = LevelLoader.GetBoolFromAtt(attributes, "endtransform", defVal: false);
		modelName = BaseGame.contentRoot + attributes["model"];
		modelColor = LevelLoader.GetColorFromAtt(attributes, "color", Color.White);
		texMode = LevelLoader.GetVector4FromAtt(attributes, "texmode", BaseGame.T_MIX);
		LevelLoader.BuildPath(node.SelectSingleNode("paths"), out pathList, BaseGame.Get().level.activeZone);
		if (node.SelectSingleNode("transforms") != null)
		{
			LevelLoader.BuildTransform(node.SelectSingleNode("transforms"), out transforms, BaseGame.Get().level.activeZone);
		}
		if (node.SelectSingleNode("effects") != null)
		{
			elementEffect = (EffectHelper)LevelLoader.MakeObj(node.SelectSingleNode("effects").FirstChild);
		}
		Update(BaseGame.Get().emptytime);
	}

	public BackgroundElement(string filename, Color col)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		LoadModel(filename, col);
	}

	public BackgroundElement(string filename, Color col, int mode)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		LoadModel(filename, col);
		_drawMode = mode;
	}

	public virtual void LoadGraphics()
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		if (_drawMode == 10 || _drawMode == 11)
		{
			mBE = BaseGame.Get().models.GetModel(modelName);
			BaseGame.SetAllEPCs(mBE.epc, "xEnableLighting", false);
			BaseGame.SetAllEPCs(mBE.epc, "TextureMix", texMode);
			BaseGame.SetAllEPCs(mBE.epc, "Alpha", 1f);
			normalDraw = true;
			if (_drawMode == 11)
			{
				wireDraw = true;
			}
		}
		else
		{
			LoadModel(modelName, modelColor);
		}
		if (elementEffect != null)
		{
			elementEffect.toUpdate = mBE.epc;
		}
	}

	public virtual void Start()
	{
	}

	public void LoadModel(string filename)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		LoadModel(filename, Color.White);
	}

	public void LoadModel(string filename, Color col)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		model = new OluModel(filename, col);
	}

	public virtual void Update(GameTime gametime)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		if (BaseGame.Get().movingToNextZone && !endTransform)
		{
			elementEffect = null;
		}
		if (elementEffect != null)
		{
			elementEffect.Update(gametime);
		}
		transforms.Update(gametime);
		pathList.Update(gametime);
		pos = pathList.curLocation();
	}

	public virtual void Draw(GameTime gametime)
	{
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.Get().matStack.PushMatrix();
		if (glow)
		{
			BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)2;
		}
		else
		{
			BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)3;
		}
		if (normalDraw)
		{
			BaseGame.Get().SwitchEffectTechnique("Textured");
		}
		else
		{
			BaseGame.Get().SwitchEffectTechnique("Colored");
		}
		if (elementEffect != null)
		{
			elementEffect.Draw(gametime);
		}
		if (wireDraw)
		{
			BaseGame.Get().graphics.GraphicsDevice.RenderState.FillMode = (FillMode)2;
		}
		BaseGame.Get().matStack.ApplyMatrix(transforms.GetScaleMatrix() * ((transforms.usePath || !BaseGame.Get().movingToNextZone) ? Matrix.CreateTranslation(pos) : Matrix.Identity) * transforms.GetMatrix());
		if (normalDraw)
		{
			BaseGame.Get().DrawModel(ref mBE);
		}
		else
		{
			model.drawModel(drawMode);
		}
		if (wireDraw)
		{
			BaseGame.Get().graphics.GraphicsDevice.RenderState.FillMode = (FillMode)3;
		}
		BaseGame.Get().matStack.PopMatrix();
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)3;
	}
}
