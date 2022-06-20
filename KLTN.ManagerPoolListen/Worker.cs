using KLTN.Common.Exceptions;
using KLTN.Common.Models;
using KLTN.Common.SmartContracts.Events;
using KLTN.Common.SmartContracts.Functions.MissionContract;
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
using Nethereum.RPC.Eth.DTOs;
using Nethereum.RPC.Reactive.Eth;
using Nethereum.RPC.Reactive.Eth.Subscriptions;
using Nethereum.Web3.Accounts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace KLTN.ManagerPoolListen
{
    public class Worker : BackgroundService
    {
        //private readonly IMongoDbContext _context;
        //private readonly IMongoCollection<SubcribedContractsListenEvent> _subcribedContractsListenEvent;

        private readonly IServiceScopeFactory _services;
        private readonly ILogger<Worker> _logger;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        Nethereum.Web3.Web3 _web3 = new Nethereum.Web3.Web3();
        string managerPoolAddress = ListenMangerPoolAppSettings.Value.ManagerPoolContractAddress;
        int chainNetworkId = ListenMangerPoolAppSettings.Value.ChainNetworkId;

        List<string> subcribedContracts = new List<string>();

        public Worker(ILogger<Worker> logger, IServiceScopeFactory serviceScopeFactory, IHostApplicationLifetime hostApplicationLifetime)
        {
            _logger = logger;
            _services = serviceScopeFactory;
            _hostApplicationLifetime = hostApplicationLifetime;
            //_context = context;
            var _account = new Account(ListenMangerPoolAppSettings.Value.PrivateKey, chainNetworkId);
            _web3 = new Nethereum.Web3.Web3(_account, ListenMangerPoolAppSettings.Value.RpcUrl);
            _web3.TransactionManager.UseLegacyAsDefault = true;
            //_subcribedContractsListenEvent = _context.GetCollection<SubcribedContractsListenEvent>(typeof(SubcribedContractsListenEvent).Name);
            //var subcribedContractsList = _subcribedContractsListenEvent.Find<SubcribedContractsListenEvent>(_ => true).ToList();
            /*foreach (var subcribedContract in subcribedContractsList)
                subcribedContracts.Add(subcribedContract.SubcribedContracts);*/

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
                    await ListenMissionLocked(client);
                    await ListenScholarshipLocked(client);
                    await ListenSubjectLocked(client);
                    await ListenTuitionLocked(client);
                    await ListenStudentRoleRevoked(client);
                    await ListenLecturerRoleRevoked(client);
                    _ = ListenActiveMissionContract(client);
                    _ = ListenActiveSubjectContract(client);
                    _ = ListenActiveScholarshipContract(client);
                    _ = ListenActiveTuitionContract(client);
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

        private async Task<Transaction> GetTransactionInput(string transactionHash)
        {
            Nethereum.RPC.Eth.DTOs.Transaction myFunctionTxn = null;
            var callTimes = 0;
            while (myFunctionTxn == null && callTimes < 5)
            {
                try
                {
                    myFunctionTxn = await _web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync(transactionHash);
                }
                catch (Exception)
                {
                    await Task.Delay(2000);
                }
                callTimes++;
            }
            if (myFunctionTxn == null)
                throw new CustomException("Transaction info is null", 400);

            return myFunctionTxn;
        }

        private async Task ListenNewTuitionEvent(StreamingWebSocketClient client)
        {
            var filter = _web3.Eth.GetEvent<NewTuitionEventDTO>(managerPoolAddress).CreateFilterInput();
            var subscription = new EthLogsObservableSubscription(client);
            subscription.GetSubscriptionDataResponsesAsObservable().
                         Subscribe(async log =>
                         {
                             EventLog<NewTuitionEventDTO> decoded = Event<NewTuitionEventDTO>.DecodeEvent(log);
                             _logger.LogInformation($"Listening Add New Tuition {decoded.Event.ContractAddress}");
                             string jsonResponse = await RequestIPFS(decoded.Event.UrlMetadata);
                             var tuitionMetadata = JsonConvert.DeserializeObject<TuitionMetadataDTO>(jsonResponse);
                             using (var scope = _services.CreateScope())
                             {
                                 var scopedProcessingService = scope.ServiceProvider.GetRequiredService<ITuitionService>();
                                 await scopedProcessingService.CreateNewTuition(new TuitionDTO
                                 {
                                     ChainNetworkId = chainNetworkId,
                                     Img = tuitionMetadata.Img,
                                     TuitionId = tuitionMetadata.TuitionId,
                                     TuitionName = tuitionMetadata.Name,
                                     TuitionAddress = decoded.Event.ContractAddress,
                                     TuitionDescription = tuitionMetadata.Description,
                                     TuitionHashIPFS = decoded.Event.UrlMetadata,
                                     SchoolYear = DateTime.Now.Year,
                                     StartTime = tuitionMetadata.StartTime,
                                     EndTime = tuitionMetadata.EndTime,
                                     TokenAmount = decimal.Parse(tuitionMetadata.AmountToken),
                                     CurrencyAmount = decimal.Parse(tuitionMetadata.AmountCurency),
                                     LecturerInCharge = tuitionMetadata.LecturerInCharge,
                                     LecturerName = tuitionMetadata.LecturerName,
                                 });
                                 await FollowTuitionCompetitionAsync(client, decoded.Event.ContractAddress);
                             }
                             _logger.LogInformation($"Stored New Tuition {decoded.Event.ContractAddress}");
                         });

            subscription.GetSubscribeResponseAsObservable().Subscribe(id => _logger.LogInformation($"Subscribed Add New Tuition Event - {DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")}"));
            await subscription.SubscribeAsync(filter);
        }

        private async Task ListenNewScholarshipEvent(StreamingWebSocketClient client)
        {
            var filter = _web3.Eth.GetEvent<NewScholarshipEventDTO>(managerPoolAddress).CreateFilterInput();
            var subscription = new EthLogsObservableSubscription(client);
            subscription.GetSubscriptionDataResponsesAsObservable().
                         Subscribe(async log =>
                         {
                             EventLog<NewScholarshipEventDTO> decoded = Event<NewScholarshipEventDTO>.DecodeEvent(log);
                             _logger.LogInformation($"Listening Add New Scholarship {decoded.Event.ContractAddress}");
                             string jsonResponse = await RequestIPFS(decoded.Event.UrlMetadata);
                             var metadata = JsonConvert.DeserializeObject<ScholarshipMetadataDTO>(jsonResponse);
                             using (var scope = _services.CreateScope())
                             {
                                 var scopedProcessingService = scope.ServiceProvider.GetRequiredService<IScholarshipService>();
                                 await scopedProcessingService.CreateNewScholarship(new Core.ScholarshipServices.DTOs.ScholarshipDTO
                                 {
                                     ChainNetworkId = chainNetworkId,
                                     ScholarshipId = metadata.ScholarshipId,
                                     ScholarshipImg = metadata.Img,
                                     ScholarshipName = metadata.Name,
                                     ScholarshipAddress = decoded.Event.ContractAddress,
                                     ScholarShipDescription = metadata.Description,
                                     ScholarshipHashIPFS = decoded.Event.UrlMetadata,
                                     StartTime = metadata.StartTime,
                                     EndTime = metadata.EndTime,
                                     EndTimeToResigter = metadata.EndTimeToRegister,
                                     EndTimeToComFirm = metadata.EndTimeToConfirm,
                                     TokenAmount = decimal.Parse(metadata.Award),
                                     LecturerInCharge = metadata.LecturerInCharge,
                                     LecturerName = metadata.LecturerName,
                                 });
                                 await FollowScholarshipCompetitionAsync(client, decoded.Event.ContractAddress);
                             }
                             _logger.LogInformation($"Stored New Scholarship {decoded.Event.ContractAddress}");
                         });

            subscription.GetSubscribeResponseAsObservable().Subscribe(id => _logger.LogInformation($"Subscribed Add New Scholarship Event - {DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")}"));
            await subscription.SubscribeAsync(filter);
        }

        private async Task ListenNewSubjectEvent(StreamingWebSocketClient client)
        {
            var filter = _web3.Eth.GetEvent<NewSubjectEventDTO>(managerPoolAddress).CreateFilterInput();
            var subscription = new EthLogsObservableSubscription(client);
            subscription.GetSubscriptionDataResponsesAsObservable().
                         Subscribe(async log =>
                         {
                             EventLog<NewSubjectEventDTO> decoded = Event<NewSubjectEventDTO>.DecodeEvent(log);
                             _logger.LogInformation($"Listening Add New Subject {decoded.Event.ContractAddress}");
                             string jsonResponse = await RequestIPFS(decoded.Event.UrlMetadata);
                             var metadata = JsonConvert.DeserializeObject<SubjectMetadataDTO>(jsonResponse);
                             using (var scope = _services.CreateScope())
                             {
                                 var scopedProcessingService = scope.ServiceProvider.GetRequiredService<ISubjectService>();
                                 await scopedProcessingService.CreateNewSubject(new Core.SubjectServices.DTOs.SubjectDTO
                                 {
                                     ChainNetworkId = chainNetworkId,
                                     SubjectId = metadata.ClassId,
                                     SubjectName = metadata.Name,
                                     SubjectAddress = decoded.Event.ContractAddress,
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
                                     LecturerAddress = metadata.LecturerInCharge,
                                     LecturerName = metadata.LecturerName,
                                 });
                                 await FollowSubjectCompetitionAsync(client, decoded.Event.ContractAddress);
                             }
                             _logger.LogInformation($"Stored New Subject {decoded.Event.ContractAddress}");
                         });

            subscription.GetSubscribeResponseAsObservable().Subscribe(id => _logger.LogInformation($"Subscribed Add New Subject Event - {DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")}"));
            await subscription.SubscribeAsync(filter);
        }

        private async Task ListenNewMissionEvent(StreamingWebSocketClient client)
        {
            var filter = _web3.Eth.GetEvent<NewMissionEventDTO>(managerPoolAddress).CreateFilterInput();
            var subscription = new EthLogsObservableSubscription(client);
            subscription.GetSubscriptionDataResponsesAsObservable().
                         Subscribe(async log =>
                         {
                             EventLog<NewMissionEventDTO> decoded = Event<NewMissionEventDTO>.DecodeEvent(log);
                             _logger.LogInformation($"Listening Add New Mission {decoded.Event.ContractAddress}");
                             string jsonResponse = await RequestIPFS(decoded.Event.UrlMetadata);
                             var metadata = JsonConvert.DeserializeObject<MissionMetadataDTO>(jsonResponse);
                             using (var scope = _services.CreateScope())
                             {
                                 var scopedProcessingService = scope.ServiceProvider.GetRequiredService<IMissionService>();
                                 await scopedProcessingService.CreateNewMission(new Core.MissionServices.DTOs.MissionDTO
                                 {
                                     ChainNetworkId = chainNetworkId,
                                     MissionId = metadata.MissionId,
                                     MissionName = metadata.Name,
                                     MissionAddress = decoded.Event.ContractAddress,
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
                                     TokenAmount = decimal.Parse(metadata.Award),
                                     LecturerName = metadata.LecturerName,
                                 });
                                 await FollowMissionCompetitionAsync(client, decoded.Event.ContractAddress);
                             }
                             _logger.LogInformation($"Stored New Mission {decoded.Event.ContractAddress}");
                         });

            subscription.GetSubscribeResponseAsObservable().Subscribe(id => _logger.LogInformation($"Subscribed Add New Mission Event - {DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")}"));
            await subscription.SubscribeAsync(filter);
        }

        private async Task ListenAddLecturerEvent(StreamingWebSocketClient client)
        {
            var filter = _web3.Eth.GetEvent<AddLecturerInfoEventDTO>(managerPoolAddress).CreateFilterInput();
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
                                     LecturerImg = metadata.Img,
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
            Console.WriteLine(managerPoolAddress);
            var filter = _web3.Eth.GetEvent<AddStudentInfoEventDTO>(managerPoolAddress).CreateFilterInput();
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
                                     StudentImg = metadata.ImgUrl,
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
            var filter = _web3.Eth.GetEvent<UpdateStudentInfoEventDTO>(managerPoolAddress).CreateFilterInput();
            var subscription = new EthLogsObservableSubscription(client);
            subscription.GetSubscriptionDataResponsesAsObservable().
                         Subscribe(async log =>
                         {
                             EventLog<UpdateStudentInfoEventDTO> decoded = Event<UpdateStudentInfoEventDTO>.DecodeEvent(log);
                             string jsonResponse = await RequestIPFS(decoded.Event.HashInfo);
                             var metadata = JsonConvert.DeserializeObject<StudentUpdateMetadataDTO>(jsonResponse);
                             using (var scope = _services.CreateScope())
                             {
                                 var scopedProcessingService = scope.ServiceProvider.GetRequiredService<IStudentService>();
                                 await scopedProcessingService.UpdateStudentIntoDatabase(decoded.Event.StudentAddr, new Core.StudentServices.DTOs.StudentUpdateDTO
                                 {
                                     StudentName = metadata.Name,
                                     StudentImg = metadata.ImgUrl,
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
                             };
                             _logger.LogInformation($"Listening Update Student Info {decoded.Event.StudentAddr}");
                         });

            subscription.GetSubscribeResponseAsObservable().Subscribe(id => _logger.LogInformation($"Subscribed Update Info Event - {DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")}"));
            await subscription.SubscribeAsync(filter);
        }

        private async Task ListenMissionLocked(StreamingWebSocketClient client)
        {
            var filter = _web3.Eth.GetEvent<MissionLockedEventDTO>(managerPoolAddress).CreateFilterInput();
            var subscription = new EthLogsObservableSubscription(client);
            subscription.GetSubscriptionDataResponsesAsObservable().
                Subscribe(async log =>
                {
                    EventLog<MissionLockedEventDTO> decoded = Event<MissionLockedEventDTO>.DecodeEvent(log);
                    using (var scope = _services.CreateScope())
                    {
                        var scopedProcessingService = scope.ServiceProvider.GetRequiredService<IMissionService>();
                        await scopedProcessingService.LockMission(decoded.Event.MissionAddrs);

                        await subscription.UnsubscribeAsync();
                        foreach (var address in decoded.Event.MissionAddrs)
                            subcribedContracts.Remove(address);
                    };
                    _logger.LogInformation($"Listening Locked Mission: {decoded.Event.MissionAddrs}");
                });

            subscription.GetSubscribeResponseAsObservable().Subscribe(id => _logger.LogInformation($"Subscribed ListenMissionLocked Event - {DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")}"));
            await subscription.SubscribeAsync(filter);
        }

        private async Task ListenSubjectLocked(StreamingWebSocketClient client)
        {
            var filter = _web3.Eth.GetEvent<SubjectLockedEventDTO>(managerPoolAddress).CreateFilterInput();
            var subscription = new EthLogsObservableSubscription(client);
            subscription.GetSubscriptionDataResponsesAsObservable().
                Subscribe(async log =>
                {
                    EventLog<SubjectLockedEventDTO> decoded = Event<SubjectLockedEventDTO>.DecodeEvent(log);
                    using (var scope = _services.CreateScope())
                    {
                        var scopedProcessingService = scope.ServiceProvider.GetRequiredService<ISubjectService>();
                        await scopedProcessingService.LockSubject(decoded.Event.SubjectAddrs);

                        await subscription.UnsubscribeAsync();
                        foreach (var address in decoded.Event.SubjectAddrs)
                            subcribedContracts.Remove(address);
                    };
                    _logger.LogInformation($"Listening Locked Subject: {decoded.Event.SubjectAddrs}");
                });

            subscription.GetSubscribeResponseAsObservable().Subscribe(id => _logger.LogInformation($"Subscribed ListenSubjectLocked Event - {DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")}"));
            await subscription.SubscribeAsync(filter);
        }

        private async Task ListenScholarshipLocked(StreamingWebSocketClient client)
        {
            var filter = _web3.Eth.GetEvent<ScholarshipLockedEventDTO>(managerPoolAddress).CreateFilterInput();
            var subscription = new EthLogsObservableSubscription(client);
            subscription.GetSubscriptionDataResponsesAsObservable().
                Subscribe(async log =>
                {
                    EventLog<ScholarshipLockedEventDTO> decoded = Event<ScholarshipLockedEventDTO>.DecodeEvent(log);
                    using (var scope = _services.CreateScope())
                    {
                        var scopedProcessingService = scope.ServiceProvider.GetRequiredService<IScholarshipService>();
                        await scopedProcessingService.LockScholarship(decoded.Event.ScholarshipAddrs);

                        await subscription.UnsubscribeAsync();
                        foreach (var address in decoded.Event.ScholarshipAddrs)
                            subcribedContracts.Remove(address);
                    };
                    _logger.LogInformation($"Listening Locked Scholarship: {decoded.Event.ScholarshipAddrs}");
                });

            subscription.GetSubscribeResponseAsObservable().Subscribe(id => _logger.LogInformation($"Subscribed ListenScholarshipLocked Event - {DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")}"));
            await subscription.SubscribeAsync(filter);
        }

        private async Task ListenTuitionLocked(StreamingWebSocketClient client)
        {
            var filter = _web3.Eth.GetEvent<TuitionLockedEventDTO>(managerPoolAddress).CreateFilterInput();
            var subscription = new EthLogsObservableSubscription(client);
            subscription.GetSubscriptionDataResponsesAsObservable().
                Subscribe(async log =>
                {
                    EventLog<TuitionLockedEventDTO> decoded = Event<TuitionLockedEventDTO>.DecodeEvent(log);
                    using (var scope = _services.CreateScope())
                    {
                        var scopedProcessingService = scope.ServiceProvider.GetRequiredService<ITuitionService>();
                        await scopedProcessingService.LockTuition(decoded.Event.TuitionAddrs);

                        await subscription.UnsubscribeAsync();
                        foreach (var address in decoded.Event.TuitionAddrs)
                            subcribedContracts.Remove(address);
                    };
                    _logger.LogInformation($"Listening Locked Tuition: {decoded.Event.TuitionAddrs}");
                });

            subscription.GetSubscribeResponseAsObservable().Subscribe(id => _logger.LogInformation($"Subscribed ListenTuitionLocked Event - {DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")}"));
            await subscription.SubscribeAsync(filter);
        }

        private async Task ListenStudentRoleRevoked(StreamingWebSocketClient client)
        {
            var filter = _web3.Eth.GetEvent<StudentRoleRevokedEventDTO>(managerPoolAddress).CreateFilterInput();
            var subscription = new EthLogsObservableSubscription(client);
            subscription.GetSubscriptionDataResponsesAsObservable().
                Subscribe(async log =>
                {
                    EventLog<StudentRoleRevokedEventDTO> decoded = Event<StudentRoleRevokedEventDTO>.DecodeEvent(log);
                    using (var scope = _services.CreateScope())
                    {
                        var scopedProcessingService = scope.ServiceProvider.GetRequiredService<IStudentService>();
                        await scopedProcessingService.RevokeStudentRole(decoded.Event.StudentAddrs);

                        await subscription.UnsubscribeAsync();
                    };
                    _logger.LogInformation($"Listening Revoke Student Role: {decoded.Event.StudentAddrs}");
                });

            subscription.GetSubscribeResponseAsObservable().Subscribe(id => _logger.LogInformation($"Subscribed ListenStudentRoleRevoked Event - {DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")}"));
            await subscription.SubscribeAsync(filter);
        }

        private async Task ListenLecturerRoleRevoked(StreamingWebSocketClient client)
        {
            var filter = _web3.Eth.GetEvent<LecturerRoleRevokedEventDTO>(managerPoolAddress).CreateFilterInput();
            var subscription = new EthLogsObservableSubscription(client);
            subscription.GetSubscriptionDataResponsesAsObservable().
                Subscribe(async log =>
                {
                    EventLog<LecturerRoleRevokedEventDTO> decoded = Event<LecturerRoleRevokedEventDTO>.DecodeEvent(log);
                    using (var scope = _services.CreateScope())
                    {
                        var scopedProcessingService = scope.ServiceProvider.GetRequiredService<ILecturerService>();
                        await scopedProcessingService.RevokeLecturerRole(decoded.Event.LecturerAddrs);

                        await subscription.UnsubscribeAsync();
                    };
                    _logger.LogInformation($"Listening Revoke Lecturer Role: {decoded.Event.LecturerAddrs}");
                });

            subscription.GetSubscribeResponseAsObservable().Subscribe(id => _logger.LogInformation($"Subscribed ListenLecturerRoleRevoked Event - {DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")}"));
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
                        //await ListenSubcribedContractReadyToCloseAsync();
                    });
                    await handler.SendRequestAsync();
                    //
                }
                minute++;
                if (minute >= 10)
                {
                    _logger.LogCritical("Restart because not receive any thing from wss");
                    _hostApplicationLifetime.StopApplication();
                }

                await Task.Delay(15000, stoppingToken);
            }
        }

        #region MISSION
        private async Task ListenActiveMissionContract(StreamingWebSocketClient client)
        {
            while (true)
            {
                using (var scope = _services.CreateScope())
                {
                    var scopedProcessingService = scope.ServiceProvider.GetRequiredService<IMissionService>();
                    var listMissionAddresses = await scopedProcessingService.GetMissionListInProgress(chainNetworkId);
                    foreach (var address in listMissionAddresses)
                    {
                        if (!subcribedContracts.Contains(address))
                            await FollowMissionCompetitionAsync(client, address);
                        /*if (subcribedContracts.Contains(address))
                            await FollowMissionCompetitionAsync(client, address, true);
                        else
                            await FollowMissionCompetitionAsync(client, address, false);*/
                    }
                }
                await Task.Delay(15000);
            }
        }

        private async Task FollowMissionCompetitionAsync(StreamingWebSocketClient client, string missionContractAddress)
        {
            try
            {
                /*                if (!isContainedInSubcribedContracts)
                                {
                                    subcribedContracts.Add(missionContractAddress);
                                    await _subcribedContractsListenEvent.InsertOneAsync(new SubcribedContractsListenEvent() { SubcribedContracts = missionContractAddress });
                                }*/
                subcribedContracts.Add(missionContractAddress);
                //await _subcribedContractsListenEvent.InsertOneAsync(new SubcribedContractsListenEvent() { SubcribedContracts = missionContractAddress});

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
                             _logger.LogInformation("Catch the Register Mission Event: " + missionContractAddress);
                             EventLog<RegisterEventDTO> decoded = Event<RegisterEventDTO>.DecodeEvent(log);

                             using (var scope = _services.CreateScope())
                             {
                                 var scopedProcessingService = scope.ServiceProvider.GetRequiredService<IMissionService>();
                                 await scopedProcessingService.UpdateStudentRegister(missionContractAddress, chainNetworkId, decoded.Event.StudentAddr);
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
                                 await scopedProcessingService.UpdateStudentCancelRegister(missionContractAddress, chainNetworkId, decoded.Event.StudentAddr);
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
                                 var myFunctionTxn = await GetTransactionInput(decoded.Log.TransactionHash);
                                 var inputData = new ConfirmCompletedAddress().DecodeTransaction(myFunctionTxn);
                                 await scopedProcessingService.UpdateLecturerConfirmComplete(missionContractAddress, chainNetworkId, inputData.StudentList);
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
                             EventLog<UnConfirmEventDTO> decoded = Event<UnConfirmEventDTO>.DecodeEvent(log);

                             using (var scope = _services.CreateScope())
                             {
                                 var scopedProcessingService = scope.ServiceProvider.GetRequiredService<IMissionService>();
                                 var myFunctionTxn = await GetTransactionInput(decoded.Log.TransactionHash);
                                 var inputData = new UnConfirmCompletedAddress().DecodeTransaction(myFunctionTxn);
                                 await scopedProcessingService.UpdateLecturerUnConfirmComplete(missionContractAddress, chainNetworkId, inputData.StudentList);
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
                                 await scopedProcessingService.CloseMission(missionContractAddress, chainNetworkId);
                                 _logger.LogInformation("Close Mission successfully with Contract: " + missionContractAddress);
                                 await subScriptionRegister.UnsubscribeAsync();
                                 await subscriptionCancelRegister.UnsubscribeAsync();
                                 await subscriptionConfirm.UnsubscribeAsync();
                                 await subscriptionUnConfirm.UnsubscribeAsync();
                                 await subscriptionClose.UnsubscribeAsync();

                                 subcribedContracts.Remove(missionContractAddress);
                                 //var filter = Builders<SubcribedContractsListenEvent>.Filter.Where(x => x.SubcribedContracts.ToLower() == missionContractAddress.ToLower());
                                 //await _subcribedContractsListenEvent.DeleteOneAsync(filter);
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

                var filterRegister = _web3.Eth.GetEvent<RegisterEventDTO>(missionContractAddress).CreateFilterInput();
                var filterCancelRegister = _web3.Eth.GetEvent<CancelRegisterEventDTO>(missionContractAddress).CreateFilterInput();
                var filterConfirm = _web3.Eth.GetEvent<ConfirmEventDTO>(missionContractAddress).CreateFilterInput();
                var filterUnConfirm = _web3.Eth.GetEvent<UnConfirmEventDTO>(missionContractAddress).CreateFilterInput();
                var filterClose = _web3.Eth.GetEvent<CloseEventDTO>(missionContractAddress).CreateFilterInput();

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
        private async Task ListenSubcribedContractReadyToCloseAsync()
        {
            try
            {
                foreach (var subcribedContract in subcribedContracts)
                {
                    var isReadyToClose = await _web3.Eth.GetContractQueryHandler<IsReadyToClose>()
                    .QueryAsync<bool>(subcribedContract);
                    if (isReadyToClose)
                    {
                        var closeHandler = _web3.Eth.GetContractTransactionHandler<Close>();
                        var closeSubcribedContract = new Close()
                        {
                            Pool = subcribedContract
                        };
                        var transactionReceipt = await closeHandler.SendRequestAndWaitForReceiptAsync(managerPoolAddress, closeSubcribedContract);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("UNKNOWN: ", ex);
            }
        }
        #endregion

        #region SUBJECT
        private async Task ListenActiveSubjectContract(StreamingWebSocketClient client)
        {
            while (true)
            {
                using (var scope = _services.CreateScope())
                {
                    var scopedProcessingService = scope.ServiceProvider.GetRequiredService<ISubjectService>();
                    var listAddresses = await scopedProcessingService.GetSubjectListInProgress(chainNetworkId);
                    foreach (var address in listAddresses)
                    {
                        /*                        if (subcribedContracts.Contains(address))
                                                    await FollowSubjectCompetitionAsync(client, address, true);
                                                else
                                                    await FollowSubjectCompetitionAsync(client, address, false);*/
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
                /*                if (!isContainedInSubcribedContracts)
                                {
                                    subcribedContracts.Add(contractAddress);
                                    await _subcribedContractsListenEvent.InsertOneAsync(new SubcribedContractsListenEvent() { SubcribedContracts = contractAddress });
                                }*/
                subcribedContracts.Add(contractAddress);
                //await _subcribedContractsListenEvent.InsertOneAsync(new SubcribedContractsListenEvent() { SubcribedContracts = contractAddress });

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
                                 await scopedProcessingService.UpdateStudentRegister(contractAddress, chainNetworkId, decoded.Event.StudentAddr);
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
                                 await scopedProcessingService.UpdateStudentCancelRegister(contractAddress, chainNetworkId, decoded.Event.StudentAddr);
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
                                 var myFunctionTxn = await GetTransactionInput(decoded.Log.TransactionHash);
                                 var inputData = new ConfirmCompletedAddress().DecodeTransaction(myFunctionTxn);
                                 await scopedProcessingService.UpdateLecturerConfirmComplete(contractAddress, chainNetworkId, inputData.StudentList);
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
                             EventLog<UnConfirmEventDTO> decoded = Event<UnConfirmEventDTO>.DecodeEvent(log);

                             using (var scope = _services.CreateScope())
                             {
                                 var scopedProcessingService = scope.ServiceProvider.GetRequiredService<ISubjectService>();
                                 var myFunctionTxn = await GetTransactionInput(decoded.Log.TransactionHash);
                                 var inputData = new UnConfirmCompletedAddress().DecodeTransaction(myFunctionTxn);
                                 await scopedProcessingService.UpdateLecturerUnConfirmComplete(contractAddress, chainNetworkId, inputData.StudentList);
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
                                 await scopedProcessingService.CloseSubject(contractAddress, chainNetworkId);
                                 _logger.LogInformation("Close Subject successfully with Contract: " + contractAddress);
                                 await subScriptionRegister.UnsubscribeAsync();
                                 await subscriptionCancelRegister.UnsubscribeAsync();
                                 await subscriptionConfirm.UnsubscribeAsync();
                                 await subscriptionUnConfirm.UnsubscribeAsync();
                                 await subscriptionClose.UnsubscribeAsync();

                                 subcribedContracts.Remove(contractAddress);
                                 //var filter = Builders<SubcribedContractsListenEvent>.Filter.Where(x => x.SubcribedContracts.ToLower() == contractAddress.ToLower());
                                 //await _subcribedContractsListenEvent.DeleteOneAsync(filter);
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

                var filterRegister = _web3.Eth.GetEvent<RegisterEventDTO>(contractAddress).CreateFilterInput();
                var filterCancelRegister = _web3.Eth.GetEvent<CancelRegisterEventDTO>(contractAddress).CreateFilterInput();
                var filterConfirm = _web3.Eth.GetEvent<ConfirmEventDTO>(contractAddress).CreateFilterInput();
                var filterUnConfirm = _web3.Eth.GetEvent<UnConfirmEventDTO>(contractAddress).CreateFilterInput();
                var filterClose = _web3.Eth.GetEvent<CloseEventDTO>(contractAddress).CreateFilterInput();

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
        #endregion

        #region SCHOLARSHIP
        private async Task ListenActiveScholarshipContract(StreamingWebSocketClient client)
        {
            while (true)
            {
                using (var scope = _services.CreateScope())
                {
                    var scopedProcessingService = scope.ServiceProvider.GetRequiredService<IScholarshipService>();
                    var listAddresses = await scopedProcessingService.GetScholarshipListInProgress(chainNetworkId);
                    foreach (var address in listAddresses)
                    {
                        /*                        if (subcribedContracts.Contains(address))
                                                    await FollowScholarshipCompetitionAsync(client, address, true);
                                                else
                                                    await FollowScholarshipCompetitionAsync(client, address, false);*/
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
                /*                if (!isContainedInSubcribedContracts)
                                {
                                    subcribedContracts.Add(contractAddress);
                                    await _subcribedContractsListenEvent.InsertOneAsync(new SubcribedContractsListenEvent() { SubcribedContracts = contractAddress });
                                }*/
                subcribedContracts.Add(contractAddress);
                //await _subcribedContractsListenEvent.InsertOneAsync(new SubcribedContractsListenEvent() { SubcribedContracts = contractAddress });

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
                             _logger.LogInformation("Catch the Register Scholarship Event: " + contractAddress);
                             EventLog<RegisterEventDTO> decoded = Event<RegisterEventDTO>.DecodeEvent(log);

                             using (var scope = _services.CreateScope())
                             {
                                 var scopedProcessingService = scope.ServiceProvider.GetRequiredService<IScholarshipService>();
                                 await scopedProcessingService.UpdateStudentRegister(contractAddress, chainNetworkId, decoded.Event.StudentAddr);
                                 _logger.LogInformation("Store Register Scholarship successfully with Contract: " + contractAddress);
                             }
                         }
                         catch (Exception ex)
                         {
                             _logger.LogError(ex, $"Fail in Store Register Scholarship: " + contractAddress);
                         }
                     });

                subscriptionCancelRegister.GetSubscriptionDataResponsesAsObservable().
                     Subscribe(async log =>
                     {
                         try
                         {
                             _logger.LogInformation("Catch Cancel Register Scholarship Event: " + contractAddress);
                             EventLog<CancelRegisterEventDTO> decoded = Event<CancelRegisterEventDTO>.DecodeEvent(log);
                             using (var scope = _services.CreateScope())
                             {
                                 var scopedProcessingService = scope.ServiceProvider.GetRequiredService<IScholarshipService>();
                                 await scopedProcessingService.UpdateStudentCancelRegister(contractAddress, chainNetworkId, decoded.Event.StudentAddr);
                                 _logger.LogInformation("Store Cancel Register Scholarship Event successfully with Contract: " + contractAddress);
                             }
                         }
                         catch (Exception ex)
                         {
                             _logger.LogError(ex, $"Fail in Cancel Register Scholarship Event: " + contractAddress);
                         }
                     });

                subscriptionConfirm.GetSubscriptionDataResponsesAsObservable().
                     Subscribe(async log =>
                     {
                         try
                         {
                             _logger.LogInformation("Catch Confirm Scholarship Event: " + contractAddress);
                             EventLog<ConfirmEventDTO> decoded = Event<ConfirmEventDTO>.DecodeEvent(log);

                             using (var scope = _services.CreateScope())
                             {
                                 var scopedProcessingService = scope.ServiceProvider.GetRequiredService<IScholarshipService>();
                                 var myFunctionTxn = await GetTransactionInput(decoded.Log.TransactionHash);
                                 var inputData = new ConfirmCompletedAddress().DecodeTransaction(myFunctionTxn);
                                 await scopedProcessingService.UpdateLecturerConfirmComplete(contractAddress, chainNetworkId, inputData.StudentList);
                                 _logger.LogInformation("Store Confirm Mission Event successfully with Scholarship Contract: " + contractAddress);
                             }
                         }
                         catch (Exception ex)
                         {
                             _logger.LogError(ex, $"Fail in Stored Confirm Scholarship: " + contractAddress);
                         }
                     });

                subscriptionUnConfirm.GetSubscriptionDataResponsesAsObservable().
                     Subscribe(async log =>
                     {
                         try
                         {
                             _logger.LogInformation("Catch Unconfirm Scholarship Event: " + contractAddress);
                             EventLog<UnConfirmEventDTO> decoded = Event<UnConfirmEventDTO>.DecodeEvent(log);

                             using (var scope = _services.CreateScope())
                             {
                                 var scopedProcessingService = scope.ServiceProvider.GetRequiredService<IScholarshipService>();
                                 var myFunctionTxn = await GetTransactionInput(decoded.Log.TransactionHash);
                                 var inputData = new UnConfirmCompletedAddress().DecodeTransaction(myFunctionTxn);
                                 await scopedProcessingService.UpdateLecturerUnConfirmComplete(contractAddress, chainNetworkId, inputData.StudentList);
                                 _logger.LogInformation("Store Unconfirm Scholarship successfully with Contract: " + contractAddress);
                             }
                         }
                         catch (Exception ex)
                         {
                             _logger.LogError(ex, $"Fail in Stored Unconfirm Scholarship: " + contractAddress);
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
                                 await scopedProcessingService.CloseScholarship(contractAddress, chainNetworkId);
                                 _logger.LogInformation("Close Subject successfully with Contract: " + contractAddress);
                                 await subScriptionRegister.UnsubscribeAsync();
                                 await subscriptionCancelRegister.UnsubscribeAsync();
                                 await subscriptionConfirm.UnsubscribeAsync();
                                 await subscriptionUnConfirm.UnsubscribeAsync();
                                 await subscriptionClose.UnsubscribeAsync();

                                 subcribedContracts.Remove(contractAddress);
                                 //var filter = Builders<SubcribedContractsListenEvent>.Filter.Where(x => x.SubcribedContracts.ToLower() == contractAddress.ToLower());
                                 //await _subcribedContractsListenEvent.DeleteOneAsync(filter);
                             }
                         }
                         catch (Exception ex)
                         {
                             _logger.LogError(ex, $"Fail in subscribe Close Scholarship event: ");
                         }
                     });

                subScriptionRegister.GetSubscribeResponseAsObservable()
                    .Subscribe(id => _logger.LogInformation($"Subscribed event Register Scholarship with Contract: {contractAddress}"));
                subscriptionCancelRegister.GetSubscribeResponseAsObservable()
                    .Subscribe(id => _logger.LogInformation($"Subscribed event Cancel Register Scholarship with Contract: {contractAddress}"));
                subscriptionConfirm.GetSubscribeResponseAsObservable()
                    .Subscribe(id => _logger.LogInformation($"Subscribed event Confirm Scholarship with Contract: {contractAddress}"));
                subscriptionUnConfirm.GetSubscribeResponseAsObservable()
                    .Subscribe(id => _logger.LogInformation($"Subscribed event Unconfirm Scholarship with Contract: {contractAddress}"));
                subscriptionClose.GetSubscribeResponseAsObservable()
                    .Subscribe(id => _logger.LogInformation($"Subscribed event Close Scholarship with Contract: {contractAddress}"));

                var filterRegister = _web3.Eth.GetEvent<RegisterEventDTO>(contractAddress).CreateFilterInput();
                var filterCancelRegister = _web3.Eth.GetEvent<CancelRegisterEventDTO>(contractAddress).CreateFilterInput();
                var filterConfirm = _web3.Eth.GetEvent<ConfirmEventDTO>(contractAddress).CreateFilterInput();
                var filterUnConfirm = _web3.Eth.GetEvent<UnConfirmEventDTO>(contractAddress).CreateFilterInput();
                var filterClose = _web3.Eth.GetEvent<CloseEventDTO>(contractAddress).CreateFilterInput();

                await subScriptionRegister.SubscribeAsync(filterRegister);
                await subscriptionCancelRegister.SubscribeAsync(filterCancelRegister);
                await subscriptionConfirm.SubscribeAsync(filterConfirm);
                await subscriptionUnConfirm.SubscribeAsync(filterUnConfirm);
                await subscriptionClose.SubscribeAsync(filterClose);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Follow Scholarship Contract Async exception: ");
            }
        }
        #endregion

        #region TUITION
        private async Task ListenActiveTuitionContract(StreamingWebSocketClient client)
        {
            while (true)
            {
                using (var scope = _services.CreateScope())
                {
                    var scopedProcessingService = scope.ServiceProvider.GetRequiredService<ITuitionService>();
                    var listAddresses = await scopedProcessingService.GetTuitionListInProgress(chainNetworkId);
                    foreach (var address in listAddresses)
                    {
                        /*                        if (subcribedContracts.Contains(address))
                                                    await FollowTuitionCompetitionAsync(client, address, true);
                                                else
                                                    await FollowTuitionCompetitionAsync(client, address, false);*/
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
                /*                if (!isContainedInSubcribedContracts)
                                {
                                    subcribedContracts.Add(contractAddress);
                                    await _subcribedContractsListenEvent.InsertOneAsync(new SubcribedContractsListenEvent() { SubcribedContracts = contractAddress });
                                }*/
                subcribedContracts.Add(contractAddress);
                //await _subcribedContractsListenEvent.InsertOneAsync(new SubcribedContractsListenEvent() { SubcribedContracts = contractAddress });

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
                                 var myFunctionTxn = await GetTransactionInput(decoded.Log.TransactionHash);
                                 var inputData = new AddStudentToTuition().DecodeTransaction(myFunctionTxn);
                                 await scopedProcessingService.AddStudentToTuition(contractAddress, chainNetworkId, inputData.StudentList);
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
                                 var myFunctionTxn = await GetTransactionInput(decoded.Log.TransactionHash);
                                 var inputData = new RemoveStudentFromTuition().DecodeTransaction(myFunctionTxn);
                                 await scopedProcessingService.RemoveStudentFromTuition(contractAddress, chainNetworkId, inputData.StudentList);
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
                                 await scopedProcessingService.UpdateStudentCompeletedPayment(contractAddress, chainNetworkId, decoded.Event.StudentsAddr);
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
                                 await scopedProcessingService.CloseTuition(contractAddress, chainNetworkId);
                                 _logger.LogInformation("Close Tuition successfully with Contract: " + contractAddress);
                                 await subScriptionAddStudentToTuition.UnsubscribeAsync();
                                 await subscriptionRemoveStudentFromTuition.UnsubscribeAsync();
                                 await subscriptionPayment.UnsubscribeAsync();
                                 await subscriptionClose.UnsubscribeAsync();

                                 subcribedContracts.Remove(contractAddress);
                                 //var filter = Builders<SubcribedContractsListenEvent>.Filter.Where(x => x.SubcribedContracts.ToLower() == contractAddress.ToLower());
                                 //await _subcribedContractsListenEvent.DeleteOneAsync(filter);
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

                var filterRegister = _web3.Eth.GetEvent<AddStudentToTuitionEventDTO>(contractAddress).CreateFilterInput();
                var filterCancelRegister = _web3.Eth.GetEvent<RemoveStudentFromTuitionEventDTO>(contractAddress).CreateFilterInput();
                var filterPayment = _web3.Eth.GetEvent<PaymentEventDTO>(contractAddress).CreateFilterInput();
                var filterClose = _web3.Eth.GetEvent<CloseEventDTO>(contractAddress).CreateFilterInput();

                await subScriptionAddStudentToTuition.SubscribeAsync(filterRegister);
                await subscriptionRemoveStudentFromTuition.SubscribeAsync(filterCancelRegister);
                await subscriptionPayment.SubscribeAsync(filterPayment);
                await subscriptionClose.SubscribeAsync(filterClose);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Follow Tuition Contract Async exception: ");
            }
        }
        #endregion
    }
}
