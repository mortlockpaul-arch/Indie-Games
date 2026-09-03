using System;
using System.Xml.Linq;
using Core;
using Core.World;
using TheMare1.World.Views.DreamMainRoom;
using TheMare1.World.Views.DreamOwnRoom;
using TheMare1.World.Views.DreamRoom11;
using TheMare1.World.Views.DreamRoom2;
using TheMare1.World.Views.DreamRoom4;

namespace TheMare1.World;

public class World : Core.World.World
{
	public World(Game game, string xml_path)
		: base(game, xml_path)
	{
		try
		{
			LoadXML();
			if (m_xml_doc != null)
			{
				XElement xElement;
				for (xElement = m_xml_doc.Root.Element("Area"); xElement != null; xElement = (XElement)xElement.NextNode)
				{
					string value = xElement.Attribute("path").Value;
					string value2 = xElement.Attribute("name").Value;
					m_areas.Add(new Area(m_game, value, value2));
				}
				xElement = null;
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	public override bool CreateHardcodedView(string view_path)
	{
		switch (view_path)
		{
		case "XMLContent/World/DreamOwnRoom/Views/PlateZoom":
			new PlateZoom(m_game, m_current_area, view_path);
			return true;
		case "XMLContent/World/DreamMainRoom/Views/Zoom_Door1":
			new Zoom_Door1(m_game, m_current_area, view_path);
			return true;
		case "XMLContent/World/DreamRoom2/Views/Puzzle":
			new global::TheMare1.World.Views.DreamRoom2.Puzzle(m_game, m_current_area, view_path);
			return true;
		case "XMLContent/World/DreamRoom4/Views/Puzzle":
			new global::TheMare1.World.Views.DreamRoom4.Puzzle(m_game, m_current_area, view_path);
			return true;
		case "XMLContent/World/DreamRoom11/Views/Left_Door_Zoom":
			new Left_Door_Zoom(m_game, m_current_area, view_path);
			return true;
		default:
			return false;
		}
	}
}
