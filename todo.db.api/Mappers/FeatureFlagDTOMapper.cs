using System.Collections.Generic;
using todo.db.api.Models;
using todo.db.Models;

namespace todo.db.api.Mappers
{
    public class FeatureFlagDTOMapper
    {
        public static FeatureFlagDTO Map(SupportedFeature supportedFeature)
        {
            if (supportedFeature == null)
                return null;

            // Database flags can not be turned off
            return new FeatureFlagDTO
            {
                Key = supportedFeature.Key,
                State = true
            }; 
        }

        public static List<FeatureFlagDTO> Map(List<SupportedFeature> supportedFeatures)
        {
            return supportedFeatures.ConvertAll(new System.Converter<SupportedFeature, FeatureFlagDTO>(Map));
        }
    }
}
