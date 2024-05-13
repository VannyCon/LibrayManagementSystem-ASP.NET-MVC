
using LibrayManagementSystemMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;

namespace LibrayManagementSystemMVC.Controllers
{
    public class UserManagementController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UserManagementController> _logger; // Add ILogger

        public UserManagementController(ILogger<UserManagementController> logger)
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7158/UserManagement")
            };
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            _logger = logger; // Initialize ILogger
        }


        public IActionResult AddUser()
        {
            return View();

        }

        //CREATE
        [HttpPost]
        public async Task<IActionResult> AddUser(AccountModel book)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    // Log the userAcc object
                    Console.WriteLine($"UserAcc object: {book}");

                    var request = new HttpRequestMessage(HttpMethod.Post, "UserManagement");
                    var jsonContent = JsonConvert.SerializeObject(book);
                    request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    var response = await _httpClient.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {

                        return RedirectToAction("Index", "UserManagement");
                    }
                    else
                    {
                        // Handle error response
                        return RedirectToAction("AddUser", "UserManagement");
                    }
                }
                else
                {
                    // If ModelState is not valid, redisplay the registration form with validation errors
                    return RedirectToAction("Index", "AdminHomepage");
                }
            }
            catch (Exception ex)
            {
                // Handle exception
                Console.WriteLine($"Exception occurred: {ex}");
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
                    var userAcc = await response.Content.ReadAsAsync<List<AccountModel>>();
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

                var response = await _httpClient.GetAsync($"UserManagement/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    // Check if the response is an array
                    if (json != null)
                    {
                        // If it's an array, deserialize it as a List<AdminHomepage>
                        var userAccList = JsonConvert.DeserializeObject<List<AccountModel>>(json);
                        // Assuming you want the first item in the array
                        var userAcc = userAccList.FirstOrDefault();
                        if (userAcc != null)
                        {
                            return View(userAcc);
                        }
                        else
                        {
                            // Handle the case where the array is empty
                            return RedirectToAction("Edit", "UserManagement");
                        }
                    }
                    else
                    {
                        // If it's not an array, deserialize it as a single AdminHomepage object
                        var userAcc = JsonConvert.DeserializeObject<AccountModel>(json);
                        return View(userAcc);
                    }
                }
                else
                {
                    // Handle error response
                    return RedirectToAction("Edit", "UserManagement");
                }
            }
            catch (Exception ex)
            {
                // Handle exception
                return RedirectToAction("Edit", "UserManagement");
            }
        }

        //UPDATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AccountModel userAcc)
        {
            try
            {
                if (userAcc == null)
                {
                    // Handle null userAcc
                    throw new ArgumentNullException(nameof(userAcc), "User account object is null.");
                }



                var request = new HttpRequestMessage(HttpMethod.Put, "UserManagement");
                var jsonContent = JsonConvert.SerializeObject(userAcc);
                request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.SendAsync(request);

                // Check if the request was successful
                if (response.IsSuccessStatusCode)
                {

                    // Log success or any relevant information
                    _logger.LogInformation("User account updated successfully.");
                    return RedirectToAction("Index", "UserManagement"); // Assuming there's an Index action method to redirect to
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

                var response = await _httpClient.GetAsync($"UserManagement/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    // Check if the response is an array
                    if (json != null)
                    {
                        // If it's an array, deserialize it as a List<AdminHomepage>
                        var userAccList = JsonConvert.DeserializeObject<List<AccountModel>>(json);
                        // Assuming you want the first item in the array
                        var userAcc = userAccList.FirstOrDefault();
                        if (userAcc != null)
                        {
                            return View(userAcc);
                        }
                        else
                        {
                            // Handle the case where the array is empty
                            return RedirectToAction("Edit", "UserManagement");
                        }
                    }
                    else
                    {
                        // If it's not an array, deserialize it as a single AdminHomepage object
                        var userAcc = JsonConvert.DeserializeObject<AccountModel>(json);
                        return View(userAcc);
                    }
                }
                else
                {
                    // Handle error response
                    return RedirectToAction("Edit", "UserManagement");
                }

            }
            catch (Exception ex)
            {
                // Handle exception
                return RedirectToAction("Index", "UserManagement");
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

                var request = new HttpRequestMessage(HttpMethod.Delete, $"UserManagement/{id}");
                var response = await _httpClient.SendAsync(request);

                // Check if the request was successful
                if (response.IsSuccessStatusCode)
                {

                    // Log success or any relevant information
                    _logger.LogInformation("User account deleted successfully.");
                    return RedirectToAction("Index", "UserManagement"); // Assuming there's an action method to redirect to
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
