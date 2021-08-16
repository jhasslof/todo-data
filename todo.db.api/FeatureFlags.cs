using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
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
}
