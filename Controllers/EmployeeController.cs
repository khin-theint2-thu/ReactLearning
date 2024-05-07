using App.Models;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
namespace App.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly ILogger<EmployeeController> _logger;
        private readonly ReactDBContext _DBContext;

        public EmployeeController(ILogger<EmployeeController> logger)
        {
            _logger = logger;
            _DBContext = CommonController.CreateMainDbContext();
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            try
            {
                List<Employee> dataRecords = _DBContext.Employees.ToList();
                return Ok(new { Status = "Success", Data = dataRecords });
            }
            catch (CultureNotFoundException ex)
            {
                _logger.LogError(ex, "CultureNotFoundException occurred while processing GetAll.");
                throw;
            }
        }

        [HttpGet("GetByID")]
        public IActionResult GetByID(int id)
        {
            try
            {
                Employee? dataRecord = _DBContext.Employees.Where(c => c.EmployeeId == id).FirstOrDefault();
                if (dataRecord != null)
                {
                    return Ok(new { Status = "Success", Data = dataRecord });
                }
                else
                {
                    return NotFound("Book not found.");
                }
            }
            catch (CultureNotFoundException ex)
            {
                _logger.LogError(ex, "CultureNotFoundException occurred while processing Get.");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpDelete("Delete")]
        public IActionResult Delete(int id)
        {
            try
            {
                Employee? dataRecord = _DBContext.Employees.Where(c => c.EmployeeId == id).FirstOrDefault();
                if (dataRecord != null)
                {
                    _DBContext.Employees.Remove(dataRecord);
                    _DBContext.SaveChanges();
                    return Ok(new { Status = "Deleted Successfully" });
                }
                else
                {
                    return NotFound("Video not found.");
                }
            }
            catch (CultureNotFoundException ex)
            {
                _logger.LogError(ex, "CultureNotFoundException occurred while processing Get.");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost("Save")]
        public IActionResult Save([FromBody] Employee employee)
        {
            try
            {
                var dataRecord = _DBContext.Employees.Where(c => c.EmployeeId == employee.EmployeeId).FirstOrDefault();
                if (dataRecord == null)
                {
                    dataRecord = new Employee()
                    ;
                    _DBContext.Employees.Add(dataRecord);
                }

                dataRecord.EmployeeName = employee.EmployeeName;
                dataRecord.Department = employee.Department;
                dataRecord.DateOfJoining = employee.DateOfJoining;
                dataRecord.ProfileName = employee.ProfileName;


                _DBContext.SaveChanges();
                return Ok(new { Status = "Saved Successfully", Data = dataRecord });
            }
            catch (CultureNotFoundException ex)
            {
                _logger.LogError(ex, "CultureNotFoundException occurred while processing Delete.");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost("UploadImage")]
        public async Task<IActionResult> UploadImage(IFormFile file, [FromQuery] int employeeId)
        {
            try
            {
                string saveResult = await CommonController.SaveFile(file);

                if (!string.IsNullOrEmpty(saveResult))
                {
                    if (employeeId > 0 && saveResult.Split("~")[0] == "Success")
                    {
                        var dataRecord = _DBContext.Employees.FirstOrDefault(c => c.EmployeeId == employeeId);
                        if (dataRecord != null)
                        {
                            dataRecord.ProfileName = saveResult.Split("~")[1];
                            _DBContext.SaveChanges();
                        }
                    }
                    else
                    {
                        throw new Exception("Failed to save file.");
                    }
                }

                return Ok(new { Status = "Success" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while processing Upload Photo.");
                return StatusCode(500, "Internal Server Error");
            }
        }

    }
}
