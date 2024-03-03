using AspNetCoreHero.ToastNotification.Abstractions;
using HelpingHands.DAL;
using HelpingHands.Models;
using Microsoft.AspNetCore.Mvc;

namespace HelpingHands.Controllers
{
    public class ConditionController : Controller
    {
        private readonly DataAccessLayer _dal;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly INotyfService _toastNotification;

        public ConditionController(DataAccessLayer dal,
            INotyfService toastNotification,
            IHttpContextAccessor httpContextAccessor)
        {
            _dal = dal;
            _contextAccessor = httpContextAccessor;
            _toastNotification = toastNotification;
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<ChronicCondition> conditionList = new List<ChronicCondition>();

            try
            {
                conditionList = _dal.GetCondition();
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
            }

            return View(conditionList);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(ChronicCondition condition)
        {

            if (!ModelState.IsValid)
            {
                TempData["errorMessage"] = "Data is not valid";
            }

            bool result = _dal.InsertCondition(condition);

            if (!result)
            {
                _toastNotification.Success("Condition is saved successfully");
                return View(condition);
            }
            else
            {
                TempData["errorMessage"] = "Data cannot be saved";
                return View(condition);
            }
            
            
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            try
            {

                ChronicCondition condition = _dal.GetConditionById(id);
                condition.ConditionId = id;
                if (condition.ConditionId == 0)
                {
                    TempData["errorMessage"] = $"Condition details with Id {id} cannot be found";
                    return RedirectToAction("Index");
                }


                return View(condition);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
        }
        [HttpPost]
        public IActionResult Edit(ChronicCondition condition)
        {

            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["errorMessage"] = "Data is not valid";
                    return View();
                }

                bool result = _dal.UpdateCondition(condition);

                if (!result)
                {
                    _toastNotification.Success("Condition is updated successfully");
                    
                    return RedirectToAction("Index","Condition");
                }
                else
                {
                    TempData["errorMessage"] = "Data cannot be updated";
                    return View();
                }
                
               
            }
            catch (Exception ex)
            {

                TempData["errorMessage"] = ex.Message;
                return View();
            }
        }


        
        
    }
}
