using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Scene;

public class PropPool
{
	private const int NUM_PROP = 7;

	private List<List<Prop>> m_props;

	private List<float> m_fPropMinPos;

	private List<float> m_fPropMaxPos;

	private int m_iLoadIndexI;

	private int m_iLoadIndexJ;

	private int m_iQ;

	public PropPool()
	{
		m_props = new List<List<Prop>>();
		m_fPropMinPos = new List<float>();
		m_fPropMaxPos = new List<float>();
		m_iLoadIndexI = 0;
		m_iLoadIndexJ = 0;
		m_iQ = 0;
		m_props.Add(new List<Prop>());
	}

	public void UpdateInit()
	{
		if (m_iLoadIndexJ >= 7)
		{
			m_fPropMinPos.Add(m_props.Last().Last().GetOutfit()
				.MinPos());
			m_fPropMaxPos.Add(m_props.Last().Last().GetOutfit()
				.MaxPos());
			m_iLoadIndexJ = 0;
			m_iLoadIndexI++;
			m_props.Add(new List<Prop>());
		}
		if (m_iLoadIndexI < 51)
		{
			if (m_iLoadIndexJ == 0)
			{
				m_props[m_iLoadIndexI].Add(new Prop((PropType)m_iLoadIndexI));
			}
			else
			{
				m_props[m_iLoadIndexI].Add(new Prop(m_props[m_iLoadIndexI][0]));
			}
			m_iLoadIndexJ++;
			m_iQ++;
			m_props.Last().Last().ResetToLocation(new Vector2(-2000 * m_iQ, 0f));
		}
	}

	public bool IsCompleteInit()
	{
		return m_iLoadIndexI == 51;
	}

	public float PercentLoaded()
	{
		return (float)m_iQ / 357f;
	}

	public float GetMinPos(PropType type)
	{
		return m_fPropMinPos[(int)type];
	}

	public float GetMaxPos(PropType type)
	{
		return m_fPropMaxPos[(int)type];
	}

	public void ResetProps()
	{
		int num = 0;
		for (int i = 0; i < m_props.Count; i++)
		{
			for (int j = 0; j < m_props[i].Count; j++)
			{
				num++;
				m_props[i][j].ResetToLocation(new Vector2(-2000 * num, 0f));
			}
		}
	}

	public Prop InitProp(PropType type, Vector2 location)
	{
		Prop prop;
		if (m_props[(int)type].Count == 0)
		{
			prop = new Prop(type);
		}
		else
		{
			prop = m_props[(int)type][0];
			m_props[(int)type].RemoveAt(0);
		}
		prop.ResetToLocation(location);
		return prop;
	}

	public void AddProp(Prop p)
	{
		m_props[(int)p.PropType].Add(p);
	}

	public void UpdateEnabled()
	{
		for (int i = 0; i < m_props.Count; i++)
		{
			for (int j = 0; j < m_props[i].Count; j++)
			{
				m_props[i][j].UpdateEnabled();
			}
		}
	}
}
