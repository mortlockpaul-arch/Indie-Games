using Microsoft.Xna.Framework.Content;

namespace DataContent;

[ContentSerializerRuntimeType("DataContent.WaveMechanics, DataContent")]
public class WaveMechanics
{
	public string WaveTitle;

	public string WaveEndMessage;

	public int MaxSpawnedAtOnce;

	public int MaxBotThisWave;

	public int TopAttackNumber;

	public int BottomAttackNumber;

	public float TimerWaveStart;

	public float TimerWaveCoolDown;

	public float RunSpeedScalar;
}
