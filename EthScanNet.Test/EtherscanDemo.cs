using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using EthScanNet.Lib;
using EthScanNet.Lib.Models.ApiRequests.Contracts;
using EthScanNet.Lib.Models.ApiResponses.Accounts;
using EthScanNet.Lib.Models.ApiResponses.Contracts;
using EthScanNet.Lib.Models.ApiResponses.Stats;
using EthScanNet.Lib.Models.ApiResponses.Tokens;
using EthScanNet.Lib.Models.EScan;
using Microsoft.Extensions.DependencyInjection;

namespace EthScanNet.Test
{
    public class EtherscanDemo
    {
        private readonly string _apiKey;
        private readonly EScanNetwork _network;

        public EtherscanDemo(string apiKey, EScanNetwork network)
        {
            this._apiKey = apiKey ?? "YourApiKeyToken";
            this._network = network ?? EScanNetwork.MainNet;
        }

        public async Task RunApiCommandsAsync()
        {
            var services = new ServiceCollection();
            services.AddHttpClient();

            var serviceProvider = services.BuildServiceProvider();
            var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
            
            Console.WriteLine("Running EtherscanDemo with APIKey: " + this._apiKey);
            
            var client = new EScanClient(_network, _apiKey, httpClientFactory);

            try
            {
                await RunAccountCommandsAsync(client);
                await RunTokenCommandsAsync(client);
                await RunStatsCommandsAsync(client);
                await RunContractCommandsAsync(client);
                Console.WriteLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private async Task RunAccountCommandsAsync(EScanClient client)
        {
            Console.WriteLine("Account test started");
            var tasks = new List<Task>();

            for (var i = 0; i < 50; i++)
            {
                tasks.Add(client.Accounts.GetBalanceAsync(new("0x0000000000000000000000000000000000001004")));
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);
            
            EScanBalance apiBalance = await client.Accounts.GetBalanceAsync(new("0x0000000000000000000000000000000000001004"));
            Console.WriteLine("GetBalanceAsync: " +  apiBalance.Message);
            EScanTransactions normalApiTransaction = await client.Accounts.GetNormalTransactionsAsync(new("0x0000000000000000000000000000000000001004"));
            Console.WriteLine("GetNormalTransactionsAsync: " + normalApiTransaction.Message);
            EScanTransactions internalApiTransaction = await client.Accounts.GetInternalTransactionsAsync(new("0x0000000000000000000000000000000000001004"));
            Console.WriteLine("GetInternalTransactionsAsync: " + internalApiTransaction.Message);
            EScanMinedBlocks apiMinedBlocks = await client.Accounts.GetMinedBlocksAsync(new("0x78f3adfc719c99674c072166708589033e2d9afe"));
            Console.WriteLine("GetMinedBlocksAsync: " + apiMinedBlocks.Message);
            EScanTokenTransferEvents apiTokenTransferEvents = await client.Accounts.GetNftErc721TokenEvents(new("0xf09f5e21f86692c614d2d7b47e3b9729dc1c436f"));
            Console.WriteLine("GetNftErc721TokenEvents: " + apiTokenTransferEvents.Message);
            Console.WriteLine("Account test complete");
            EScanERC20TokenTransferEvents apiERC20TokenTransferEvents = await client.Accounts.GetERC20TokenEvents(new("0xf09f5e21f86692c614d2d7b47e3b9729dc1c436f"));
            Console.WriteLine("GetNftErc721TokenEvents: " + apiTokenTransferEvents.Message);
            Console.WriteLine("Account test complete");
        }

        private async Task RunTokenCommandsAsync(EScanClient client)
        {
            EScanAddress holderAddress = new("0x2b7fc60fd13f32fed8730113a14e3468d2f17cdc");
            EScanAddress contractAddress = new("0xf7844cb890f4c339c497aeab599abdc3c874b67a");
            Console.WriteLine("Token test started");
            EScanTokenSupply apiTokenSupplyM = await client.Tokens.GetMaxSupply(contractAddress);
            Console.WriteLine("GetMaxSupply: " + apiTokenSupplyM.Message);
            EScanBalance balance = await client.Accounts.GetTokenBalanceForAddress(holderAddress, contractAddress);
            Console.WriteLine("GetTokenBalanceForAddress: " + balance.Message);
            Console.WriteLine("Token test complete");
        }

        private async Task RunStatsCommandsAsync(EScanClient client)
        {
            Console.WriteLine("Stats test started");
            EScanTotalCoinSupply totalSupply = await client.Stats.GetTotalSupply();
            Console.WriteLine("GetTotalSupply: " + totalSupply.Message);
            Console.WriteLine("Stats test complete");
        }

        private async Task RunContractCommandsAsync(EScanClient client)
        {
            Console.WriteLine("Contracts test started");

            EScanAddress contractAddress = new EScanAddress("0xfB6916095ca1df60bB79Ce92cE3Ea74c37c5d359");
            EScanAbiResponse abiResponse = await client.Contracts.GetAbiAsync(contractAddress);
            Console.WriteLine("ABI: " + abiResponse.Message);

            EScanSourceCodeResponse sourceCodeResponse = await client.Contracts.GetSourceCodeAsync(contractAddress);
            Console.WriteLine("Source Code: " + sourceCodeResponse.Message);

            // EScanNetwork.RinkebyNet
            string guid = "brv6gjya7rne8rvyniysycu8qcvb5nqn49akwx7wdxgx5udpgj";
            EScanSourceCodeVerificationStatusResponse verificationStatusResponse = await client.Contracts.GetSourceCodeVerificationStatusAsync(guid);
            Console.WriteLine("Verification status: " + verificationStatusResponse.Message);

            var verificationPayload = new EScanContractCodeVerificationModel
            {
                ContractAddress = "0x29137a31592885EF4E6Ab2C1A7BB81d0D4311954",
                SourceCode = @"
                // SPDX-License-Identifier: MIT

                pragma solidity >=0.7.0 <0.9.0;

                contract Storage {

                    uint256 number;

                    constructor(uint defaultNum_) {
                        number = defaultNum_;
                    }

                    function store(uint256 num) public {
                        number = num;
                    }

                    function retrieve() public view returns (uint256){
                        return number;
                    }
                }",
                CodeFormat = "solidity-single-file",
                ContractName = "Storage",
                CompilerVersion = "v0.8.7+commit.e28d00a7",
                OptimizationUsed = "1",
                Runs = "200",
                ContstructorArguments = "uint defaultNum_",
                EvmVersion = "3",
                LicenseType = "1"
            };
            EScanSourceCodeVerificationResponse verificationResponse = await client.Contracts.VerifySmartContractAsync(verificationPayload);
            Console.WriteLine("Verification: " + verificationResponse.Guid);

            Console.WriteLine("Contracts test complete");
        }
    }
}