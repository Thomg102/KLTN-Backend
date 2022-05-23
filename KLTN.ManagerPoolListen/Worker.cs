using KLTN.Common.Models;
using KLTN.Common.SmartContracts.Events;
using KLTN.Core.MissionServices.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nethereum.Contracts;
using Nethereum.JsonRpc.WebSocketStreamingClient;
using Nethereum.RPC.Reactive.Eth;
using Nethereum.RPC.Reactive.Eth.Subscriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace KLTN.ManagerPoolListen
{
    public class Worker : BackgroundService
    {
        private readonly IServiceScopeFactory _services;
        private readonly ILogger<Worker> _logger;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        Nethereum.Web3.Web3 web3 = new Nethereum.Web3.Web3(ListenMangerPoolAppSettings.Value.RpcUrl);
        string managerPoolAddress = ListenMangerPoolAppSettings.Value.ManagerPoolContractAddress;
        int chainNetworkId = ListenMangerPoolAppSettings.Value.ChainNetworkId;

        List<string> subcribedContracts = new List<string>();

        public Worker(ILogger<Worker> logger, IServiceScopeFactory serviceScopeFactory, IHostApplicationLifetime hostApplicationLifetime)
        {
            _logger = logger;
            _services = serviceScopeFactory;
            _hostApplicationLifetime = hostApplicationLifetime;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var client = new StreamingWebSocketClient(ListenMangerPoolAppSettings.Value.WssUrl))
            {
                try
                {
                    await client.StartAsync();
                    await ListenAddStudentEvent(client);
                    await ListenAddLecturerEvent(client);
                    await ListenUpdateStudentInfoEvent(client);
                    await ListenNewMissionEvent(client);
                    await ListenNewSubjectEvent(client);
                    await ListenNewScholarshipEvent(client);
                    await ListenNewTuitionEvent(client);
                    _ = ListenActiveContract(client);
                    await PingAliveWS(client, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "ExecuteAsync exception: ");
                    _hostApplicationLifetime.StopApplication();
                }
            }
        }

        private async Task ListenNewTuitionEvent(StreamingWebSocketClient client)
        {
            var filter = web3.Eth.GetEvent<NewTuitionEventDTO>(managerPoolAddress).CreateFilterInput();
            var subscription = new EthLogsObservableSubscription(client);
            subscription.GetSubscriptionDataResponsesAsObservable().
                         Subscribe(async log =>
                         {
                             EventLog<NewTuitionEventDTO> decoded = Event<NewTuitionEventDTO>.DecodeEvent(log);
                             _logger.LogInformation($"Listening Add New Tuition {decoded.Log.Address}");
                             //await AddNewTuitionIntoDatabase(decoded.Event.StudentAddr, decoded.Event.HashInfo);
                         });

            subscription.GetSubscribeResponseAsObservable().Subscribe(id => _logger.LogInformation($"Subscribed Add New Tuition Event - {DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")}"));
            await subscription.SubscribeAsync(filter);
        }

        private async Task ListenNewScholarshipEvent(StreamingWebSocketClient client)
        {
            var filter = web3.Eth.GetEvent<NewScholarshipEventDTO>(managerPoolAddress).CreateFilterInput();
            var subscription = new EthLogsObservableSubscription(client);
            subscription.GetSubscriptionDataResponsesAsObservable().
                         Subscribe(async log =>
                         {
                             EventLog<NewScholarshipEventDTO> decoded = Event<NewScholarshipEventDTO>.DecodeEvent(log);
                             _logger.LogInformation($"Listening Add New Scholarship {decoded.Log.Address}");
                             //await AddNewScholarshipIntoDatabase(decoded.Event.StudentAddr, decoded.Event.HashInfo);
                         });

            subscription.GetSubscribeResponseAsObservable().Subscribe(id => _logger.LogInformation($"Subscribed Add New Scholarship Event - {DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")}"));
            await subscription.SubscribeAsync(filter);
        }

        private async Task ListenNewSubjectEvent(StreamingWebSocketClient client)
        {
            var filter = web3.Eth.GetEvent<NewSubjectEventDTO>(managerPoolAddress).CreateFilterInput();
            var subscription = new EthLogsObservableSubscription(client);
            subscription.GetSubscriptionDataResponsesAsObservable().
                         Subscribe(async log =>
                         {
                             EventLog<NewSubjectEventDTO> decoded = Event<NewSubjectEventDTO>.DecodeEvent(log);
                             _logger.LogInformation($"Listening Add New Subject {decoded.Log.Address}");
                             //await AddNewSubjectIntoDatabase(decoded.Event.StudentAddr, decoded.Event.HashInfo);
                         });

            subscription.GetSubscribeResponseAsObservable().Subscribe(id => _logger.LogInformation($"Subscribed Add New Subject Event - {DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")}"));
            await subscription.SubscribeAsync(filter);
        }

        private async Task ListenNewMissionEvent(StreamingWebSocketClient client)
        {
            var filter = web3.Eth.GetEvent<NewMissionEventDTO>(managerPoolAddress).CreateFilterInput();
            var subscription = new EthLogsObservableSubscription(client);
            subscription.GetSubscriptionDataResponsesAsObservable().
                         Subscribe(async log =>
                         {
                             EventLog<NewMissionEventDTO> decoded = Event<NewMissionEventDTO>.DecodeEvent(log);
                             _logger.LogInformation($"Listening Add New Mission {decoded.Log.Address}");
                             //await AddNewMissionIntoDatabase(decoded.Event.StudentAddr, decoded.Event.HashInfo);
                         });

            subscription.GetSubscribeResponseAsObservable().Subscribe(id => _logger.LogInformation($"Subscribed Add New Mission Event - {DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")}"));
            await subscription.SubscribeAsync(filter);
        }

        private async Task ListenUpdateStudentInfoEvent(StreamingWebSocketClient client)
        {
            var filter = web3.Eth.GetEvent<UpdateStudentInfoEventDTO>(managerPoolAddress).CreateFilterInput();
            var subscription = new EthLogsObservableSubscription(client);
            subscription.GetSubscriptionDataResponsesAsObservable().
                         Subscribe(async log =>
                         {
                             EventLog<UpdateStudentInfoEventDTO> decoded = Event<UpdateStudentInfoEventDTO>.DecodeEvent(log);
                             _logger.LogInformation($"Listening Update Student Info {decoded.Event.StudentAddr}");
                             //await UpdateStudentIntoDatabase(decoded.Event.StudentAddr, decoded.Event.HashInfo);
                         });

            subscription.GetSubscribeResponseAsObservable().Subscribe(id => _logger.LogInformation($"Subscribed Update Info Event - {DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")}"));
            await subscription.SubscribeAsync(filter);
        }

        private async Task ListenAddLecturerEvent(StreamingWebSocketClient client)
        {
            var filter = web3.Eth.GetEvent<AddLecturerInfoEventDTO>(managerPoolAddress).CreateFilterInput();
            var subscription = new EthLogsObservableSubscription(client);
            subscription.GetSubscriptionDataResponsesAsObservable().
                         Subscribe(async log =>
                         {
                             EventLog<AddLecturerInfoEventDTO> decoded = Event<AddLecturerInfoEventDTO>.DecodeEvent(log);
                             _logger.LogInformation($"Listening Add Lecturer Info {decoded.Event.LecturerAddr}");
                             //await AddNewLecturerIntoDatabase(decoded.Event.StudentAddr, decoded.Event.HashInfo);
                         });

            subscription.GetSubscribeResponseAsObservable().Subscribe(id => _logger.LogInformation($"Subscribed AddLecturerInfo Event - {DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")}"));
            await subscription.SubscribeAsync(filter);
        }

        private async Task ListenAddStudentEvent(StreamingWebSocketClient client)
        {
            var filter = web3.Eth.GetEvent<AddStudentInfoEventDTO>(managerPoolAddress).CreateFilterInput();
            var subscription = new EthLogsObservableSubscription(client);
            subscription.GetSubscriptionDataResponsesAsObservable().
                         Subscribe(async log =>
                         {
                             EventLog<AddStudentInfoEventDTO> decoded = Event<AddStudentInfoEventDTO>.DecodeEvent(log);
                             _logger.LogInformation($"Listening Add Student Info {decoded.Event.StudentAddr}");
                             //await AddNewStudentIntoDatabase(decoded.Event.StudentAddr, decoded.Event.HashInfo);
                         });

            subscription.GetSubscribeResponseAsObservable().Subscribe(id => _logger.LogInformation($"Subscribed AddStudentInfo Event - {DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")}"));
            await subscription.SubscribeAsync(filter);
        }

        private async Task PingAliveWS(StreamingWebSocketClient client, CancellationToken stoppingToken)
        {
            int repeat = 1;
            int minute = 0;
            while (!stoppingToken.IsCancellationRequested)
            {

                while (++repeat > 4)
                {
                    repeat = 0;
                    var handler = new EthBlockNumberObservableHandler(client);
                    handler.GetResponseAsObservable().Subscribe(x =>
                    {
                        Console.WriteLine(x.Value);
                        minute = 0;
                    });
                    await handler.SendRequestAsync();
                }
                minute++;
                if (minute >= 10)
                {
                    _logger.LogCritical("Restart beacause not receive any thing from wss");
                    _hostApplicationLifetime.StopApplication();
                }

                await Task.Delay(15000, stoppingToken);
            }
        }


        private async Task ListenActiveContract(StreamingWebSocketClient client)
        {
            while (true)
            {
                using (var scope = _services.CreateScope())
                {
                    var scopedProcessingService = scope.ServiceProvider.GetRequiredService<IMissionService>();
                    //GetListMissionInProgress goi vao db de lay ra dangh sach cac contract tuition address
                    var listMissionAddresses = await scopedProcessingService.GetListMissionInProgress(chainNetworkId);
                    foreach (var address in listMissionAddresses)
                    {
                        if (!subcribedContracts.Contains(address))
                            //await FollowCompetitionAsync(client, address);
                    }

                }
                await Task.Delay(15000);
            }
        }
    }
}
