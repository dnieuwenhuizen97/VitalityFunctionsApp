using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Services
{
    public class InputSanitizationService : IInputSanitizationService
    {
        public Task<string> SanitizeInput(string input)
        {
            string pattern = @"[^0-9a-zA-Z:,.?@ '()!]+";

            string result = Regex.Replace(input, pattern, "");

            return Task.FromResult(result);
        }
    }
}
