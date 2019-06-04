using ICD.Common.Utils.Xml;

namespace ICD.Connect.Sources.Roku
{
	public sealed class RokuDeviceInformation
	{
		private const string UDN_ELEMENT = "udn";
		private const string SERIAL_NUMBER_ELEMENT = "serial-number";
		private const string DEVICE_ID_ELEMENT = "device-id";
		private const string ADVERTISING_ID_ELEMENT = "advertising-id";
		private const string VENDOR_NAME_ELEMENT = "vendor-name";
		private const string MODEL_NAME_ELEMENT = "model-name";
		private const string MODEL_NUMBER_ELEMENT = "model-number";
		private const string MODEL_REGION_ELEMENT = "model-region";
		private const string IS_TV_ELEMENT = "is-tv";
		private const string IS_STICK_ELEMENT = "is-stick";
		private const string SUPPORTS_ETHERNET_ELEMENT = "supports-ethernet";
		private const string WIFI_MAC_ELEMENT = "wifi-mac";
		private const string WIFI_DRIVER_ELEMENT = "wifi-driver";
		private const string NETWORK_TYPE_ELEMENT = "network-type";
		private const string NETWORK_NAME_ELEMENT = "network-name";
		private const string FRIENDLY_DEVICE_NAME_ELEMENT = "friendly-device-name";
		private const string FRIENDLY_MODEL_NAME_ELEMENT = "friendly-model-name";
		private const string DEFAULT_DEVICE_NAME_ELEMENT = "default-device-name";
		private const string USER_DEVICE_NAME_ELEMENT = "user-device-name";
		private const string BUILD_NUMBER_ELEMENT = "build-number";
		private const string SOFTWARE_VERSION_ELEMENT = "software-version";
		private const string SOFTWARE_BUILD_ELEMENT = "software-build";
		private const string SECURE_DEVICE_ELEMENT = "secure-device";
		private const string LANGUAGE_ELEMENT = "language";
		private const string COUNTRY_ELEMENT = "country";
		private const string LOCALE_ELEMENT = "locale";
		private const string TIME_ZONE_AUTO_ELEMENT = "time-zone-auto";
		private const string TIME_ZONE_ELEMENT = "time-zone";
		private const string TIME_ZONE_NAME_ELEMENT = "time-zone-name";
		private const string TIME_ZONE_TZ_ELEMENT = "time-zone-tz";
		private const string TIME_ZONE_OFFSET_ELEMENT = "time-zone-offset";
		private const string CLOCK_FORMAT_ELEMENT = "clock-format";
		private const string UPTIME_ELEMENT = "uptime";
		private const string POWER_MODE_ELEMENT = "power-mode";
		private const string SUPPORTS_SUSPEND_ELEMENT = "supports-suspend";
		private const string SUPPORTS_FIND_REMOTE_ELEMENT = "supports-find-remote";
		private const string SUPPORTS_AUDIO_GUIDE_ELEMENT = "supports-audio-guide";
		private const string SUPPORTS_RVA_ELEMENT = "supports-rva";
		private const string DEVELOPER_ENABLED_ELEMENT = "developer-enabled";
		private const string KEYED_DEVELOPER_IDS_ELEMENT = "keyed-developer-id";
		private const string SEARCH_ENABLED_ELEMENT = "search-enabled";
		private const string SEARCH_CHANNELS_ENABLED_ELEMENT = "search-channels-enabled";
		private const string VOICE_SEARCH_ENABLED_ELEMENT = "voice-search-enabled";
		private const string NOTIFICATIONS_ENABLED_ELEMENT = "notifications-enabled";
		private const string NOTIFICATIONS_FIRST_USE_ELEMENT = "notifications-first-use";
		private const string SUPPORTS_PRIVATE_LISTENING_ELEMENT = "supports-private-listening";
		private const string HEADPHONES_CONNECTED_ELEMENT = "headphones-connected";
		private const string SUPPORTS_ECS_TEXTEDIT_ELEMENT = "supports-ecs-textedit";
		private const string SUPPORTS_ECS_MICROPHONE_ELEMENT = "supports-ecs-microphone";
		private const string SUPPORTS_AWAKE_ON_WLAN_ELEMENT = "supports-awake-on-wlan";
		private const string HAS_PLAY_ON_ROKU_ELEMENT = "has-play-on-roku";
		private const string HAS_MOBILE_SCREENSAVER_ELEMENT = "has-mobile-screensaver";
		private const string SUPPORT_URL_ELEMENT = "support-url";
		private const string GRANDCENTRAL_VERSION_ELEMENT = "grandcentral-version";
		private const string DAVINCI_VERSION_ELEMENT = "davinci-version";

		public string Udn { get; set; }
		public string SerialNumber { get; set; }
		public string DeviceId { get; set; }
		public string AdvertisingId { get; set; }
		public string VendorName { get; set; }
		public string ModelName { get; set; }
		public string ModelNumber { get; set; }
		public string ModelRegion { get; set; }
		public bool IsTv { get; set; }
		public bool IsStick { get; set; }
		public bool SupportsEthernet { get; set; }
		public string WifiMac { get; set; }
		public string WifiDriver { get; set; }
		public string NetworkType { get; set; }
		public string NetworkName { get; set; }
		public string FriendlyDeviceName { get; set; }
		public string FriendlyModelName { get; set; }
		public string DefaultDeviceName { get; set; }
		public string UserDeviceName { get; set; }
		public string BuildNumber { get; set; }
		public string SoftwareVersion { get; set; }
		public int SoftwareBuild { get; set; }
		public bool SecureDevice { get; set; }
		public string Language { get; set; }
		public string Country { get; set; }
		public string Locale { get; set; }
		public bool TimeZoneAuto { get; set; }
		public string TimeZone { get; set; }
		public string TimeZoneName { get; set; }
		public string TimeZoneTz { get; set; }
		public string TimeZoneOffset { get; set; }
		public string ClockFormat { get; set; }
		public int UpTime { get; set; }
		public string PowerMode { get; set; }
		public bool SupportsSuspend { get; set; }
		public bool SupportsFindRemote { get; set; }
		public bool SupportsAudioGuide { get; set; }
		public bool SupportsRva { get; set; }
		public bool DeveloperEnabled { get; set; }
		public string KeyedDelevoperId { get; set; }
		public bool SearchEnabled { get; set; }
		public bool SearchChannelsEnabled { get; set; }
		public bool VoiceSearchEnabled { get; set; }
		public bool NotificationsEnabled { get; set; }
		public bool NotificationsFirstUse { get; set; }
		public bool SupportsPrivateListening { get; set; }
		public bool HeadphonesConnected { get; set; }
		public bool SupportsEcsTextedit { get; set; }
		public bool SupportsEcsMicrophone { get; set; }
		public bool SupportsWakeOnWlan { get; set; }
		public bool HasPlayOnRoku { get; set; }
		public bool HasMobileScreensaver { get; set; }
		public string SupportUrl { get; set; }
		public string GrandcentralVersion { get; set; }
		public string DavinciVersion { get; set; }

		public static RokuDeviceInformation ReadDeviceInformationFromXml(string xml)
		{
			return new RokuDeviceInformation
			{
				Udn = XmlUtils.TryReadChildElementContentAsString(xml, UDN_ELEMENT),
				SerialNumber = XmlUtils.TryReadChildElementContentAsString(xml, SERIAL_NUMBER_ELEMENT),
				DeviceId = XmlUtils.TryReadChildElementContentAsString(xml,DEVICE_ID_ELEMENT),
				AdvertisingId = XmlUtils.TryReadChildElementContentAsString(xml,ADVERTISING_ID_ELEMENT),
				VendorName = XmlUtils.TryReadChildElementContentAsString(xml, VENDOR_NAME_ELEMENT),
				ModelName = XmlUtils.TryReadChildElementContentAsString(xml, MODEL_NAME_ELEMENT),
				ModelNumber = XmlUtils.TryReadChildElementContentAsString(xml, MODEL_NUMBER_ELEMENT),
				ModelRegion = XmlUtils.TryReadChildElementContentAsString(xml, MODEL_REGION_ELEMENT),
				IsTv = XmlUtils.TryReadChildElementContentAsBoolean(xml, IS_TV_ELEMENT) ?? false,
				IsStick = XmlUtils.TryReadChildElementContentAsBoolean(xml, IS_STICK_ELEMENT) ?? false,
				SupportsEthernet = XmlUtils.TryReadChildElementContentAsBoolean(xml, SUPPORTS_ETHERNET_ELEMENT) ?? false,
				WifiMac = XmlUtils.TryReadChildElementContentAsString(xml, WIFI_MAC_ELEMENT),
				WifiDriver = XmlUtils.TryReadChildElementContentAsString(xml, WIFI_DRIVER_ELEMENT),
				NetworkType = XmlUtils.TryReadChildElementContentAsString(xml, NETWORK_TYPE_ELEMENT),
				NetworkName = XmlUtils.TryReadChildElementContentAsString(xml, NETWORK_NAME_ELEMENT),
				FriendlyDeviceName = XmlUtils.TryReadChildElementContentAsString(xml, FRIENDLY_DEVICE_NAME_ELEMENT),
				FriendlyModelName = XmlUtils.TryReadChildElementContentAsString(xml, FRIENDLY_MODEL_NAME_ELEMENT),
				DefaultDeviceName = XmlUtils.TryReadChildElementContentAsString(xml, DEFAULT_DEVICE_NAME_ELEMENT),
				UserDeviceName = XmlUtils.TryReadChildElementContentAsString(xml, USER_DEVICE_NAME_ELEMENT),
				BuildNumber = XmlUtils.TryReadChildElementContentAsString(xml, BUILD_NUMBER_ELEMENT),
				SoftwareVersion = XmlUtils.TryReadChildElementContentAsString(xml, SOFTWARE_VERSION_ELEMENT),
				SoftwareBuild = XmlUtils.TryReadChildElementContentAsInt(xml, SOFTWARE_BUILD_ELEMENT) ?? 0,
				SecureDevice = XmlUtils.TryReadChildElementContentAsBoolean(xml, SECURE_DEVICE_ELEMENT) ?? false,
				Language = XmlUtils.TryReadChildElementContentAsString(xml, LANGUAGE_ELEMENT),
				Country = XmlUtils.TryReadChildElementContentAsString(xml, COUNTRY_ELEMENT),
				Locale = XmlUtils.TryReadChildElementContentAsString(xml, LOCALE_ELEMENT),
				TimeZoneAuto = XmlUtils.TryReadChildElementContentAsBoolean(xml, TIME_ZONE_AUTO_ELEMENT) ?? false,
				TimeZone = XmlUtils.TryReadChildElementContentAsString(xml, TIME_ZONE_ELEMENT),
				TimeZoneName = XmlUtils.TryReadChildElementContentAsString(xml, TIME_ZONE_NAME_ELEMENT),
				TimeZoneTz = XmlUtils.TryReadChildElementContentAsString(xml, TIME_ZONE_TZ_ELEMENT),
				TimeZoneOffset = XmlUtils.TryReadChildElementContentAsString(xml, TIME_ZONE_OFFSET_ELEMENT),
				ClockFormat = XmlUtils.TryReadChildElementContentAsString(xml, CLOCK_FORMAT_ELEMENT),
				UpTime = XmlUtils.TryReadChildElementContentAsInt(xml, UPTIME_ELEMENT) ?? 0,
				PowerMode = XmlUtils.TryReadChildElementContentAsString(xml, POWER_MODE_ELEMENT),
				SupportsSuspend = XmlUtils.TryReadChildElementContentAsBoolean(xml, SUPPORTS_SUSPEND_ELEMENT) ?? false,
				SupportsFindRemote = XmlUtils.TryReadChildElementContentAsBoolean(xml, SUPPORTS_FIND_REMOTE_ELEMENT) ?? false,
				SupportsAudioGuide = XmlUtils.TryReadChildElementContentAsBoolean(xml, SUPPORTS_AUDIO_GUIDE_ELEMENT) ?? false,
				SupportsRva = XmlUtils.TryReadChildElementContentAsBoolean(xml, SUPPORTS_RVA_ELEMENT) ?? false,
				DeveloperEnabled = XmlUtils.TryReadChildElementContentAsBoolean(xml, DEVELOPER_ENABLED_ELEMENT) ?? false,
				KeyedDelevoperId = XmlUtils.TryReadChildElementContentAsString(xml, KEYED_DEVELOPER_IDS_ELEMENT),
				SearchEnabled = XmlUtils.TryReadChildElementContentAsBoolean(xml, SEARCH_ENABLED_ELEMENT) ?? false,
				SearchChannelsEnabled = XmlUtils.TryReadChildElementContentAsBoolean(xml, SEARCH_CHANNELS_ENABLED_ELEMENT) ?? false,
				VoiceSearchEnabled = XmlUtils.TryReadChildElementContentAsBoolean(xml, VOICE_SEARCH_ENABLED_ELEMENT) ?? false,
				NotificationsEnabled = XmlUtils.TryReadChildElementContentAsBoolean(xml, NOTIFICATIONS_ENABLED_ELEMENT) ?? false,
				NotificationsFirstUse = XmlUtils.TryReadChildElementContentAsBoolean(xml, NOTIFICATIONS_FIRST_USE_ELEMENT) ?? false,
				SupportsPrivateListening = XmlUtils.TryReadChildElementContentAsBoolean(xml, SUPPORTS_PRIVATE_LISTENING_ELEMENT) ?? false,
				HeadphonesConnected = XmlUtils.TryReadChildElementContentAsBoolean(xml, HEADPHONES_CONNECTED_ELEMENT) ?? false,
				SupportsEcsTextedit = XmlUtils.TryReadChildElementContentAsBoolean(xml, SUPPORTS_ECS_TEXTEDIT_ELEMENT) ?? false,
				SupportsEcsMicrophone = XmlUtils.TryReadChildElementContentAsBoolean(xml, SUPPORTS_ECS_MICROPHONE_ELEMENT) ?? false,
				SupportsWakeOnWlan = XmlUtils.TryReadChildElementContentAsBoolean(xml, SUPPORTS_AWAKE_ON_WLAN_ELEMENT) ?? false,
				HasPlayOnRoku = XmlUtils.TryReadChildElementContentAsBoolean(xml, HAS_PLAY_ON_ROKU_ELEMENT) ?? false,
				HasMobileScreensaver = XmlUtils.TryReadChildElementContentAsBoolean(xml, HAS_MOBILE_SCREENSAVER_ELEMENT) ?? false,
				SupportUrl = XmlUtils.TryReadChildElementContentAsString(xml, SUPPORT_URL_ELEMENT),
				GrandcentralVersion = XmlUtils.TryReadChildElementContentAsString(xml, GRANDCENTRAL_VERSION_ELEMENT),
				DavinciVersion = XmlUtils.TryReadChildElementContentAsString(xml, DAVINCI_VERSION_ELEMENT)
			};
		}
	}
}
