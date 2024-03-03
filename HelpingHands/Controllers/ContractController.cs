using AspNetCoreHero.ToastNotification.Abstractions;
using HelpingHands.DAL;
using HelpingHands.Models;
using HelpingHands.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data;
using Microsoft.Data.SqlClient;
using NToastNotify;
using System.Diagnostics.Contracts;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace HelpingHands.Controllers
{
    public class ContractController : Controller
    {
        private readonly DataAccessLayer _dal;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly INotyfService _toastNotification;

        public ContractController(DataAccessLayer dal, 
            IHttpContextAccessor contextAccessor,
            INotyfService toastNotification)
        {
            _dal = dal;
            _contextAccessor = contextAccessor;
            _toastNotification = toastNotification;
        }


       
        

        public IActionResult CreateContract()
        {
            ViewBag.SuburbList = new SelectList(_dal.GetSuburb(), "SuburbId", "SuburbName");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateContract(CareContract contract)
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            HttpContext.Session.SetInt32("UserId", userId);

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Model is not valid";
            }

            contract.ContractDate = DateTime.Now;
            int patientId = _dal.GetPatientId(userId);
            contract.Status = "New";
            if(patientId <= 0)
            {
                TempData["UpdateProfile"] = "You cannot create contract because you have not updated profile";
            }
            else if (patientId != 0)
            {
                contract.PatientId = patientId;                

               
                bool result = _dal.InsertContract(contract);
                    
                if (!result)
                {
                    _toastNotification.Success("Contract create successfully successfully", 5);

                }

                return RedirectToAction("Patient", "Dashboard");
                
            }

            ViewBag.SuburbList = new SelectList(_dal.GetSuburb(), "SuburbId", "SuburbName");
            return View(contract);
        }

        [HttpGet]
        public IActionResult UpdateCareContract(int id)
        {
            
            try
            {
                CareContract careContract = _dal.GetContractById(id);
                if (careContract.ContractId == 0)
                {
                    TempData["errorMessage"] = $"Contract details with Id {id} cannot be found";
                }
                ViewBag.SuburbList = new SelectList(_dal.GetSuburb(), "SuburbId", "SuburbName");
                return View(careContract);
            }
            catch (Exception ex)
            {

                TempData["errorMessage"] = ex.Message;
                ViewBag.SuburbList = new SelectList(_dal.GetSuburb(), "SuburbId", "SuburbName");
                return View();
            }
            
        }
        [HttpPost]
        public IActionResult UpdateCareContract(ContractVM contract)
        {

            int userId = HttpContext.Session.GetInt32("UserId")??0;
            int patientId = _dal.GetPatientId(userId);
            contract.ContractDate = DateTime.Now;
            
            contract.Status = "New";

            if (!ModelState.IsValid)
            {
                TempData["errorMessage"] = "Data is not valid";
                return View();
            }
            try
            {
                if (ModelState.IsValid)
                {
                    contract.PatientId = patientId;
                    bool result = _dal.UpdatePatientContract(contract);

                    if (!result)
                    {
                        _toastNotification.Success("Contract is updated succesfully");
                        return View("GetPatientContract", "Contract");
                    }
                    else
                    {
                        TempData["errorMessage"] = "Contract cannot be updated";
                        return View();
                    }

                }
            }catch(Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View();
            }
            

            ViewBag.SuburbList = new SelectList(_dal.GetSuburb(), "SuburbId", "SuburbName");
            return View(contract);
               
            
        }

        [HttpPost]
        public IActionResult CancelContract(int id)
        {
            CareContract condition = _dal.GetContractById(id);

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Model is not valid";
            }

            if (ModelState.IsValid)
            {

                bool result = _dal.CancelContract(condition);

                if (!result)
                {
                    _toastNotification.Success("Condition is cancelled successfully", 4);
                    return RedirectToAction("GetPatientContract", "Contract");
                }
                else
                {
                    TempData["ErrorMessage"] = "Unable to to delete condtion";
                }


            }
            return View("GetPatientContract", "Contract");
        }

        [HttpGet]
        public IActionResult GetPatientContract()
        {
            List<CareContract> contractList = new List<CareContract>();
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            HttpContext.Session.SetInt32("UserId", userId);
            int patientId = _dal.GetPatientId(userId);
            try
            {
                contractList = _dal.GetPatientContract(patientId);
                return View(contractList);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
            }

            return View(contractList);
        }

       



        //Getting New Contracts By Nurse And Preferred Suburb
        [HttpGet]
        public IActionResult GetNurseContract()
        {
            List<CareContract> contractList = new List<CareContract>();
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            HttpContext.Session.SetInt32("UserId", userId);
            int nurseId = _dal.GetNurseId(userId);
            try
            {
                contractList = _dal.GetNewNurseContract(nurseId);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
            }
            
            return View(contractList);
        }

        //Nurse choose a contract they will work on
        [HttpGet]
        public IActionResult NurseSelectContract(int id)
        {
            
            try
            {
                CareContractVM careContract = _dal.GetNurseCareContractById(id);
                careContract.ContractId = id;
                if (id == 0)
                {
                    TempData["errorMessage"] = $"Contract details with Id {id} cannot be found";
                }
               
                return View(careContract);
            }
            catch (Exception ex)
            {

                TempData["errorMessage"] = ex.Message;
                return View();
            }
        }
        [HttpPost]
        public IActionResult NurseSelectContract(CareContractVM contract)
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            HttpContext.Session.SetInt32("UserId", userId);
            int nurseId = _dal.GetNurseId(userId);

            if (!ModelState.IsValid)
            {
                TempData["errorMessage"] = "Data is not valid";
                return View();
            }
                       
            
            contract.Status = "Assigned";
            
            if (ModelState.IsValid)
            {
                if(nurseId <= 0)
                {
                    
                    
                        TempData["UpdateProfile"] = "You cannot create contract because you have not updated profile";
                    
                }
                else
                {
                    if (contract.StartDate < DateTime.Today)
                    {
                        TempData["errorMessage"] = "Start date can only be from today onwards.";
                    }
                    else
                    {
                        contract.NurseId = nurseId;
                        bool result = _dal.NurseSelectContract(contract);


                        if (!result)
                        {
                            _toastNotification.Success("You have successfully selected the contract", 5);

                            return View();
                        }

                    }
                }

               
                
                return RedirectToAction("Nurse", "Dashboard");
            }

           
            return View(contract);
        }

        public IActionResult GetNurseAssignedContract()
        {
            List<CareContract> contractList = new List<CareContract>();
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            HttpContext.Session.SetInt32("UserId", userId);
            int nurseId = _dal.GetNurseId(userId);
            try
            {
                contractList = _dal.GetNurseAssignedContract(nurseId);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
            }

            return View(contractList);
        }
        public IActionResult GetNurseClosedContract()
        {
            List<CareContract> contractList = new List<CareContract>();
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            HttpContext.Session.SetInt32("UserId", userId);
            int nurseId = _dal.GetNurseId(userId);
            try
            {
                contractList = _dal.GetNurseClosedContract(nurseId);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
            }

            return View(contractList);
        }

        [HttpPost]
        public IActionResult CloseContract(int id)
        {
            CareContract contract = _dal.GetContractToCloseByIditDetailsById(id);

            contract.EndDate = DateTime.Today;
            contract.Status = "Closed";

            if (ModelState.IsValid)
            {
 
                bool result = _dal.CloseContract(contract);

                if (!result)
                {
                    _toastNotification.Success("You have successfully closed the contract", 5);

                    return RedirectToAction("GetNurseAssignedContract", "Contract");
                }
                                

                
            }
            return View();
        }
        
    }
}
