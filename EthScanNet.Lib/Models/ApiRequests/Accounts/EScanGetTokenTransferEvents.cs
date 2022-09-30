using EthScanNet.Lib.Enums;
using EthScanNet.Lib.Models.ApiResponses.Accounts;
using EthScanNet.Lib.Models.EScan;

namespace EthScanNet.Lib.Models.ApiRequests.Accounts
{
    internal class EScanGetTokenTransferEvents : EScanAccountRequest
    {
        public int? Page { get; set; }
        public int? Offset { get; set; }
        public ulong? StartBlock { get; private set; }
        public ulong? EndBlock { get; private set; }

        public EScanGetTokenTransferEvents(EScanAddress address, ulong? startBlock, ulong? endBlock,
            int? page,
            int? offset, EScanClient eScanClient)
            : base(address, eScanClient, EScanModules.Account, EScanActions.TokenNftTx, typeof(EScanTokenTransferEvents))
        {
            this.StartBlock = startBlock;
            this.EndBlock = endBlock;
            this.Page = page;
            this.Offset = offset;
        }
    }
}