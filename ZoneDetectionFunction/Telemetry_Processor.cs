/*
 * Copyright (c) 2023 Sony Semiconductor Solutions Corporation
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

using static ZoneDetectionFunction.Models;
using Google.FlatBuffers;
using SmartCamera;

namespace ZoneDetectionFunction
{
    public class Telemetry_Processor
    {
        private const string Signalr_Hub = "telemetryHub";
        private static ILogger _logger = null;

        [FunctionName("Telemetry_Processor")]
        public async Task Run([EventHubTrigger("sonysmartcamera", Connection = "EventHubConnectionString")] EventData[] eventData,
            [SignalR(HubName = Signalr_Hub)] IAsyncCollector<SignalRMessage> signalRMessage,
            ILogger logger)
        {
            var exceptions = new List<Exception>();

            _logger = logger;

            foreach (EventData ed in eventData)
            {
                try
                {
                    DateTime now = DateTime.Now;

                    if (now.Date > ed.EnqueuedTime.Date)
                    {
                        continue;
                    }

                    Dictionary<string, object> jsonEventBody = JsonConvert.DeserializeObject<Dictionary<string, object>>(ed.EventBody.ToString());

                    if (jsonEventBody.ContainsKey("backdoor-EA_Main/placeholder"))
                    {
                        EVENTBODY telemetryMessage = JsonConvert.DeserializeObject<EVENTBODY>(jsonEventBody["backdoor-EA_Main/placeholder"].ToString());
                        string deviceId = telemetryMessage.DeviceID;
                        string modelId = telemetryMessage.ModelID;
                        _logger.LogInformation($"Telemetry Message : {ed.EventBody.ToString()}");
                        DateTimeOffset enqueuedTime = (DateTimeOffset)ed.SystemProperties["x-opt-enqueued-time"];
                        //// Initialize SignalR Data
                        SIGNALR_DATA signalrData = new SIGNALR_DATA
                        {
                            eventId = ed.SystemProperties["x-opt-sequence-number"].ToString(),
                            eventTime = enqueuedTime.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
                            deviceId = deviceId,
                            dtDataSchema = modelId,
                            data = null
                        };

                        List<INFERENCE_ITEM> deserialize = Deserialize(telemetryMessage.Inferences[0].O);
                        INFERENCE_RESULT inferenceResult = new INFERENCE_RESULT
                        {
                            T = telemetryMessage.Inferences[0].T,
                            inferenceResults = deserialize
                        };

                        signalrData.data = JsonConvert.SerializeObject(inferenceResult);
                        _logger.LogInformation(JsonConvert.SerializeObject(signalrData));

                        if (signalrData.data != null)
                        {
                            // send to SignalR Hub
                            var data = JsonConvert.SerializeObject(signalrData);
                            _logger.LogInformation($"SignalR Message (EventHub): {data.Length}");

                            await signalRMessage.AddAsync(new SignalRMessage
                            {
                                Target = "DeviceTelemetry",
                                Arguments = new[] { data }
                            });
                        }

                        signalrData = null;
                    }
                    else
                    {
                        _logger.LogInformation("Unsupported Message Source");
                    }
                }
                catch (Exception e)
                {
                    exceptions.Add(e);
                }
            }

            if (exceptions.Count > 1)
                throw new AggregateException(exceptions);

            if (exceptions.Count == 1)
                throw exceptions.Single();
        }

        private static List<INFERENCE_ITEM> Deserialize(string inferenceData)
        {
            byte[] buf = Convert.FromBase64String(inferenceData);
            ObjectDetectionTop objectDetectionTop = ObjectDetectionTop.GetRootAsObjectDetectionTop(new ByteBuffer(buf));
            ObjectDetectionData objectData = objectDetectionTop.Perception ?? new ObjectDetectionData();
            int resNum = objectData.ObjectDetectionListLength;
            _logger.LogInformation($"NumOfDetections: {resNum.ToString()}");

            List<INFERENCE_ITEM> inferenceResults = new List<INFERENCE_ITEM>();
            for (int i = 0; i < resNum; i++)
            {
                GeneralObject objectList = objectData.ObjectDetectionList(i) ?? new GeneralObject();
                BoundingBox unionType = objectList.BoundingBoxType;
                if (unionType == BoundingBox.BoundingBox2d)
                {
                    var bbox2d = objectList.BoundingBox<BoundingBox2d>().Value;
                    INFERENCE_ITEM data = new INFERENCE_ITEM();
                    data.C = objectList.ClassId;
                    data.P = (float)Math.Round(objectList.Score, 6, MidpointRounding.AwayFromZero);
                    data.iou = objectList.Iou;
                    data.Zoneflag = objectList.Zoneflag;
                    data.Left = bbox2d.Left;
                    data.Top = bbox2d.Top;
                    data.Right = bbox2d.Right;
                    data.Bottom = bbox2d.Bottom;

                    inferenceResults.Add(data);
                }
            }
            return inferenceResults;
        }
    }
}