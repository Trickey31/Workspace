using System.Text.RegularExpressions;

namespace Workspace.Contract
{
    public static class CommonExtensions
    {
        public static string ConvertEmailToUserName(string email)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentNullException(nameof(email));

            var atIndex = email.IndexOf('@');
            var username = atIndex < 0 ? email : email.Substring(0, atIndex);

            return username.ToLower();
        }

        private static readonly string[] VietNamChar = new string[15]
        {
            "aAeEoOuUiIdDyY", "áàạảãâấầậẩẫăắằặẳẵ", "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ", "éèẹẻẽêếềệểễ", "ÉÈẸẺẼÊẾỀỆỂỄ", "óòọỏõôốồộổỗơớờợởỡ", "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ", "úùụủũưứừựửữ", "ÚÙỤỦŨƯỨỪỰỬỮ", "íìịỉĩ",
            "ÍÌỊỈĨ", "đ", "Đ", "ýỳỵỷỹ", "ÝỲỴỶỸ"
        };

        //public static string GenerateSlug(string input)
        //{
        //    if (string.IsNullOrEmpty(input))
        //    {
        //        return "";
        //    }

        //    input = input.ToLower();

        //    for (int i = 1; i < VietNamChar.Length; i++)
        //    {
        //        for (int j = 0; j < VietNamChar[i].Length; j++)
        //        {
        //            input = input.Replace(VietNamChar[i][j], VietNamChar[0][i - 1]);
        //        }
        //    }

        //    input = Regex.Replace(input, @"[^a-z0-9\s-]", "");
        //    input = input.Replace(" ", "-");

        //    input = Regex.Replace(input, @"-{2,}", "-");

        //    input = input.Substring(0, Math.Min(input.Length, 100)).Trim('-');

        //    string hash;
        //    using (var sha256 = System.Security.Cryptography.SHA256.Create())
        //    {
        //        byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
        //        byte[] hashBytes = sha256.ComputeHash(inputBytes);
        //        hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        //    }

        //    return hash;
        //}
        public static string GenerateSlug(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return "";
            }

            input = input.ToLower();

            for (int i = 1; i < VietNamChar.Length; i++)
            {
                for (int j = 0; j < VietNamChar[i].Length; j++)
                {
                    input = input.Replace(VietNamChar[i][j], VietNamChar[0][i - 1]);
                }
            }

            string[] words = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string acronym = "";
            if (words.Length > 0)
            {
                foreach (char c in words[0])
                {
                    if (char.IsLetter(c))
                    {
                        acronym += c;
                    }
                }
            }

            // Tạo 6 chữ số ngẫu nhiên
            Random rnd = new Random();
            int randomNumber = rnd.Next(100000, 999999);
            string randomDigits = randomNumber.ToString();

            // Kết hợp chữ cái đầu và 6 chữ số ngẫu nhiên
            string slug = acronym.ToUpper() + randomDigits;

            return slug;
        }
    }
}
