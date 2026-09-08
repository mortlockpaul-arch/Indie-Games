using System.Diagnostics.CodeAnalysis;

namespace System.ComponentModel.Design;

public interface IDesignerHost : IServiceContainer, IServiceProvider
{
	[FeatureSwitchDefinition("System.ComponentModel.Design.IDesignerHost.IsSupported")]
	[FeatureGuard(typeof(RequiresUnreferencedCodeAttribute))]
	internal static bool IsSupported
	{
		get
		{
			if (!AppContext.TryGetSwitch("System.ComponentModel.Design.IDesignerHost.IsSupported", out var isEnabled))
			{
				return true;
			}
			return isEnabled;
		}
	}

	bool Loading { get; }

	bool InTransaction { get; }

	IContainer Container { get; }

	IComponent RootComponent { get; }

	string RootComponentClassName { get; }

	string TransactionDescription { get; }

	event EventHandler Activated;

	event EventHandler Deactivated;

	event EventHandler LoadComplete;

	event DesignerTransactionCloseEventHandler TransactionClosed;

	event DesignerTransactionCloseEventHandler TransactionClosing;

	event EventHandler TransactionOpened;

	event EventHandler TransactionOpening;

	void Activate();

	IComponent CreateComponent(Type componentClass);

	IComponent CreateComponent(Type componentClass, string name);

	DesignerTransaction CreateTransaction();

	DesignerTransaction CreateTransaction(string description);

	void DestroyComponent(IComponent component);

	IDesigner? GetDesigner(IComponent component);

	Type? GetType(string typeName);
}
