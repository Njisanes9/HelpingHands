using HelpingHands.DAL;
using HelpingHands.Models;
using Microsoft.AspNetCore.Mvc;

namespace HelpingHands.Controllers
{
    public class ConditionController : Controller
    {
        private readonly DataAccessLayer _dal;

        public ConditionController(DataAccessLayer dal)
        {
            _dal = dal;
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
