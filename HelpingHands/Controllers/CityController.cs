using HelpingHands.DAL;
using HelpingHands.Models;
using Microsoft.AspNetCore.Mvc;

namespace HelpingHands.Controllers
{
    public class CityController : Controller
    {
        private readonly DataAccessLayer _dal;

        public CityController(DataAccessLayer dal)
        {
            _dal = dal;
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
                TempData["errorMessage"] = "Data is not valid";
            }

            bool result = _dal.InsertCity(city);

            if (!result)
            {
                TempData["errorMessage"] = "Data cannot be saved";
                return View();
            }
            TempData["successMessage"] = "Record saved successfully";
            return RedirectToAction("Index");
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
                }
                return RedirectToAction("Index");
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

                bool result = _dal.UpdateCity(city);

                if (!result)
                {
                    TempData["errorMessage"] = "Data cannot be updated";
                    return View();
                }
                TempData["successMessage"] = "Record update successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {

                TempData["errorMessage"] = ex.Message; 
                return View();
            }
        }


        [HttpGet]
        public IActionResult Delete(int id)
        {
            try
            {
                City city = _dal.GetCityById(id);
                if (city.CityId == 0)
                {
                    TempData["errorMessage"] = $"City details with Id {id} cannot be found";
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {

                TempData["errorMessage"] = ex.Message;
                return View();
            }
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
                TempData["successMessage"] = "Record deleted successfully";
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
