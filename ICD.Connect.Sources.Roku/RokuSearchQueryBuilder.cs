using System;
using System.Collections.Generic;
using ICD.Common.Utils;
using ICD.Common.Utils.Collections;

namespace ICD.Connect.Sources.Roku
{
	public enum eRokuSearchPar
	{
		Keyword,
		Title,
		Type,
		Tmsid,
		Season,
		Showunavailable,
		MatchAny,
		ProviderId,
		Provider,
		Launch
	}

	public sealed class RokuSearchQueryBuilder
	{
		private static readonly Dictionary<eRokuSearchPar, string> s_SearchPars =
			new Dictionary<eRokuSearchPar, string>
			{
				{eRokuSearchPar.Keyword, "keyword"},
				{eRokuSearchPar.Title, "title"},
				{eRokuSearchPar.Type, "type"},
				{eRokuSearchPar.Tmsid, "tmsid"},
				{eRokuSearchPar.Season, "season"},
				{eRokuSearchPar.Showunavailable, "show-unavailable"},
				{eRokuSearchPar.MatchAny, "match-any"},
				{eRokuSearchPar.ProviderId, "provider-id"},
				{eRokuSearchPar.Provider, "provider"},
				{eRokuSearchPar.Launch, "launch"}
			};

		private static readonly IcdHashSet<string> s_SearchTypes =
			new IcdHashSet<string>
			{
				"movie",
				"tv-show",
				"person",
				"channel",
				"game"
			};

		private readonly UriQueryBuilder m_Builder;

		public RokuSearchQueryBuilder()
		{
			m_Builder = new UriQueryBuilder();
		}

		public RokuSearchQueryBuilder Append(eRokuSearchPar parameter, string value)
		{
			if (parameter == eRokuSearchPar.Type && !s_SearchTypes.Contains(value))
				throw new ArgumentException("Unexpected type value", "value");

			string param = s_SearchPars[parameter];
			m_Builder.Append(param, value);
			return this;
		}

		public RokuSearchQueryBuilder Append(IEnumerable<KeyValuePair<eRokuSearchPar, string>> parameters)
		{
			if (parameters == null)
				throw new ArgumentNullException("parameters");

			foreach (KeyValuePair<eRokuSearchPar, string> kvp in parameters)
				Append(kvp.Key, kvp.Value);

			return this;
		}

		public override string ToString()
		{
			return m_Builder.ToString();
		}
	}
}