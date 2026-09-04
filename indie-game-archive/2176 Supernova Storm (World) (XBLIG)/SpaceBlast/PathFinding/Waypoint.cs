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

	public Vector2 Position
	{
		get
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			return new Vector2((float)m_PositionX, (float)m_PositionY);
		}
	}

	public int MaxSpeed => m_MaxSpeed * 10;

	public int MaxSize => m_MaxSize * 10;

	public WaypointLink[] Links => m_Links;

	public Waypoint(XmlNode node)
	{
		LoadXML(node);
	}

	private void LoadXML(XmlNode node)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		string value = node.Attributes["position"].Value;
		Vector2 val = Utils.StringToVector2(value);
		m_PositionX = (uint)(val.X / 1f);
		m_PositionY = (uint)(val.Y / 1f);
		m_MaxSpeed = (byte)(Convert.ToInt32(node.Attributes["maxspeed"].Value) / 10);
		m_MaxSize = (byte)(Convert.ToInt32(node.Attributes["maxsize"].Value) / 10);
		m_DynamicCostMultiplier = new HalfSingle(1f);
		HalfSingle baseCost = default(HalfSingle);
		((HalfSingle)(ref baseCost))._002Ector(Convert.ToSingle(node.Attributes["costmultiplier"].Value));
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
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		WaypointLink[] links = m_Links;
		foreach (WaypointLink waypointLink in links)
		{
			Waypoint waypoint = waypointlist[waypointLink.DestWaypoint];
			float num = (float)(m_MaxSpeed + waypoint.m_MaxSpeed) / 2f;
			Vector2 val = waypoint.Position - Position;
			float num2 = ((Vector2)(ref val)).Length();
			float num3 = num2 / num;
			waypointLink.BaseCost = new HalfSingle(num3 * ((HalfSingle)(ref waypointLink.BaseCost)).ToSingle());
		}
	}
}
