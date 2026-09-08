namespace Quasar.GameUtils.Logic;

public interface IReusableEntity
{
	bool Released { get; }

	void Release();
}
