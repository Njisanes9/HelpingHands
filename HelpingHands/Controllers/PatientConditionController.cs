using AspNetCoreHero.ToastNotification.Abstractions;
using HelpingHands.DAL;
using HelpingHands.Models;
using HelpingHands.ViewModels;
using iText.Kernel.Colors;
using iText.Kernel.Events;
using iText.Kernel.Font;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics.Contracts;
using iText.Layout;
using iText.IO.Image;

namespace HelpingHands.Controllers
{
    public class PatientConditionController : Controller
    {
        private readonly DataAccessLayer _dal;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly INotyfService _toastNotification;

        public PatientConditionController(DataAccessLayer dal,
            IHttpContextAccessor httpContextAccessor,
            INotyfService notyfService)
        {
            _dal = dal;
            _contextAccessor = httpContextAccessor;
            _toastNotification = notyfService;
        }
        public IActionResult Index()
        {
            List<PatientCondition> conditions = new List<PatientCondition>();
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            HttpContext.Session.SetInt32("UserId", userId);
            int patientId = _dal.GetPatientId(userId);

            try
            {
                conditions = _dal.GetPatientCondition(patientId);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
            }
            return View(conditions);
        }

        public IActionResult GetPatientsByNurse()
        {
            List<Patient> patients = new List<Patient>();
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;            
            int patientId = _dal.GetNurseId(userId);

            try
            {
                patients = _dal.sp_GetPatientsByAssignedNurse(patientId);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
            }
            return View(patients);
        }


        public IActionResult GetPatientCondition(int id)
        {
            List<PatientCondition> patients = new List<PatientCondition>();

            try
            {
                patients = _dal.GetPatientCondition(id);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
            }

            return View(patients);
        }

        [HttpGet]
        public IActionResult CreatePatientCondition()
        {
            ViewBag.ConditionList = _dal.GetCondition().Select(c => new ConditionDescription
            {
                Value = c.ConditionId.ToString(),
                Text = c.ConditionName,
                ConditionDescr = c.ConditionDescr
            });
                       
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreatePatientCondition(PatientCondition condition)
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            HttpContext.Session.SetInt32("UserId", userId);

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Model is not valid";
                
                ViewBag.ConditionList = _dal.GetCondition().Select(c => new ConditionDescription
                {
                    Value = c.ConditionId.ToString(),
                    Text = c.ConditionName,
                    ConditionDescr = c.ConditionDescr
                });
                return View(condition);
            }

            int patientId = _dal.GetPatientId(userId);

            if (patientId != null)
            {
                condition.PatientId = patientId;
                if (ModelState.IsValid)
                {
                    bool result = _dal.InsertPatientCondition(condition);

                    if (!result)
                    {
                        _toastNotification.Success("Record is added successfully");
                   
                        
                        ViewBag.ConditionList = _dal.GetCondition().Select(c => new ConditionDescription
                        {
                            Value = c.ConditionId.ToString(),
                            Text = c.ConditionName,
                            ConditionDescr = c.ConditionDescr
                        });
                        return View(condition);
                    }

                  
                    return RedirectToAction("Index");
                }
            }

            ViewBag.ConditionList = _dal.GetCondition().Select(c => new ConditionDescription
            {
                Value = c.ConditionId.ToString(),
                Text = c.ConditionName,
                ConditionDescr = c.ConditionDescr
            });

            return View(condition);
        }

        [HttpPost]
        public IActionResult DeleteCondition(int id)
        {
            PatientCondition condition = _dal.GetPatientConditionById(id);
           
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Model is not valid";
            }

            if (ModelState.IsValid)
            {
                
                bool result = _dal.DeletePatientCondition(condition);

                if (!result)
                {
                    _toastNotification.Success("Condition is deleted successfully", 4);
                    return RedirectToAction("Index", "PatientCondition");
                }
                else
                {
                    TempData["ErrorMessage"] = "Unable to to delete condtion";
                }
                                

            }
            return View("Index", "PatientCondition");
        }

        public IActionResult GenerateReport()
        {

            DateTime date = DateTime.Now;
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            int nurseId = _dal.GetNurseId(userId);
            // Retrieve product data from the database
            var careVisits = _dal.GetPatientConditionReport(nurseId);

            // Generate report and save as PDF
            var filePath = "wwwroot/css/PatientCondition.pdf";
            GeneratePDFReport(careVisits, filePath);

            // Return a file download response
            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "application/pdf", "PatientCondition.pdf");
        }
        private void GeneratePDFReport(List<PatientConditionVM> conditions, string filePath)
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
                    headerTable.AddHeaderCell(CreateHeaderCell("Patient Conditions").SetHorizontalAlignment(HorizontalAlignment.RIGHT));
                    headerTable.AddHeaderCell(CreateHeaderCell("---------------------------").SetHorizontalAlignment(HorizontalAlignment.RIGHT));
                    headerTable.AddCell(CreateTableCell($"Date: {DateTime.Now.ToString("d-MMMM-yyyy")}").SetHorizontalAlignment(HorizontalAlignment.RIGHT));
                    headerTable.AddCell(CreateTableCell($"Time: {DateTime.Now.ToString("HH:mm")}").SetHorizontalAlignment(HorizontalAlignment.RIGHT));

                    document.Add(headerTable);

                    

                    // Create a dictionary to store conditions grouped by Name and Surname
                    Dictionary<(string, string), List<PatientConditionVM>> groupedConditions = conditions
                        .GroupBy(c => (c.Name, c.Surname))
                        .ToDictionary(g => g.Key, g => g.ToList());

                    // Create a dictionary to store the total count of each condition name
                    Dictionary<string, int> conditionTotalCounts = new Dictionary<string, int>();

                    // Add product table
                    var table = new iText.Layout.Element.Table(UnitValue.CreatePercentArray(3)).UseAllAvailableWidth();

                    // Set table headers
                    table.AddHeaderCell(CreateHeaderCell("Full Name"));
                    table.AddHeaderCell(CreateHeaderCell("Condition Name"));
                    table.AddHeaderCell(CreateHeaderCell("Condition Description"));

                    foreach (var group in groupedConditions)
                    {
                        var (name, surname) = group.Key;
                        List<PatientConditionVM> conditionsForPatient = group.Value;

                        // Add a row for the current patient's full name
                        table.AddCell(CreateTableCell($"{name} {surname}"));
                        table.AddCell(new Cell(1, 2).Add(new Paragraph(""))); // Empty cells for spacing

                        // Group conditions by condition name for the current patient
                        var groupedConditionsByName = conditionsForPatient
                            .GroupBy(c => c.ConditionName)
                            .ToDictionary(g => g.Key, g => g.ToList());

                        // Add a row for each condition name for the current patient
                        foreach (var groupByName in groupedConditionsByName)
                        {
                            string conditionName = groupByName.Key;
                            List<PatientConditionVM> conditionsForName = groupByName.Value;

                            // Update the total count of the condition name
                            conditionTotalCounts.TryGetValue(conditionName, out int totalCount);
                            totalCount += conditionsForName.Count;
                            conditionTotalCounts[conditionName] = totalCount;

                            // Add a row for the condition name and condition description
                            table.AddCell(new Cell().Add(new Paragraph("")));
                            table.AddCell(CreateTableCell(conditionName));
                            table.AddCell(CreateTableCell(conditionsForName.FirstOrDefault()?.ConditionDescr ?? ""));
                        }
                    }

                    document.Add(table);

                    // Add a section below the table for unique condition names and their total count
                    document.Add(new Paragraph().SetMarginTop(20));
                    document.Add(new Paragraph("Unique Condition Names and Total Counts").SetBold());

                    var conditionTotalTable = new iText.Layout.Element.Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth();
                    conditionTotalTable.AddHeaderCell(CreateHeaderCell("Condition Name"));
                    conditionTotalTable.AddHeaderCell(CreateHeaderCell("Total Count"));

                    foreach (var conditionTotal in conditionTotalCounts)
                    {
                        conditionTotalTable.AddCell(CreateTableCell(conditionTotal.Key));
                        conditionTotalTable.AddCell(CreateTableCell(conditionTotal.Value.ToString()));
                    }

                    document.Add(conditionTotalTable);

                    // Add page numbering
                    var pageNumber = new PageNumberDocumentEventHandler();
                    pdfDocument.AddEventHandler(PdfDocumentEvent.END_PAGE, pageNumber);

                    document.Close();
                }
            }
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
