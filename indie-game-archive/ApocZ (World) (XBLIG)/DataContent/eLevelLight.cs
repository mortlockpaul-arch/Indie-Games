using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace DataContent;

[ContentSerializerRuntimeType("DataContent.eLevelLight, DataContent")]
public class eLevelLight
{
	public string Name;

	public LightTypes eType;

	public Vector3 Position;

	public Vector3 SpotDirection;

	public Vector3 SpotParameters;

	public Color LightColor;

	public Color ShadowColor;

	public Matrix Transform;

	public float Timer;

	public float Intensity;

	public float FlickerIntensity;

	public float DecayRate;

	public float DecayRadius;

	public float Multiplyer;

	public float ShadowMultiplyer;

	public float FarAttenStart;

	public float FarAttenEnd;

	public float NearAttenStart;

	public float NearAttenEnd;

	[ContentSerializerIgnore]
	public float Luminance;

	[ContentSerializerIgnore]
	public bool IsDynamic;
}
