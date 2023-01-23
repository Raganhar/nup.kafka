﻿using Amazon.SQS;
using Amazon.SQS.Model;
using KafkaAndSqsShoveller;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using WebApplication1.Config;
using WebApplication1.Models;

namespace WebApplication1.SqsStuff;

public interface IAWSSQSHelper
{
    Task<bool> SendMessageAsync(UserDetail userDetail, string eventType);
    Task<bool> SendMessageAsync(string @event, string eventType);
    Task<List<Message>> ReceiveMessageAsync();
    Task<bool> DeleteMessageAsync(string messageReceiptHandle);
}

public class AWSSQSHelper : IAWSSQSHelper
{
    private readonly IAmazonSQS _sqs;
    private readonly ServiceConfiguration _settings;

    public AWSSQSHelper(
        IAmazonSQS sqs,
        IOptions<ServiceConfiguration> settings)
    {
        this._sqs = sqs;
        this._settings = settings.Value;
    }

    public async Task<bool> SendMessageAsync(string @event, string eventType)
    {
        try
        {
            var sendRequest = new SendMessageRequest(_settings.AWSSQS.QueueUrl, @event);
            sendRequest.MessageAttributes = new Dictionary<string, MessageAttributeValue>
            {
                {
                    LegacySqsConsts.Event, new MessageAttributeValue
                    {
                        StringValue = eventType,
                        DataType = LegacySqsConsts.String
                    }
                }
            };
            // Post message or payload to queue  
            var sendResult = await _sqs.SendMessageAsync(sendRequest);

            return sendResult.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public async Task<bool> SendMessageAsync(UserDetail @event, string eventType)
    {
        return await SendMessageAsync(JsonConvert.SerializeObject(@event, settings: new JsonSerializerSettings()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        }),eventType);
    }

    public async Task<List<Message>> ReceiveMessageAsync()
    {
        try
        {
            //Create New instance  
            var request = new ReceiveMessageRequest
            {
                QueueUrl = _settings.AWSSQS.QueueUrl,
                MaxNumberOfMessages = 10,
                WaitTimeSeconds = 5,
                MessageAttributeNames = new List<string>{".*", LegacySqsConsts.Event},
            };
            //CheckIs there any new message available to process  
            var result = await _sqs.ReceiveMessageAsync(request);

            return result.Messages.Any() ? result.Messages : new List<Message>();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<bool> DeleteMessageAsync(string messageReceiptHandle)
    {
        try
        {
            //Deletes the specified message from the specified queue  
            var deleteResult = await _sqs.DeleteMessageAsync(_settings.AWSSQS.QueueUrl, messageReceiptHandle);
            return deleteResult.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}