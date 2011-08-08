using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using NLog;
using NServiceBus.Utils.Reflection;

namespace NServiceBus.Host.Internal
{
    /// <summary>
    /// Scans and loads profile handlers from the given assemblies
    /// </summary>
    public class ProfileManager
    {
        private readonly IEnumerable<Assembly> assembliesToScan;
        private readonly IEnumerable<Type> activeProfiles;
        private readonly IConfigureThisEndpoint specifier;

        /// <summary>
        /// Initializes the manager with the assemblies to scan and the endpoint configuration to use
        /// </summary>
        /// <param name="assembliesToScan"></param>
        /// <param name="specifier"></param>
        /// <param name="args"></param>
        public ProfileManager(IEnumerable<Assembly> assembliesToScan, IConfigureThisEndpoint specifier, string[] args)
        {
            this.assembliesToScan = assembliesToScan;
            this.specifier = specifier;

            activeProfiles = new List<Type>(GetProfilesFrom(assembliesToScan).Where(t => args.Any(a => t.FullName.ToLower() == a.ToLower())));

            if (activeProfiles.Count() == 0)
                activeProfiles = DefaultProfile;
        }

        /// <summary>
        /// Activates the profilehandlers that handle the previously identified active profiles. 
        /// </summary>
        /// <returns></returns>
        public void ActivateProfileHandlers()
        {
            foreach (var p in activeProfiles)
                Logger.Info("Going to activate profile: " + p.AssemblyQualifiedName);

            var handlers = new List<Type>();

            foreach (var assembly in assembliesToScan)
                foreach (var type in assembly.GetTypes())
                {
                    if (null != type.GetGenericallyContainedType(typeof (IHandleProfile<>), typeof (IProfile)))
                        handlers.Add(type);
                }

            var activeHandlers = handlers.Where(t => activeProfiles.Any(p => typeof(IHandleProfile<>).MakeGenericType(p).IsAssignableFrom(t)));

            var profileHandlers = new List<IHandleProfile>();
            foreach (var h in activeHandlers)
            {
                profileHandlers.Add(Activator.CreateInstance(h) as IHandleProfile);
                Logger.Debug("Activating profile handler: " + h.AssemblyQualifiedName);
            }

            profileHandlers.Where(ph => ph is IWantTheEndpointConfig).ToList().ForEach(
                ph => (ph as IWantTheEndpointConfig).Config = specifier);

            profileHandlers.ForEach(hp => hp.ProfileActivated());
        }


        private static IEnumerable<Type> GetProfilesFrom(IEnumerable<Assembly> assembliesToScan)
        {
            IEnumerable<Type> profiles = new List<Type>();

            foreach (var assembly in assembliesToScan)
                profiles = profiles.Union(assembly.GetTypes().Where(t => typeof(IProfile).IsAssignableFrom(t) && !t.IsInterface));

            return profiles;
        }

        private static readonly IEnumerable<Type> DefaultProfile = new[] { typeof(Lite) };
        private static Logger Logger = LogManager.GetCurrentClassLogger();
    }
}