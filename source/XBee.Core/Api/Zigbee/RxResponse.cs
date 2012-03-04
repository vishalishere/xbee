namespace Gadgeteer.Modules.GHIElectronics.Api.Zigbee
{
    /// <summary>
    /// Series 2 XBee. This packet is received when a remote XBee sends a TxRequest
    /// API ID: 0x90
    /// </summary>
    /// <remarks>
    /// ZNet RX packets do not include RSSI since it is a mesh network and potentially requires several
    /// hops to get to the destination.  The RSSI of the last hop is available using the DB AT command.
    /// If your network is not mesh (i.e. composed of a single coordinator and end devices -- no routers) 
    /// then the DB command should provide accurate RSSI.
    /// </remarks>
    public class RxResponse : XBeeResponse, INoRequestResponse
    {
        public enum Options
        {
            PacketAcknowledged = 0x01,
            BroadcastPacket = 0x02
        }

        public XBeeAddress64 SourceSerial { get; set; }
        public XBeeAddress16 SourceAddress { get; set; }
        public Options Option { get; set; }
        public byte[] Payload { get; set; }

        public override void Parse(IPacketParser parser)
        {
            SourceSerial = parser.ParseAddress64();
            SourceAddress = parser.ParseAddress16();
            Option = (Options)parser.Read("ZNet RX Option");
            Payload = parser.ReadRemainingBytes();	
        }

        public override string ToString()
        {
            return base.ToString()
                   + ",sourceSerial=" + SourceSerial
                   + ",sourceAddress=" + SourceAddress
                   + ",option=" + Option
                   + ",payload=byte[" + Payload.Length + "]";
        }
    }
}