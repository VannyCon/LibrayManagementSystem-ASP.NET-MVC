
using LibrayManagementSystemMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;

namespace LibrayManagementSystemMVC.Controllers
{
    public class AdminHomepageController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly HttpClient _httpClient2;
        private readonly HttpClient _httpClient3;
        private readonly ILogger<AdminHomepageController> _logger; // Add ILogger

        public AdminHomepageController(ILogger<AdminHomepageController> logger)
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7158/LibraryLog")
            };
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            _httpClient2 = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7158/Library")
            };
            _httpClient2.DefaultRequestHeaders.Accept.Clear();
            _httpClient2.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            _httpClient3 = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7158/UserManagement")
            };
            _httpClient3.DefaultRequestHeaders.Accept.Clear();
            _httpClient3.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            _logger = logger; // Initialize ILogger
        }


        public async Task<ActionResult> AddBooksLogs()
        {
            var response = await _httpClient2.GetAsync("");
            var response2 = await _httpClient3.GetAsync("");

            var json = await response.Content.ReadAsStringAsync();
            var books = JsonConvert.DeserializeObject<List<Books>>(json);
            var json2 = await response2.Content.ReadAsStringAsync();
            var studentInfos = JsonConvert.DeserializeObject<List<AccountModel>>(json2);
            var filteredStudentInfos = studentInfos?.Where(si => si.student_id != 0).ToList();
            // You cannot pass multiple models directly to the view, 
            // so you need to use a ViewModel or ViewBag to pass multiple data to the view.

            // Using a ViewModel approach
            var viewModel = new IndexViewModel
            {
                books = books ?? new List<Books>(),
                studentinfo = filteredStudentInfos ?? new List<AccountModel>()
            };
            // Pass ViewModel to the view
            return View(viewModel);
        }

        //CREATE
        [HttpPost]
        public async Task<IActionResult> AddBooksLogs(BooksLog book)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    // Log the userAcc object
                    Console.WriteLine($"UserAcc object: {book}");

                    var request = new HttpRequestMessage(HttpMethod.Post, "LibraryLog");
                    var jsonContent = JsonConvert.SerializeObject(book);
                    request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    var response = await _httpClient.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {

                        return RedirectToAction("Index", "AdminHomepage");
                    }
                    else
                    {
                        // Handle error response
                        return RedirectToAction("AddBooksLogs", "AdminHomepage");
                    }
                }
                else
                {
                    // If ModelState is not valid, redisplay the registration form with validation errors
                    return RedirectToAction("AddBooksLog", "AdminHomepage");
                }
            }
            catch (Exception ex)
            {
                // Handle exception
                
                return RedirectToAction("Index", "Home");
            }
        }


        //READ
        public async Task<ActionResult> Index()
        {
            try
            {

                var response = await _httpClient.GetAsync("");

                if (response.IsSuccessStatusCode)
                {
                    ViewBag.Hasauthorization = true;
                    ViewBag.Username = "admin";
                    var userAcc = await response.Content.ReadAsAsync<List<BooksLog>>();
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

        //GET USER
        public async Task<ActionResult> Edit(int id)
        {
            try
            {

                var response = await _httpClient.GetAsync($"LibraryLog/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    // Check if the response is an array
                    if (json != null)
                    {
                        // If it's an array, deserialize it as a List<AdminHomepage>
                        var userAccList = JsonConvert.DeserializeObject<List<BooksLog>>(json);
                        // Assuming you want the first item in the array
                        var userAcc = userAccList.FirstOrDefault();
                        if (userAcc != null)
                        {
                            return View(userAcc);
                        }
                        else
                        {
                            // Handle the case where the array is empty
                            return RedirectToAction("Edit", "AdminHomepage");
                        }
                    }
                    else
                    {
                        // If it's not an array, deserialize it as a single AdminHomepage object
                        var userAcc = JsonConvert.DeserializeObject<BooksLog>(json);
                        return View(userAcc);
                    }
                }
                else
                {
                    // Handle error response
                    return RedirectToAction("Edit", "AdminHomepage");
                }
            }
            catch (Exception ex)
            {
                // Handle exception
                return RedirectToAction("Edit", "AdminHomepage");
            }
        }

        //UPDATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BooksLog userAcc)
        {
            try
            {
                if (userAcc == null)
                {
                    // Handle null userAcc
                    throw new ArgumentNullException(nameof(userAcc), "User account object is null.");
                }



                var request = new HttpRequestMessage(HttpMethod.Put, "LibraryLog");
                var jsonContent = JsonConvert.SerializeObject(userAcc);
                request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.SendAsync(request);

                // Check if the request was successful
                if (response.IsSuccessStatusCode)
                {

                    // Log success or any relevant information
                    _logger.LogInformation("User account updated successfully.");
                    return RedirectToAction("Index", "AdminHomepage"); // Assuming there's an Index action method to redirect to
                }
                else
                {
                    // Log error or handle the error response appropriately
                    _logger.LogError($"Failed to update user account. Status code: {response.StatusCode}");
                    return View(userAcc); // Return the view with the userAcc model to display errors
                }
            }
            catch (Exception ex)
            {
                // Log or handle any exceptions that occur during the update process
                _logger.LogError($"An error occurred while updating user account: {ex.Message}");
                return View(userAcc); // Return the view with the userAcc model to display errors
            }
        }


        //GET USER
        public async Task<ActionResult> Delete(int id)
        {
            try
            {

                var response = await _httpClient.GetAsync($"LibraryLog/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    // Check if the response is an array
                    if (json != null)
                    {
                        // If it's an array, deserialize it as a List<AdminHomepage>
                        var userAccList = JsonConvert.DeserializeObject<List<BooksLog>>(json);
                        // Assuming you want the first item in the array
                        var userAcc = userAccList.FirstOrDefault();
                        if (userAcc != null)
                        {
                            return View(userAcc);
                        }
                        else
                        {
                            // Handle the case where the array is empty
                            return RedirectToAction("AdminHomepage", "Edit");
                        }
                    }
                    else
                    {
                        // If it's not an array, deserialize it as a single AdminHomepage object
                        var userAcc = JsonConvert.DeserializeObject<BooksLog>(json);
                        return View(userAcc);
                    }
                }
                else
                {
                    // Handle error response
                    return RedirectToAction("AdminHomepage", "Edit");
                }

            }
            catch (Exception ex)
            {
                // Handle exception
                return RedirectToAction("AdminHomepage", "AdminHomepage");
            }
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                if (id == null)
                {
                    // Handle null id
                    throw new ArgumentNullException(nameof(id), "ID parameter is null.");
                }

                var request = new HttpRequestMessage(HttpMethod.Delete, $"LibraryLog/{id}");
                var response = await _httpClient.SendAsync(request);

                // Check if the request was successful
                if (response.IsSuccessStatusCode)
                {

                    // Log success or any relevant information
                    _logger.LogInformation("User account deleted successfully.");
                    return RedirectToAction("Index", "AdminHomepage"); // Assuming there's an action method to redirect to
                }
                else
                {
                    // Log error or handle the error response appropriately
                    _logger.LogError($"Failed to delete user account. Status code: {response.StatusCode}");
                    return View(id); // Return the view with the ID to display errors
                }
            }
            catch (Exception ex)
            {
                // Log or handle any exceptions that occur during the delete process
                _logger.LogError($"An error occurred while deleting user account: {ex.Message}");
                return View(id); // Return the view with the ID to display errors
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
