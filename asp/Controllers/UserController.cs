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
    [Route("api/user")]
    public class UserController : Controller
    {
        private readonly UserService _resp;

        public UserController(UserService resp)
        {
            _resp = resp;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers(int page , int size )
        {
            var skipAmount = (page - 1) * size;
            List<Users> datas;
            long totalUsers ;
            datas = await _resp.GetAllAsync(skipAmount, size);
            totalUsers = await _resp.CountAsync();

            if (datas != null && datas.Count > 0)
            {
                var response = new
                {
                    message = "success",
                    datas,
                    totalPages = (int)Math.Ceiling((double)totalUsers / size),
                    currentPage = page,
                    totalRecords = totalUsers
                };

                return Ok(response);
            }
            else
            {
                var errorObject = new { error = "Đã xảy ra lỗi" };
                return Json(errorObject);
            }
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
            var response = new
            {
                message = "success",
                data = user
            };
            return Ok(response);
        }
        [HttpGet("tendangnhap/{tendangnhap}")]
        public async Task<IActionResult> GetByTenDangNhapAsync(string tendangnhap)
        {


            var user = await _resp.GetByTenDangNhapAsync(tendangnhap);
            if (user == null)
            {
                return NotFound();
            }
            var response = new {
                message = "success",
                data = user,
            };
            return Ok(response);
        }
        [HttpGet("department/{idDepartment}")]
        public async Task<IActionResult> GetByIdDepartmentAsync(string idDepartment)
        {
            var user = await _resp.GetByIdDepartmentAsync(idDepartment);
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
                return BadRequest(new { errorMessage = "Tên đăng nhập hoặc mật khẩu không chính xác. Xin vui lòng thử lại." });
            }

            var user = await _resp.GetUserByTenDangNhapAndPassword(model.tendangnhap, model.matkhau);
            if (user == null)
            {
                return Ok(new { errorMessage = "Tên đăng nhập hoặc mật khẩu không chính xác. Xin vui lòng thử lại." });
            }
            return Ok(new {message = "Đăng nhập thành công"});
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] Users newEntity)
        {
            if (newEntity == null)
            {
                return BadRequest();
            }
            try
            {
                await _resp.CreateAsync(newEntity);
                var response = new { message = "Thêm người dùng thành công" };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpPost("createMany")]
        public async Task<IActionResult> InsertManyUsers(List<Users> users)
        {
            try
            {
                long insertedCount = await _resp.CreatetManyAsync(users);
                return Ok(new { message = $"Đã thêm thành công {insertedCount} người dùng." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Đã xảy ra lỗi: {ex.Message}");
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] Users updatedUser)
        {
            
            try
            {
                await _resp.UpdateAsync(id, updatedUser);
                return Ok(new { message = "Cập nhật người dùng thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                await _resp.RemoveAsync(id);
                return Ok(new {message = "Xóa người dùng thành công."}); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpDelete("deleteByIds")]
        public async Task<IActionResult> DeleteUsers(List<string> ids)
        {
            try
            {
                var deletedCount = await _resp.DeleteByIdsAsync(ids);
                var response = new { message = $"Xóa thành công {deletedCount} người dùng" };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); 
            }
        }
    }
}
