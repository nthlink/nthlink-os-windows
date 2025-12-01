using nthLink.Header.Interface;
using System.Collections.Generic;

namespace nthLink.Wpf.Model
{
    class BypassSetProvider : IBypassSetProvider
    {
        private HashSet<string>? bypassSet;

        private readonly string[] defaultBypassArray;

        public BypassSetProvider()
        {
            this.defaultBypassArray = new string[]
            {
                "BitComet", // https://www.bitcomet.com/tw
                "uTorrent", // https://www.utorrent.com/intl/zh_tw/
                "BitTorrent", // https://www.bittorrent.com/
                "qbittorrent", // https://www.qbittorrent.org/
                "transmission-daemon", // https://transmissionbt.com/
                "deluged", // https://deluge-torrent.org/
                "vuze", // https://www.vuze.com/
                "FrostWire", // https://www.frostwire.com/
                "emule", // https://www.emule-project.net/home/perl/general.cgi?l=1
                "Shareaza", // http://shareaza.sourceforge.net/
                "Freenet", // https://freenetproject.org/
                "Soulseek" // https://www.slsknet.org/
            };
        }

        public HashSet<string> GetBypassSetNames()
        {
            HashSet<string> result = new HashSet<string>();

            foreach (string item in this.defaultBypassArray)
            {
                result.Add(item);
            }

            if (this.bypassSet != null)
            {
                foreach (string name in this.bypassSet)
                {
                    result.Add(name);
                }
            }

            return result;
        }

        internal void SetBypassNames(HashSet<string> bypassSet)
        {
            this.bypassSet = bypassSet;
        }
    }
}
