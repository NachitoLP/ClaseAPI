using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ClaseAPI.Models;

namespace ClaseAPI.Controllers
{
    [Route("GetOperationsByStatus")]
    [ApiController]
    public class BrokerController : ControllerBase
    {
        // GET: BrokerController
        [HttpGet]
        public  List<Actions> GetOperationsByStatus(string? Status, DateTime? OrderDate)
        {
            List<Actions> actions = new List<Actions>();
            actions = Utils.getOrdersFromDB(Status, OrderDate);

            return actions;
        }

        [HttpPost]
        public Actions Post(Actions action)
        {
            Actions newOrder = Utils.PostOrder(action);
            return newOrder;
        }
    }
}
