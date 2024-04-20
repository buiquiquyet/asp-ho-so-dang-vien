/*using asp.Services;*/
using asp.Models;
using asp.Respositories;
using asp.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace asp.Controllers 
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly MongoDBService<Users> _resp;

        public UserController(MongoDBService<Users> resp)
        {
            _resp = resp;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _resp.GetAllAsync();
            return Ok(users);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            if (!IsValidObjectId(id))
            {
                return BadRequest("Invalid ObjectId format.");
            }

            var user = await _resp.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        private bool IsValidObjectId(string id)
        {
            return ObjectId.TryParse(id, out _);
        }
        [HttpPost("/api/auth/login")]
        public async Task<IActionResult> GetUser([FromBody] Users model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { errorMessage = "Email/ Tên đăng nhập hoặc mật khẩu không chính xác. Xin vui lòng thử lại." });
            }

            var user = await _resp.GetUserByEmailOrNameAndPassword(model.email, model.password);
            if (user == null)
            {
                return Ok(new { errorMessage = "Email/ Tên đăng nhập hoặc mật khẩu không chính xác. Xin vui lòng thử lại." });
            }
            return Ok(user);
        }
        [HttpPost("/api/auth/register")]
        public async Task<IActionResult> CreateEntity([FromBody] Users entityModel)
        {
            try
            {
                await _resp.CreateAsync(entityModel);
                return Ok("Tạo tài khoản thành công");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }




        /*[HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, Users updatedUser)
        {
            await _resp.UpdateAsync(id, updatedUser);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            await _resp.RemoveAsync(id);
            return NoContent();
        }*/
    }
}
