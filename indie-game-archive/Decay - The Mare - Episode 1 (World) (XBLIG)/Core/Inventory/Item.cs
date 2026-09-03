using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SGSCore;

namespace Core.Inventory;

public class Item
{
	public SGSModel m_model;

	public Vector3 m_cam_pos = Vector3.Zero;

	public Texture2D m_icon;

	public Texture2D m_examine_image;

	public TextureAnimation m_examine_anim;

	public string m_id = "";

	public string m_name = "";

	public string m_pickup_name = "";

	public string m_desc = "";

	public List<string> m_combine_id = new List<string>();

	public List<string> m_combine_result_id = new List<string>();

	public List<string> m_bundle_ids = new List<string>();

	public string m_examine_use_text = "";

	public string m_examine_model_from_item = "";

	public bool m_use_scrolling;

	public float m_min_scroll_y;

	protected Game m_game;

	protected DepthStencilState m_depth_stencil_state;

	protected RasterizerState m_rasterizer_state;

	protected BlendState m_examine_BS;

	public Item(Game game)
	{
		m_game = game;
		m_depth_stencil_state = new DepthStencilState();
		m_depth_stencil_state.DepthBufferEnable = true;
		m_examine_BS = new BlendState();
		m_examine_BS.ColorDestinationBlend = Blend.InverseSourceAlpha;
		m_examine_BS.ColorSourceBlend = Blend.SourceAlpha;
		m_rasterizer_state = new RasterizerState();
		m_rasterizer_state.MultiSampleAntiAlias = true;
	}

	public virtual void Clear()
	{
		m_game = null;
		if (m_depth_stencil_state != null)
		{
			m_depth_stencil_state.Dispose();
			m_depth_stencil_state = null;
		}
		if (m_rasterizer_state != null)
		{
			m_rasterizer_state.Dispose();
			m_rasterizer_state = null;
		}
		if (m_examine_BS != null)
		{
			m_examine_BS.Dispose();
			m_examine_BS = null;
		}
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
		if (m_bundle_ids != null)
		{
			m_bundle_ids.Clear();
			m_bundle_ids = null;
		}
		if (m_model != null)
		{
			m_model.Clear();
			m_model = null;
		}
		if (m_icon != null)
		{
			m_icon.Dispose();
			m_icon = null;
		}
		if (m_examine_image != null)
		{
			m_examine_image.Dispose();
			m_examine_image = null;
		}
		if (m_examine_anim != null)
		{
			m_examine_anim.Clear();
			m_examine_anim = null;
		}
	}

	public virtual void Reset()
	{
		try
		{
			if (m_examine_anim != null)
			{
				m_examine_anim.SetFrame(0);
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	public virtual void LoadExamineModel(ContentManager CM, string path)
	{
		m_model = new SGSModel();
		SGSXML sGSXML = CM.Load<SGSXML>(path + "ModelData");
		m_model.Load(CM, path, sGSXML, m_game.m_shader);
		SGSXMLData data = sGSXML.GetData("Model");
		m_cam_pos = (Vector3)data.GetField(3);
		sGSXML.Clear();
		sGSXML = null;
	}

	public virtual void onLoadExamine(SGSContentLoader CL)
	{
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
		if (m_examine_anim != null)
		{
			m_examine_anim.Update(elapsed);
		}
	}

	public virtual void RenderExamineModel(GraphicsDevice device, RenderTarget2D RT, SGSCamera camera)
	{
		try
		{
			if (device != null && RT != null && camera != null && m_model != null)
			{
				device.DepthStencilState = m_depth_stencil_state;
				device.RasterizerState = m_rasterizer_state;
				if (m_game.m_GDM.GraphicsProfile == GraphicsProfile.HiDef)
				{
					device.BlendState = m_examine_BS;
				}
				device.SetRenderTarget(RT);
				device.Clear(ClearOptions.Target, new Color(0, 0, 0, 0), 0f, 0);
				m_model.Draw(device, camera);
				device.DepthStencilState = DepthStencilState.Default;
				device.RasterizerState = RasterizerState.CullCounterClockwise;
				if (m_game.m_GDM.GraphicsProfile == GraphicsProfile.HiDef)
				{
					device.BlendState = BlendState.Opaque;
				}
				device.SetRenderTarget(m_game.m_RT);
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine("Item.RenderExamineModel: " + ex.Message);
		}
	}

	public virtual void DrawExamineModel(SpriteBatch SB, RenderTarget2D RT, Color color)
	{
		if (SB != null && RT != null)
		{
			SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			SB.Draw(RT, Game.VIEW_RECT, color);
			SB.End();
		}
	}

	public virtual void DrawExamineImage(SpriteBatch SB, Color color)
	{
		if (m_examine_image != null)
		{
			SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			SB.Draw(m_examine_image, Game.VIEW_RECT, color);
			SB.End();
		}
		if (m_examine_anim != null)
		{
			m_examine_anim.Draw(SB, color);
		}
	}

	public virtual void DrawAskPickupImage(SpriteBatch SB, Color color)
	{
		try
		{
			if (m_examine_anim != null)
			{
				m_examine_anim.SetFrame(0);
				m_examine_anim.Draw(SB, color);
			}
			else if (m_icon != null)
			{
				Rectangle destinationRectangle = new Rectangle((int)Math.Round(((float)Game.VIEW_RECT.Width - (float)m_icon.Width * 1.25f) * 0.5f), (int)Math.Round(((float)Game.VIEW_RECT.Height - (float)m_icon.Height * 1.25f) * 0.5f), (int)Math.Round((float)m_icon.Width * 1.25f), (int)Math.Round((float)m_icon.Height * 1.25f));
				SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
				SB.Draw(m_icon, destinationRectangle, color);
				SB.End();
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}
}
