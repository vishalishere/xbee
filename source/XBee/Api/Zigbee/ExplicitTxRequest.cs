﻿using NETMF.OpenSource.XBee.Util;

namespace NETMF.OpenSource.XBee.Api.Zigbee
{
    /// <summary>
    /// Series 2 XBee.  Sends a packet to a remote radio.  
    /// The remote radio receives the packet as a ExplicitRxResponse packet.
    /// API ID: 0x11
    /// </summary>
    public class ExplicitTxRequest : TxRequest
    {
        public enum Endpoint
        {
            ZdoEndpoint = 0,
            Command = 0xe6,
            Data = 0xe8
        }

        public enum ClusterIds
        {
            TransparentSerial = 0x11,
            SerialLoopback = 0x12,
            IOSample = 0x92,
            XbeeSensor = 0x94,
            NodeIdentification = 0x95
        }

        public byte SourceEndpoint { get; set; }
        public byte DestinationEndpoint { get; set; }
        public ushort ClusterId { get; set; }
        public ushort ProfileId { get; set; }

        public static ushort ZnetProfileId = 0xC105;
        public static ushort ZdoProfileId = 0x0000;

        // this is one big ctor ;)

        public ExplicitTxRequest(XBeeAddress64 destSerial, XBeeAddress16 destAddress, byte[] payload, byte srcEndpoint, byte destEndpoint, ushort clusterId, ushort profileId) 
            : base(destSerial, payload)
        {
            DestinationAddress = destAddress;
            SourceEndpoint = srcEndpoint;
            DestinationEndpoint = destEndpoint;
            ClusterId = clusterId;
            ProfileId = profileId;
        }

        public override byte[] GetFrameData()
        {
            // get frame id from tx request
            var frameData = GetFrameDataAsIntArrayOutputStream();

            // api id
            frameData.Write((byte)ApiId);

            // frame id (arbitrary byte that will be sent back with ack)
            frameData.Write(FrameId);

            // add 64-bit dest address
            frameData.Write(DestinationSerial.Address);

            // add 16-bit dest address
            frameData.Write(DestinationAddress.Address);

            // source endpoint
            frameData.Write(SourceEndpoint);
            // dest endpoint
            frameData.Write(DestinationEndpoint);
            // cluster id
            frameData.Write(ClusterId);
            // profile id
            frameData.Write(ProfileId);

            // write broadcast radius
            frameData.Write(BroadcastRadius);

            // write options byte
            frameData.Write((byte)Option);

            frameData.Write(Payload);

            return frameData.ToArray();
        }

        public override ApiId ApiId
        {
            get { return ApiId.ZnetExplicitTxRequest; }
        }

        public override string ToString()
        {
            return base.ToString() +
                ",sourceEndpoint=" + ByteUtils.ToBase16(SourceEndpoint) +
                ",destinationEndpoint=" + ByteUtils.ToBase16(DestinationEndpoint) +
                ",clusterId=" + ByteUtils.ToBase16(ClusterId) +
                ",profileId=" + ByteUtils.ToBase16(ProfileId);
        }
    }
}