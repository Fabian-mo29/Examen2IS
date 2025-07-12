using Microsoft.AspNetCore.Mvc;
using back_end.Application;
using back_end.Domain;

namespace back_end.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentCommand _paymentCommand;

        public PaymentController(IPaymentCommand paymentCommand)
        {
            _paymentCommand = paymentCommand;
        }

        [HttpPost]
        public ActionResult<List<CashUnit>> Create()
        {
            try
            {
                var cashUnits = _paymentCommand.Create();
                return Ok(cashUnits);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "Error creating payment",
                    details = ex.Message
                });
            }
        }
        
    }
}