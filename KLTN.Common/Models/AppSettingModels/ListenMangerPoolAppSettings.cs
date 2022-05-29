using System;
using System.Collections.Generic;
using System.Text;

namespace KLTN.Common.Models
{
    public class ListenMangerPoolAppSettings
    {
        public static ListenMangerPoolAppSettings Value { get; private set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string RpcUrl { get; set; }
        public string WssUrl { get; set; }
        public int ChainNetworkId { get; set; }
        public string ManagerPoolContractAddress { get; set; }
        public string PrivateKey { get; set; }
        public static void SetValue(ListenMangerPoolAppSettings configuration)
        {
            var properties = configuration.GetType().GetProperties();
            foreach (var property in properties)
            {
                if (property.Name == nameof(Value)) continue;
                var value = property.GetValue(configuration);
                if (value == null)
                {
                    throw new Exception($"Config {property.Name} in App Setting is null");
                }
            }
            Value = configuration;
        }
    }
}
