using System;
using System.Collections.Generic;
using System.Linq;

namespace Game
{
    public static class ServiceLocator
    {
        private static List<IService> _services = new List<IService>();

        public static void Register(IService service)
        {
            if (_services.Contains(service))
                throw new Exception($"{service.GetType().Name} already registered!");

            _services.Add(service);
            LogUtility.WriteLog($"Registered: {service.GetType().Name} service");
        }
        public static void Unregister(IService service)
        {
            if (_services.Contains(service))
            {
                _services.Remove(service);
                LogUtility.WriteLog($"Unregistered: {service.GetType().Name} service");
            }
        }
        public static void UnregisterAllServices()
        {
            _services.Clear();
            LogUtility.WriteLog("Unregistered all services");
        }
        public static T GetService<T>() where T : class, IService
        {
            var service = (T)_services.FirstOrDefault(x => x is T);

            if (service == default(T))
                throw new NullReferenceException($"No service: {typeof(T).Name} in list!");

            return service;
        }
        public static T TryGetService<T>() where T : class, IService
        {
            var service = (T)_services.FirstOrDefault(x => x is T);

            return service;
        }
    }
}
