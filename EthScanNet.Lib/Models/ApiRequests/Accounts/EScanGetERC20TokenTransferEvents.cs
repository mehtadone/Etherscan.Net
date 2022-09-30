using EthScanNet.Lib.Enums;
using EthScanNet.Lib.Models.ApiResponses.Accounts;
using EthScanNet.Lib.Models.EScan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EthScanNet.Lib.Models.ApiRequests.Accounts
{
    internal class EScanGetERC20TokenTransferEvents: EScanAccountRequest
    {
        public int? Page { get; set; }
        public int? Offset { get; set; }
        public ulong? StartBlock { get; private set; }
        public ulong? EndBlock { get; private set; }

        public EScanGetERC20TokenTransferEvents(EScanAddress address, ulong? startBlock,
            ulong? endBlock,
            int? page,
            int? offset, EScanClient eScanClient)
           : base(address, eScanClient, EScanModules.Account, EScanActions.TxErc20Token, typeof(EScanERC20TokenTransferEvents))
        {
            this.StartBlock = startBlock;
            this.EndBlock = endBlock;
            this.Page = page;
            this.Offset = offset;
        }
    }
}
