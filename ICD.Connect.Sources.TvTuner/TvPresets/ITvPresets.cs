﻿using System.Collections.Generic;

namespace ICD.Connect.Sources.TvTuner.TvPresets
{
	/// <summary>
	/// ITvPresets provides functionality for managing a set of stored TV stations.
	/// </summary>
	public interface ITvPresets : ICollection<Station>
	{
	}
}
