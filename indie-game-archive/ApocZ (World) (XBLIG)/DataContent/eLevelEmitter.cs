using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace DataContent;

[ContentSerializerRuntimeType("DataContent.eLevelEmitter, DataContent")]
public class eLevelEmitter
{
	public string Name;

	public EmitterType eType;

	public bool Flicker;

	public float Scale;

	public float Timer;

	public Vector3 Position;

	public Vector3 Direction;

	public Color LightColor;

	public List<int> ChildLights;
}
