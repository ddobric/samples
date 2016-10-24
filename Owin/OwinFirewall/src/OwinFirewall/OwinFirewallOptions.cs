using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Daenet.Owin
{
    public class OwinFirewallOptions
    {
        private List<string> m_MatchingAddresses = new List<string>();

        public List<string> AllowedIPs
        {
            get
            {
                return m_MatchingAddresses;
            }
        }

        public OwinFirewallOptions(IEnumerable<string> cidrList)
        {
            foreach (var cidr in cidrList)
            {
                int addressCounter = 0;

                var tokens = cidr.Split('/');
                if (tokens.Length == 2)
                {
                    int mask ;
                    if (int.TryParse(tokens[1], out mask))
                    {
                        mask = 32 - mask;
                        IPAddress netAddr = IPAddress.Parse(tokens[0]);
                        for (int i = 0; i < Math.Pow(2, mask); i++)
                        {
                            long intAddress = (long)(uint)IPAddress.NetworkToHostOrder((int)BitConverter.ToInt32(netAddr.GetAddressBytes(),0));
                            addressCounter++;
                            string ipAddress = IPAddress.Parse((intAddress + addressCounter).ToString()).ToString();

                            m_MatchingAddresses.Add(ipAddress);

                        }
                    }
                }
                else if (tokens.Length == 1)
                {
                    m_MatchingAddresses.Add(tokens[0]);
                }
            }
        }


    }
}
