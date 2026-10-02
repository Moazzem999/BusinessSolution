using System.ComponentModel;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace BusinessSolution.Shared.Utilities
{
    public static class Helper
    {
        public static bool IsValidEmail(string email)
        {
            string pattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";
            Regex regex = new(pattern);
            Match match = regex.Match(email);
            return match.Success;
        }

        public static bool VerifyPassword(string inputPassword, string? storedHash)
        {
            if (string.IsNullOrEmpty(storedHash)) return false;

            // BCrypt extracts the salt from the storedHash and compares it to the input
            return BCrypt.Net.BCrypt.Verify(inputPassword, storedHash);
        }

        public static string GenerateUniqueFileName(string fileName)
        {
            DateTime currentTime = DateTime.UtcNow;
            string formattedDateTime = currentTime.ToString("dd-MMM-yyyy_HH_mm_ss_tt");

            // Combine the original file name, formatted date and time, and extension
            var uniqueFileName = $"{Path.GetFileNameWithoutExtension(fileName)}_{formattedDateTime}{Path.GetExtension(fileName)}";
            return uniqueFileName;
        }

        public static (decimal value, string unit) ConvertStorage(decimal storageInKB)
        {
            if (storageInKB >= 1024 * 1024)
            {
                return (Math.Round(storageInKB / (1024 * 1024), 2), "GB");
            }
            else if (storageInKB >= 1024)
            {
                return (Math.Round(storageInKB / 1024, 2), "MB");
            }
            else
            {
                return (Math.Round(storageInKB, 2), "KB");
            }
        }

        public static decimal ConvertKBToGB(decimal storageInKB)
        {
            return storageInKB / (1024 * 1024); // 1 GB = 1024 * 1024 KB
        }

        public static string GetContentType(string extension)
        {
            return extension switch
            {
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".txt" => "text/plain",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".pdf" => "application/pdf",
                _ => "application/octet-stream",
            };
        }

        public static string GenerateRandomPassword(int length)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$&";
            var randomBytes = new byte[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }

            var result = new StringBuilder(length);

            foreach (byte b in randomBytes)
            {
                result.Append(validChars[b % (validChars.Length)]);
            }

            return result.ToString();
        }

        public static string GenerateRandomHashCode()
        {
            // Generate a random input
            var randomBytes = new byte[8]; // 8 bytes of randomness
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }

            // Compute the hash
            using (var sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(randomBytes);

                // Convert the hash to a Base64 string and take the first 16 characters
                // Base64 is compact and will give a mix of alphanumeric characters
                string base64Hash = Convert.ToBase64String(hashBytes);

                // Remove non-alphanumeric characters from Base64 and truncate to 16 characters
                var shortHashBuilder = new StringBuilder();
                foreach (var c in base64Hash)
                {
                    if (char.IsLetterOrDigit(c))
                    {
                        shortHashBuilder.Append(c);
                        if (shortHashBuilder.Length == 16) break;
                    }
                }

                return shortHashBuilder.ToString();
            }
        }

        public static string GenerateInvoiceNumber()
        {
            DateTime now = DateTime.Now;
            string datePart = now.ToString("yyyyMMdd");

            // Generate a new GUID and take the first 10 characters
            string guidPart = Guid.NewGuid().ToString("N")[..10];
            string invoiceNumber = $"{datePart}-{guidPart}";
            return invoiceNumber;
        }

        public static bool IsDateIncreasedBySevenDays(DateTime date)
        {
            DateTime newDate = date.AddDays(7);

            return newDate > DateTime.Today;
        }

        public static string GenerateRandomPin()
        {
            Random random = new();
            return random.Next(100000, 999999).ToString("D6");
        }

        public static string GetEnumDescription(this System.Enum enumeration)
        {
            FieldInfo fi = enumeration.GetType().GetField(enumeration.ToString())!;
            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0 && !string.IsNullOrEmpty(attributes[0].Description))
            {
                return attributes[0].Description;
            }

            return enumeration.ToString();
        }
    }
}
