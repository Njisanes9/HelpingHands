using AspNetCoreHero.ToastNotification.Abstractions;
using HelpingHands.DAL;
using HelpingHands.Models;
using HelpingHands.ViewModels;
using Microsoft.AspNetCore.Mvc;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout;
using iText.Kernel.Events;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf.Canvas;
using iText.Layout.Borders;
using iText.IO.Image;

namespace HelpingHands.Controllers
{
    public class CareVisitController : Controller
    {
        private readonly DataAccessLayer _dal;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly INotyfService _toastNotification;

        public CareVisitController(DataAccessLayer dal,
            IHttpContextAccessor contextAccessor,
            INotyfService toastNotification)
        {
            _dal = dal;
            _contextAccessor = contextAccessor;
            _toastNotification = toastNotification;
        }

        [HttpGet]
        public IActionResult CreateCareVisit(int id)
        {
            var careVisit = new CareVisitVM();
            int userId = HttpContext.Session.GetInt32("UserId")??0;
            int nurseId = _dal.GetNurseId(userId);
                                   

            try
            {
                careVisit.ContractId = id;
                if(careVisit.ContractId == 0)
                {
                    TempData["ErrorMessage"] = $"Care contract with id {careVisit.ContractId} is not found";
                }

                return View(careVisit);

            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
            
            
        }
        [HttpPost]
        public IActionResult CreateCareVisit(CareVisitVM visit)
        {
            if (!ModelState.IsValid)
            {
                TempData["errorMessage"] = "Model is not valid";
            }

            if (ModelState.IsValid)
            {
                bool result = _dal.InsertCareVisit(visit);

                if (!result)
                {
                    _toastNotification.Success("Care visit has been created successfully");
                    
                    return View();
                }
                TempData["errorMessage"] = "Care visit details cannot be saved";
                return RedirectToAction("Nurse", "Dashboard");
            }
            return View(visit);
        }

        public IActionResult GetPatientCareVisitByContract(int id)
        {
            List<CareVisit> visits = new List<CareVisit>();

            try
            {
                visits = _dal.GetCareVisitByContractId(id);
                if (visits == null)
                {
                    TempData["errorMessage"] = $"No care visit was found with the id {id}";
                }
                return View(visits);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
            }

            return View(visits);
        }

		public IActionResult GetPatientCareVisit(int id)
		{
			List<CareVisitDetailsVM> visits = new List<CareVisitDetailsVM>();
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            int patientId = _dal.GetPatientId(userId);  

			try
			{
				visits = _dal.GetPatientCareVisit(patientId);
				if (visits == null)
				{
					TempData["errorMessage"] = $"No care visit was found with the id {id}";
				}
				return View(visits);
			}
			catch (Exception ex)
			{
				TempData["errorMessage"] = ex.Message;
			}

			return View(visits);
		}


		public IActionResult GetAllCareVisits()
		{
			List<CareVisitDetailsVM> visits = new List<CareVisitDetailsVM>();
			

			try
			{
				visits = _dal.GetAllCareVisit();
				if (visits == null)
				{
					TempData["errorMessage"] = $"No care visit was found with the id";
				}
				return View(visits);
			}
			catch (Exception ex)
			{
				TempData["errorMessage"] = ex.Message;
			}

			return View(visits);
		}


		[HttpGet]
        public IActionResult GetCareVisitByContract(int id)
        {
            List<CareVisit> visits = new List<CareVisit>();            
            
            try
            {
                visits = _dal.GetCareVisitByContractId(id);
                if (visits == null)
                {
                    TempData["errorMessage"] = $"No care visit was found with the id {id}";
                }
                return View(visits);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
            }

            return View(visits);
        }

        [HttpGet]
        public IActionResult GetUpComingVisit(DateTime date, int nurseId)
        {
            List<UpComingVisitVM> visits = new List<UpComingVisitVM>();

            var visit = new UpComingVisitVM();
            date = DateTime.Now;
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            nurseId = _dal.GetNurseId(userId);
            try
            {
                visits = _dal.GetUpComingVisits(nurseId, date);
                if (visits == null)
                {
                    TempData["errorMessage"] = $"No care visit was found nurse {nurseId}";
                }
                return View(visits);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
            }

            return View(visits);
        }




        //Updating Care Visit Details
        [HttpGet]
        public IActionResult CareVisitDetailsByContract(int id)
        {
            try
            {
                VisitDetailsVM visit = _dal.GetCareVisitDetailsById(id);
                if (visit.VisitId == 0)
                {
                    TempData["errorMessage"] = $"Care visit details with Id {id} cannot be found";
                    return RedirectToAction("Nurse", "Dashboard");
                }

                return View(visit);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
           
        }
        [HttpPost]
        public IActionResult CareVisitDetailsByContract(VisitDetailsVM visitDetails)
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
                    if(visitDetails.VisitArrivalTime > visitDetails.VisitDepartTime)
                    {
                        TempData["errorMessage"] = "Arrival time cannot be greater than depart time";
                        return View();
                    }
                    else if(  visitDetails.VisitDepartTime < visitDetails.VisitArrivalTime)
                    {
                        TempData["errorMessage"] = "Depart time cannot be less than arrival time";
                        return View();
                    }
                    else
                    {
                        bool result = _dal.InsertCareVisitDetails(visitDetails);

                        if (!result)
                        {
                            _toastNotification.Success("Care visit details updated successfully");

                        }
                        else
                        {

                            TempData["errorMessage"] = "Care visit cannot be updated";
                            return View();

                        }
                    }
                    
                }

                return RedirectToAction("GetNurseAssignedContract", "Contract");


            }
            catch (Exception ex)
            {

                TempData["errorMessage"] = ex.Message;
                return View();
            }
        }


        public IActionResult GenerateReport()
        {

            DateTime date = DateTime.Now;
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            int nurseId = _dal.GetNurseId(userId);
            // Retrieve product data from the database
            var careVisits = _dal.GetUpcomingVisitReport(nurseId,date);

            // Generate report and save as PDF
            var filePath = "wwwroot/css/UpcomingCareVisit.pdf";
            GeneratePDFReport(careVisits, filePath);

            // Return a file download response
            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "application/pdf", "UpcomingCareVisit.pdf");
        }
        private void GeneratePDFReport(List<UpComingVisitVM> careVisits, string filePath)
        {

            List<HelpingHandsBusiness> businessInfo = _dal.GetBusinessInformation();
            using (var writer = new PdfWriter(filePath))
            {
                using (var pdfDocument = new PdfDocument(writer))
                {
                    var document = new Document(pdfDocument);

                    // Add header
                    foreach (var business in businessInfo)
                    {
                        // Check if there is a picture available
                        if (business.picture != null && business.picture.Length > 0)
                        {
                            // Assuming you are using iTextSharp Image class
                            var image = new Image(ImageDataFactory.Create(business.picture)).SetWidth(100f);
                            document.Add(image);
                        }

                        // Add other business information
                        document.Add(new Paragraph(business.orgName).SetBold());
                        document.Add(new Paragraph($"NPO Number: {business.npoNumber}"));
                        document.Add(new Paragraph($"Email: {business.Email}"));
                        document.Add(new Paragraph($"Phone: {business.contactNumber}"));
                        document.Add(new Paragraph($"Operating Hours: {business.operatingHours}"));
                        document.Add(new Paragraph($"Address: {business.address}"));
                        document.Add(new Paragraph().SetMarginBottom(20));
                    }

                    // Add header on the right
                    var headerTable = new iText.Layout.Element.Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth();
                    headerTable.AddHeaderCell(CreateHeaderCell("Upcoming Care Visits").SetHorizontalAlignment(HorizontalAlignment.RIGHT));
                    headerTable.AddHeaderCell(CreateHeaderCell("---------------------------").SetHorizontalAlignment(HorizontalAlignment.RIGHT));
                    headerTable.AddCell(CreateTableCell($"Date: {DateTime.Now.ToString("d-MMMM-yyyy")}").SetHorizontalAlignment(HorizontalAlignment.RIGHT));
                    headerTable.AddCell(CreateTableCell($"Time: {DateTime.Now.ToString("HH:mm")}").SetHorizontalAlignment(HorizontalAlignment.RIGHT));

                    document.Add(headerTable);

                    var table = new iText.Layout.Element.Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth();

                    foreach (var visit in careVisits)
                    {
                        table.AddCell(CreateTableCell(visit.ApproxArrivalTime.ToString(@"hh\:mm")));
                        table.AddCell(CreateTableCell(visit.VisitDate.ToString("dd/MM/yyyy")));
                        table.AddCell(CreateTableCell(visit.SuburbName));
                        table.AddCell(CreateTableCell(visit.AddressLine1.ToString()));

                         
                    }

                    // Add table footer with total
                    table.AddFooterCell(new Cell().SetBorder(Border.NO_BORDER)); 
                    table.AddFooterCell(new Cell().SetBorder(Border.NO_BORDER)); 
                    table.AddFooterCell(new Cell().SetBorder(Border.NO_BORDER)); 
                    
                    table.AddFooterCell(new Cell().SetBorder(Border.NO_BORDER)); 
                    table.AddFooterCell(new Cell().SetBorder(Border.NO_BORDER)); 
                    table.AddFooterCell(new Cell().SetBorder(Border.NO_BORDER)); 
                    

                    document.Add(table);

                    // Add page numbering
                    var pageNumber = new PageNumberDocumentEventHandler();
                    pdfDocument.AddEventHandler(PdfDocumentEvent.END_PAGE, pageNumber);

                    document.Close();
                }
            }

            Console.WriteLine("Report generated successfully and saved to the specified PDF file.");
        }

        private Cell CreateHeaderCell(string text)
        {
            var cell = new Cell().SetTextAlignment(TextAlignment.CENTER).SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                .SetPadding(5).SetFontColor(DeviceRgb.WHITE).SetFont(PdfFontFactory.CreateFont());

            cell.Add(new Paragraph(text).SetBold());

            return cell;
        }

        private Cell CreateTableCell(string text)
        {
            return new Cell().SetTextAlignment(TextAlignment.CENTER).SetPadding(5).Add(new Paragraph(text));
        }
        public class PageNumberDocumentEventHandler : IEventHandler
        {
            private int pageCount;

            public void HandleEvent(Event currentEvent)
            {
                PdfDocumentEvent docEvent = (PdfDocumentEvent)currentEvent;
                PdfDocument pdfDoc = docEvent.GetDocument();
                PdfPage page = docEvent.GetPage();

                pageCount = pdfDoc.GetNumberOfPages();
                PdfCanvas canvas = new PdfCanvas(page.NewContentStreamBefore(), page.GetResources(), pdfDoc);
                iText.Kernel.Geom.Rectangle pageSize = page.GetPageSizeWithRotation();
                canvas.BeginText()
                    .SetFontAndSize(PdfFontFactory.CreateFont(), 10)
                    .MoveText(pageSize.GetWidth() - 100, 20)
                    .ShowText($"Page {pageCount}")
                    .EndText();
                canvas.Release();
            }
        }

    }
}
