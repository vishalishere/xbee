﻿using System.Threading;
using Microsoft.SPOT;
using NETMF.OpenSource.XBee.Api;
using NETMF.OpenSource.XBee.Api.Wpan;
using NETMF.OpenSource.XBee.Util;

namespace NETMF.Tester
{
    public static class WpanTest
    {
        public static void Run(XBee xbee1, XBee xbee2)
        {
            Debug.Print("XBee 1: " + (xbee1.Send(AtCmd.CoordinatorEnable).GetResponsePayload()[0] > 0 ? "coordinator" : "end device"));
            Debug.Print("XBee 2: " + (xbee2.Send(AtCmd.CoordinatorEnable).GetResponsePayload()[0] > 0 ? "coordinator" : "end device"));
            
            Debug.Print("Performing energy scan...");
            var result = xbee1.Send(AtCmd.EnergyScan, new byte[] { 3 }).GetResponsePayload();
            for (var i = 0; i < result.Length; i++)
                Debug.Print("Channel " + (i + 0x0B) + ": " + result[i] + "-dBi");

            Debug.Print("Active channel: " + xbee1.Send(AtCmd.Channel).GetResponsePayload()[0]);

            // disovering nodes

            Debug.Print("Discovering nodes from xbee 1...");
            var foundNodes = xbee1.DiscoverNodes();

            Debug.Print("Found: " + foundNodes.Length + " nodes");

            for (var i = 0; i < foundNodes.Length; i++)
                Debug.Print("#" + (i + 1) + " - " + foundNodes[i]);

            Debug.Print("Discovering nodes from xbee 2...");
            foundNodes = xbee2.DiscoverNodes();

            Debug.Print("Found: " + foundNodes.Length + " nodes");

            for (var i = 0; i < foundNodes.Length; i++)
                Debug.Print("#" + (i + 1) + " - " + foundNodes[i]);

            // setting address 1 to xbee1 and address 2 to xbee2 if not available

            Debug.Print("Setting addresses...");

            var xbee1Address = GetAddress(xbee1);
            var xbee2Address = GetAddress(xbee2);

            xbee1Address.Address = 1;
            SetAddress(xbee1, xbee1Address);

            xbee2Address.Address = 2;
            SetAddress(xbee2, xbee2Address);

            Debug.Print("XBee 1 new address: " + xbee1Address);
            Debug.Print("XBee 2 new address: " + xbee2Address);

            // checking xbee I/O
            // after we send AtCommand we receive AtResponse followed by IoSampleResponse
            // the second response occurs only if the module has any I/O enabled (?)

            Debug.Print("Checking I/O of both XBee");

            var listener = new PacketListener(new PacketCountFilter(2, typeof(IoSampleResponse)));

            xbee1.AddPacketListener(listener);
            xbee2.AddPacketListener(listener);

            xbee1.Send(AtCmd.ForceSample).NoResponse();
            xbee2.Send(AtCmd.ForceSample).NoResponse();

            var packets = listener.GetPackets(5000);

            foreach (var packet in packets)
                Debug.Print("I/O sample of address " + ((IoSampleResponse)packet).Source + ": " + packet);  

            if (packets.Length == 0)
                Debug.Print("Didn't receive any I/O packet");

            // if the listener is finished it will be discarded automatically
            // if the listener is not finished (didn't receive 2 frames) we can remove it manualy
            if (!listener.Finished)
            {
                xbee1.RemovePacketListener(listener);
                xbee2.RemovePacketListener(listener);
            }

            // performing unicast message sending

            var xbee1Serial = xbee1.Config.SerialNumber;
            var xbee2Serial = xbee2.Config.SerialNumber;

            xbee1.DataReceived += OnDataReceived;
            xbee2.DataReceived += OnDataReceived;

            const string message1 = "serial unicast";
            Debug.Print(xbee1Address + " -> " + xbee2Serial + " (" + message1 + ")");
            xbee1.Send(message1).To(xbee2Serial).NoResponse();

            Thread.Sleep(1000);

            const string message2 = "address unicast";
            Debug.Print(xbee2Address + " -> " + xbee1Address + " (" + message2 + ")");
            xbee2.Send(message2).To(xbee1Address).NoResponse();

            Thread.Sleep(1000);

            // performing broadcast message sending

            const string message3 = "serial broadcast";
            Debug.Print(xbee1Address + " -> " + XBeeAddress64.Broadcast + " (" + message3 + ")");
            xbee1.Send(message3).To(XBeeAddress64.Broadcast).NoResponse();

            Thread.Sleep(1000);

            const string message4 = "address broadcast";
            Debug.Print(xbee1Address + " -> "+ XBeeAddress16.Broadcast + " (" + message4 + ")");
            xbee1.Send(message4).To(XBeeAddress16.Broadcast).NoResponse();
        }

        private static void SetAddress(XBee xbee, XBeeAddress16 address)
        {
            var request = xbee.Send(AtCmd.SourceAddress, (address as XBeeAddress).Address);
            request.GetResponse();
        }

        private static XBeeAddress16 GetAddress(XBee xbee)
        {
            var request = xbee.Send(AtCmd.SourceAddress);
            return new XBeeAddress16(request.GetResponsePayload());
        }

        private static void OnDataReceived(XBee receiver, byte[] data, XBeeAddress sender)
        {
            Debug.Print(receiver.Config.SerialNumber + " <- '" + Arrays.ToString(data) + "' from " + sender);
        }
    }
}