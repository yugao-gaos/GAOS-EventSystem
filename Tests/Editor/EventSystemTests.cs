using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using NUnit.Framework;
using GAOS.ServiceLocator;
using GAOS.ServiceLocator.Editor;
using GAOS.ServiceLocator.TestUtils;
using GAOS.EventSystem;

namespace GAOS.EventSystem.Editor.Tests
{
    public interface ITestEventData : IDataInterface
    {
        string Message { get; }
        int Value { get; }
    }

    public interface ITestReturnData : IDataInterface
    {
        string Result { get; }
    }

    public class TestEventData : ITestEventData
    {
        public string Message { get; set; }
        public int Value { get; set; }
    }

    public class TestReturnData : ITestReturnData
    {
        public string Result { get; set; }
    }

    public class EventSystemTests
    {
        private IEventSystem _eventSystem;
        private ITestServiceUtility _registration;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _registration = ServiceLocatorTestUtils.GetTestRegistration();
            // Initialize ServiceLocator for testing
            _registration.SetSkipEditorContextValidation(true);
            _registration.Register<IEventSystem, EventSystem>("EventSystem");


            // The event system should be auto-registered by the ServiceLocator
            _eventSystem = GAOS.ServiceLocator.ServiceLocator.GetService<IEventSystem>("EventSystem");
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            // Reset ServiceLocator state
            _registration.Clear();
            ServiceTypeCacheBuilder.RebuildTypeCache(isUnitTest: false, enableLogging: false);
            _registration.SetSkipEditorContextValidation(false);
        }

        [TearDown]
        public void TearDown()
        {
            // Clear all events after each test
            if (_eventSystem is EventSystem eventSystem)
            {
                eventSystem.ClearAllEvents();
            }
        }

        [Test]
        public void RegisterEvent_WithValidName_Succeeds()
        {
            // Arrange
            string eventName = "TestEvent";

            // Act & Assert
            Assert.DoesNotThrow(() => _eventSystem.RegisterEvent(eventName));
        }

        [Test]
        public void RegisterEvent_WithGenericParameter_Succeeds()
        {
            // Arrange
            string eventName = "TestParameterEvent";

            // Act & Assert
            Assert.DoesNotThrow(() => _eventSystem.RegisterEvent<ITestEventData>(eventName));
        }

        [Test]
        public void RegisterEvent_WithGenericParameterAndReturn_Succeeds()
        {
            // Arrange
            string eventName = "TestFullEvent";

            // Act & Assert
            Assert.DoesNotThrow(() => _eventSystem.RegisterEvent<ITestEventData, ITestReturnData>(eventName));
        }

        [Test]
        public void Subscribe_ToUnregisteredEvent_ThrowsException()
        {
            // Arrange
            string eventName = "UnregisteredEvent";
            Action handler = () => { };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _eventSystem.Subscribe(eventName, handler));
        }

        [Test]
        public void Subscribe_WithValidHandler_Succeeds()
        {
            // Arrange
            string eventName = "SubscriptionTest";
            _eventSystem.RegisterEvent(eventName);
            Action handler = () => { };

            // Act & Assert
            Assert.DoesNotThrow(() => _eventSystem.Subscribe(eventName, handler));
        }

        [Test]
        public void Unsubscribe_ExistingHandler_Succeeds()
        {
            // Arrange
            string eventName = "UnsubscribeTest";
            _eventSystem.RegisterEvent(eventName);
            Action handler = () => { };
            _eventSystem.Subscribe(eventName, handler);

            // Act & Assert
            Assert.DoesNotThrow(() => _eventSystem.Unsubscribe(eventName, handler));
        }

        [Test]
        public void TriggerEvent_WithNoListeners_DoesNotThrow()
        {
            // Arrange
            string eventName = "NoListenerEvent";
            _eventSystem.RegisterEvent(eventName);

            // Act & Assert
            Assert.DoesNotThrow(() => _eventSystem.TriggerEvent(eventName));
        }

        [Test]
        public void TriggerEvent_WithListener_InvokesHandler()
        {
            // Arrange
            string eventName = "ListenerEvent";
            _eventSystem.RegisterEvent(eventName);
            bool handlerCalled = false;
            Action handler = () => handlerCalled = true;
            _eventSystem.Subscribe(eventName, handler);

            // Act
            _eventSystem.TriggerEvent(eventName);

            // Assert
            Assert.That(handlerCalled, Is.True);
        }

        [Test]
        public void TriggerEvent_WithParameters_PassesCorrectData()
        {
            // Arrange
            string eventName = "ParameterEvent";
            _eventSystem.RegisterEvent<ITestEventData>(eventName);
            ITestEventData receivedData = null;
            Action<ITestEventData> handler = data => receivedData = data;
            _eventSystem.Subscribe(eventName, handler);

            var testData = new TestEventData { Message = "Test", Value = 42 };

            // Act
            _eventSystem.TriggerEvent(eventName, testData);

            // Assert
            Assert.That(receivedData, Is.Not.Null);
            Assert.That(receivedData.Message, Is.EqualTo("Test"));
            Assert.That(receivedData.Value, Is.EqualTo(42));
        }

        [Test]
        public async Task TriggerEventAsync_WithMultipleListeners_ReturnsAllResults()
        {
            // Arrange
            string eventName = "AsyncEvent";
            _eventSystem.RegisterEvent<ITestEventData, ITestReturnData>(eventName);

            _eventSystem.Subscribe<ITestEventData, ITestReturnData>(eventName, 
                data => Task.FromResult<ITestReturnData>(new TestReturnData { Result = $"First: {data.Message}" }));
            
            _eventSystem.Subscribe<ITestEventData, ITestReturnData>(eventName, 
                data => Task.FromResult<ITestReturnData>(new TestReturnData { Result = $"Second: {data.Message}" }));

            var testData = new TestEventData { Message = "Test", Value = 42 };

            // Act
            var trigger = await _eventSystem.TriggerEventAsync<ITestEventData, ITestReturnData>(
                eventName, 
                testData
            );

            var results = trigger.CompletedResults;

            // Assert
            Assert.That(results.Count, Is.EqualTo(2));
            Assert.That(results[0].Result, Is.EqualTo("First: Test"));
            Assert.That(results[1].Result, Is.EqualTo("Second: Test"));
        }

        [Test]
        public async Task TriggerEventAsync_WithProgressCallback_TracksProgress()
        {
            // Arrange
            string eventName = "ProgressEvent";
            _eventSystem.RegisterEvent<ITestEventData, ITestReturnData>(eventName);
            int progressCallCount = 0;
            int lastCurrent = 0;
            int lastTotal = 0;

            _eventSystem.Subscribe<ITestEventData, ITestReturnData>(eventName, 
                data => Task.FromResult<ITestReturnData>(new TestReturnData { Result = "First" }));
            
            _eventSystem.Subscribe<ITestEventData, ITestReturnData>(eventName, 
                data => Task.FromResult<ITestReturnData>(new TestReturnData { Result = "Second" }));

            var testData = new TestEventData { Message = "Test", Value = 42 };

            // Act
            var trigger = await _eventSystem.TriggerEventAsync<ITestEventData, ITestReturnData>(
                eventName, 
                testData,
                async (result, current, total) => {
                    progressCallCount++;
                    lastCurrent = current;
                    lastTotal = total;
                }
            );

            // Assert
            Assert.That(progressCallCount, Is.EqualTo(2));
            Assert.That(lastCurrent, Is.EqualTo(2));
            Assert.That(lastTotal, Is.EqualTo(2));
        }

        [Test]
        public async Task TriggerEventAsync_WithPriority_ExecutesInOrder()
        {
            // Arrange
            string eventName = "PriorityEvent";
            _eventSystem.RegisterEvent<ITestEventData, ITestReturnData>(eventName);
            var executionOrder = new List<string>();

            _eventSystem.Subscribe<ITestEventData, ITestReturnData>(eventName, 
                data => {
                    executionOrder.Add("Low");
                    return Task.FromResult<ITestReturnData>(new TestReturnData { Result = "Low" });
                }, -1);

            _eventSystem.Subscribe<ITestEventData, ITestReturnData>(eventName, 
                data => {
                    executionOrder.Add("High");
                    return Task.FromResult<ITestReturnData>(new TestReturnData { Result = "High" });
                }, 1);

            _eventSystem.Subscribe<ITestEventData, ITestReturnData>(eventName, 
                data => {
                    executionOrder.Add("Normal");
                    return Task.FromResult<ITestReturnData>(new TestReturnData { Result = "Normal" });
                }, 0);

            var testData = new TestEventData { Message = "Test", Value = 42 };

            // Act
            var trigger = await _eventSystem.TriggerEventAsync<ITestEventData, ITestReturnData>(
                eventName, 
                testData
            );

            // Assert
            Assert.That(executionOrder[0], Is.EqualTo("High"));
            Assert.That(executionOrder[1], Is.EqualTo("Normal"));
            Assert.That(executionOrder[2], Is.EqualTo("Low"));
        }

        [Test]
        public void MultipleEventRegistrations_WithSameName_OnlyRegistersOnce()
        {
            // Arrange
            string eventName = "DuplicateEvent";

            // Act
            _eventSystem.RegisterEvent(eventName);
            _eventSystem.RegisterEvent(eventName);

            bool handlerCalled = false;
            Action handler = () => handlerCalled = true;
            _eventSystem.Subscribe(eventName, handler);
            _eventSystem.TriggerEvent(eventName);

            // Assert
            Assert.That(handlerCalled, Is.True);
        }

        [Test]
        public void UnsubscribeNonexistentHandler_DoesNotThrow()
        {
            // Arrange
            string eventName = "NonexistentHandler";
            _eventSystem.RegisterEvent(eventName);
            Action handler = () => { };

            // Act & Assert
            Assert.DoesNotThrow(() => _eventSystem.Unsubscribe(eventName, handler));
        }
        
        [Test]
        public async Task TriggerEventAsync_WithAsyncHandler_ProcessesCorrectly()
        {
            // Arrange
            string eventName = "AsyncHandlerEvent";
            _eventSystem.RegisterEvent<ITestEventData, ITestReturnData>(eventName);
            
            // Register an async handler with a delay to simulate async work
            _eventSystem.Subscribe<ITestEventData, ITestReturnData>(eventName, 
                async data => {
                    await Task.Delay(50); // Simulate async operation
                    return new TestReturnData { Result = $"Async result for: {data.Message}" };
                });

            var testData = new TestEventData { Message = "Test", Value = 42 };

            // Act
            var trigger = await _eventSystem.TriggerEventAsync<ITestEventData, ITestReturnData>(
                eventName, 
                testData
            );

            // Assert
            Assert.That(trigger.CompletedResults.Count, Is.EqualTo(1));
            Assert.That(trigger.CompletedResults[0].Result, Is.EqualTo("Async result for: Test"));
        }
    }
} 