using Consul;

namespace ServiceDiscoveryLibrary
{
    public class ConsulRegistrar
    {
        private readonly IConsulClient _consulClient;

        public ConsulRegistrar(IConsulClient consulClient)
        {
            _consulClient = consulClient;
        }
        
        public async Task RegisterServiceAsync(string serviceName,
            string serviceId, 
            string address,
            int port)
        {
            var registration = new AgentServiceRegistration
            {
                ID = serviceId,
                Name = serviceName,
                Address = address,
                Port = port,
                Check = new AgentServiceCheck
                {
                    HTTP = $"http://{address}:{port}/health",
                    Interval = TimeSpan.FromSeconds(10)
                }
            };

            await _consulClient.Agent.ServiceRegister(registration);
            Console.WriteLine($"Service {serviceName} registered with Consul.");
        }
        
        public async Task<string> DiscoverServiceAsync(string serviceName)
        {
            var services = await _consulClient.Catalog.Service(serviceName);
            var service = services.Response.FirstOrDefault();
            if (service == null)
                throw new Exception($"Service {serviceName} is not found.");

            return $"http://{service.ServiceAddress}:{service.ServicePort}";
        }
    }
}