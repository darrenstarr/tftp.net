﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Tftp.Net.Transfer.States;
using System.IO;
using Tftp.Net.TransferOptions;

namespace Tftp.Net.UnitTests
{
    [TestFixture]
    class StartIncomingReadState_Test
    {
        private TransferStub transfer;
        private OptionHandlerStub optionHandler;

        [SetUp]
        public void Setup()
        {
            transfer = new TransferStub();
            transfer.SetState(new StartIncomingRead(transfer));
            optionHandler = new OptionHandlerStub("blub");
            TransferOptionHandlers.Add(optionHandler);
        }

        [TearDown]
        public void Teardown()
        {
            TransferOptionHandlers.Remove(optionHandler);
        }

        [Test]
        public void CanCancel()
        {
            transfer.Cancel(TftpErrorPacket.IllegalOperation);
            Assert.IsTrue(transfer.CommandWasSent(typeof(Error)));
            Assert.IsInstanceOf<Closed>(transfer.State);
        }

        [Test]
        public void IgnoresCommands()
        {
            transfer.OnCommand(new Error(5, "Hallo Welt"));
            Assert.IsInstanceOf<StartIncomingRead>(transfer.State);
        }

        [Test]
        public void CanStart()
        {
            transfer.Start(new MemoryStream(new byte[50000]));
            Assert.IsInstanceOf<Sending>(transfer.State);
        }

        [Test]
        public void CanStartWithOptions()
        {
            //Simulate that we got a request for a option
            transfer.Options.Request("blub", "123");

            transfer.Start(new MemoryStream(new byte[50000]));
            Assert.IsInstanceOf<SendOptionAcknowledgementForReadRequest>(transfer.State);
            Assert.IsTrue(optionHandler.AcknowledgeWasCalled);
            Assert.IsTrue(transfer.Options.First().IsAcknowledged);
        }

        [Test]
        public void CanStartRejectOptions()
        {
            //Simulate that we got a request for an option
            transfer.Options.Request("non-acceptable-option", "123");

            Assert.IsFalse(optionHandler.AcknowledgeWasCalled);
            transfer.Start(new MemoryStream(new byte[50000]));
            Assert.IsTrue(optionHandler.AcknowledgeWasCalled);
            Assert.IsFalse(transfer.CommandWasSent(typeof(OptionAcknowledgement)));
            Assert.IsInstanceOf<Sending>(transfer.State);
        }
    }
}
