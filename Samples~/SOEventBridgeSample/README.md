# ScriptableObject Event Bridge Sample

This sample demonstrates how to use ScriptableObject-based events with bridges to create a flexible, decoupled architecture in Unity.

## Key Concepts

1. **Complete Decoupling**: Components have zero direct references to each other but still communicate effectively
2. **ScriptableObject Events**: Events as assets that persist across scenes and editor sessions
3. **Event Bridges**: Connect ScriptableObject events to UnityEvents for editor configuration
4. **Separation of Concerns**: Each component has a single responsibility
5. **Callback-Based Async Communication**: Using callbacks and Tasks for asynchronous operations

## Architecture Overview

The sample consists of these key components:

### ScriptableObject Events (Assets)

- **SimpleNotificationEvent**: Basic event with no parameters
- **CalculationEvent**: Event with parameters and return values (ICalculationRequest → ICalculationResult)

### Bridge Components (MonoBehaviours)

- **SimpleNotificationBridge**: Connects SimpleNotificationEvent to UnityEvents
- **CalculationEventBridge**: Connects CalculationEvent to callback-based UnityEvents

### Processing Components (MonoBehaviours)

- **CalculationProcessor**: Performs calculations with no dependencies on other components
- **SOEventDemo**: UI controller that communicates only with ScriptableObject events

## Decoupling Benefits Demonstrated

1. **UI Controller (SOEventDemo.cs)**:
   - Knows about UI components and SO events, but not about processors
   - Sends notifications and calculation requests through SO events
   - Displays results received through event callbacks

2. **Processors (CalculationProcessor.cs)**:
   - Completely standalone with no reference to event system
   - Pure business logic that receives input and returns output
   - Can be tested independently from UI or event system

3. **Bridge Components**:
   - Connect the independent components at runtime
   - Subscribe to SO events in OnEnable(), unsubscribe in OnDisable()
   - Forward events to UnityEvents that can be connected in the Inspector

## How It Works

1. **Event Flow**:
   ```
   UI Interaction → SOEventDemo → SO Event → Bridge → UnityEvent → Processor → Result → UI
   ```

2. **For Notification Events**:
   ```csharp
   // SOEventDemo.cs
   public void SendNotification()
   {
       if (notificationEvent != null)
       {
           notificationEvent.Raise();
       }
   }
   ```

3. **For Calculation Events**:
   ```csharp
   // SOEventDemo.cs
   public async void PerformCalculation()
   {
       // Create request and trigger the event
       var request = new CalculationRequest(valueA, valueB, operation);
       var trigger = await calculationEvent.RaiseAsync(request);
       
       if (trigger.CompletedResults.Count > 0)
       {
           var result = trigger.CompletedResults[0];
           SetResult($"Result: {result.Result}", result.Success);
       }
   }
   ```

4. **Processor (completely independent)**:
   ```csharp
   // CalculationProcessor.cs
   public void ProcessCalculation(ICalculationRequest request, Action<ICalculationResult> callback)
   {
       // Process calculation
       float result = PerformOperation(request);
       
       // Return result via callback
       callback(new CalculationResult(result, true));
   }
   ```

5. **All connections are made through the Inspector**:
   - Assign SO event assets to bridges and controllers
   - Connect bridge UnityEvents to processor methods
   - No code references between components

## Key Implementation Highlights

1. **TaskCompletionSource for Async Operations**:
   - Bridges between callback-based and Task-based code
   - Allows using `await` with event callbacks

2. **EventBridgeBase Abstract Class**:
   - Handles common subscription functionality
   - Ensures proper cleanup in OnDisable()

3. **Thread Safety**:
   - ExecuteOnMainThread() ensures Unity operations run on the main thread

## Setting Up Your Own System

1. **Create ScriptableObject Event Assets**:
   - SimpleNotificationEvent for simple triggers
   - CalculationEvent for request-response patterns

2. **Create Bridge Components**:
   - Attach to GameObject in scenes
   - Assign SO event assets in Inspector
   - Connect UnityEvents to processors

3. **Connect Events in the Inspector**:
   - Wire everything together without code dependencies
   - Makes the system flexible and easy to reconfigure

## Benefits Over Traditional Approaches

1. **Persistence**: ScriptableObject events persist across scenes and play sessions
2. **Decoupling**: Components have zero direct references to each other
3. **Testability**: Each component can be tested independently
4. **Editor Integration**: Events can be configured and monitored in the Unity Editor
5. **Modularity**: Easy to add/remove/replace components without breaking the system
6. **Event Debugging**: Events can be monitored and debugged in the Inspector 