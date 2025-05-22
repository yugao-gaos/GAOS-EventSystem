# Changelog
All notable changes to the GAOS Event System package will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.9.1] - 2025-05-21
### Added
- Added `autoRegister` parameter to all `TriggerEvent` methods
- Option to automatically register events when triggered if not already registered
- Default behavior preserved: Core system defaults to `autoRegister = false`, ScriptableObject wrappers default to `autoRegister = true`
- Updated documentation to reflect new parameter options

## [0.9.0] - 2025-05-15
### Added
- Integration with GAOS.Logger package for improved logging capabilities
- Dependency on com.gaos.logger package

## [0.8.0] - 2025-05-05
### Added
- Initial release of the GAOS Event System
- Complete implementation of the event system core functionality
- Support for different event types:
  - Simple void events (no parameters or return values)
  - Events with parameters
  - Events with return values
  - Events with both parameters and return values
- Type-safe event handling with IDataInterface requirement
- Asynchronous event handling with Task support
- Priority-based event execution
- Event trigger tracking and cancellation support
- Comprehensive documentation:
  - Quick Start Guide
  - Advanced Topics
  - API Reference
  - README with examples

### Dependencies
- Requires com.gaos.servicelocator package
- Requires com.unity.nuget.newtonsoft-json package

---
*Package created and maintained by [Yu Gao](https://www.linkedin.com/in/yugao-luckyvr)* 