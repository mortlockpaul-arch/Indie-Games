using RuntimeXNA.Objects;

namespace RuntimeXNA.Conditions;

public interface IChooseValue
{
	bool evaluate(CObject pHo, int v);
}
