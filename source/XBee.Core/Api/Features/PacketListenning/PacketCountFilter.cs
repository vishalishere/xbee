﻿using System;

namespace Gadgeteer.Modules.GHIElectronics.Api
{
    public class PacketCountFilter : PacketTypeFilter
    {
        private int _expectedCount;
        private bool _finished;

        public PacketCountFilter(int expectedCount, Type expectedType)
            : base(expectedType)
        {
            _expectedCount = expectedCount;
        }

        public override bool Accepted(XBeeResponse packet)
        {
            if (!base.Accepted(packet))
                return false;

            if (_expectedCount-- > 0)
                return true;
        
            _finished = true;
            return false;
        }

        public override bool Finished()
        {
            return _finished;
        }
    }
}