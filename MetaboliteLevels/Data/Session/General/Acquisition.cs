﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetaboliteLevels.Controls.Lists;
using MetaboliteLevels.Data.Session.Associational;

namespace MetaboliteLevels.Data.Session.General
{
    [Serializable]
    class Acquisition
    {
        [XColumn] public readonly int Replicate;
        [XColumn] public readonly BatchInfo Batch;
        [XColumn] public readonly int Order;
        [XColumn] public readonly string Id;

        public Acquisition( string v, int repId, BatchInfo batchInfo, int acquisition )
        {
            this.Id = v;
            this.Replicate = repId;
            this.Batch = batchInfo;
            this.Order = acquisition;
        }

        public override string ToString()
        {
            return Id;
        }
    }
}
