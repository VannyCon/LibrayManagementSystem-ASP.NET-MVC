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

        //This Part is Responsible to POST REQUEST to WEB API which is responsible also to LOG IN
        [HttpPost]
        public async Task<IActionResult> Index(AccountModel model) // Use the correct model type
        {
            //declare the Variable that get from the MODEL
            string user = model.username;
            string password = model.password;
            //Serialize the Model
            var jsonBody = JsonConvert.SerializeObject(new { username = user, password = password });

            try // Handle potential exceptions during HTTP request
            {
                //This will make a respost to WEB API
                var response = await _httpClient.PostAsync("https://localhost:7158/Login/Request", new StringContent(jsonBody, Encoding.UTF8, "application/json"));

                //If success then the block of code will be run
                if (response.IsSuccessStatusCode)
                {
                    //This is line of code is responsible read the response from the WEB API
                    string content = await response.Content.ReadAsStringAsync();
                    //This line will put the response json into content then deserialize, its mean that from json you can instance it now like this ex. jsonObject.authorization
                    dynamic jsonObject = JsonConvert.DeserializeObject(content); // Assuming JSON response

                    //read the json authorization if authorization is admin then this line of code will be run
                    if (jsonObject.authorization == "admin")
                    {
                        //This line is responsible to put the jsonObject.authorization content into Local Storage which is on session so i can access this globally
                        HttpContext.Session.SetString("authorization", $"{jsonObject.authorization}");
                        //This part is put those thing into global using viewbag
                        ViewBag.Hasauthorization = true;
                        ViewBag.Username = "admin";
                        HttpContext.Session.SetString("user", user);
                        //This line is responsible to put the jsonObject.studentId  which is (int) content into Local Storage which is on session so we can access this globally
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
