using System.Collections.Generic;
using System.Configuration;
using System.Net;
using Nbtc.Util;

namespace Nbtc.Client
{
    public sealed class DnsSeeder
    {
        private readonly ILogger _logger;

        public DnsSeeder(ILogger logger)
        {
            _logger = logger.For<DnsSeeder>();
        }
        private readonly DnsSeederConfig _config = new DnsSeederConfig();

        public IEnumerable<(string, IPAddress[])> Seed()
        {
            var seeds = _config.Seeds();
            _logger.Debug("Seed {@seeds}", seeds);
            foreach (SeedElement seed in seeds)
            {
                IPAddress[] ips = Dns.GetHostAddresses(seed.Value);
                yield return (seed.Value, ips);
            }
        }
    }
    
    public class DnsSeederConfig
    {
        private readonly DnsSeederSection _config = ConfigurationManager.GetSection("dnsSeederSection") as DnsSeederSection;
        public SeedElementCollection Seeds()
        {
            return _config.Seeds;
        }
    }
    
    public class DnsSeederSection : ConfigurationSection
    {
        [ConfigurationProperty("seeds")]
        public SeedElementCollection Seeds
        {
            get { return (SeedElementCollection)this["seeds"]; }
        }
    }

    [ConfigurationCollection(typeof(SeedElement))]
    public class SeedElementCollection : ConfigurationElementCollection
    {
        public SeedElement this[int index]
        {
            get { return (SeedElement)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                    BaseRemoveAt(index);

                BaseAdd(index, value);
            }
        }
        protected override ConfigurationElement CreateNewElement()
        {
            return new SeedElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((SeedElement)element).Value;
        }
    }
    
    public class SeedElement : ConfigurationElement
    {
        public SeedElement() { }

        [ConfigurationProperty("value", DefaultValue="", IsKey = true, IsRequired = true)]
        public string Value
        {
            get { return (string)this["value"]; }
            set { this["value"] = value; }
        }
    }
}