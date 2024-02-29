using ContainerPacking2.Entities;
using System.Collections.Generic;

namespace ContainerPacking2.Algorithms
{
	public abstract class AlgorithmBase
	{
		public abstract ContainerPackingResult Run(Container container, List<Item> items);
	}
}