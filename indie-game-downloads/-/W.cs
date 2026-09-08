using _0004;
using D;
using P;
using Y;
using d;
using r;

namespace _000F
{
	internal struct W
	{
		public _0004.b Contact;

		public P.h Support;

		public bool HasTraction;
	}
}
namespace _0002
{
	internal struct W
	{
		internal D.h a5h;

		internal Y.a a5b;

		internal W(Y.a P_0, D.h P_1)
		{
			a5b = P_0;
			a5h = P_1;
		}
	}
}
namespace _0001
{
	internal class W : global::_0001._0018<v>
	{
		public W(global::r.B timeStepSettings)
			: base(timeStepSettings)
		{
		}

		public W(global::r.B timeStepSettings, d.b threadManager)
			: base(timeStepSettings, threadManager)
		{
		}

		protected override void MultithreadedUpdate(int i)
		{
			if (simultaneouslyUpdatedUpdateables[i].IsUpdating)
			{
				simultaneouslyUpdatedUpdateables[i].Update(timeStepSettings.TimeStepDuration);
			}
		}

		protected override void SequentialUpdate(int i)
		{
			if (sequentiallyUpdatedUpdateables[i].IsUpdating)
			{
				sequentiallyUpdatedUpdateables[i].Update(timeStepSettings.TimeStepDuration);
			}
		}
	}
}
