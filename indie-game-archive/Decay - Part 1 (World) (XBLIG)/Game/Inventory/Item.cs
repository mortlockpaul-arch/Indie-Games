using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SGSCore;

namespace Game.Inventory;

public class Item
{
	public SGSModel m_model;

	public Vector3 m_cam_pos;

	public Texture2D m_icon_medium;

	public Texture2D m_icon_large;

	public Texture2D m_examine_image;

	public string m_id;

	public string m_name;

	public string m_desc;

	public List<string> m_combine_id;

	public List<string> m_combine_result_id;

	public string m_examine_use_text;

	public string m_examine_model_from_item;

	public bool m_use_scrolling;

	public float m_min_scroll_y;

	protected Game m_game;

	public Item(Game game)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		m_cam_pos = Vector3.Zero;
		m_id = "";
		m_name = "";
		m_desc = "";
		m_combine_id = new List<string>();
		m_combine_result_id = new List<string>();
		m_examine_use_text = "";
		m_examine_model_from_item = "";
		base._002Ector();
		m_game = game;
	}

	public virtual void Clear()
	{
		m_game = null;
		if (m_combine_id != null)
		{
			m_combine_id.Clear();
			m_combine_id = null;
		}
		if (m_combine_result_id != null)
		{
			m_combine_result_id.Clear();
			m_combine_result_id = null;
		}
		if (m_model != null)
		{
			m_model.Clear();
			m_model = null;
		}
		if (m_icon_large != null)
		{
			((GraphicsResource)m_icon_large).Dispose();
			m_icon_large = null;
		}
		if (m_icon_medium != null)
		{
			((GraphicsResource)m_icon_medium).Dispose();
			m_icon_medium = null;
		}
		if (m_examine_image != null)
		{
			((GraphicsResource)m_examine_image).Dispose();
			m_examine_image = null;
		}
	}

	public virtual void Reset()
	{
	}

	public virtual void LoadExamineModel(ContentManager CM, string path)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		m_model = new SGSModel();
		SGSXML sGSXML = CM.Load<SGSXML>(path + "ModelData");
		m_model.Load(CM, path, sGSXML, Game.INST.m_shader);
		SGSXMLData data = sGSXML.GetData("Model");
		m_cam_pos = (Vector3)data.GetField(3);
		sGSXML.Clear();
		sGSXML = null;
	}

	public virtual void onExamineUse()
	{
	}

	public virtual void onCloseExamine()
	{
	}

	public virtual void onExamineGameMenu()
	{
	}

	public virtual void onExamineGameMenuClosed()
	{
	}

	public virtual bool RemoveOnCombine(string combine_id)
	{
		return true;
	}

	public virtual void Update(TimeSpan elapsed)
	{
	}

	public virtual void RenderExamineModel(GraphicsDevice device, RenderTarget2D RT, SGSCamera camera)
	{
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		if (device != null && RT != null && camera != null && m_model != null)
		{
			device.RenderState.DepthBufferEnable = true;
			device.RenderState.AlphaBlendEnable = true;
			device.RenderState.AlphaFunction = (CompareFunction)5;
			device.RenderState.AlphaTestEnable = true;
			device.RenderState.DestinationBlend = (Blend)6;
			device.RenderState.SourceBlend = (Blend)5;
			device.RenderState.MultiSampleAntiAlias = true;
			device.SetRenderTarget(0, RT);
			device.Clear((ClearOptions)1, new Color((byte)0, (byte)0, (byte)0, (byte)0), 0f, 0);
			m_model.Draw(device, camera);
			device.RenderState.DepthBufferEnable = false;
			device.RenderState.AlphaBlendEnable = true;
			device.RenderState.AlphaFunction = (CompareFunction)5;
			device.RenderState.AlphaTestEnable = true;
			device.RenderState.DestinationBlend = (Blend)6;
			device.RenderState.SourceBlend = (Blend)5;
			device.SetRenderTarget(0, (RenderTarget2D)null);
		}
	}

	public virtual void DrawExamineModel(SpriteBatch SB, RenderTarget2D RT, Color color)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (SB != null && RT != null)
		{
			SB.Begin((SpriteBlendMode)1);
			SB.Draw(RT.GetTexture(), Game.VIEW_RECT, color);
			SB.End();
		}
	}

	public virtual void DrawExamineImage(SpriteBatch SB, Color color)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (m_examine_image != null)
		{
			SB.Begin((SpriteBlendMode)1);
			SB.Draw(m_examine_image, Game.VIEW_RECT, color);
			SB.End();
		}
	}

	public virtual void DrawAskPickupImage(SpriteBatch SB, Color color)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		DrawExamineImage(SB, color);
	}
}
