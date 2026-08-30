using System;

namespace Maximinus.DebugTools;

[Flags]
public enum Alignment
{
	None = 0,
	Left = 1,
	Right = 2,
	HorizontalCenter = 4,
	Top = 8,
	Bottom = 0x10,
	VerticalCenter = 0x20,
	TopLeft = Left | Top,
	TopRight = Right | Top,
	TopCenter = HorizontalCenter | Top,
	BottomLeft = Left | Bottom,
	BottomRight = Right | Bottom,
	BottomCenter = HorizontalCenter | Bottom,
	CenterLeft = Left | VerticalCenter,
	CenterRight = Right | VerticalCenter,
	Center = HorizontalCenter | VerticalCenter
}
