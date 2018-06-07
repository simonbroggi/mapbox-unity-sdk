using System;
namespace Mapbox.Unity.MeshGeneration.Factories
{
	public interface IFactoryInjectionStrategy
	{
		AbstractTileFactory CreateFactory();
	}
}
