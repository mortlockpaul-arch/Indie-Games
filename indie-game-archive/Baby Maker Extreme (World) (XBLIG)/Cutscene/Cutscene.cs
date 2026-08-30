using System.Collections.Generic;
using System.Xml;
using ImportData;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Renderer;

namespace Cutscene;

public class Cutscene
{
	private int m_iStartTime;

	private int m_iEndTime;

	private List<CutsceneProp> m_props;

	private int m_iCurrentAnimationTime;

	private CutsceneCamera m_camera;

	private List<TimedTextBox> m_textBoxes;

	public Cutscene()
	{
		m_textBoxes = new List<TimedTextBox>();
		m_iStartTime = 0;
		m_iEndTime = 5000;
		m_iCurrentAnimationTime = 0;
		m_props = new List<CutsceneProp>();
		m_camera = new CutsceneCamera();
	}

	public void AddProp(CutsceneProp p)
	{
		m_props.Add(p);
	}

	public List<CutsceneProp> GetProps()
	{
		return m_props;
	}

	public CutsceneCamera GetCamera()
	{
		return m_camera;
	}

	public void SetTime(int i)
	{
		foreach (TimedTextBox textBox in m_textBoxes)
		{
			textBox.SetTime(i);
		}
		m_iCurrentAnimationTime = i;
		InternalUpdate(null);
	}

	public int GetTime()
	{
		return m_iCurrentAnimationTime;
	}

	public int NextCameraKeyframe()
	{
		return m_camera.GetNextTimeAction(m_iCurrentAnimationTime);
	}

	public int PrevCameraKeyframe()
	{
		return m_camera.GetPrevTimeAction(m_iCurrentAnimationTime);
	}

	public void HandleInput(TimeTracker gameTime)
	{
		foreach (TimedTextBox textBox in m_textBoxes)
		{
			if (textBox.IsShowing)
			{
				textBox.HandleInput(gameTime);
			}
		}
	}

	public bool IsWaitingInput()
	{
		foreach (TimedTextBox textBox in m_textBoxes)
		{
			if (textBox.IsShowing)
			{
				return true;
			}
		}
		return false;
	}

	public void Update(TimeTracker gameTime)
	{
		bool flag = false;
		foreach (TimedTextBox textBox in m_textBoxes)
		{
			if (!flag)
			{
				textBox.Update(gameTime);
				if (textBox.IsShowing)
				{
					flag = true;
				}
			}
		}
		if (!flag)
		{
			m_iCurrentAnimationTime += gameTime.ElapsedMilli;
			InternalUpdate(gameTime);
		}
	}

	private void InternalUpdate(TimeTracker gameTime)
	{
		foreach (CutsceneProp prop in m_props)
		{
			prop.GetStateAtTime(m_iCurrentAnimationTime);
			if (gameTime != null)
			{
				prop.Update(gameTime);
			}
		}
		m_camera.GetStateAtTime(m_iCurrentAnimationTime);
	}

	public void Draw(TimeTracker gameTime)
	{
		foreach (TimedTextBox textBox in m_textBoxes)
		{
			if (textBox.IsShowing)
			{
				textBox.Draw(gameTime);
			}
		}
		foreach (CutsceneProp prop in m_props)
		{
			prop.Draw(gameTime);
		}
	}

	public void SaveCutscene(string filename)
	{
		XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
		xmlWriterSettings.ConformanceLevel = ConformanceLevel.Fragment;
		XmlWriter xmlWriter = XmlWriter.Create(filename, xmlWriterSettings);
		xmlWriter.WriteStartElement("root");
		foreach (CutsceneProp prop in m_props)
		{
			prop.SaveProp(xmlWriter);
		}
		m_camera.SaveCamera(xmlWriter, this);
		xmlWriter.WriteEndElement();
		xmlWriter.Close();
	}

	public void LoadCutscene(string filename, ContentManager content)
	{
		CutsceneImportData sceneData;
		if (content == null)
		{
			XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
			xmlReaderSettings.ConformanceLevel = ConformanceLevel.Fragment;
			xmlReaderSettings.IgnoreComments = true;
			xmlReaderSettings.IgnoreWhitespace = true;
			XmlReader xmlReader = XmlReader.Create(filename, xmlReaderSettings);
			m_props.Clear();
			sceneData = new CutsceneImportData(xmlReader);
			xmlReader.Close();
		}
		else
		{
			sceneData = content.Load<CutsceneImportData>(filename);
		}
		Read(sceneData);
	}

	public void Read(CutsceneImportData sceneData)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		foreach (TextImportData textDatum in sceneData.GetTextData())
		{
			m_textBoxes.Add(new TimedTextBox(textDatum.ActiveTime, textDatum.AlignLeft, textDatum.AlignBottom, SpriteManager.GetSprite(textDatum.ImageFile, textDatum.Bounding, default(Vector2), 0.4f), textDatum.Text));
		}
		foreach (PropImportData prop in sceneData.GetProps())
		{
			m_props.Add(new CutsceneProp(prop));
		}
		m_camera.ReadCamera(sceneData.GetCamera(), this);
	}
}
