﻿using Cosmos.HAL;
using Cosmos.System.Network.Config;
using Cosmos.System.Network.IPv4;
using Cosmos.System.Network.IPv4.TCP;
using Cosmos.System.Network.IPv4.UDP.DHCP;
using Cosmos.System.Network.IPv4.UDP.DNS;
using System;

namespace PrismOS.Network
{
    public static class Framework
    {
        public static Address GateWay { get; } = new(10, 0, 0, 1);
        public static Address Local { get; } = new(127, 0, 0, 1);
        public static Address Subnet { get; } = new(255, 255, 255, 0);
        public static Address DNS { get; } = new(1, 1, 1, 1);

        public static void Init()
        {
            IPConfig.Enable(GetNetworkDevice("eth0"), Local, Subnet, GateWay);

            using DHCPClient xClient = new();
            xClient.SendDiscoverPacket();
            xClient.Dispose();
        }

        public static Address GetLocalAddress()
        {
            return DHCPClient.DHCPServerAddress(GetNetworkDevice("eth0"));
        }

        public static NetworkDevice GetNetworkDevice(string DeviceName)
        {
            return NetworkDevice.GetDeviceByName(DeviceName);
        }

        public static Address AskFor(string Domain)
        {
            try
            {
                using DnsClient xClient = new();
                xClient.Connect(DNS);
                xClient.SendAsk(Domain);
                return xClient.Receive();
            }
            catch (Exception aException)
            {
                throw new Exception("An error occured in the DNS system.\n(" + aException.Message + ")");
            }
        }

        public static void ByteShark(Address IP, int Port)
        {
            EndPoint end = new(Address.Zero, (ushort)Port);
            TcpClient clnt = new(IP, Port);
            clnt.Connect(IP, Port);

            while (true)
            {
                clnt.Send(System.Text.Encoding.UTF8.GetBytes("Hey, if you can read this, i am just testing shtuff. no need to be worried."));
                Console.WriteLine("Raw Data: " + clnt.Receive(ref end));
            }
        }
    }
}