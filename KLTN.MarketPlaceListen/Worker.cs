using KLTN.Common.Exceptions;
using KLTN.Common.Models.AppSettingModels;
using KLTN.Common.SmartContracts.Events;
using KLTN.Common.Utilities.Constants;
using KLTN.Core.ProductServices.Interfaces;
using KLTN.Core.RequestActivateServices.Interfaces;
using KLTN.DAL;
using KLTN.MarketPlaceListen.DTOs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nethereum.Contracts;
using Nethereum.JsonRpc.WebSocketStreamingClient;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.RPC.Reactive.Eth;
using Nethereum.RPC.Reactive.Eth.Subscriptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace KLTN.MarketPlaceListen
{
    public class Worker : BackgroundService
    {
        private readonly IMongoDbContext _context;
        //private readonly IMongoCollection<SubcribedContractsListenEvent> _subcribedContractsListenEvent;

        private readonly IServiceScopeFactory _services;
        private readonly ILogger<Worker> _logger;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        Nethereum.Web3.Web3 _web3 = new Nethereum.Web3.Web3();
        string marketplaceAddress = ListenMarketplaceAppSetting.Value.MarketplaceContractAddress;
        int chainNetworkId = ListenMarketplaceAppSetting.Value.ChainNetworkId;

        public Worker(ILogger<Worker> logger, IServiceScopeFactory serviceScopeFactory, IHostApplicationLifetime hostApplicationLifetime)
        {
            _logger = logger;
            _services = serviceScopeFactory;
            _hostApplicationLifetime = hostApplicationLifetime;
            _web3 = new Nethereum.Web3.Web3(ListenMarketplaceAppSetting.Value.RpcUrl);
            _web3.TransactionManager.UseLegacyAsDefault = true;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var client = new StreamingWebSocketClient(ListenMarketplaceAppSetting.Value.WssUrl))
            {
                try
                {
                    await client.StartAsync();
                    await ListenCreateNewProductOnSale(client);
                    await ListenListProductOnSale(client);
                    await ListenDelistProductOnSale(client);
                    await ListenBuyProductOnSale(client);
                    await ListenUpdateBuyPriceProductOnSale(client);
                    await ListenUpdateAmountProductOnSale(client);

                    await ListenRequestActivateNFT(client);
                    await ListenCancelRequestActivateNFT(client);
                    await ActivateRequestNFT(client);



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
                    _logger.LogCritical("Restart because not receive any thing from wss");
                    _hostApplicationLifetime.StopApplication();
                }

                await Task.Delay(15000, stoppingToken);
            }
        }

        private async Task ListenCreateNewProductOnSale(StreamingWebSocketClient client)
        {
            Console.WriteLine(marketplaceAddress);
            var filter = _web3.Eth.GetEvent<CreateAndListProductEvent>(marketplaceAddress).CreateFilterInput();
            var subscription = new EthLogsObservableSubscription(client);
            subscription.GetSubscriptionDataResponsesAsObservable().
                Subscribe(async log =>
                {
                    EventLog<CreateAndListProductEvent> decoded = Event<CreateAndListProductEvent>.DecodeEvent(log);
                    string jsonResponse = await RequestIPFS(decoded.Event.HashInfo);
                    var metadata = JsonConvert.DeserializeObject<ProductOnSaleMetadataDTO>(jsonResponse);
                    using (var scope = _services.CreateScope())
                    {
                        var scopedProcessingService = scope.ServiceProvider.GetRequiredService<IProductService>();
                        await scopedProcessingService.CreateNewProductOnSale(new Core.ProductServices.DTOs.ProductOnSaleDTO
                        {
                            //ProductName = metadata.,
                            //ProductImg = metadata.,
                            //ProductId = decoded.Event.ProductId,
                            //ProductHahIPFS = decoded.Event.HashInfo,
                            //AmountOnSale = decoded.Event.AmountOnSale,
                            //PriceOfOneItem = decoded.Event.PriceOfOneItem,
                            //ProductTypeName = metadata.,
                            //ProductDescription = metadata.,
                            //SaleAddress = decoded.Event.SaleAddress
                        });
                    }
                    _logger.LogInformation($"Listening create and list product with ProductId: " + decoded.Event.ProductId);
                });

            subscription.GetSubscribeResponseAsObservable().Subscribe(id => _logger.LogInformation($"Subscribed ListenCreateNewProductOnSale Event - {DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")}"));
            await subscription.SubscribeAsync(filter);
        }

        private async Task ListenListProductOnSale(StreamingWebSocketClient client)
        {
            Console.WriteLine(marketplaceAddress);
            var filter = _web3.Eth.GetEvent<ListProductOnSaleEvent>(marketplaceAddress).CreateFilterInput();
            var subscription = new EthLogsObservableSubscription(client);
            subscription.GetSubscriptionDataResponsesAsObservable().
                Subscribe(async log =>
                {
                    EventLog<ListProductOnSaleEvent> decoded = Event<ListProductOnSaleEvent>.DecodeEvent(log);
                    using (var scope = _services.CreateScope())
                    {
                        var scopedProcessingService = scope.ServiceProvider.GetRequiredService<IProductService>();
                        await scopedProcessingService.ListProductOnSale(new Core.ProductServices.DTOs.ProductStudentListOnSaleDTO
                        {
                            ProductId = decoded.Event.ProductId,
                            AmountOnSale = decoded.Event.AmountOnSale,
                            PriceOfOneItem = decoded.Event.PriceOfOneItem,
                            SaleAddress = decoded.Event.SaleAddress
                        });
                    }
                    _logger.LogInformation($"Listening list product on sale with ProductId: " + decoded.Event.ProductId);
                });

            subscription.GetSubscribeResponseAsObservable().Subscribe(id => _logger.LogInformation($"Subscribed ListenListProductOnSale Event - {DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")}"));
            await subscription.SubscribeAsync(filter);
        }

        private async Task ListenDelistProductOnSale(StreamingWebSocketClient client)
        {
            Console.WriteLine(marketplaceAddress);
            var filter = _web3.Eth.GetEvent<DelistProductOnSaleEvent>(marketplaceAddress).CreateFilterInput();
            var subscription = new EthLogsObservableSubscription(client);
            subscription.GetSubscriptionDataResponsesAsObservable().
                Subscribe(async log =>
                {
                    EventLog<DelistProductOnSaleEvent> decoded = Event<DelistProductOnSaleEvent>.DecodeEvent(log);
                    using (var scope = _services.CreateScope())
                    {
                        var scopedProcessingService = scope.ServiceProvider.GetRequiredService<IProductService>();
                        await scopedProcessingService.DelistProductOnSale(new Core.ProductServices.DTOs.ProductStudentDelistOnSaleDTO
                        {
                            ProductId = decoded.Event.ProductId,
                            AmountOnSale = decoded.Event.AmountOnSale,
                            SaleAddress = decoded.Event.SaleAddress
                        });
                    }
                    _logger.LogInformation($"Listening delist product on sale with ProductId: " + decoded.Event.ProductId);
                });

            subscription.GetSubscribeResponseAsObservable().Subscribe(id => _logger.LogInformation($"Subscribed ListenDelistProductOnSale Event - {DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")}"));
            await subscription.SubscribeAsync(filter);
        }

        private async Task ListenBuyProductOnSale(StreamingWebSocketClient client)
        {
            Console.WriteLine(marketplaceAddress);
            var filter = _web3.Eth.GetEvent<BuyProductOnSaleEvent>(marketplaceAddress).CreateFilterInput();
            var subscription = new EthLogsObservableSubscription(client);
            subscription.GetSubscriptionDataResponsesAsObservable().
                Subscribe(async log =>
                {
                    EventLog<BuyProductOnSaleEvent> decoded = Event<BuyProductOnSaleEvent>.DecodeEvent(log);
                    using (var scope = _services.CreateScope())
                    {
                        var scopedProcessingService = scope.ServiceProvider.GetRequiredService<IProductService>();
                        await scopedProcessingService.BuyProductOnSale(new Core.ProductServices.DTOs.ProductStudentBuyOnSaleDTO
                        {
                            ProductId = decoded.Event.ProductId,
                            BuyAmount = decoded.Event.Amount,
                            BuyerAddress = decoded.Event.Buyer,
                            SellerAddress = decoded.Event.Seller
                        });
                    }
                    _logger.LogInformation($"Listening buy product on sale with ProductId: " + decoded.Event.ProductId);
                });

            subscription.GetSubscribeResponseAsObservable().Subscribe(id => _logger.LogInformation($"Subscribed ListenBuyProductOnSale Event - {DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")}"));
            await subscription.SubscribeAsync(filter);
        }

        private async Task ListenUpdateBuyPriceProductOnSale(StreamingWebSocketClient client)
        {
            Console.WriteLine(marketplaceAddress);
            var filter = _web3.Eth.GetEvent<UpdateBuyPriceProductOnSaleEvent>(marketplaceAddress).CreateFilterInput();
            var subscription = new EthLogsObservableSubscription(client);
            subscription.GetSubscriptionDataResponsesAsObservable().
                Subscribe(async log =>
                {
                    EventLog<UpdateBuyPriceProductOnSaleEvent> decoded = Event<UpdateBuyPriceProductOnSaleEvent>.DecodeEvent(log);
                    using (var scope = _services.CreateScope())
                    {
                        var scopedProcessingService = scope.ServiceProvider.GetRequiredService<IProductService>();
                        await scopedProcessingService.UpdateBuyPriceProductOnSale(new Core.ProductServices.DTOs.ProductUpdateBuyPriceOnSaleDTO
                        {
                            ProductId = decoded.Event.ProductId,
                            PriceOfOneItem = decoded.Event.PriceOfOneItem,
                            SaleAddress = decoded.Event.SaleAddress
                        });
                    }
                    _logger.LogInformation($"Listening update buyPrice of product on sale with ProductId: " + decoded.Event.ProductId + " and price of one item is:" + decoded.Event.PriceOfOneItem);
                });

            subscription.GetSubscribeResponseAsObservable().Subscribe(id => _logger.LogInformation($"Subscribed ListenUpdateBuyPriceProductOnSale Event - {DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")}"));
            await subscription.SubscribeAsync(filter);
        }

        private async Task ListenUpdateAmountProductOnSale(StreamingWebSocketClient client)
        {
            Console.WriteLine(marketplaceAddress);
            var filter = _web3.Eth.GetEvent<UpdateAmountProductOnSaleEvent>(marketplaceAddress).CreateFilterInput();
            var subscription = new EthLogsObservableSubscription(client);
            subscription.GetSubscriptionDataResponsesAsObservable().
                Subscribe(async log =>
                {
                    EventLog<UpdateAmountProductOnSaleEvent> decoded = Event<UpdateAmountProductOnSaleEvent>.DecodeEvent(log);
                    using (var scope = _services.CreateScope())
                    {
                        var scopedProcessingService = scope.ServiceProvider.GetRequiredService<IProductService>();
                        await scopedProcessingService.UpdateAmountProductOnSale(new Core.ProductServices.DTOs.ProductUpdateAmountOnSaleDTO
                        {
                            ProductId = decoded.Event.ProductId,
                            AmountOnSale = decoded.Event.AmountOnSale,
                            SaleAddress = decoded.Event.SaleAddress
                        });
                    }
                    _logger.LogInformation($"Listening admin update amount of product on sale with ProductId: " + decoded.Event.ProductId + " and amount to sale:" + decoded.Event.AmountOnSale);
                });

            subscription.GetSubscribeResponseAsObservable().Subscribe(id => _logger.LogInformation($"Subscribed ListenUpdateAmountProductOnSale Event - {DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")}"));
            await subscription.SubscribeAsync(filter);
        }

        private async Task ListenRequestActivateNFT(StreamingWebSocketClient client)
        {
            Console.WriteLine(marketplaceAddress);
            var filter = _web3.Eth.GetEvent<RequestActivateNFTEvent>(marketplaceAddress).CreateFilterInput();
            var subscription = new EthLogsObservableSubscription(client);
            subscription.GetSubscriptionDataResponsesAsObservable().
                Subscribe(async log =>
                {
                    EventLog<RequestActivateNFTEvent> decoded = Event<RequestActivateNFTEvent>.DecodeEvent(log);
                    using (var scope = _services.CreateScope())
                    {
                        var scopedProcessingService = scope.ServiceProvider.GetRequiredService<IActivateRequestService>();
                        await scopedProcessingService.CreateNewActivateRequest(new Core.ActivateRequestServices.DTOs.RequestActivateDTO
                        {
                            ProductId = decoded.Event.ProductId,
                            RequestId = decoded.Event.RequestId,
                            AmountToActivate = decoded.Event.AmountToActive,
                            RequestedTime = decoded.Event.RequestedTime,
                            StudentAddress = decoded.Event.StudentAddress
                        });
                    }
                    _logger.LogInformation($"Listening admin update amount of product on sale with ProductId: " + decoded.Event.ProductId + " and amount to sale:" + decoded.Event.AmountOnSale);
                });

            subscription.GetSubscribeResponseAsObservable().Subscribe(id => _logger.LogInformation($"Subscribed ListenUpdateAmountProductOnSale Event - {DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")}"));
            await subscription.SubscribeAsync(filter);
        }

        private async Task ListenCancelRequestActivateNFT(StreamingWebSocketClient client)
        {
            Console.WriteLine(marketplaceAddress);
            var filter = _web3.Eth.GetEvent<CancelRequestActivateNFTEvent>(marketplaceAddress).CreateFilterInput();
            var subscription = new EthLogsObservableSubscription(client);
            subscription.GetSubscriptionDataResponsesAsObservable().
                Subscribe(async log =>
                {
                    EventLog<CancelRequestActivateNFTEvent> decoded = Event<CancelRequestActivateNFTEvent>.DecodeEvent(log);
                    using (var scope = _services.CreateScope())
                    {
                        var scopedProcessingService = scope.ServiceProvider.GetRequiredService<IActivateRequestService>();
                        await scopedProcessingService.CancelActivateRequest(decoded.Event.RequestIds);
                    }
                    _logger.LogInformation($"Listening ListenCancelRequestActivateNFT successfully");
                });

            subscription.GetSubscribeResponseAsObservable().Subscribe(id => _logger.LogInformation($"Subscribed ListenCancelRequestActivateNFT Event - {DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")}"));
            await subscription.SubscribeAsync(filter);
        }

        private async Task ActivateRequestNFT(StreamingWebSocketClient client)
        {
            Console.WriteLine(marketplaceAddress);
            var filter = _web3.Eth.GetEvent<ActivateRequestNFTEvent>(marketplaceAddress).CreateFilterInput();
            var subscription = new EthLogsObservableSubscription(client);
            subscription.GetSubscriptionDataResponsesAsObservable().
                Subscribe(async log =>
                {
                    EventLog<ActivateRequestNFTEvent> decoded = Event<ActivateRequestNFTEvent>.DecodeEvent(log);
                    using (var scope = _services.CreateScope())
                    {
                        var scopedProcessingService = scope.ServiceProvider.GetRequiredService<IActivateRequestService>();
                        await scopedProcessingService.ActivateRequest(new Core.ActivateRequestServices.DTOs.ActivateRequestDTO
                        {
                            RequestId = decoded.Event.RequestId,
                            ActivatedTime = decoded.Event.ActivatedTime,
                            IsIdependentNFT = decoded.Event.IsIdependentNFT
                        });
                    }
                    _logger.LogInformation($"Listening ActivateRequestNFT successfully");
                });

            subscription.GetSubscribeResponseAsObservable().Subscribe(id => _logger.LogInformation($"Subscribed ActivateRequestNFT Event - {DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")}"));
            await subscription.SubscribeAsync(filter);
        }
    }
}
