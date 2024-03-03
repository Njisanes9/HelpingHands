using AspNetCoreHero.ToastNotification.Abstractions;
using HelpingHands.DAL;
using HelpingHands.Models;
using Microsoft.AspNetCore.Mvc;

namespace HelpingHands.Controllers
{
    public class BusinessController : Controller
    {
        private readonly DataAccessLayer _dal;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly INotyfService _toastNotification;

        public BusinessController(DataAccessLayer dal,
            IHttpContextAccessor contextAccessor,
            INotyfService toastNotification)
        {
            _dal = dal;
            _contextAccessor = contextAccessor;
            _toastNotification = toastNotification;
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<HelpingHandsBusiness> businessInfo = new List<HelpingHandsBusiness>();

            try
            {
                businessInfo = _dal.GetBusinessInformation();
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
            }

            return View(businessInfo);
        }

        public bool IsImage(string name)
        {
            var extentionAccept = new[] { ".jpg", ".jpeg", ".png", ".gif" };

            var extention = Path.GetExtension(name).ToLower();
            return extentionAccept.Contains(extention);
        }

        [HttpGet]
        public IActionResult Create()
        {
            
            return View();
        }
        [HttpPost]
        public IActionResult Create(HelpingHandsBusiness business, IFormFile imageFile)
        {

            if (!ModelState.IsValid)
            {
                TempData["errorMessage"] = "Data is not valid";
            }

            if (imageFile != null && imageFile.Length > 0)
            {
                if (IsImage(imageFile.FileName))
                {
                    using(var stream = new MemoryStream())
                    {
                        imageFile.CopyTo(stream);
                        business.picture = stream.ToArray();
                    }
                  
                    bool result = _dal.InsertBusinessInformation(business);
                    if (ModelState.IsValid)
                    {
                        if (!result)
                        {
                            _toastNotification.Success("Record is saved successfully");
                            
                            return View();
                        }
                        TempData["errorMessage"] = "Data cannot be saved";
                        return RedirectToAction("Index");
                    }
                }
            }
                 

            
            return View(business);
        }

       

        [HttpGet]
        public IActionResult Edit(int id)
        {
           
            try
            {
                HelpingHandsBusiness business = _dal.GetBusinessById(id);
                if (business.businessId == 0)
                {
                    TempData["errorMessage"] = $"Business details with Id {id} cannot be found";
                }
                return View(business);
            }
            catch (Exception ex)
            {

                TempData["errorMessage"] = ex.Message;
                return View();
            }
        }
        [HttpPost]
        public IActionResult Edit(HelpingHandsBusiness business, IFormFile imageFile)
        {

            if (!ModelState.IsValid)
            {
                TempData["errorMessage"] = "Data is not valid";
            }

            if (imageFile != null && imageFile.Length > 0)
            {
                if (IsImage(imageFile.FileName))
                {
                    using (var stream = new MemoryStream())
                    {
                        imageFile.CopyTo(stream);
                        business.picture = stream.ToArray();
                    }
                    bool result = _dal.UpdateBusinessInformation(business);
                    if (ModelState.IsValid)
                    {
                        if (!result)
                        {
                            _toastNotification.Success("Record is updated successfully");                            
                            return View();
                        }
                        TempData["errorMessage"] = "Data cannot be saved";
                        return RedirectToAction("Index");
                    }
                }
            }



            return View(business);

        }

    }
}
