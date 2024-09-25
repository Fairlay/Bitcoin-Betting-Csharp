using System;
using System.Collections.Generic;
using System.Text;

namespace BBetModels.DAG.SUTypeModels
{
    //can only forbid OrderAlteration mining   if signed by the same user. Before
    class ForbidMiningTX
    {
        //Request mining confirmation from Miner.  If miner Signs with a hash from a previous HashIteration  he may not mine a matching confirmation.
        // If miner does not answer??
        // We force this Forbid TX to another miner in the network!

        //Intended for user to have assurance of cancellation!

        public string ForbiddenTX;

    }
}
