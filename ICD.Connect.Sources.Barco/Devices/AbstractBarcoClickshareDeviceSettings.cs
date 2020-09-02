﻿using System;
using ICD.Common.Utils.Xml;
using ICD.Connect.Devices;
using ICD.Connect.Protocol.Network.Ports.Web;
using ICD.Connect.Protocol.Network.Settings;
using ICD.Connect.Settings.Attributes.SettingsProperties;

namespace ICD.Connect.Sources.Barco.Devices
{
	/// <summary>
	/// Settings for the BarcoClickshareDevice.
	/// </summary>
	public abstract class AbstractBarcoClickshareDeviceSettings : AbstractDeviceSettings, IUriSettings, IWebProxySettings
	{
		private const string PORT_ELEMENT = "Port";
		private const string API_VERSION_ELEMENT = "ApiVersion";

		private const string DEFAULT_API_VERSION = "1.0";

		private readonly UriProperties m_UriProperties;
		private readonly WebProxyProperties m_WebProxyProperties;

		#region Properties

		[OriginatorIdSettingsProperty(typeof(IWebPort))]
		public int? Port { get; set; }

		public string ApiVersion { get; set; }

		#endregion

		#region URI

		/// <summary>
		/// Gets/sets the configurable URI username.
		/// </summary>
		public string UriUsername { get { return m_UriProperties.UriUsername; } set { m_UriProperties.UriUsername = value; } }

		/// <summary>
		/// Gets/sets the configurable URI password.
		/// </summary>
		public string UriPassword { get { return m_UriProperties.UriPassword; } set { m_UriProperties.UriPassword = value; } }

		/// <summary>
		/// Gets/sets the configurable URI host.
		/// </summary>
		public string UriHost { get { return m_UriProperties.UriHost; } set { m_UriProperties.UriHost = value; } }

		/// <summary>
		/// Gets/sets the configurable URI port.
		/// </summary>
		public ushort? UriPort { get { return m_UriProperties.UriPort; } set { m_UriProperties.UriPort = value; } }

		/// <summary>
		/// Gets/sets the configurable URI scheme.
		/// </summary>
		public string UriScheme { get { return m_UriProperties.UriScheme; } set { m_UriProperties.UriScheme = value; } }

		/// <summary>
		/// Gets/sets the configurable URI path.
		/// </summary>
		public string UriPath { get { return m_UriProperties.UriPath; } set { m_UriProperties.UriPath = value; } }

		/// <summary>
		/// Gets/sets the configurable URI query.
		/// </summary>
		public string UriQuery { get { return m_UriProperties.UriQuery; } set { m_UriProperties.UriQuery = value; } }

		/// <summary>
		/// Gets/sets the configurable URI fragment.
		/// </summary>
		public string UriFragment { get { return m_UriProperties.UriFragment; } set { m_UriProperties.UriFragment = value; } }

		/// <summary>
		/// Clears the configured values.
		/// </summary>
		void IUriProperties.ClearUriProperties()
		{
			m_UriProperties.ClearUriProperties();
		}

		#endregion

		#region Proxy

		/// <summary>
		/// Gets/sets the configurable proxy username.
		/// </summary>
		public string ProxyUsername { get { return m_WebProxyProperties.ProxyUsername; } set { m_WebProxyProperties.ProxyUsername = value; } }

		/// <summary>
		/// Gets/sets the configurable proxy password.
		/// </summary>
		public string ProxyPassword { get { return m_WebProxyProperties.ProxyPassword; } set { m_WebProxyProperties.ProxyPassword = value; } }

		/// <summary>
		/// Gets/sets the configurable proxy host.
		/// </summary>
		public string ProxyHost { get { return m_WebProxyProperties.ProxyHost; } set { m_WebProxyProperties.ProxyHost = value; } }

		/// <summary>
		/// Gets/sets the configurable proxy port.
		/// </summary>
		public ushort? ProxyPort { get { return m_WebProxyProperties.ProxyPort; } set { m_WebProxyProperties.ProxyPort = value; } }

		/// <summary>
		/// Gets/sets the configurable proxy scheme.
		/// </summary>
		public string ProxyScheme { get { return m_WebProxyProperties.ProxyScheme; } set { m_WebProxyProperties.ProxyScheme = value; } }

		/// <summary>
		/// Gets/sets the configurable proxy authentication method.
		/// </summary>
		public eProxyAuthenticationMethod? ProxyAuthenticationMethod
		{
			get { return m_WebProxyProperties.ProxyAuthenticationMethod; }
			set { m_WebProxyProperties.ProxyAuthenticationMethod = value; }
		}

		/// <summary>
		/// Clears the configured values.
		/// </summary>
		public void ClearProxyProperties()
		{
			m_WebProxyProperties.ClearProxyProperties();
		}

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		protected AbstractBarcoClickshareDeviceSettings()
		{
			m_UriProperties = new UriProperties();
			m_WebProxyProperties = new WebProxyProperties();

			UpdateUriDefaults();
		}

		/// <summary>
		/// Writes property elements to xml.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteElements(IcdXmlTextWriter writer)
		{
			base.WriteElements(writer);

			writer.WriteElementString(PORT_ELEMENT, IcdXmlConvert.ToString(Port));
			writer.WriteElementString(API_VERSION_ELEMENT, IcdXmlConvert.ToString(API_VERSION_ELEMENT));

			m_UriProperties.WriteElements(writer);
			m_WebProxyProperties.WriteElements(writer);
		}

		/// <summary>
		/// Updates the settings from xml.
		/// </summary>
		/// <param name="xml"></param>
		public override void ParseXml(string xml)
		{
			base.ParseXml(xml);

			Port = XmlUtils.TryReadChildElementContentAsInt(xml, PORT_ELEMENT);
			ApiVersion = XmlUtils.TryReadChildElementContentAsString(xml, API_VERSION_ELEMENT) ?? DEFAULT_API_VERSION;

			m_UriProperties.ParseXml(xml);
			m_WebProxyProperties.ParseXml(xml);

			UpdateUriDefaults();
		}

		private void UpdateUriDefaults()
		{
			m_UriProperties.ApplyDefaultValues(null, null, null, 4001, Uri.UriSchemeHttps, null, null, null);
		}
	}
}
