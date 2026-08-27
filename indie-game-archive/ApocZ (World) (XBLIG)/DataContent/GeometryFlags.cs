namespace DataContent;

public enum GeometryFlags
{
	Clear = 0,
	Walkable = 1,
	Renderable = 2,
	MetalMesh = 4,
	Ocluder = 8,
	LOD = 0x10,
	WalkOnly = 0x20,
	Pathable = 0x40,
	AI_Climb = 0x10000,
	AI_Window = 0x20000
}
