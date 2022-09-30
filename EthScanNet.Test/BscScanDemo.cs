using System;
using System.Net.Http;
using System.Threading.Tasks;
using EthScanNet.Lib;
using EthScanNet.Lib.Models.ApiResponses.Accounts;
using EthScanNet.Lib.Models.ApiResponses.Proxy;
using EthScanNet.Lib.Models.ApiResponses.Stats;
using EthScanNet.Lib.Models.ApiResponses.Tokens;
using EthScanNet.Lib.Models.EScan;
using Microsoft.Extensions.DependencyInjection;

namespace EthScanNet.Test
{
    public class BscScanDemo
    {
        private string _apiKey;

        public BscScanDemo(string apiKey = null)
        {
            _apiKey = apiKey ?? "YourApiKeyToken";
        }

        public async Task RunApiCommandsAsync()
        {
            var services = new ServiceCollection();
            services.AddHttpClient();

            var serviceProvider = services.BuildServiceProvider();
            var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
            
            Console.WriteLine("Running BSCScanDemo with APIKey: " + this._apiKey);
            
            var client = new EScanClient(EScanNetwork.BscMainNet, _apiKey, httpClientFactory);

            try
            {
                await RunTestCommandAsync(client);
                // await RunAccountCommandsAsync(client);
                // await RunTokenCommandsAsync(client);
                // await RunStatsCommandsAsync(client);
                Console.WriteLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private async Task RunTestCommandAsync(EScanClient client)
        {
            //1603fa0d-e27a-4a15-901d-4df32be5260c
            EScanEthCurrentBlock currentBlock = await client.Proxy.CurrentBlock();
            Console.WriteLine("CurrentBlock: " + currentBlock.Result);
            
            EScanTransactions internalApiTransaction = await client.Accounts.GetInternalTransactionsAsync("0x0032438e1f8d51cd3c13c23587e048d1a180449e19ab8ea930fe379f8c2678a7");
            Console.WriteLine("GetInternalTransactionsAsync: " + internalApiTransaction.Message);
        }

        private async Task RunAccountCommandsAsync(EScanClient client)
        {
            Console.WriteLine("Account test started");
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
            EScanBalance tokenBalanceForAddress = await client.Accounts.GetTokenBalanceForAddress(new("0x2b7fc60fd13f32fed8730113a14e3468d2f17cdc"), new("0xf7844cb890f4c339c497aeab599abdc3c874b67a"));
            Console.WriteLine("GetTokenBalanceForAddress: " + tokenBalanceForAddress.Message);
            Console.WriteLine("Account test complete");
        }

        private async Task RunTokenCommandsAsync(EScanClient client)
        {
            EScanAddress contractAddress = new("0xf7844cb890f4c339c497aeab599abdc3c874b67a");
            
            Console.WriteLine("Token test started");
            EScanTokenSupply apiTokenSupplyM = await client.Tokens.GetMaxSupply(contractAddress);
            Console.WriteLine("GetMaxSupply: " + apiTokenSupplyM.Message);
            EScanTokenSupply apiTokenSupplyC = await client.Tokens.GetCirculatingSupply(contractAddress);
            Console.WriteLine("GetCirculatingSupply: " + apiTokenSupplyC.Message);
            Console.WriteLine("Token test complete");
        }

        private async Task RunStatsCommandsAsync(EScanClient client)
        {
            EScanAddress contractAddress = new("0xf09f5e21f86692c614d2d7b47e3b9729dc1c436f");
            Console.WriteLine("Stats test started");
            EScanTotalCoinSupply totalSupply = await client.Stats.GetTotalSupply();
            Console.WriteLine(totalSupply.TotalCoin);
            //EScanTotalCoinSupply apiTokenSupplyEth = await client.Stats.EtherScan.GetTotalSupply();
            Console.WriteLine("Stats test complete");
        }
    }
}