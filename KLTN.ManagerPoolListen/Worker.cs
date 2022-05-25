using KLTN.Common.Models;
using KLTN.Common.SmartContracts.Events;
using KLTN.Common.Utilities.Constants;
using KLTN.Core.LecturerServicess.Interfaces;
using KLTN.Core.MissionServices.Interfaces;
using KLTN.Core.ScholarshipServices.Interfaces;
using KLTN.Core.StudentServices.Interfaces;
using KLTN.Core.SubjectServices.Interfaces;
using KLTN.Core.TuitionServices.DTOs;
using KLTN.Core.TuitionServices.Interfaces;
using KLTN.ManagerPoolListen.DTOs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nethereum.Contracts;
using Nethereum.JsonRpc.WebSocketStreamingClient;
using Nethereum.RPC.Reactive.Eth;
using Nethereum.RPC.Reactive.Eth.Subscriptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
                    _logger.LogInformation("Application start ");
                    await client.StartAsync();
                    await ListenAddStudentEvent(client);
                    await ListenAddLecturerEvent(client);
                    await ListenUpdateStudentInfoEvent(client);
                    await ListenNewMissionEvent(client);
                    await ListenNewSubjectEvent(client);
                    await ListenNewScholarshipEvent(client);
                    await ListenNewTuitionEvent(client);
                    /*                 _ = ListenActiveMissionContract(client);
                                     _ = ListenActiveSubjectContract(client);
                                     _ = ListenActiveScholarshipContract(client);
                                     _ = ListenActiveTuitionContract(client);*/
                    await PingAliveWS(client, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "ExecuteAsync exception: ");
                    _hostApplicationLifetime.StopApplication();
                }
            }
        }

        private async Task<string> RequestIPFS(string hashInfo)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            var response = httpClient.GetAsync($"{Constants.IPFS_BASE_URL}{hashInfo}").Result;
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                //var fixtureStreamUrl = JsonConvert.DeserializeObject<>(json);
                return json;
            }
            return String.Empty;
        }

        private async Task ListenNewTuitionEvent(StreamingWebSocketClient client)
        {
            var filter = web3.Eth.GetEvent<NewTuitionEventDTO>(managerPoolAddress).CreateFilterInput();
            var subscription = new EthLogsObservableSubscription(client);
            subscription.GetSubscriptionDataResponsesAsObservable().
                         Subscribe(async log =>
                         {
                             EventLog<NewTuitionEventDTO> decoded = Event<NewTuitionEventDTO>.DecodeEvent(log);
                             string jsonResponse = await RequestIPFS("QmYTz3GW3Xho27ZY8WgN8By4i1DwmkKCeGCJCKnBcKX16k");
                             var tuitionMetadata = JsonConvert.DeserializeObject<TuitionMetadataDTO>(jsonResponse);
                             using (var scope = _services.CreateScope())
                             {
                                 var scopedProcessingService = scope.ServiceProvider.GetRequiredService<ITuitionService>();
                                 await scopedProcessingService.CreateNewTuition(new TuitionDTO
                                 {
                                     ChainNetwork = chainNetworkId,
                                     Img = tuitionMetadata.Img,
                                     TuitionName = tuitionMetadata.Name,
                                     TuitionAddress = decoded.Log.Address,
                                     TuitionDescription = tuitionMetadata.Description,
                                     TuitionHashIPFS = decoded.Event.UrlMetadata,
                                     SchoolYear = DateTime.Now.Year,
                                     StartTime = tuitionMetadata.StartTime,
                                     EndTime = tuitionMetadata.EndTime,
                                     TokenAmount = long.Parse(tuitionMetadata.AmountToken),
                                     CurrencyAmount = long.Parse(tuitionMetadata.AmountCurency),
                                     LecturerInCharge = tuitionMetadata.LecturerInCharge
                                 });
                             }
                             _logger.LogInformation($"Listening Add New Tuition {decoded.Log.Address}");
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
                             string jsonResponse = await RequestIPFS(decoded.Event.UrlMetadata);
                             var metadata = JsonConvert.DeserializeObject<ScholarshipMetadataDTO>(jsonResponse);
                             using (var scope = _services.CreateScope())
                             {
                                 var scopedProcessingService = scope.ServiceProvider.GetRequiredService<IScholarshipService>();
                                 await scopedProcessingService.CreateNewScholarship(new Core.ScholarshipServices.DTOs.ScholarshipDTO
                                 {
                                     ChainNetworkId = chainNetworkId,
                                     ScholarshipName = metadata.Name,
                                     ScholarshipAddress = decoded.Log.Address,
                                     ScholarShipDescription = metadata.Description,
                                     ScholarshipHashIPFS = decoded.Event.UrlMetadata,
                                     StartTime = metadata.StartTime,
                                     EndTime = metadata.EndTime,
                                     TokenAmount = long.Parse(metadata.Award),
                                     LecturerInCharge = metadata.LecturerInCharge
                                 });
                             }
                             _logger.LogInformation($"Listening Add New Scholarship {decoded.Log.Address}");
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
                             string jsonResponse = await RequestIPFS(decoded.Event.UrlMetadata);
                             var metadata = JsonConvert.DeserializeObject<SubjectMetadataDTO>(jsonResponse);
                             using (var scope = _services.CreateScope())
                             {
                                 var scopedProcessingService = scope.ServiceProvider.GetRequiredService<ISubjectService>();
                                 await scopedProcessingService.CreateNewSubject(new Core.SubjectServices.DTOs.SubjectDTO
                                 {
                                     ChainNetworkId = chainNetworkId,
                                     SubjectName = metadata.Name,
                                     SubjectAddress = decoded.Log.Address,
                                     SubjectShortenName = metadata.ShortName,
                                     SubjectImg = metadata.Img,
                                     SubjectDescription = metadata.Description,
                                     SubjectHashIPFS = decoded.Event.UrlMetadata,
                                     DepartmentName = metadata.Faculty,
                                     StartTime = metadata.StartTime,
                                     EndTime = metadata.EndTime,
                                     EndTimeToResigter = metadata.EndTimeToRegister,
                                     EndTimeToComFirm = metadata.EndTimeToConfirm,
                                     MaxStudentAmount = int.Parse(metadata.MaxEntrant),
                                     LecturerAddress = metadata.LecturerInCharge
                                 });
                             }
                             _logger.LogInformation($"Listening Add New Subject {decoded.Log.Address}");
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
                             string jsonResponse = await RequestIPFS(decoded.Event.UrlMetadata);
                             var metadata = JsonConvert.DeserializeObject<MissionMetadataDTO>(jsonResponse);
                             using (var scope = _services.CreateScope())
                             {
                                 var scopedProcessingService = scope.ServiceProvider.GetRequiredService<IMissionService>();
                                 await scopedProcessingService.CreateNewMission(new Core.MissionServices.DTOs.MissionDTO
                                 {
                                     ChainNetworkId = chainNetworkId,
                                     MissionName = metadata.Name,
                                     MissionAddress = decoded.Log.Address,
                                     MissionShortenName = metadata.ShortName,
                                     MissionImg = metadata.Img,
                                     MissionDescription = metadata.Description,
                                     MissionHashIPFS = decoded.Event.UrlMetadata,
                                     DepartmentName = metadata.Faculty,
                                     StartTime = metadata.StartTime,
                                     EndTime = metadata.EndTime,
                                     EndTimeToResigter = metadata.EndTimeToRegister,
                                     EndTimeToComFirm = metadata.EndTimeToConfirm,
                                     MaxStudentAmount = int.Parse(metadata.MaxEntrant),
                                     LecturerAddress = metadata.LecturerInCharge,
                                     TokenAmount = long.Parse(metadata.Award),
                                     LecturerName = metadata.LecturerName
                                 });
                             }
                             _logger.LogInformation($"Listening Add New Mission {decoded.Log.Address}");
                         });

            subscription.GetSubscribeResponseAsObservable().Subscribe(id => _logger.LogInformation($"Subscribed Add New Mission Event - {DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")}"));
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
                             string jsonResponse = await RequestIPFS(decoded.Event.HashInfo);
                             var metadata = JsonConvert.DeserializeObject<LecturerMetadataDTO>(jsonResponse);
                             using (var scope = _services.CreateScope())
                             {
                                 var scopedProcessingService = scope.ServiceProvider.GetRequiredService<ILecturerService>();
                                 await scopedProcessingService.CreateNewLectuter(new Core.LecturerServices.DTOs.LecturerDTO
                                 {
                                     LecturerName = metadata.Name,
                                     LecturerId = metadata.LecturerId,
                                     LecturerAddress = decoded.Event.LecturerAddr,
                                     DepartmentName = metadata.Faculty,
                                     DepartmentShortenName = metadata.FacultyShortName,
                                     Sex = metadata.Gender,
                                     DateOfBirth = metadata.DateOfBirth,
                                     LecturerHashIPFS = decoded.Event.HashInfo
                                 });
                             }
                             _logger.LogInformation($"Listening Add Lecturer Info {decoded.Event.LecturerAddr}");
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
                             string jsonResponse = await RequestIPFS(decoded.Event.HashInfo);
                             var metadata = JsonConvert.DeserializeObject<StudentMetadataDTO>(jsonResponse);
                             using (var scope = _services.CreateScope())
                             {
                                 var scopedProcessingService = scope.ServiceProvider.GetRequiredService<IStudentService>();
                                 await scopedProcessingService.CreateNewStudent(new Core.StudentServices.DTOs.StudentDTO
                                 {
                                     StudentName = metadata.Name,
                                     StudentId = metadata.StudentId,
                                     StudentAddress = decoded.Event.StudentAddr,
                                     MajorName = metadata.Major,
                                     ClassroomName = metadata.Class,
                                     DepartmentName = metadata.Faculty,
                                     DepartmentShortenName = metadata.FacultyShortName,
                                     SchoolYear = int.Parse(metadata.SchoolYear),
                                     Sex = metadata.Gender,
                                     DateOfBirth = metadata.Birthday,
                                     BirthPlace = metadata.PlaceOfBirth,
                                     Ethnic = metadata.Nation,
                                     NationalId = metadata.Cmnd,
                                     DateOfNationalId = metadata.IssuranceDate,
                                     PlaceOfNationalId = metadata.IssuancePlace,
                                     PermanentAddress = metadata.Address,
                                     StudentHashIPFS = decoded.Event.HashInfo
                                 });
                             }
                             _logger.LogInformation($"Listening Add Student Info {decoded.Event.StudentAddr}");
                         });

            subscription.GetSubscribeResponseAsObservable().Subscribe(id => _logger.LogInformation($"Subscribed AddStudentInfo Event - {DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")}"));
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
        /*
        private async Task ListenActiveMissionContract(StreamingWebSocketClient client)
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
                            await FollowMissionCompetitionAsync(client, address);
                    }

                }
                await Task.Delay(15000);
            }
        }

        private async Task FollowMissionCompetitionAsync(StreamingWebSocketClient client, string missionContractAddress)
        {
            try
            {
                subcribedContracts.Add(missionContractAddress);

                var subScriptionRegister = new EthLogsObservableSubscription(client);
                var subscriptionCancelRegister = new EthLogsObservableSubscription(client);
                var subscriptionConfirm = new EthLogsObservableSubscription(client);
                var subscriptionUnConfirm = new EthLogsObservableSubscription(client);
                var subscriptionClose = new EthLogsObservableSubscription(client);

                subScriptionRegister.GetSubscriptionDataResponsesAsObservable().
                     Subscribe(async log =>
                     {
                         try
                         {
                             _logger.LogInformation("Catched the Register Mission Event: " + missionContractAddress);
                             EventLog<RegisterEventDTO> decoded = Event<RegisterEventDTO>.DecodeEvent(log);

                             using (var scope = _services.CreateScope())
                             {
                                 var scopedProcessingService = scope.ServiceProvider.GetRequiredService<IMissionService>();
                                 var competition = await scopedProcessingService.UpdateRegisterToDatabase(*//*parameter*//*);
                                 _logger.LogInformation("Store Register Mission successfully with Contract: " + missionContractAddress);
                             }
                         }
                         catch (Exception ex)
                         {
                             _logger.LogError(ex, $"Fail in Store Register Mission: " + missionContractAddress);
                         }
                     });

                subscriptionCancelRegister.GetSubscriptionDataResponsesAsObservable().
                     Subscribe(async log =>
                     {
                         try
                         {
                             _logger.LogInformation("Catch Cancel Register Mission Event: " + missionContractAddress);
                             EventLog<CancelRegisterEventDTO> decoded = Event<CancelRegisterEventDTO>.DecodeEvent(log);
                             using (var scope = _services.CreateScope())
                             {
                                 var scopedProcessingService = scope.ServiceProvider.GetRequiredService<IMissionService>();
                                 var competition = await scopedProcessingService.UpdateCancelRegisterToDabase(*//*parameter*//*);
                                 _logger.LogInformation("Store Cancel Register Mission Event successfully with Contract: " + missionContractAddress);
                             }
                         }
                         catch (Exception ex)
                         {
                             _logger.LogError(ex, $"Fail in Cancel Register Mission Event: " + missionContractAddress);
                         }
                     });

                subscriptionConfirm.GetSubscriptionDataResponsesAsObservable().
                     Subscribe(async log =>
                     {
                         try
                         {
                             _logger.LogInformation("Catch Confirm Mission Event: " + missionContractAddress);
                             EventLog<ConfirmEventDTO> decoded = Event<ConfirmEventDTO>.DecodeEvent(log);

                             using (var scope = _services.CreateScope())
                             {
                                 var scopedProcessingService = scope.ServiceProvider.GetRequiredService<IMissionService>();
                                 var competition = await scopedProcessingService.UpdateConfirmToDatabase(*//*parameter*//*);
                                 _logger.LogInformation("Store Confirm Mission Event successfully with Contract: " + missionContractAddress);
                             }
                         }
                         catch (Exception ex)
                         {
                             _logger.LogError(ex, $"Fail in Stored Confirm Mission: " + missionContractAddress);
                         }
                     });

                subscriptionUnConfirm.GetSubscriptionDataResponsesAsObservable().
                     Subscribe(async log =>
                     {
                         try
                         {
                             _logger.LogInformation("Catch Unconfirm Mission Event: " + missionContractAddress);
                             EventLog<ConfirmEventDTO> decoded = Event<ConfirmEventDTO>.DecodeEvent(log);

                             using (var scope = _services.CreateScope())
                             {
                                 var scopedProcessingService = scope.ServiceProvider.GetRequiredService<IMissionService>();
                                 await scopedProcessingService.UpdateUnconfirmToDatabase(*//*parameter*//*);
                                 _logger.LogInformation("Store Unconfirm Mission successfully with Contract: " + missionContractAddress);
                             }
                         }
                         catch (Exception ex)
                         {
                             _logger.LogError(ex, $"Fail in Stored Unconfirm Mission: " + missionContractAddress);
                         }
                     });

                subscriptionClose.GetSubscriptionDataResponsesAsObservable().
                     Subscribe(async log =>
                     {
                         try
                         {
                             _logger.LogInformation("EventCloseDTO Mission: " + missionContractAddress);
                             EventLog<CloseEventDTO> decoded = Event<CloseEventDTO>.DecodeEvent(log);

                             using (var scope = _services.CreateScope())
                             {
                                 var scopedProcessingService = scope.ServiceProvider.GetRequiredService<IMissionService>();
                                 await scopedProcessingService.CloseMission(*//*parameter*//*);
                                 _logger.LogInformation("Close Mission successfully with Contract: " + missionContractAddress);
                                 await subScriptionRegister.UnsubscribeAsync();
                                 await subscriptionCancelRegister.UnsubscribeAsync();
                                 await subscriptionConfirm.UnsubscribeAsync();
                                 await subscriptionUnConfirm.UnsubscribeAsync();
                                 await subscriptionClose.UnsubscribeAsync();
                             }
                         }
                         catch (Exception ex)
                         {
                             _logger.LogError(ex, $"Fail in subscribe Close Mission event: ");
                         }
                     });

                subScriptionRegister.GetSubscribeResponseAsObservable()
                    .Subscribe(id => _logger.LogInformation($"Subscribed event Register Mission with Contract: {missionContractAddress}"));
                subscriptionCancelRegister.GetSubscribeResponseAsObservable()
                    .Subscribe(id => _logger.LogInformation($"Subscribed event Cancel Register Mission with Contract: {missionContractAddress}"));
                subscriptionConfirm.GetSubscribeResponseAsObservable()
                    .Subscribe(id => _logger.LogInformation($"Subscribed event Confirm Mission with Contract: {missionContractAddress}"));
                subscriptionUnConfirm.GetSubscribeResponseAsObservable()
                    .Subscribe(id => _logger.LogInformation($"Subscribed event Unconfirm Mission with Contract: {missionContractAddress}"));
                subscriptionClose.GetSubscribeResponseAsObservable()
                    .Subscribe(id => _logger.LogInformation($"Subscribed event Close Mission with Contract: {missionContractAddress}"));

                var filterRegister = web3.Eth.GetEvent<RegisterEventDTO>(missionContractAddress).CreateFilterInput();
                var filterCancelRegister = web3.Eth.GetEvent<CancelRegisterEventDTO>(missionContractAddress).CreateFilterInput();
                var filterConfirm = web3.Eth.GetEvent<ConfirmEventDTO>(missionContractAddress).CreateFilterInput();
                var filterUnConfirm = web3.Eth.GetEvent<UnConfirmEventDTO>(missionContractAddress).CreateFilterInput();
                var filterClose = web3.Eth.GetEvent<CloseEventDTO>(missionContractAddress).CreateFilterInput();

                await subScriptionRegister.SubscribeAsync(filterRegister);
                await subscriptionCancelRegister.SubscribeAsync(filterCancelRegister);
                await subscriptionConfirm.SubscribeAsync(filterConfirm);
                await subscriptionUnConfirm.SubscribeAsync(filterUnConfirm);
                await subscriptionClose.SubscribeAsync(filterClose);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Follow Mission Contract Async exception: ");
            }
        }

        private async Task ListenActiveSubjectContract(StreamingWebSocketClient client)
        {
            while (true)
            {
                using (var scope = _services.CreateScope())
                {
                    var scopedProcessingService = scope.ServiceProvider.GetRequiredService<ISubjectService>();
                    //GetListMissionInProgress goi vao db de lay ra dangh sach cac contract tuition address
                    var listAddresses = await scopedProcessingService.GetListSubjectInProgress(chainNetworkId);
                    foreach (var address in listAddresses)
                    {
                        if (!subcribedContracts.Contains(address))
                            await FollowSubjectCompetitionAsync(client, address);
                    }
                }
                await Task.Delay(15000);
            }
        }

        private async Task FollowSubjectCompetitionAsync(StreamingWebSocketClient client, string contractAddress)
        {
            try
            {
                subcribedContracts.Add(contractAddress);

                var subScriptionRegister = new EthLogsObservableSubscription(client);
                var subscriptionCancelRegister = new EthLogsObservableSubscription(client);
                var subscriptionConfirm = new EthLogsObservableSubscription(client);
                var subscriptionUnConfirm = new EthLogsObservableSubscription(client);
                var subscriptionClose = new EthLogsObservableSubscription(client);

                subScriptionRegister.GetSubscriptionDataResponsesAsObservable().
                     Subscribe(async log =>
                     {
                         try
                         {
                             _logger.LogInformation("Catched the Register Subject Event: " + contractAddress);
                             EventLog<RegisterEventDTO> decoded = Event<RegisterEventDTO>.DecodeEvent(log);

                             using (var scope = _services.CreateScope())
                             {
                                 var scopedProcessingService = scope.ServiceProvider.GetRequiredService<ISubjectService>();
                                 var competition = await scopedProcessingService.UpdateRegisterToDatabase(*//*parameter*//*);
                                 _logger.LogInformation("Store Register Subject successfully with Contract: " + contractAddress);
                             }
                         }
                         catch (Exception ex)
                         {
                             _logger.LogError(ex, $"Fail in Store Register Subject: " + contractAddress);
                         }
                     });

                subscriptionCancelRegister.GetSubscriptionDataResponsesAsObservable().
                     Subscribe(async log =>
                     {
                         try
                         {
                             _logger.LogInformation("Catch Cancel Register Subject Event: " + contractAddress);
                             EventLog<CancelRegisterEventDTO> decoded = Event<CancelRegisterEventDTO>.DecodeEvent(log);
                             using (var scope = _services.CreateScope())
                             {
                                 var scopedProcessingService = scope.ServiceProvider.GetRequiredService<ISubjectService>();
                                 var competition = await scopedProcessingService.UpdateCancelRegisterToDabase(*//*parameter*//*);
                                 _logger.LogInformation("Store Cancel Register Subject Event successfully with Contract: " + contractAddress);
                             }
                         }
                         catch (Exception ex)
                         {
                             _logger.LogError(ex, $"Fail in Cancel Register Subject Event: " + contractAddress);
                         }
                     });

                subscriptionConfirm.GetSubscriptionDataResponsesAsObservable().
                     Subscribe(async log =>
                     {
                         try
                         {
                             _logger.LogInformation("Catch Confirm Subject Event: " + contractAddress);
                             EventLog<ConfirmEventDTO> decoded = Event<ConfirmEventDTO>.DecodeEvent(log);

                             using (var scope = _services.CreateScope())
                             {
                                 var scopedProcessingService = scope.ServiceProvider.GetRequiredService<ISubjectService>();
                                 var competition = await scopedProcessingService.UpdateConfirmToDatabase(*//*parameter*//*);
                                 _logger.LogInformation("Store Confirm Subject Event successfully with Contract: " + contractAddress);
                             }
                         }
                         catch (Exception ex)
                         {
                             _logger.LogError(ex, $"Fail in Stored Confirm Subject: " + contractAddress);
                         }
                     });

                subscriptionUnConfirm.GetSubscriptionDataResponsesAsObservable().
                     Subscribe(async log =>
                     {
                         try
                         {
                             _logger.LogInformation("Catch Unconfirm Subject Event: " + contractAddress);
                             EventLog<ConfirmEventDTO> decoded = Event<ConfirmEventDTO>.DecodeEvent(log);

                             using (var scope = _services.CreateScope())
                             {
                                 var scopedProcessingService = scope.ServiceProvider.GetRequiredService<ISubjectService>();
                                 await scopedProcessingService.UpdateUnconfirmToDatabase(*//*parameter*//*);
                                 _logger.LogInformation("Store Unconfirm Subject successfully with Contract: " + contractAddress);
                             }
                         }
                         catch (Exception ex)
                         {
                             _logger.LogError(ex, $"Fail in Stored Unconfirm Subject: " + contractAddress);
                         }
                     });

                subscriptionClose.GetSubscriptionDataResponsesAsObservable().
                     Subscribe(async log =>
                     {
                         try
                         {
                             _logger.LogInformation("EventCloseDTO Subject: " + contractAddress);
                             EventLog<CloseEventDTO> decoded = Event<CloseEventDTO>.DecodeEvent(log);

                             using (var scope = _services.CreateScope())
                             {
                                 var scopedProcessingService = scope.ServiceProvider.GetRequiredService<ISubjectService>();
                                 await scopedProcessingService.CloseMission(*//*parameter*//*);
                                 _logger.LogInformation("Close Subject successfully with Contract: " + contractAddress);
                                 await subScriptionRegister.UnsubscribeAsync();
                                 await subscriptionCancelRegister.UnsubscribeAsync();
                                 await subscriptionConfirm.UnsubscribeAsync();
                                 await subscriptionUnConfirm.UnsubscribeAsync();
                                 await subscriptionClose.UnsubscribeAsync();
                             }
                         }
                         catch (Exception ex)
                         {
                             _logger.LogError(ex, $"Fail in subscribe Close Subject event: ");
                         }
                     });

                subScriptionRegister.GetSubscribeResponseAsObservable()
                    .Subscribe(id => _logger.LogInformation($"Subscribed event Register Subject with Contract: {contractAddress}"));
                subscriptionCancelRegister.GetSubscribeResponseAsObservable()
                    .Subscribe(id => _logger.LogInformation($"Subscribed event Cancel Register Subject with Contract: {contractAddress}"));
                subscriptionConfirm.GetSubscribeResponseAsObservable()
                    .Subscribe(id => _logger.LogInformation($"Subscribed event Confirm Subject with Contract: {contractAddress}"));
                subscriptionUnConfirm.GetSubscribeResponseAsObservable()
                    .Subscribe(id => _logger.LogInformation($"Subscribed event Unconfirm Subject with Contract: {contractAddress}"));
                subscriptionClose.GetSubscribeResponseAsObservable()
                    .Subscribe(id => _logger.LogInformation($"Subscribed event Close Subject with Contract: {contractAddress}"));

                var filterRegister = web3.Eth.GetEvent<RegisterEventDTO>(contractAddress).CreateFilterInput();
                var filterCancelRegister = web3.Eth.GetEvent<CancelRegisterEventDTO>(contractAddress).CreateFilterInput();
                var filterConfirm = web3.Eth.GetEvent<ConfirmEventDTO>(contractAddress).CreateFilterInput();
                var filterUnConfirm = web3.Eth.GetEvent<UnConfirmEventDTO>(contractAddress).CreateFilterInput();
                var filterClose = web3.Eth.GetEvent<CloseEventDTO>(contractAddress).CreateFilterInput();

                await subScriptionRegister.SubscribeAsync(filterRegister);
                await subscriptionCancelRegister.SubscribeAsync(filterCancelRegister);
                await subscriptionConfirm.SubscribeAsync(filterConfirm);
                await subscriptionUnConfirm.SubscribeAsync(filterUnConfirm);
                await subscriptionClose.SubscribeAsync(filterClose);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Follow Subject Contract Async exception: ");
            }
        }

        private async Task ListenActiveScholarshipContract(StreamingWebSocketClient client)
        {
            while (true)
            {
                using (var scope = _services.CreateScope())
                {
                    var scopedProcessingService = scope.ServiceProvider.GetRequiredService<IScholarshipService>();
                    //GetListMissionInProgress goi vao db de lay ra dangh sach cac contract tuition address
                    var listAddresses = await scopedProcessingService.GetListScholarshipInProgress(chainNetworkId);
                    foreach (var address in listAddresses)
                    {
                        if (!subcribedContracts.Contains(address))
                            await FollowScholarshipCompetitionAsync(client, address);
                    }
                }
                await Task.Delay(15000);
            }
        }

        private async Task FollowScholarshipCompetitionAsync(StreamingWebSocketClient client, string contractAddress)
        {
            try
            {
                subcribedContracts.Add(contractAddress);

                var subScriptionAddStudentToScholarship = new EthLogsObservableSubscription(client);
                var subscriptionRemoveStudentFromScholarship = new EthLogsObservableSubscription(client);
                var subscriptionClose = new EthLogsObservableSubscription(client);

                subScriptionAddStudentToScholarship.GetSubscriptionDataResponsesAsObservable().
                     Subscribe(async log =>
                     {
                         try
                         {
                             _logger.LogInformation("Catched the Add Student To Scholarship Event: " + contractAddress);
                             EventLog<AddStudentToScholarshipEventDTO> decoded = Event<AddStudentToScholarshipEventDTO>.DecodeEvent(log);

                             using (var scope = _services.CreateScope())
                             {
                                 var scopedProcessingService = scope.ServiceProvider.GetRequiredService<IScholarshipService>();
                                 var competition = await scopedProcessingService.UpdateRegisterToDatabase(*//*parameter*//*);
                                 _logger.LogInformation("Store Add Student To Scholarship Event successfully with Contract: " + contractAddress);
                             }
                         }
                         catch (Exception ex)
                         {
                             _logger.LogError(ex, $"Fail in Store Add Student To Scholarship Event: " + contractAddress);
                         }
                     });

                subscriptionRemoveStudentFromScholarship.GetSubscriptionDataResponsesAsObservable().
                     Subscribe(async log =>
                     {
                         try
                         {
                             _logger.LogInformation("Catch Remove Student From Scholarship Event: " + contractAddress);
                             EventLog<RemoveStudentFromScholarshipEventDTO> decoded = Event<RemoveStudentFromScholarshipEventDTO>.DecodeEvent(log);
                             using (var scope = _services.CreateScope())
                             {
                                 var scopedProcessingService = scope.ServiceProvider.GetRequiredService<IScholarshipService>();
                                 var competition = await scopedProcessingService.UpdateCancelRegisterToDabase(*//*parameter*//*);
                                 _logger.LogInformation("Store Remove Student From Scholarship Event successfully with Contract: " + contractAddress);
                             }
                         }
                         catch (Exception ex)
                         {
                             _logger.LogError(ex, $"Fail in Remove Student From Scholarship Event: " + contractAddress);
                         }
                     });

                subscriptionClose.GetSubscriptionDataResponsesAsObservable().
                     Subscribe(async log =>
                     {
                         try
                         {
                             _logger.LogInformation("EventCloseDTO Scholarship: " + contractAddress);
                             EventLog<CloseEventDTO> decoded = Event<CloseEventDTO>.DecodeEvent(log);

                             using (var scope = _services.CreateScope())
                             {
                                 var scopedProcessingService = scope.ServiceProvider.GetRequiredService<IScholarshipService>();
                                 await scopedProcessingService.CloseScholarship(*//*parameter*//*);
                                 _logger.LogInformation("Close Subject successfully with Contract: " + contractAddress);
                                 await subScriptionAddStudentToScholarship.UnsubscribeAsync();
                                 await subscriptionRemoveStudentFromScholarship.UnsubscribeAsync();
                                 await subscriptionClose.UnsubscribeAsync();
                             }
                         }
                         catch (Exception ex)
                         {
                             _logger.LogError(ex, $"Fail in subscribe Close Scholarship event: ");
                         }
                     });

                subScriptionAddStudentToScholarship.GetSubscribeResponseAsObservable()
                    .Subscribe(id => _logger.LogInformation($"Subscribed event Add Student To Scholarship with Contract: {contractAddress}"));
                subscriptionRemoveStudentFromScholarship.GetSubscribeResponseAsObservable()
                    .Subscribe(id => _logger.LogInformation($"Subscribed event Remove Student To Scholarship with Contract: {contractAddress}"));
                subscriptionClose.GetSubscribeResponseAsObservable()
                    .Subscribe(id => _logger.LogInformation($"Subscribed event Close Scholarship with Contract: {contractAddress}"));

                var filterRegister = web3.Eth.GetEvent<AddStudentToScholarshipEventDTO>(contractAddress).CreateFilterInput();
                var filterCancelRegister = web3.Eth.GetEvent<RemoveStudentFromScholarshipEventDTO>(contractAddress).CreateFilterInput();
                var filterClose = web3.Eth.GetEvent<CloseEventDTO>(contractAddress).CreateFilterInput();

                await subScriptionAddStudentToScholarship.SubscribeAsync(filterRegister);
                await subscriptionRemoveStudentFromScholarship.SubscribeAsync(filterCancelRegister);
                await subscriptionClose.SubscribeAsync(filterClose);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Follow Scholarship Contract Async exception: ");
            }
        }

        private async Task ListenActiveTuitionContract(StreamingWebSocketClient client)
        {
            while (true)
            {
                using (var scope = _services.CreateScope())
                {
                    var scopedProcessingService = scope.ServiceProvider.GetRequiredService<ITuitionService>();
                    //GetListMissionInProgress goi vao db de lay ra dangh sach cac contract tuition address
                    var listAddresses = await scopedProcessingService.GetListTuitionInProgress(chainNetworkId);
                    foreach (var address in listAddresses)
                    {
                        if (!subcribedContracts.Contains(address))
                            await FollowTuitionCompetitionAsync(client, address);
                    }
                }
                await Task.Delay(15000);
            }
        }

        private async Task FollowTuitionCompetitionAsync(StreamingWebSocketClient client, string contractAddress)
        {
            try
            {
                subcribedContracts.Add(contractAddress);

                var subScriptionAddStudentToTuition = new EthLogsObservableSubscription(client);
                var subscriptionRemoveStudentFromTuition = new EthLogsObservableSubscription(client);
                var subscriptionPayment = new EthLogsObservableSubscription(client);
                var subscriptionClose = new EthLogsObservableSubscription(client);

                subScriptionAddStudentToTuition.GetSubscriptionDataResponsesAsObservable().
                     Subscribe(async log =>
                     {
                         try
                         {
                             _logger.LogInformation("Catched the Add Student To Tuition Event: " + contractAddress);
                             EventLog<AddStudentToTuitionEventDTO> decoded = Event<AddStudentToTuitionEventDTO>.DecodeEvent(log);

                             using (var scope = _services.CreateScope())
                             {
                                 var scopedProcessingService = scope.ServiceProvider.GetRequiredService<ITuitionService>();
                                 var competition = await scopedProcessingService.UpdateRegisterToDatabase(*//*parameter*//*);
                                 _logger.LogInformation("Store Add Student To Tuition Event successfully with Contract: " + contractAddress);
                             }
                         }
                         catch (Exception ex)
                         {
                             _logger.LogError(ex, $"Fail in Store Add Student To Tuition Event: " + contractAddress);
                         }
                     });

                subscriptionRemoveStudentFromTuition.GetSubscriptionDataResponsesAsObservable().
                     Subscribe(async log =>
                     {
                         try
                         {
                             _logger.LogInformation("Catch Remove Student From Tuition Event: " + contractAddress);
                             EventLog<RemoveStudentFromTuitionEventDTO> decoded = Event<RemoveStudentFromTuitionEventDTO>.DecodeEvent(log);
                             using (var scope = _services.CreateScope())
                             {
                                 var scopedProcessingService = scope.ServiceProvider.GetRequiredService<ITuitionService>();
                                 var competition = await scopedProcessingService.UpdateCancelRegisterToDabase(*//*parameter*//*);
                                 _logger.LogInformation("Store Remove Student From Tuition Event successfully with Contract: " + contractAddress);
                             }
                         }
                         catch (Exception ex)
                         {
                             _logger.LogError(ex, $"Fail in Remove Student From Tuition Event: " + contractAddress);
                         }
                     });

                subscriptionPayment.GetSubscriptionDataResponsesAsObservable().
                     Subscribe(async log =>
                     {
                         try
                         {
                             _logger.LogInformation("Catch Payment Tuition Event: " + contractAddress);
                             EventLog<PaymentEventDTO> decoded = Event<PaymentEventDTO>.DecodeEvent(log);
                             using (var scope = _services.CreateScope())
                             {
                                 var scopedProcessingService = scope.ServiceProvider.GetRequiredService<ITuitionService>();
                                 var competition = await scopedProcessingService.UpdateCancelRegisterToDabase(*//*parameter*//*);
                                 _logger.LogInformation("Store Payment Tuition Event successfully with Contract: " + contractAddress);
                             }
                         }
                         catch (Exception ex)
                         {
                             _logger.LogError(ex, $"Fail in Payment Tuition Event: " + contractAddress);
                         }
                     });

                subscriptionClose.GetSubscriptionDataResponsesAsObservable().
                     Subscribe(async log =>
                     {
                         try
                         {
                             _logger.LogInformation("EventCloseDTO Tuition: " + contractAddress);
                             EventLog<CloseEventDTO> decoded = Event<CloseEventDTO>.DecodeEvent(log);

                             using (var scope = _services.CreateScope())
                             {
                                 var scopedProcessingService = scope.ServiceProvider.GetRequiredService<ITuitionService>();
                                 await scopedProcessingService.CloseTuition(*//*parameter*//*);
                                 _logger.LogInformation("Close Tuition successfully with Contract: " + contractAddress);
                                 await subScriptionAddStudentToTuition.UnsubscribeAsync();
                                 await subscriptionRemoveStudentFromTuition.UnsubscribeAsync();
                                 await subscriptionPayment.UnsubscribeAsync();
                                 await subscriptionClose.UnsubscribeAsync();
                             }
                         }
                         catch (Exception ex)
                         {
                             _logger.LogError(ex, $"Fail in subscribe Close Tuition event: ");
                         }
                     });

                subScriptionAddStudentToTuition.GetSubscribeResponseAsObservable()
                    .Subscribe(id => _logger.LogInformation($"Subscribed event Add Student To Tuition with Contract: {contractAddress}"));
                subscriptionRemoveStudentFromTuition.GetSubscribeResponseAsObservable()
                    .Subscribe(id => _logger.LogInformation($"Subscribed event Remove Student To Tuition with Contract: {contractAddress}"));
                subscriptionPayment.GetSubscribeResponseAsObservable()
                    .Subscribe(id => _logger.LogInformation($"Subscribed event Payment Tuition with Contract: {contractAddress}"));
                subscriptionClose.GetSubscribeResponseAsObservable()
                    .Subscribe(id => _logger.LogInformation($"Subscribed event Close Tuition with Contract: {contractAddress}"));

                var filterRegister = web3.Eth.GetEvent<AddStudentToTuitionEventDTO>(contractAddress).CreateFilterInput();
                var filterCancelRegister = web3.Eth.GetEvent<RemoveStudentFromTuitionEventDTO>(contractAddress).CreateFilterInput();
                var filterPayment = web3.Eth.GetEvent<PaymentEventDTO>(contractAddress).CreateFilterInput();
                var filterClose = web3.Eth.GetEvent<CloseEventDTO>(contractAddress).CreateFilterInput();

                await subScriptionAddStudentToTuition.SubscribeAsync(filterRegister);
                await subscriptionRemoveStudentFromTuition.SubscribeAsync(filterCancelRegister);
                await subscriptionPayment.SubscribeAsync(filterPayment);
                await subscriptionClose.SubscribeAsync(filterClose);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Follow Tuition Contract Async exception: ");
            }
        }*/
    }
}
