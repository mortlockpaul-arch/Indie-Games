namespace EGEngine;

public struct ObjectDefinition(string n, ObjectTypes t)
{
	public string objName = n;

	public ObjectTypes objType = t;
}
