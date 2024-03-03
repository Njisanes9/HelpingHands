using HelpingHands.DAL;
using HelpingHands.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HelpingHands.Controllers
{
    public class SuburbController : Controller
    {
        private readonly DataAccessLayer _dal;

        public SuburbController(DataAccessLayer dal)
        {
            _dal = dal;
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<Suburb> suburbList = new List<Suburb>();

            try
            {
                suburbList = _dal.GetSuburb();
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
            }

            return View(suburbList);
        }
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.CityList = new SelectList(_dal.GetCities(), "CityId", "CityName");
            return View();
        }
        [HttpPost]
        public IActionResult Create(Suburb suburb)
        {

            if (!ModelState.IsValid)
            {
                TempData["errorMessage"] = "Data is not valid";
            }

            bool result = _dal.InsertSuburb(suburb);

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
                Suburb suburb = _dal.GetSuburbById(id);
                if (suburb.SuburbId == 0)
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
        public IActionResult Edit(Suburb suburb)
        {

            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["errorMessage"] = "Data is not valid";
                    return View();
                }

                bool result = _dal.UpdateSuburb(suburb);

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
                ChronicCondition condition = _dal.GetConditionById(id);
                if (condition.ConditionId == 0)
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
        public IActionResult DeleteConfirmed(ChronicCondition condition)
        {

            try
            {

                bool result = _dal.DeleteCity(condition.ConditionId);

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
