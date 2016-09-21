using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Exploider.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        const string TypeTokenCharacterPattern = @"-!#-'*+.\w^`|~"; // !#$%&'*+-.0-9A-Z^_`a-z|~
        const string TypeTokenPartPattern = "[" + TypeTokenCharacterPattern + "]+";
        const string TypeTokenPattern = "^" + TypeTokenPartPattern + "/" + TypeTokenPartPattern + "$";
        public IActionResult Data()
        {
            var query = Request.QueryString.Value;
            if (string.IsNullOrEmpty(query))
                return BadRequest();

            query = Uri.UnescapeDataString(query);

            var comma = query.IndexOf(',');
            if (~comma == 0)
                return BadRequest();

            var type = query.Substring(1, comma - 1);
            var data = query.Substring(comma + 1);

            var parts = type.Split(';');
            var partCount = parts.Length;

            var isBase64 = "base64".Equals(parts[partCount - 1].Trim(), StringComparison.OrdinalIgnoreCase);
            if (isBase64)
                partCount--;

            var charset = "US-ASCII";

            switch (partCount)
            {
                case 1:
                    type = parts[0].Trim();
                    if (type.Length == 0)
                        type = "text/plain";
                    break;
                case 2:
                    charset = parts[1].Trim();
                    if (!charset.StartsWith("charset=", StringComparison.OrdinalIgnoreCase))
                        return BadRequest();
                    charset = charset.Substring(8);
                    goto case 1;
                default:
                    return BadRequest();
            }

            if (!Regex.IsMatch(type, TypeTokenPattern))
                return BadRequest();

            if (!isBase64)
                return Content(query, type, Encoding.GetEncoding(charset));
            return File(Convert.FromBase64String(data), type + ";charset=" + charset);
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
