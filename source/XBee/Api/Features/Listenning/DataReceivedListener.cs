﻿using System;

namespace NETMF.OpenSource.XBee.Api
{
    public class DataReceivedListener : IPacketListener
    {
        private readonly XBee _xbee;

        public DataReceivedListener(XBee xbee)
        {
            _xbee = xbee;
        }

        public bool Finished
        {
            get { return false; }
        }

        public void ProcessPacket(XBeeResponse packet)
        {
            if (packet is Wpan.RxResponse)
            {
                var response = packet as Wpan.RxResponse;
                _xbee.NotifyDataReceived(response.Payload, response.Source);
            }
            else if (packet is Zigbee.RxResponse)
            {
                if (packet is Zigbee.ExplicitRxResponse)
                {
                    var profileId = (packet as Zigbee.ExplicitRxResponse).ProfileId;
                    var clusterId = (packet as Zigbee.ExplicitRxResponse).ClusterId;

                    // if module AtCmd.ApiOptions has been set to value other than default (0)
                    // received API frames will be transported using explicit frames
                    // those frames have profile id set to Zigbee.ProfileId.Digi
                    if (profileId != (ushort)Zigbee.ProfileId.Digi)
                        return;

                    // cluster id will be set to ApiId value
                    switch ((ApiId)clusterId)
                    {
                        case ApiId.TxRequest16:
                        case ApiId.TxRequest64:
                        case ApiId.ZnetTxRequest:
                        case ApiId.ZnetExplicitTxRequest:
                            break;

                        default:
                            return;
                    }
                }

                var response = packet as Zigbee.RxResponse;
                _xbee.NotifyDataReceived(response.Payload, response.SourceSerial);
            }
        }

        public XBeeResponse[] GetPackets(int timeout)
        {
            throw new NotSupportedException();
        }
    }
}