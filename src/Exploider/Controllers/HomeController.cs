using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace Exploider.Controllers
{
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

            var isBase64 = false;
            var charset = "US-ASCII";

            switch (partCount)
            {
                case 1:
                    type = parts[0].Trim();
                    if (string.IsNullOrWhiteSpace(type))
                        type = "text/plain";
                    break;
                case 2:
                    type = parts[1].Trim();
                    if (type.Length == 0)
                        return BadRequest();

                    if (type.Equals("base64", StringComparison.OrdinalIgnoreCase))
                    {
                        isBase64 = true;
                        goto case 1;
                    }

                    charset = type;
                    if (!charset.StartsWith("charset", StringComparison.OrdinalIgnoreCase))
                        return BadRequest();

                    charset = charset.Substring(7).TrimStart();
                    if (charset.Length == 0 || charset[0] != '=')
                        return BadRequest();

                    charset = charset.Substring(1).TrimStart();
                    if (charset.Length == 0)
                        return BadRequest();

                    type = parts[0].Trim();
                    if (string.IsNullOrWhiteSpace(type))
                        return BadRequest(); // improperable
                    break;
                default:
                    return BadRequest();
            }

            if (isBase64)
                return File(Convert.FromBase64String(data), type + ";charset=" + charset);
            return Content(data, type, Encoding.GetEncoding(charset));
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
