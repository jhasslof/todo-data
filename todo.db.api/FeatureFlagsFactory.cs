﻿using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using todo.db.api.Mappers;
using todo.db.api.Models;

namespace todo.db.api
{
    public interface IFeatureFlags
    {
        IEnumerable<FeatureFlagDTO> GetFeatureFlagList();

        bool FeatureFlagIsActive(string featureFlagKey);
    }

    public class FeatureFlags : IFeatureFlags
    {
        IEnumerable<FeatureFlagDTO> _featureFlags;

        public FeatureFlags(IConfiguration configuration, ITodoDbContext context)
        {
            _featureFlags = FeatureFlagsFactory.GetFeatureFlagsInUse(configuration, context);
        }

        IEnumerable<FeatureFlagDTO> IFeatureFlags.GetFeatureFlagList()
        {
            return _featureFlags;
        }

        public bool FeatureFlagIsActive(string featureFlagKey)
        {
            var flag = _featureFlags.SingleOrDefault(f => f.Key == featureFlagKey);
            return (flag == null ? false : flag.State);
        }
    }

    public class FeatureFlagsFactory
    {
        private static readonly object bindConfigLock = new object();

        private static LaunchDarklyCredentials LaunchDarklyCredentials { get; set; }

        private readonly static IEnumerable<FeatureFlagDTO> _apiSupportedFeatureFlags = new[] {
            new FeatureFlagDTO {
                Key = "todo.db.api-TodoItems",
                PreReqKeys = new List<FeatureFlagDTO>{
                    new FeatureFlagDTO { Key = "todo.db-TodoItems" }
                }
            }
        };

        public static IEnumerable<FeatureFlagDTO> GetFeatureFlagsInUse(IConfiguration configuration, ITodoDbContext context)
        {
            if (null == LaunchDarklyCredentials)
            {
                InitializeFeatureFlagConfiguration(configuration);
            }

            // We can only activate feature flags when all pre-req features are active
            // If pre-req flags is empty, the flag is only implemented inside this servive

            var featurFlagsInUse = CloneFlags(_apiSupportedFeatureFlags);
            IEnumerable<FeatureFlagDTO> dbSupportedFeatureFlags = GetDbSupportedFeatureFlags(context);

            //Validate pre req
            foreach (var apiFlag in featurFlagsInUse)
            {
                foreach (var dbFlag in apiFlag.PreReqKeys)
                {
                    dbFlag.State = dbSupportedFeatureFlags.Any(f => f.Key == dbFlag.Key);
                }
            }

            if (LaunchDarklyCredentials.LdClient != null)
            {
                foreach (var featureFlag in featurFlagsInUse)
                {
                    featureFlag.State = LaunchDarklyCredentials.LdClient.BoolVariation(featureFlag.Key, LaunchDarklyCredentials.LdUser, false);
                }
            }
            return featurFlagsInUse;
        }

        private static void InitializeFeatureFlagConfiguration(IConfiguration configuration)
        {
            lock (bindConfigLock)
            {

                var flagDefaultValuesConfig = configuration.GetSection("FeatureFlags:Defaults").GetChildren().ToList();
                foreach (var flagDefaultValueConfig in flagDefaultValuesConfig)
                {
                    var flagDefault = new FeatureFlagDefault();
                    flagDefaultValueConfig.Bind(flagDefault);
                    var featureFlag = _apiSupportedFeatureFlags.Single(f => f.Key == flagDefault.Key);
                    featureFlag.State = flagDefault.State;
                }
                LaunchDarklyCredentials = new LaunchDarklyCredentials();
                configuration.GetSection("FeatureFlags:LaunchDarklyCredentials").Bind(LaunchDarklyCredentials);
            }
        }

        private static IEnumerable<FeatureFlagDTO> GetDbSupportedFeatureFlags(ITodoDbContext context)
        {
            var task = Task.Run(() => context.ListSupportedFeatures());
            var dbFlags = (IEnumerable<db.Models.SupportedFeature>)task.GetType().GetProperty("Result").GetValue(task);
            IEnumerable<FeatureFlagDTO> dbSupportedFeatureFlags = dbFlags.ToList().ConvertAll(new Converter<db.Models.SupportedFeature, FeatureFlagDTO>(FeatureFlagDTOMapper.Map));
            return dbSupportedFeatureFlags;
        }

        private static IEnumerable<FeatureFlagDTO> CloneFlags(IEnumerable<FeatureFlagDTO> flags)
        {
            var flagsCopy = new List<FeatureFlagDTO>();
            foreach (var flag in flags)
            {
                var flagCopy = new FeatureFlagDTO { Key = flag.Key, State = flag.GetInternalState() };
                foreach (var preReqFlag in flag.PreReqKeys)
                {
                    flagCopy.PreReqKeys.Add(new FeatureFlagDTO { Key = preReqFlag.Key, State = preReqFlag.GetInternalState() });
                }
                flagsCopy.Add(flagCopy);
            }
            return flagsCopy.ToArray();
        }
    }

    public class FeatureFlagDefault
    {
        public string Key { get; set; }
        public bool State { get; set; }
    }

    public class LaunchDarklyCredentials
    {
        private string user;
        private string sdkKey;

        public string User
        {
            get => user;
            set
            {
                if (!string.IsNullOrEmpty(value) && string.IsNullOrEmpty(user))
                {
                    user = value;
                    LdUser = LaunchDarkly.Sdk.User.WithKey(value);
                }
            }

        }
        public string SdkKey
        {
            get => sdkKey;
            set
            {
                if (!string.IsNullOrEmpty(value) && string.IsNullOrEmpty(sdkKey))
                {
                    sdkKey = value;
                    LdClient = new LaunchDarkly.Sdk.Server.LdClient(value);
                }

            }
        }

        //LdClient should be a singleton
        //https://docs.launchdarkly.com/sdk/server-side/dotnet
        public LaunchDarkly.Sdk.Server.LdClient LdClient { get; private set; }

        public LaunchDarkly.Sdk.User LdUser { get; private set; }

    }
}