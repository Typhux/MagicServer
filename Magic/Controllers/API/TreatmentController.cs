using Magic.Helpers;
using System.Web.Http;

namespace Magic.Controllers.API
{

    public class TreatmentController : ApiController
    {
        private readonly TreatmentHelper _treatmentHelper = new TreatmentHelper();


        [Route("api/treatment")]
        [HttpGet]
        public IHttpActionResult TreatAllNoneTreat()
        {
            return Ok(_treatmentHelper.TreatAllNoneTreat());
        }

        [Route("api/treatment/edition/{id}")]
        [HttpGet]
        public IHttpActionResult TreatEdition(int id)
        {
            return Ok(_treatmentHelper.TreatEdition(id));
        }

        [Route("api/treatment/{codeName}")]
        [HttpGet]
        public IHttpActionResult TreatCard(string codeName)
        {
           return Ok(_treatmentHelper.TreatCard(codeName));
        }

        [Route("api/treatment/setAllAsNotTreated")]
        [HttpGet]
        public IHttpActionResult SetAllAsNotTreated()
        {
            return Ok(_treatmentHelper.SetAllAsNotTreated());
        }
    }
}
