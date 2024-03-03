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
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Mvc;
using iText.Layout;
using iText.IO.Image;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace HelpingHands.Controllers
{
	public class OfficeManagerController : Controller
	{

		private readonly DataAccessLayer _dal;
		private readonly INotyfService _toastNotification;

		public OfficeManagerController(DataAccessLayer dal,
			INotyfService notyfService)
		{
			_dal = dal;
			_toastNotification = notyfService;
		}
		public IActionResult GetAllPatient()
		{
			List<Patient> patients = new List<Patient>();

			try
			{
				patients = _dal.GetAllPatients();
			}
			catch (Exception ex)
			{
				TempData["errorMessage"] = ex.Message;
			}

			return View(patients);
		}

        public IActionResult GetAllNurses()
        {
            List<Nurse> nurses = new List<Nurse>();

            try
            {
                nurses = _dal.GetAllNurses();
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
            }

            return View(nurses);
        }

        public IActionResult GetNurseSuburb(int id)
        {
            List<PrefferedSuburb> suburbs = new List<PrefferedSuburb>();

            try
            {
                suburbs = _dal.GetNurseSuburb(id);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
            }

            return View(suburbs);
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
        public IActionResult GetContractCareVisits()
        {
			List<ContractCareVisits> visits = new List<ContractCareVisits>();

			try
			{
				visits = _dal.GetContractCareVisits();
			}
			catch (Exception ex)
			{
				TempData["errorMessage"] = ex.Message;
			}

			return View(visits);
		}

		public IActionResult GetNewContracts()
        {
            List<CareContract> contracts = new List<CareContract>();

            try
            {
                contracts = _dal.GetNewContracts();
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
            }

            return View(contracts);
        }

        public IActionResult GetCareContracts()
        {
            List<CareContract> contracts = new List<CareContract>();

            try
            {
                contracts = _dal.GetAllContracts();
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
            }

            return View(contracts);
        }

        public IActionResult GetAllContracts()
        {
            List<CareContract> contracts = new List<CareContract>();

            try
            {
                contracts = _dal.GetAllContracts();
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
            }

            return View(contracts);
        }


        [HttpGet]
        public IActionResult GetNurseByContractSuburb(int Id,int anotherId)
        {
            var contract = _dal.GetContractById(anotherId);
            var nurses = _dal.GetNurseByContractSuburbId(Id);
            return View(new Tuple<CareContract, List<Nurse>>(contract, nurses));
        }

        [HttpPost]
        public IActionResult AssignContract(int nurseId, int contractId)
        {
            try
            {
                var contract = _dal.GetContractById(contractId);
                if (contract == null)
                {
                    TempData["errorMessage"] = "Invalid contract.";
                    return RedirectToAction("GetNewContracts", "OfficeManager");
                }
                contract.NurseId = nurseId;
                contract.Status = "Assigned";
                if (ModelState.IsValid)
                {
                    bool result = _dal.AssignContract(contract);

                    if (!result)
                    {
                        _toastNotification.Success("Contract assigned successfully");
                        return RedirectToAction("GetNewContracts", "OfficeManager");
                    }
                    else
                    {
                        TempData["errorMessage"] = "Care contract cannot be assigned";
                        return View("ErrorView");
                    }
                }
                

                
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View("ErrorView"); 
            }
            return View();
        }


        public IActionResult GenerateContractReport()
        {


            // Retrieve product data from the database
            var contract = _dal.GetContractCareVisits();

            // Generate report and save as PDF
            var filePath = "wwwroot/css/ContractCareVisit.pdf";
            GenerateContractReport(contract, filePath);

            // Return a file download response
            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "application/pdf", "ContractCareVisit.pdf");
        }
        private void GenerateContractReport(List<ContractCareVisits> contractCareVisits, string filePath)
        {
            // Retrieve business information
            List<HelpingHandsBusiness> businessInfo = _dal.GetBusinessInformation();

            using (var writer = new PdfWriter(filePath))
            {
                using (var pdfDocument = new PdfDocument(writer))
                {
                    var document = new Document(pdfDocument);

                    // Add business information section on the left
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
                    headerTable.AddHeaderCell(CreateHeaderCell("Patient Contract").SetHorizontalAlignment(HorizontalAlignment.RIGHT));
                    headerTable.AddHeaderCell(CreateHeaderCell("---------------------------").SetHorizontalAlignment(HorizontalAlignment.RIGHT));
                    headerTable.AddCell(CreateTableCell($"Date: {DateTime.Now.ToString("d-MMMM-yyyy")}").SetHorizontalAlignment(HorizontalAlignment.RIGHT));
                    headerTable.AddCell(CreateTableCell($"Time: {DateTime.Now.ToString("HH:mm")}").SetHorizontalAlignment(HorizontalAlignment.RIGHT));

                    document.Add(headerTable);

                    // Group contract visits by Full Name and Address
                    var groupedContracts = contractCareVisits
                        .GroupBy(c => (c.Firstname, c.Surname, c.AddressLine1))
                        .ToList();

                    // Iterate through grouped contracts
                    foreach (var group in groupedContracts)
                    {
                        var (firstname, surname, address) = group.Key;
                        var contracts = group.ToList();

                        // Add Full Name and Address as subheading
                        document.Add(new Paragraph($"{firstname} {surname} - {address}").SetBold());

                        // Add contract details under each Full Name
                        var contractTable = new iText.Layout.Element.Table(UnitValue.CreatePercentArray(5)).UseAllAvailableWidth();
                        contractTable.AddHeaderCell(CreateHeaderCell("Contract Date"));
                        contractTable.AddHeaderCell(CreateHeaderCell("Visit Date"));                        
                        contractTable.AddHeaderCell(CreateHeaderCell("Arrival Time"));
                        contractTable.AddHeaderCell(CreateHeaderCell("Departure Time"));
                        contractTable.AddHeaderCell(CreateHeaderCell("Wound Condition"));

                        // Add rows for each contract visit
                        foreach (var contract in contracts)
                        {
                            Console.WriteLine($"ApproxArrival: {contract.ApproxArrival}, ArrivalTime: {contract.ArrivalTime}, DepartTime: {contract.DepartTime}");
                            contractTable.AddCell(CreateTableCell(contract.ContractDate.ToString("d-MMM-yyyy")));
                            contractTable.AddCell(CreateTableCell(contract.VisitDate.ToString("d-MMM-yyyy")));                            
                            contractTable.AddCell(CreateTableCell(contract.ArrivalTime.ToString(@"hh\:mm")));
                            contractTable.AddCell(CreateTableCell(contract.DepartTime.ToString(@"hh\:mm")));
                            contractTable.AddCell(CreateTableCell(contract.WoundCondition));
                        }

                        document.Add(contractTable);
                        document.Add(new Paragraph().SetMarginBottom(20)); // Add some space between each group
                    }

                    // Add page numbering
                    var pageNumber = new PageNumberDocumentEventHandler();
                    pdfDocument.AddEventHandler(PdfDocumentEvent.END_PAGE, pageNumber);

                    document.Close();
                }
            }
        }




        //Nurse And Suburbs Report
        public IActionResult GenerateReport()
        {

            
            // Retrieve product data from the database
            var careVisits = _dal.GetNurseSuburbReport();

            // Generate report and save as PDF
            var filePath = "wwwroot/css/SuburbPerNurse.pdf";
            GeneratePDFReport(careVisits, filePath);

            // Return a file download response
            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "application/pdf", "SuburbPerNurse.pdf");
        }
        private void GeneratePDFReport(List<NurseSuburbVM> conditions, string filePath)
        {
            // Retrieve business information
            List<HelpingHandsBusiness> businessInfo = _dal.GetBusinessInformation();

            using (var writer = new PdfWriter(filePath))
            {
                using (var pdfDocument = new PdfDocument(writer))
                {
                    var document = new Document(pdfDocument);

                    // Add business information section on the left
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
                    headerTable.AddHeaderCell(CreateHeaderCell("Suburbs Per Nurse").SetHorizontalAlignment(HorizontalAlignment.RIGHT));
                    headerTable.AddHeaderCell(CreateHeaderCell("---------------------------").SetHorizontalAlignment(HorizontalAlignment.RIGHT));
                    headerTable.AddCell(CreateTableCell($"Date: {DateTime.Now.ToString("d-MMMM-yyyy")}").SetHorizontalAlignment(HorizontalAlignment.RIGHT));
                    headerTable.AddCell(CreateTableCell($"Time: {DateTime.Now.ToString("HH:mm")}").SetHorizontalAlignment(HorizontalAlignment.RIGHT));

                    document.Add(headerTable);

                    // Create a dictionary to store suburbs grouped by Name and Surname
                    Dictionary<(string, string), List<string>> groupedSuburbs = conditions
                        .GroupBy(c => (c.Name, c.Surname))
                        .ToDictionary(g => g.Key, g => g.Select(c => c.Surburb).ToList());

                    // Add product table
                    var table = new iText.Layout.Element.Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth();

                    // Set table headers
                    table.AddHeaderCell(CreateHeaderCell("Full Name"));
                    table.AddHeaderCell(CreateHeaderCell("Suburbs"));

                    foreach (var group in groupedSuburbs)
                    {
                        var (name, surname) = group.Key;
                        List<string> suburbsForNurse = group.Value;

                        // Add a row for the current nurse's full name and list of suburbs
                        table.AddCell(CreateTableCell($"{name} {surname}"));
                        table.AddCell(CreateTableCell(string.Join(", ", suburbsForNurse)));
                    }

                    document.Add(table);

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
