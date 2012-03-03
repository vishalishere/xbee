﻿using System;

namespace Gadgeteer.Modules.GHIElectronics.Util
{
    public interface IOutputStream : IDisposable
    {
        void Write(byte data);
        void Write(int data);
        void Write(ushort data);
        void Write(string data);
        void Write(byte[] data);
        byte[] ToArray();
    }
}