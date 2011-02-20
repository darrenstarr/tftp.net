﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Tftp.Net.Transfer.States
{
    class StartOutgoingWrite : BaseState
    {
        public StartOutgoingWrite(TftpTransfer context)
            : base(context) { }

        public override void OnStart()
        {
            Context.SetState(new StartingOutgoingWrite(Context));
        }

        public override void OnCancel()
        {
            Context.SetState(new Closed(Context));
        }
    }
}