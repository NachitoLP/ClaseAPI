using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ClaseAPI.Models;

namespace ClaseAPI.Controllers
{
    [ApiController]
    public class BrokerController : ControllerBase
    {
        [Route("GetAllOperations")]
        [HttpGet]
        public List<Actions> GetAllOperations()
        {
            List<Actions> actions = new List<Actions>();
            actions = Utils.getAllOrders();

            return actions;
        }

        [Route("GetOperationsByParameter")]
        [HttpGet]
        public List<Actions> GetOperationsByParameter(string? Status, int? year)
        {
            List<Actions> actions = new List<Actions>();
            actions = Utils.getOrdersByParameter(Status, year);

            return actions;
        }

        [Route("PostOperation")]
        [HttpPost]
        public Actions Post(Actions action)
        {
            Actions newOrder = Utils.PostOrder(action);
            return newOrder;
        }

        [Route("ChangeStatus")]
        [HttpPut]
        public Actions Put(int txnumber, string status)
        {
            Actions action = new Actions();
            action = Utils.ChangeStatus(txnumber, status);

            return action;
        }
    }
}
