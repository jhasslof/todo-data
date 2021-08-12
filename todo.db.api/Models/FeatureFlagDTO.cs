using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace todo.db.api.Models
{
    public class FeatureFlagDTO
    {
        public string Key { get; set; }

        private bool state = false;
        public bool State
        {
            get
            {
                if (PreReqOk())
                {
                    return state;
                }
                return false;
            }
            set
            {
                state = value;
            }
        }

        [JsonIgnore]
        public List<FeatureFlagDTO> PreReqKeys { get; set; } = new List<FeatureFlagDTO>();

        private bool PreReqOk()
        {
            return !PreReqKeys.Any(f => f.State == false);
        }

        public bool GetInternalState()
        {
            // Used to copy the internal state
            return state;
        }

        public override bool Equals(object obj)
        {
            return this.Key == ((FeatureFlagDTO)obj).Key;
        }
        public override int GetHashCode()
        {
            //removes CS0659
            return base.GetHashCode();
        }
    }
}
