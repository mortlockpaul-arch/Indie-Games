using System;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace SpaceBlast.PathFinding;

internal class Waypoint
{
	private uint m_PositionX;

	private uint m_PositionY;

	private byte m_MaxSpeed;

	private byte m_MaxSize;

	private HalfSingle m_DynamicCostMultiplier;

	private WaypointLink[] m_Links;

	public Vector2 Position => new Vector2(m_PositionX, m_PositionY);

	public int MaxSpeed => m_MaxSpeed * 10;

	public int MaxSize => m_MaxSize * 10;

	public WaypointLink[] Links => m_Links;

	public Waypoint(XmlNode node)
	{
		LoadXML(node);
	}

	private void LoadXML(XmlNode node)
	{
		string value = node.Attributes["position"].Value;
		Vector2 vector = Utils.StringToVector2(value);
		m_PositionX = (uint)(vector.X / 1f);
		m_PositionY = (uint)(vector.Y / 1f);
		m_MaxSpeed = (byte)(Convert.ToInt32(node.Attributes["maxspeed"].Value) / 10);
		m_MaxSize = (byte)(Convert.ToInt32(node.Attributes["maxsize"].Value) / 10);
		m_DynamicCostMultiplier = new HalfSingle(1f);
		HalfSingle baseCost = new HalfSingle(Convert.ToSingle(node.Attributes["costmultiplier"].Value));
		XmlNodeList xmlNodeList = node.SelectNodes("waypointlink");
		m_Links = new WaypointLink[xmlNodeList.Count];
		for (int i = 0; i < xmlNodeList.Count; i++)
		{
			XmlNode xmlNode = xmlNodeList[i];
			short destWaypoint = Convert.ToInt16(xmlNode.Attributes["destindex"].Value);
			m_Links[i] = new WaypointLink();
			m_Links[i].DestWaypoint = destWaypoint;
			m_Links[i].BaseCost = baseCost;
		}
	}

	public void CalculateBaseLinkCosts(WaypointList waypointlist)
	{
		WaypointLink[] links = m_Links;
		foreach (WaypointLink waypointLink in links)
		{
			Waypoint waypoint = waypointlist[waypointLink.DestWaypoint];
			float num = (float)(m_MaxSpeed + waypoint.m_MaxSpeed) / 2f;
			float num2 = (waypoint.Position - Position).Length();
			float num3 = num2 / num;
			waypointLink.BaseCost = new HalfSingle(num3 * waypointLink.BaseCost.ToSingle());
		}
	}
}
