
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
        private readonly ILogger<AdminHomepageController> _logger; // Add ILogger

        public AdminHomepageController(ILogger<AdminHomepageController> logger)
        {

            //This part is to put in _httpCLient the URL of the API

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7158/LibraryLog")
            };
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


            _logger = logger; // Initialize ILogger
        }

        //Return View To AddBooksLogs.cshtml
        public async Task<ActionResult> AddBooksLogs()
        {

            return View();
        }

        //This Part is Responsible to POST REQUEST to WEB API which is responsible also to INSERT DATA to Database
        [HttpPost]
        public async Task<IActionResult> AddBooksLogs(BooksLog book)
        {
            try
            {

                if (ModelState.IsValid)
                {


                    //This part is how to make HTTP Request POST to EndPOINT  which is "LibraryLog" 
                    var request = new HttpRequestMessage(HttpMethod.Post, "LibraryLog");
                    var jsonContent = JsonConvert.SerializeObject(book);
                    request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    //This is the line of code to SEND HTTP REQUEST
                    var response = await _httpClient.SendAsync(request);

                    //If the Response is Success then this block of code will be run
                    if (response.IsSuccessStatusCode)
                    {
                        //if Success then go to AdminHomepage/Index.cshtml
                        return RedirectToAction("Index", "AdminHomepage");
                    }
                    else
                    {
                        //if Have Error then go to AdminHomepage/AddBooksLogs.cshtml which will return to same page
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


        //This Part is Responsible to GET REQUEST to WEB API which is responsible also to RETREIVE DATA to Database
        public async Task<ActionResult> Index()
        {
            try
            {
                //This part will make a get http request to web api
                var response = await _httpClient.GetAsync("");

                if (response.IsSuccessStatusCode)
                {
                   //if success the viewbag will have a value where it will use to other logic like display a navbar
                    ViewBag.Hasauthorization = true;
                    ViewBag.Username = "admin";
                    //put to content in variable which post
                    var post = await response.Content.ReadAsAsync<List<BooksLog>>();
                    //this will return to view/ which on index.cshtml
                    return View(post);
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

        //This Part is Responsible to GET USER BY ID REQUEST to WEB API which is responsible also to RETREIVE A SPECIFIC DATA to Database
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
                            //this will return to view which will be use to return on update 
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

        //This Part is Responsible to PUT REQUEST to WEB API which is responsible also to UPDATE A SPECIFIC DATA to Database
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


                //This part is how to make HTTP Request POST to EndPOINT  which is "LibraryLog" 
                var request = new HttpRequestMessage(HttpMethod.Put, "LibraryLog");
                var jsonContent = JsonConvert.SerializeObject(userAcc);
                request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                //This part will send a request to web api
                var response = await _httpClient.SendAsync(request);

                // Check if the request was successful
                if (response.IsSuccessStatusCode)
                {

                    //if response is success which is 200 status this part will be redirect to Admin Homepage Index to show the Table Update
                    // Log success or any relevant information
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

        //This Part is Responsible to GET USER BY ID REQUEST to WEB API which is responsible also to RETREIVE A SPECIFIC DATA to Database
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

        //This Part is Responsible to DELETE REQUEST to WEB API which is responsible also to DELETE A SPECIFIC DATA to Database
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
