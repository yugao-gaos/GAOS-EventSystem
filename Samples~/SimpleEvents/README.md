# SimpleEvents Sample - GAOS Event System

This sample demonstrates how to use the GAOS Event System for communication between Unity GameObjects using a centralized event system service with callbacks.

## What to Learn from This Sample

This sample demonstrates several key concepts:

1. **Service-Based Event Communication**: How to use a central event system service for decoupled communication
2. **Type-Safe Events**: Using interfaces like ICalculationRequest and ICalculationResult to ensure type safety
3. **UI Integration**: Connecting UI elements to event-based systems
4. **Async Event Patterns**: Using async/await for events that need to return results

## Key Components

- **ObjectA**: Acts as an event sender, triggering notification and calculation events
- **ObjectB**: Acts as an event receiver, handling events and returning results for calculations
- **SimpleEventsUIController**: Connects UI elements to the event system
- **LogListener**: Updates UI based on Debug.Log messages
- **DataTypes.cs**: Contains interface definitions for event data

## How to Use This Sample

1. **Open the Scene**: Load the SimpleEvents scene in Unity
2. **Examine the Hierarchy**:
   - Notice the ObjectA and ObjectB GameObjects
   - Look at the UI elements (SimpleEventsCanvas) 

3. **Inspect the Scripts**:
   - Review ObjectA.cs to see how events are registered and triggered
   - Review ObjectB.cs to see how event handlers are registered
   - Review SimpleEventsUIController.cs to see how UI elements connect to the event system

4. **Try the Sample**:
   - Click the "Send Notification" button to send a simple notification event
   - Enter values, select an operation, and click "Calculate" to trigger a calculation event
   - Watch the console and UI for feedback

## Implementation Details

### Event Registration

```csharp
// In ObjectA.Start():
_eventSystem.RegisterEvent(SIMPLE_NOTIFICATION);
_eventSystem.RegisterEvent<ICalculationRequest, ICalculationResult>(CALCULATE_REQUEST);
```

### Event Subscription

```csharp
// In ObjectB.Start():
_eventSystem.Subscribe(SIMPLE_NOTIFICATION, _onSimpleNotification);
_eventSystem.Subscribe<ICalculationRequest, ICalculationResult>(
    CALCULATE_REQUEST, _onCalculateRequest);
```

### Triggering Events

```csharp
// Simple event:
_eventSystem.TriggerEvent(SIMPLE_NOTIFICATION);

// Event with parameters and return value:
var trigger = await _eventSystem.TriggerEventAsync<ICalculationRequest, ICalculationResult>(
    CALCULATE_REQUEST, request);
```

## Best Practices Demonstrated

1. **Storing Delegate References**: Keep references to delegates to properly unsubscribe
2. **Cleanup on Destroy**: Always unsubscribe from events in OnDestroy
3. **Direct UI Connections**: UI elements directly connect to event-triggering code
4. **Type-Safe Interfaces**: Using interfaces to ensure type safety in event data
5. **Error Handling**: Proper error handling in async event responses

## Common Issues

- Ensure the ServiceLocator package is properly installed
- If events aren't being received, check the event key strings match exactly
- Verify ObjectA and ObjectB are both active in the scene
- Check the console for initialization errors

## Testing Unity Package Samples

This directory contains a copy of the package samples for testing within the Unity Editor. Since samples stored in the `Samples~` folder of a package aren't directly accessible until imported through the Package Manager, we maintain this copy for development and testing.

## Testing Process

1. **Develop in Assets/SamplesForTesting**:
   - Make changes to the sample scripts here
   - Open the TestScene.unity scene
   - Test functionality in the Unity Editor
   - Iterate until everything works correctly

2. **Copy to Package Samples Folder**:
   - Once tested and working, copy the files to:
   - `Packages/com.gaos.eventsystem/Samples~/SimpleGameObjectEvents/`

3. **Test in a New Project**:
   - For final verification, test the samples by importing them through Package Manager in a new project

## Additional Setup

The TestScene.unity scene contains:
- ObjectA and ObjectB GameObjects
- Attach the ObjectA and ObjectB components from the SimpleEventSample.cs file
- Use the inspector buttons to trigger events and observe the console output 

## Next Steps

Once you've finished testing:

```bash
# Copy the files to the package samples folder
cp -r Assets/SamplesForTesting/SimpleGameObjectEvents/* Packages/com.gaos.eventsystem/Samples~/SimpleGameObjectEvents/
``` 