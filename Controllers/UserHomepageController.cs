
using LibrayManagementSystemMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;

namespace LibrayManagementSystemMVC.Controllers
{
    public class UserHomepageController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly HttpClient _httpClient2;
        private readonly ILogger<UserHomepageController> _logger; // Add ILogger

        public UserHomepageController(ILogger<UserHomepageController> logger)
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7158/Library")
            };
            _httpClient2 = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7158/LibraryLog")
            };
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient2.DefaultRequestHeaders.Accept.Clear();
            _httpClient2.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _logger = logger; // Initialize ILogger
        }



        //READ ONLY BOOKS
        public async Task<ActionResult> Index()
        {
            try
            {

                var response = await _httpClient.GetAsync("");

                if (response.IsSuccessStatusCode)
                {
                    ViewBag.Hasauthorization = true;
                    ViewBag.Username = "user";
                    var userAcc = await response.Content.ReadAsAsync<List<Books>>();
                    return View(userAcc);
                }
                else
                {
                    // Handle error response
                    return RedirectToAction("Index", "Home");
                }

            }
            catch (Exception ex)
            {
                // Handle exception
                return RedirectToAction("Index", "Home");
            }
        }

        //READ ONLY LOGS
        public async Task<ActionResult> BooksLogs()
        {
            try
            {
                var response = await _httpClient2.GetAsync(""); // Provide your API endpoint here

                if (response.IsSuccessStatusCode)
                {
                    ViewBag.Hasauthorization = true;
                    ViewBag.Username = "user";

                    // Ensure id is of type int
                    int? id = HttpContext.Session.GetInt32("studentId");
                    var userAcc = await response.Content.ReadAsAsync<List<BooksLog>>();

                    _logger.LogInformation($"stage 1 Student ID retrieved from session: {id}");
                    // Check if id is not null before filtering
                    if (id != null)
                    {
                        _logger.LogInformation($" stage 2 Student ID retrieved from session: {id}");
                        // Filter the list based on the provided id
                        var filteredUserAcc = userAcc.Where(x => x.student_id_fk == id).ToList();
                        return View(filteredUserAcc);
                    }
                    else
                    {
                        // Handle missing id
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    // Handle error response
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                // Handle exception
                return RedirectToAction("Index", "Home");
            }
        }




        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
