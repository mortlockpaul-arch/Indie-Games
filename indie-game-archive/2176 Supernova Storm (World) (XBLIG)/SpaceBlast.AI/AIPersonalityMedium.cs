namespace SpaceBlast.AI;

internal class AIPersonalityMedium : AIPersonality
{
	public AIPersonalityMedium()
	{
		VisualRange = 50000;
		ShieldRetreatThreshold = 60;
		StrengthRetreatThreshold = 60;
		Accuracy = 0.5f;
	}
}
