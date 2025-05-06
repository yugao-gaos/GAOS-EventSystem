# GAOS Event System

A type-safe event system for Unity, designed to handle events with flexibility, synchronous and asynchronous support, and strict type safety.

## Overview

The GAOS Event System provides a robust event management solution built specifically for Unity. It seamlessly integrates with Unity's component lifecycle while delivering:

- **Type-Safe Events**: Interface-based event parameters and return values
- **Flexible Event Types**: Support for void events, events with parameters, events with return values, or both
- **Asynchronous Support**: Handle long-running operations with async/await
- **Progress Tracking**: Monitor event execution and results in real-time
- **Priority-Based Execution**: Control the order of event handler execution
- **ScriptableObject Events**: Use ScriptableObject-based events for persistent, inspector-configurable event assets

## Key Features

### ScriptableObject Event System

The package includes a complete ScriptableObject-based event system that provides:

- **Persistence**: Events that survive scene transitions and play mode changes
- **Editor Integration**: Configure and assign events through Unity's Inspector
- **Event Bridges**: Connect SO events to UnityEvents for component interactions
- **Zero Dependencies**: Create fully decoupled architectures where components have no direct references to each other

```csharp
// Create a ScriptableObject event in the editor and use it like this:
[SerializeField] private SimpleNotificationEvent notificationEvent;

// Trigger the event
notificationEvent.Raise();

// For events with parameters and return values
var request = new CalculationRequest(valueA, valueB, operation);
var results = await calculationEvent.RaiseAsync(request);
```

## Installation

The GAOS Event System is automatically registered as a service in your Unity project through the ServiceLocator system. No manual installation is required beyond importing the package:

```
com.gaos.eventsystem
```

### Dependencies
- com.gaos.servicelocator
- com.unity.nuget.newtonsoft-json

## Documentation

For detailed usage instructions and examples, please refer to the included documentation:

- [Quick Start Guide](Documentation~/quick-start.html) - Get up and running in minutes
- [Advanced Topics](Documentation~/advanced.html) - In-depth exploration of features
- [SOEvent Wrapper](Documentation~/soevents.html) - Using ScriptableObject-based events
- [API Reference](Documentation~/api.html) - Complete method and interface reference

## Using the Event System

The GAOS Event System is accessed through the ServiceLocator:

```csharp
// Get the event system service
IEventSystem eventSystem = ServiceLocator.GetService<IEventSystem>("EventSystem");
```

From there, you can register, subscribe to, and trigger events. See the documentation for complete examples.

## Best Practices

1. Always unsubscribe from events when they're no longer needed
2. Use strongly-typed interfaces for event parameters and return types
3. Follow the `IDataInterface` contract for all event data
4. Consider using async/await for long-running operations
5. Use priority values thoughtfully to control execution order
6. Use ScriptableObject events for cross-scene communication
7. Consider using event bridges to decouple component interactions

## Changelog

For a list of changes and version history, see the [Changelog](CHANGELOG.md).

---
*Package created and maintained by [Yu Gao](https://www.linkedin.com/in/yugao-luckyvr)*
