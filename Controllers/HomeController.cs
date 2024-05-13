using LibrayManagementSystemMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;

namespace LibrayManagementSystemMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HomeController> _logger;


        public HomeController(ILogger<HomeController> logger)
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7261/AdminLogin/Request")
            };
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Index(AccountModel model) // Use the correct model type
        {
            string user = model.username;
            string password = model.password;
            var jsonBody = JsonConvert.SerializeObject(new { username = user, password = password });

            try // Handle potential exceptions during HTTP request
            {
                var response = await _httpClient.PostAsync("https://localhost:7158/Login/Request", new StringContent(jsonBody, Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    dynamic jsonObject = JsonConvert.DeserializeObject(content); // Assuming JSON response


                    if (jsonObject.authorization == "admin")
                    {
                        HttpContext.Session.SetString("authorization", $"{jsonObject.authorization}");
                        ViewBag.Hasauthorization = true;
                        ViewBag.Username = "admin";
                        HttpContext.Session.SetString("user", user);
                        HttpContext.Session.SetInt32("studentId", (int)jsonObject.studentId);
                        return RedirectToAction("Index", "AdminHomepage"); // Use correct controller name
                    }
                    else if(jsonObject.authorization == "user")
                    {
                        ViewBag.Hasauthorization = true;
                        ViewBag.Username = "user";
                        HttpContext.Session.SetString("user", user);
                        HttpContext.Session.SetInt32("studentId", (int)jsonObject.studentId);
                        // Handle missing access token in response
                        return RedirectToAction("Index", "UserHomepage");
                    }else {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    _logger.LogError($"Failed to login with status code: {response.StatusCode}");
                    // Handle login failure (e.g., display error message)
                    return View(); // Or redirect to appropriate error page
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during login: {ex.Message}");
                // Handle exception (e.g., display generic error message)
                return View(); // Or redirect to appropriate error page
            }
        }

        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Remove("authorization");
            HttpContext.Session.Remove("studentId");
            // Clear all cookies related to the session
            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }
            TempData["LogoutSuccessfully"] = "Log Out Successfully!";
            return RedirectToAction("Index", "Home"); // Redirect to login page
        }

        public IActionResult Index()
        {

            var username = HttpContext.Session.GetString("authorization");
            if (username != null) // Simplified check
            {
                ViewBag.Username = "admin";
                ViewBag.Hasauthorization = true;
                return RedirectToAction("Index", "AdminHomePage");
            }
            else
            {
                return View();

            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
