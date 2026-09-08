using System.Xml.Linq;
using Quasar.Global;
using Quasar.Particles;

namespace Quasar.Xml;

internal class ParticleSystemElementLoader : BaseElementLoader
{
	public override Element Load(XElement xe, string currentPath)
	{
		ParticleSystem particleSystem = ParticleSystem.Load(XDocHelper.GetAttribute(xe, "source"));
		LoadCommonData(particleSystem, xe, currentPath, loadChildren: true);
		return particleSystem;
	}
}
