using System.Collections.Concurrent;
using ExampleEvents;
using FluentAssertions;
using Newtonsoft.Json;
using nup.kafka.DatabaseStuff;

namespace nup.kafka.tests;

public class CompetingConsumersTests
{
    private KafkaWrapper _client;

    [SetUp]
    public void Setup()
    {
        _client = new KafkaWrapper("TestApp", new KafkaOptions
        {
            PartitionCount = 4,
            Brokers = TestConsts.brokers
        });
    }

    [Test]
    public async Task CompetingConsumers()
    {
        void EventHandler(ConcurrentDictionary<string, int> processedEvents, SampleEvent1 e, string processer)
        {
            var v = processedEvents.GetOrAdd(processer, 0);
            processedEvents.AddOrUpdate(processer, v + 1, (i, event1) => v + 1);
            Console.WriteLine($"{processer} received: {e.Name}");
        }

        try
        {
            var consumerOptions = new KafkaOptions()
            {
                AppName = "TestApp",
                Brokers = TestConsts.brokers
            };
            var _consumer1 = new KafkaWrapperConsumer(consumerOptions, new EventProcesser(),"consumer1").WithDatabase(KafkaMysqlDbContext.ConnectionString);
            var _consumer2 = new KafkaWrapperConsumer(consumerOptions, new EventProcesser(),"consumer2").WithDatabase(KafkaMysqlDbContext.ConnectionString);

            var processedEvents = new ConcurrentDictionary<string, int>();
            
            _consumer1.Consume(new CancellationToken(),(SampleEvent1 e)=> { EventHandler(processedEvents, e, "handler1"); });
            _consumer2.Consume(new CancellationToken(),(SampleEvent1 e)=> { EventHandler(processedEvents, e, "handler2"); });

            await Task.Delay(3000);
            foreach (var i in Enumerable.Range(0,10))
            {
                await _client.Send(new SampleEvent1
                {
                    Age = i,
                    Name = "bobsilol"
                });   
            }

            await Task.Delay(3000);
            processedEvents.Values.Sum().Should().Be(10);
            processedEvents.Keys.Count.Should().Be(2);
            foreach (var processedEventsKey in processedEvents.Keys)
            {
                processedEvents[processedEventsKey].Should().BeGreaterThan(0);
            }

            Console.WriteLine($"Data: {JsonConvert.SerializeObject(processedEvents,Formatting.Indented)}");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}