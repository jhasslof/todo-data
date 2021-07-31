using Microsoft.AspNetCore.Mvc;
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

        // GET: api/TodoFeatureFlags
        [HttpGet]
        public IEnumerable<Models.FeatureFlagDTO> Index()
        {
            return new[] { new Models.FeatureFlagDTO { Key = "todo-extra-info" } };
        }
    }
}
