# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [17.2.2] - 2021-10-04
### Changed
 - Barco Clickshare - removed SafeCriticalSection.TryEnter, modified polling timer to not be recurring, to prevent poll method re-entry

## [7.2.1] - 2021-08-03
### Changed
 - Barco Clickshare - Fixed handling of null datetime responses for buttons

## [7.2.0] - 2021-05-14
### Added
 - AppleTv driver, a source routing device that can be controlled with an IR port.

### Changed
 - Barco Clickshare - Clarify API Version in console status
 - Remove StreamTvTunerRouteSourceControl & have StreamTvTunerDevice use new StreamSourceDeviceRoutingControl
 - BarcoClickshareConferenceDevice now implements IByodHubDevice

## [7.1.1] - 2021-07-16
### Changed
 - Barco Clickshare - Clarify API Version in console status
 - Barco Clickshare - Copy API Version on CopySettings
 - Bacro Clickshare - Don't request WLAN settings for CSM-1 devices (unsupported)
 - Barco ClickshareConference - Don't start a conference if just speaker is being used

## [7.1.0] - 2021-01-14
### Added
 - Barco Clickshare CX30 support through implementation of their V2 API
 - IStreamTvTunerDevice for TV Tuner devices that stream from a URI
 - StreamTvTunerDevice - implementation of IStreamTvTunerDevice
 
### Changed
 - Barco Clickshare - Moved telemetry properties to Monitored Device Info

## [7.0.2] - 2020-09-24
### Changed
 - Fixed a bug where clickshare activities were not being initialized

## [7.0.1] - 2020-08-13
### Changed
 - Telemetry namespace changed

## [7.0.0] - 2020-06-19
### Changed
 - MockTvTuner now implements IMockDevice
 - Using new logging context

## [6.5.0] - 2020-10-06
### Changed
 - BarcoClickshare - Implemented StartSetings to start communications with device

## [6.4.0] - 2020-03-20
### Changed
 - Fixed web requests to use new web port response.
 - Barco Clickshare times are tracked in UTC

## [6.3.0] - 2019-11-20
### Added
 - Added web proxy settings to Roku

## [6.2.0] - 2019-10-07
### Added
 - TV Presets features moved from Misc project

## [6.1.1] - 2019-09-16
### Changed
 - Barco ClickShare JSON deserialization improvements for better DateTime compatibility

## [6.1.0] - 2019-06-04
### Added
 - Roku Device API Implementation
 - Roku Device xml parsing Unit Tests

## [6.0.0] - 2019-01-10
### Added
 - Added port configuration features to source devices

## [5.2.0] - 2020-04-30
### Changed
 - Barco Clickshare - added polling for IP, Hostname, Serial, Model and added telemetry properties for them

## [5.1.2] - 2019-05-16
### Changed
 - Failing gracefully when a Crestron Thread fails to instantiate
 - When the barco fails an http request, do not immediately mark it offline

## [5.1.1] - 2018-10-30
### Changed
 - Fixing loading issue where devices do not fail gracefully when a port is not available

## [5.1.0] - 2018-09-14
### Changed
 - Routing performance improvements

## [5.0.0] - 2018-04-23
### Added
 - Adding API attributes to TV tuner

### Changed
 - Removed suffix from assembly name
