using Microsoft.AspNetCore.Mvc;

namespace User_management_api.Controllers
{
    [ApiController, Route("/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_userService.GetAllUsers());
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var user = _userService.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(user);
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] User user)
        {
            if (user == null || string.IsNullOrEmpty(user.Name))
            {
                return BadRequest("Invalid user data");
            }
            var createdUser = _userService.AddUser(user);
            return CreatedAtAction(nameof(GetById), new { id = createdUser.Id }, createdUser);
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            bool isDeleted = _userService.DeleteUserById(id);
            if (isDeleted)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }
    }
}