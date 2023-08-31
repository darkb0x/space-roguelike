using System;
using System.Collections.Generic;

namespace Game
{
    public static class ServiceLocator
    {
        private static Dictionary<Type, IService> _services = new Dictionary<Type, IService>();

        public static void Register(IService service)
        {
            if(!HaveService(service))
            {
                _services.Add(service.GetType(), service);
                LogUtility.WriteLog($"Registered: {service.GetType().Name} service");
            }
        }
        public static void Unregister(IService service)
        {
            if (HaveService(service))
            {
                _services.Remove(service.GetType());
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
            if(HaveService(typeof(T)))
            {
                return _services[typeof(T)] as T;
            }

            return default;
        }

        private static bool HaveService(IService service)
        {
            if (_services.ContainsKey(service.GetType()))
                return true;

            return false;
        }
        private static bool HaveService(Type type)
        {
            if (_services.ContainsKey(type))
                return true;

            return false;
        }
    }
}
