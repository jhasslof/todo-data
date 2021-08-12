using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace todo.db.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoFeatureFlagsController : Controller
    {

        private readonly IFeatureFlags _featureFlags;

        public TodoFeatureFlagsController(IFeatureFlags featureFlags)
        {
            _featureFlags = featureFlags;
        }

        // GET: api/TodoFeatureFlags
        [HttpGet]
        public IEnumerable<Models.FeatureFlagDTO> Index()
        {
            return _featureFlags.GetFeatureFlagList();
        }
    }
}
