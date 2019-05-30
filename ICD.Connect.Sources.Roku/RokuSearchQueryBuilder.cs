using System;
using System.Collections.Generic;
using System.Text;
using ICD.Common.Utils.Collections;

namespace ICD.Connect.Sources.Roku
{
	public sealed class UriQueryBuilder
	{
		private readonly Dictionary<string, string> m_Parameters;

		public UriQueryBuilder()
		{
			m_Parameters = new Dictionary<string, string>();
		}

		public UriQueryBuilder Append(string key, string value)
		{
			m_Parameters.Add(key, value);
			return this;
		}

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder("?");

			bool first = true;

			foreach (KeyValuePair<string, string> kvp in m_Parameters)
			{
				if (!first)
					builder.Append('&');
				first = false;

				builder.Append(kvp.Key);
				builder.Append('=');
				builder.Append(kvp.Value);
			}

			return builder.ToString();
		}
	}

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

	public enum eRokuSearchType
	{
		Movie,
		TvShow,
		Person,
		Channel,
		Game
	}

	public class RokuSearchQueryBuilder
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