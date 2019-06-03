using System;
using System.Collections.Generic;
using System.Text;
using ICD.Common.Utils.Xml;

namespace ICD.Connect.Sources.Roku
{
	class RokuDeviceInformation
	{
		private const string UDN_ATTRIBUTE = "udn";
		private const string SERIAL_NUMBER_ATTRIBUTE = "serial-number";
		private const string DEVICE_ID_ATTRIBUTE = "device-id";
		private const string ADVERTISING_ID_ATTRIBUTE = "advertising-id";
		private const string VENDOR_NAME_ATTRIBUTE = "vendor-name";
		private const string MODEL_NAME_ATTRIBUTE = "model-name";
		private const string MODEL_NUMBER_ATTRIBUTE = "model-number";
		private const string MODEL_REGION_ATTRIBUTE = "model-region";
		private const string IS_TV_ATTRIBUTE = "is-tv";
		private const string IS_STICK_ATTRIBUTE = "is-stick";
		private const string SUPPORTS_ETHERNET_ATTRIBUTE = "supports-ethernet";
		private const string WIFI_MAC_ATTRIBUTE = "wifi-mac";
		private const string WIFI_DRIVER_ATTRIBUTE = "wifi-driver";
		private const string NETWORK_TYPE_ATTRIBUTE = "network-type";
		private const string NETWORK_NAME_ATTRIBUTE = "network-name";
		private const string FRIENDLY_DEVICE_NAME_ATTRIBUTE = "friendly-device-name";
		private const string FRIENDLY_MODEL_NAME_ATTRIBUTE = "friendly-model-name";
		private const string DEFAULT_DEVICE_NAME_ATTRIBUTE = "default-device-name";
		private const string USER_DEVICE_NAME_ATTRIBUTE = "user-device-name";
		private const string BUILD_NUMBER_ATTRIBUTE = "build-number";
		private const string SOFTWARE_VERSION_ATTRIBUTE = "software-version";
		private const string SOFTWARE_BUILD_ATTRIBUTE = "software-build";
		private const string SECURE_DEVICE_ATTRIBUTE = "secure-device";
		private const string LANGUAGE_ATTRIBUTE = "language";
		private const string COUNTRY_ATTRIBUTE = "country";
		private const string LOCALE_ATTRIBUTE = "locale";
		private const string TIME_ZONE_AUTO_ATTRIBUTE = "time-zone-auto";
		private const string TIME_ZONE_ATTRIBUTE = "time-zone";
		private const string TIME_ZONE_NAME_ATTRIBUTE = "time-zone-name";
		private const string TIME_ZONE_TZ_ATTRIBUTE = "time-zone-tz";
		private const string TIME_ZONE_OFFSET_ATTRIBUTE = "time-zone-offset";
		private const string CLOCK_FORMAT_ATTRIBUTE = "clock-format";
		private const string UPTIME_ATTRIBUTE = "uptime";
		private const string POWER_MODE_ATRIBUTE = "power-mode";
		private const string SUPPORTS_SUSPEND_ATRIBUTE = "supports-suspend";
		private const string SUPPORTS_FIND_REMOTE_ATRIBUTE = "supports-find-remote";
		private const string SUPPORTS_AUDIO_GUIDE_ATRIBUTE = "supports-audio-guide";
		private const string SUPPORTS_RVA_ATRIBUTE = "supports-rva";
		private const string DEVELOPER_ENABLED_ATRIBUTE = "developer-enabled";
		private const string KEYED_DEVELOPER_IDS_ATRIBUTE = "keyed-developer-id";
		private const string SEARCH_ENABLED_ATRIBUTE = "search-enabled";
		private const string SEARCH_CHANNELS_ENABLED_ATRIBUTE = "search-channels-enabled";
		private const string VOICE_SEARCH_ENABLED_ATRIBUTE = "voice-search-enabled";
		private const string NOTIFICATIONS_ENABLED_ATRIBUTE = "notifications-enabled";
		private const string NOTIFICATIONS_FIRST_USE_ATRIBUTE = "notifications-first-use";
		private const string SUPPORTS_PRIVATE_LISTENING_ATRIBUTE = "supports-private-listening";
		private const string HEADPHONES_CONNECTED_ATRIBUTE = "headphones-connected";
		private const string SUPPORTS_ECS_TEXTEDIT_ATRIBUTE = "supports-ecs-textedit";
		private const string SUPPORTS_ECS_MICROPHONE_ATRIBUTE = "supports-ecs-microphone";
		private const string SUPPORTS_AWAKE_ON_WLAN_ATRIBUTE = "supports-awake-on-wlan";
		private const string HAS_PLAY_ON_ROKU_ATRIBUTE = "has-play-on-roku";
		private const string HAS_MOBILE_SCREENSAVER_ATRIBUTE = "has-mobile-screensaver";
		private const string SUPPORT_URL_ATRIBUTE = "support-url";
		private const string GRANDCENTRAL_VERSION_ATRIBUTE = "grandcentral-version";
		private const string DAVINCI_VERSION_ATRIBUTE = "davinci-version";

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
		public int Uptime { get; set; }
		public string PowerMode { get; set; }
		public bool SupportsSuspended { get; set; }
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
				Udn = XmlUtils.GetAttribute(xml, UDN_ATTRIBUTE),
				SerialNumber = XmlUtils.GetAttribute(xml, SERIAL_NUMBER_ATTRIBUTE),
				DeviceId = XmlUtils.GetAttribute(xml,DEVICE_ID_ATTRIBUTE),
				IsTv = XmlUtils.GetAttributeAsBool(xml, IS_TV_ATTRIBUTE)
			};
		}
	}
}
