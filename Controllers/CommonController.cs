using App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace App.Controllers
{
    public class CommonController : ControllerBase
    {
        public CommonController() { }

        public static ReactDBContext CreateMainDbContext()
        {
            DbContextOptionsBuilder<ReactDBContext> _optionsBuilder = new DbContextOptionsBuilder<ReactDBContext>();
            _optionsBuilder.UseSqlServer(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["DefaultConnection"]);

            ReactDBContext _DbContext = new ReactDBContext(_optionsBuilder.Options);
            return _DbContext;
        }

        public static async Task<string> SaveFile(IFormFile file)
        {
            try
            {
                var fileName = "";
                var filePath = "";
                var directory = "";
                string fileSizeString = "";

                if (file.Length > 0)
                {
                    fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                    var extension = Path.GetExtension(fileName);
                    if (CommonController.FileTypeDirectoryMap.TryGetValue(extension, out directory))
                    {
                        var fileTypeDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", directory);
                        filePath = Path.Combine(fileTypeDirectory, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        long fileSize = file.Length; // Get the file size in bytes
                        fileSizeString = FormatFileSize(fileSize); // Convert file size to human-readable format
                    }
                }

                return "Success~" + filePath + "~" + directory + "~" + fileSizeString; // Return the file size as a string along with other details
            }
            catch (Exception ex)
            {
                return "Error-" + ex.Message;
            }
        }

        public static string GenerateRandomId(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var stringBuilder = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                stringBuilder.Append(chars[random.Next(chars.Length)]);
            }

            return stringBuilder.ToString();
        }

        private static string FormatFileSize(long fileSize)
        {
            if (fileSize == 0)
            {
                return "0 Bytes";
            }
            else
            {
                string[] sizes = { "Bytes", "KB", "MB", "GB", "TB" };
                int i = Convert.ToInt32(Math.Floor(Math.Log(fileSize, 1024)));
                double formattedSize = Math.Round(fileSize / Math.Pow(1024, i), 2);
                return $"{formattedSize} {sizes[i]}";
            }
        }


        public static readonly Dictionary<string, string> FileTypeDirectoryMap = new Dictionary<string, string>
        {
            { ".m4a", "Audios" },
            { ".mp4", "Videos" },
            { ".pdf", "Pdfs" },
            { ".jpg", "Images" },
            { ".jpeg", "Images" },
            { ".png", "Images" },
        };
    }
}
