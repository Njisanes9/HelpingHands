using AspNetCoreHero.ToastNotification.Abstractions;
using HelpingHands.DAL;
using HelpingHands.Models;
using HelpingHands.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace HelpingHands.Controllers
{
    public class SuburbController : Controller
    {
        private readonly DataAccessLayer _dal;
        private readonly INotyfService _toastNotification;
        public SuburbController(DataAccessLayer dal,
            INotyfService notyfService)
        {
            _dal = dal;
            _toastNotification = notyfService;
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
            if (ModelState.IsValid)
            {
                if (!result)                    
                {
                    _toastNotification.Success("Suburb is saved successfully");
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["errorMessage"] = "Data cannot be saved";
                    return View();
                }
                
                
            }
            
            ViewBag.CityList = new SelectList(_dal.GetCities(), "CityId", "CityName");
            return View(suburb);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            ViewBag.CityList = new SelectList(_dal.GetCities(), "CityId", "CityName");
            try
            {

                Suburb suburb = _dal.GetSuburbById(id);
                suburb.SuburbId = id;
                if (suburb.SuburbId == 0)
                {
                    TempData["errorMessage"] = $"Suburb details with Id {id} cannot be found";
                    return RedirectToAction("Index");
                }


                return View(suburb);
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

            if (!ModelState.IsValid)
            {
                TempData["errorMessage"] = "Data is not valid";
                return View();
            }
            if(ModelState.IsValid)
            {
                bool result = _dal.UpdateSuburb(suburb);

                if (!result)
                {
                    _toastNotification.Success("Suburb is updated successfully", 5);
                    return RedirectToAction("Index");
                    
                }
                else
                {
                    TempData["errorMessage"] = "Data cannot be updated";
                    return View();
                }
                
            }
            ViewBag.CityList = new SelectList(_dal.GetCities(), "CityId", "CityName");
            return View(suburb);
            
        }

       
        [HttpGet]
        public IActionResult CreatePreferredSuburb()
        {
           
            ViewBag.SuburbList = new SelectList(_dal.GetSuburb(), "SuburbId", "SuburbName");
            return View();
        }
        [HttpPost]
        public IActionResult CreatePreferredSuburb(PrefferedSuburb prefSuburb)
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
                                                

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Model is not valid";
                
                ViewBag.SuburbList = new SelectList(_dal.GetSuburb(), "SuburbId", "SuburbName");

                return View(prefSuburb);
            }

            int nurseId = _dal.GetNurseId(userId);

            if (nurseId != 0)
            {
                prefSuburb.NurseId = nurseId;
                if (ModelState.IsValid)
                {
                    bool result = _dal.InsertPreferredSuburb(prefSuburb);

                    if (!result)
                    {
                        _toastNotification.Success("Record has been inserted successfully",4);
                        
                        ViewBag.SuburbList = new SelectList(_dal.GetSuburb(), "SuburbId", "SuburbName");
                        return View(prefSuburb);
                    }


                    return RedirectToAction("Nurse", "Dashboard");
                }
            }
            
            ViewBag.SuburbList = new SelectList(_dal.GetSuburb(), "SuburbId", "SuburbName");
            
            return View(prefSuburb);
        }
        [HttpGet]
        public IActionResult GetPreferredSuburb()
        {
            List<PreferredSuburb> suburbList = new List<PreferredSuburb>();
            int userId = HttpContext.Session.GetInt32("UserId")??0;
            int nurseId = _dal.GetNurseId(userId);
            try
            {
                suburbList = _dal.GetNurseSuburbs(nurseId);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
            }

            return View(suburbList);
        }

        //Delete
        [HttpPost]
        public IActionResult DeletePreferredSuburb(int prefSuburbId, int nurseId)
        {
            try
            {
                int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
                nurseId = _dal.GetNurseId(userId);

                PrefferedSuburb suburb = _dal.GetPreferredSuburbById(prefSuburbId);

                if (suburb == null)
                {
                    TempData["errorMessage"] = $"Suburb details with Id {prefSuburbId} cannot be found";
                    return RedirectToAction("Nurse", "Dashboard");
                }

                bool result = _dal.DeletePreferredSuburb(prefSuburbId, nurseId);

                if (result)
                {
                    _toastNotification.Success("Preferred suburb deleted successfully");
                }

                return RedirectToAction("Nurse", "Dashboard");
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return RedirectToAction("Nurse", "Dashboard");
            }

        }
       

        public IActionResult GetSuburbs(int cityId)
        {
            var suburbs = _dal.GetSuburbByCityId(cityId);
            var suburbList = suburbs.Select(suburb => new SelectListItem
            {
                Text = suburb.SuburbName,
                Value = suburb.SuburbId.ToString()
            });

            return Json(suburbList);
        }
    }
}
