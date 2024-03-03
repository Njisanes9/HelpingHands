using AspNetCoreHero.ToastNotification.Abstractions;
using HelpingHands.DAL;
using HelpingHands.Models;
using Microsoft.AspNetCore.Mvc;

namespace HelpingHands.Controllers
{
    public class CityController : Controller
    {
        private readonly DataAccessLayer _dal;
        private readonly INotyfService _toastNotification;

        public CityController(DataAccessLayer dal,
            INotyfService notyfService)
        {
            _dal = dal;
            _toastNotification = notyfService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<City> cityList = new List<City>();

            try
            {
                cityList = _dal.GetCities();
            }
            catch(Exception ex) 
            {
                TempData["errorMessage"] = ex.Message;
            }

            return View(cityList);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(City city)
        {

            if (!ModelState.IsValid)
            {
                TempData["errorMessage"] = "Model is not valid";
            }

            if (city !=null)
            {
                bool result = _dal.InsertCity(city);

                if (!result)
                {
                    _toastNotification.Success("Record has been inserted successfully");
                    return RedirectToAction("Index");
                    
                }
                else
                {
                    TempData["errorMessage"] = "City details cannot be saved";
                    return View();
                }
                
            }
            return View(city);
        }
            

        [HttpGet]
        public IActionResult Edit(int id)
        {
            try
            {
                
                City city = _dal.GetCityById(id);
                 
                if (city.CityId == 0)
                {
                    TempData["errorMessage"] = $"City details with Id {id} cannot be found";
                    return RedirectToAction("Index");
                }
                                
                return View(city);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
        }
        [HttpPost]
        public IActionResult Edit(City city)
        {
            
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["errorMessage"] = "Data is not valid";
                    return View();
                }

                if (ModelState.IsValid)
                {
                    bool result = _dal.UpdateCity(city);

                    if (!result)
                    {
                        _toastNotification.Success("City is updated successfully");
                        
                    }
                    else
                    {

                        TempData["errorMessage"] = "City cannot be updated";
                        return View();

                    }
                   
                }
                                
                return RedirectToAction("Index");
                
                
            }
            catch (Exception ex)
            {

                TempData["errorMessage"] = ex.Message; 
                return View();
            }
           
        }


        [HttpGet]
        public JsonResult Delete(int id)
        {
            try
            {
                City city = _dal.GetCityById(id);
                if (city.CityId == 0)
                {
                    TempData["errorMessage"] = $"City details with Id {id} cannot be found";
                }
                return Json($"{id}");
            }
            catch (Exception ex)
            {

                TempData["errorMessage"] = ex.Message;
               
            }
            return Json($"{id}");
        }
        [HttpPost]
        public IActionResult DeleteConfirmed(City city)
        {

            try
            {
               
                bool result = _dal.DeleteCity(city.CityId);

                if (!result)
                {
                    TempData["errorMessage"] = "Data cannot be updated";
                    return View();
                }
                _toastNotification.Success("City has been deleted succeessfully",4);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {

                TempData["errorMessage"] = ex.Message;
                return View();
            }
        }


    }
}
