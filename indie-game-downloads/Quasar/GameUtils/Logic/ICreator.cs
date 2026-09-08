namespace Quasar.GameUtils.Logic;

public interface ICreator<T> where T : class
{
	T Create();
}
