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
        public async Task<ActionResult<IEnumerable<Contact>>> GetContactsByUser(string userId)
        {
            var contacts = await _contactService.GetContactsByUserId(userId);
            return Ok(contacts);
        }

        [HttpGet]
        [Route("GetContact")]
        public async Task<ActionResult<Contact>> GetContactById(Guid contactId)
        {
            var contact = await _contactService.GetContactById(contactId);
            if (contact is null)
                return NotFound();

            return Ok(contact);
        }

        [HttpPost]
        [Route("Synchronize")]
        public async Task<ActionResult<IEnumerable<Contact>>> SynchronizeContacts([FromBody]IEnumerable<PhoneContact> phoneContacts, string userId)
        {
            try
            {
                var synchronizedContacts = await _contactService.SynchronizeContacts(phoneContacts, userId);
                return Ok(synchronizedContacts);
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpPost]
        [Route("ChangeOptions")]
        public async Task<ActionResult<ContactSettingsResponse>> SetSettings([FromBody] ContactSettingsRequest contactSettings)
        {
            try
            {
                var result = await _contactService.SetSettings(contactSettings);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }
    }
}
