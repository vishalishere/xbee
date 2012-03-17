﻿namespace NETMF.OpenSource.XBee.Api.At
{
    /// <summary>
    /// AT Command Queue
    /// API ID: 0x9
    /// </summary>
    public class AtCommandQueue : AtCommand
    {
        public AtCommandQueue(ushort command, byte[] value = null) 
            : base(command, value)
        {
        }

        public override ApiId ApiId
        {
            get { return ApiId.AtCommandQueue; }
        }
    }
}