using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace SGSCore;

public class SGSContentLoader
{
	public long m_memory;

	public ContentManager m_CM;

	private List<SGSContent> m_content;

	public SGSContentLoader(IServiceProvider SP)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		base._002Ector();
		m_CM = new ContentManager(SP);
		m_CM.RootDirectory = "Content/";
		m_content = new List<SGSContent>();
	}

	public SGSContent GetContent(string path)
	{
		if (m_content == null)
		{
			return null;
		}
		for (int i = 0; i < m_content.Count; i++)
		{
			if (m_content[i] != null && m_content[i].m_path == path)
			{
				return m_content[i];
			}
		}
		return null;
	}

	public virtual Texture2D LoadTexture(string path)
	{
		if (m_CM == null)
		{
			return null;
		}
		SGSContent content = GetContent(path);
		SGSTextureContent sGSTextureContent = null;
		if (content == null)
		{
			sGSTextureContent = new SGSTextureContent(path);
			sGSTextureContent.m_texture = m_CM.Load<Texture2D>(path);
			AddContent(sGSTextureContent);
		}
		else
		{
			sGSTextureContent = (SGSTextureContent)content;
		}
		return sGSTextureContent.m_texture;
	}

	public virtual Video LoadVideo(string path)
	{
		if (m_CM == null)
		{
			return null;
		}
		SGSContent content = GetContent(path);
		SGSVideoContent sGSVideoContent = null;
		if (content == null)
		{
			sGSVideoContent = new SGSVideoContent(path);
			sGSVideoContent.m_video = m_CM.Load<Video>(path);
			AddContent(sGSVideoContent);
		}
		else
		{
			sGSVideoContent = (SGSVideoContent)content;
		}
		return sGSVideoContent.m_video;
	}

	public virtual SpriteFont LoadFont(string path)
	{
		if (m_CM == null)
		{
			return null;
		}
		SGSContent content = GetContent(path);
		SGSFontContent sGSFontContent = null;
		if (content == null)
		{
			sGSFontContent = new SGSFontContent(path);
			sGSFontContent.m_font = m_CM.Load<SpriteFont>(path);
			AddContent(sGSFontContent);
		}
		else
		{
			sGSFontContent = (SGSFontContent)content;
		}
		return sGSFontContent.m_font;
	}

	public virtual SoundEffect LoadSound(string path)
	{
		if (m_CM == null)
		{
			return null;
		}
		SGSContent content = GetContent(path);
		SGSSoundContent sGSSoundContent = null;
		if (content == null)
		{
			sGSSoundContent = new SGSSoundContent(path);
			sGSSoundContent.m_sound = m_CM.Load<SoundEffect>(path);
			AddContent(sGSSoundContent);
		}
		else
		{
			sGSSoundContent = (SGSSoundContent)content;
		}
		return sGSSoundContent.m_sound;
	}

	public virtual void AddContent(SGSContent C)
	{
		if (m_CM != null)
		{
			m_content.Add(C);
		}
	}

	public virtual void Clear()
	{
		m_memory = 0L;
		if (m_content != null)
		{
			for (int i = 0; i < m_content.Count; i++)
			{
				if (m_content[i] != null)
				{
					m_content[i].Clear();
					m_content[i] = null;
				}
			}
		}
		if (m_CM != null)
		{
			m_CM.Unload();
			m_CM.Dispose();
			m_CM = null;
		}
	}
}
