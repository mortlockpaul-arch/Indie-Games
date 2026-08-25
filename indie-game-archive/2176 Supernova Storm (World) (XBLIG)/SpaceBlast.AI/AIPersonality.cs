namespace SpaceBlast.AI;

internal abstract class AIPersonality
{
	public int VisualRange = 60000;

	public int ShieldRetreatThreshold = 60;

	public int StrengthRetreatThreshold = 60;

	public float Accuracy = 0.1f;
}
