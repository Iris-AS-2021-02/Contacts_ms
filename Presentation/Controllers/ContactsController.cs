using BusinessLogic.ServiceContracts;
using Microsoft.AspNetCore.Mvc;
using Support.Dtos;
using Support.Entities;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ContactsController : Controller
    {
        private readonly IContactService _contactService;
        public ContactsController(IContactService contactService)
        {
            _contactService = contactService;
        }

        [HttpGet]
        [Route("GetContacts")]
        public ActionResult<IEnumerable<Contact>> GetContactsByUser(int userId)
        {
            var contacts = _contactService.GetContactsByUserId(userId);
            return Ok(contacts);
        }

        [HttpGet]
        [Route("GetContact")]
        public ActionResult<IEnumerable<Contact>> GetContactById(int contactId)
        {
            var contact = _contactService.GetContactById(contactId);
            if (contact is null)
                return NotFound();

            return Ok(contact);
        }

        [HttpPost]
        [Route("Synchronize")]
        public ActionResult<IEnumerable<Contact>> SynchronizeContacts([FromBody]IEnumerable<PhoneContact> phoneContacts, int userId)
        {
            var synchronizedContacts = _contactService.SynchronizeContacts(phoneContacts, userId);
            return Ok(synchronizedContacts);
        }

        [HttpPost]
        [Route("ChangeOptions")]
        public ActionResult SetSettings([FromBody] ContactSettings contactSettings)
        {
            try
            {
                var result = _contactService.SetSettings(contactSettings);

                if(result)
                    return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
            
            return NotFound();
        }

    }
}
