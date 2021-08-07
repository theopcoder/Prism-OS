﻿using Cosmos.System.Network.IPv4.TCP;
using Cosmos.System.Network.IPv4.UDP.DHCP;
using Cosmos.System.Network.IPv4.UDP.DNS;
using Cosmos.System.Network.IPv4;
using System.Net;
using System.Text;
using EndPoint = Cosmos.System.Network.IPv4.EndPoint;
using Cosmos.System.Network.Config;

namespace PrismProject.System2
{
    internal class Network
    {
        /// <summary>
        /// Get local address for current machine.
        /// </summary>
        /// <returns>IP adress (as string)</returns>
        public static string GetLoaclAddress()
        {
            return NetworkConfig.CurrentConfig.Value.IPAddress.ToString();
        }

        /// <summary>
        /// Always run ip/domains through this, it will automaticly parse a url/ip and get the ip or return error
        /// </summary>
        /// <param name="serverIP"></param>
        /// <returns>ip address or error</returns>
        public static dynamic ParseURL(string serverIP)
        {
            byte[] newIP = Convert.ToByteArray(serverIP);
            if (IPAddress.TryParse(serverIP, out IPAddress addr))
                return new Address(newIP[0], newIP[1], newIP[2], newIP[3]);

            if (!IPAddress.TryParse(serverIP, out addr))
                using (var xClient = new DnsClient())
                {
                    xClient.Connect(new Address(1, 1, 1, 1));
                    xClient.SendAsk(newIP.ToString());
                    return xClient.Receive(10000);
                }

            else
                return false;
            }

        /// <summary>
        /// get method using TCP
        /// </summary>
        /// <param name="serverIP"></param>
        /// <returns></returns>
        public static string GetTCP(string serverIP)
        {
            Address loc = Network.ParseURL(serverIP);
            using (var xClient = new TcpClient(80))
                {
                    xClient.Connect(loc, 80);
                    var endpoint = new EndPoint(loc, 80);
                    var data = xClient.Receive(ref endpoint);  //set endpoint to remote machine IP:port
                    var data2 = xClient.NonBlockingReceive(ref endpoint); //retrieve receive buffer without waiting
                    return data2.ToString();
                }
        }

        /// <summary>
        /// Obtain a useable IP adress from a router, if connected.
        /// </summary>
        public static void DHCP()
        {
            using (var xClient = new DHCPClient())
            {
                /** Send a DHCP Discover packet **/
                xClient.SendDiscoverPacket();
            }
        }

        /// <summary>
        /// Ping a specified IP address or hostname.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static dynamic Ping(string address, int port, int timeout = 5000)
        {
            dynamic Connection = ParseURL(address);
            if (!Connection)
                return null;
            else
                using (var client = new ICMPClient())
                {
                    var point = new EndPoint(Address.Zero, (ushort)port);
                    client.Connect(Connection);
                    client.SendEcho();
                    return client.Receive(ref point, timeout);
                }
        }
    }
}