using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using OfficeOpenXml;
using Web.Data;
using Web.Models;

namespace Web.Controllers
{
    public class KLHKG_PriceBookController : Basic
    {
        private readonly FinanceContext _context;
        private IHostingEnvironment _hostingEnvironment;
        public KLHKG_PriceBookController(FinanceContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: KLHKG_PriceBook
        public IActionResult Index(string menu)
        {
            ViewData["menu"] = menu;

            return View();
        }
        [HttpPost]
        public async Task<String> GetData()
        {
            var jsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(await _context.KLHKG_PriceBook.ToListAsync());
            return jsonStr;
        }
        [HttpGet]
        public async Task<IActionResult> ExportExcel()
        {
            string sWebRootFolder = _hostingEnvironment.WebRootPath;
            string sFileName = @"assets/download/hkprice/HK Price Data_" + DateTime.Now.ToString("yyyyMMddhhmmssfff") + ".xlsx";
            string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, sFileName);
            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            var memory = new MemoryStream();
            using (var fs = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Create, FileAccess.Write))
            {
                IWorkbook workbook;
                workbook = new XSSFWorkbook();
                ISheet excelSheet = workbook.CreateSheet("Sheet1");
                IRow row = excelSheet.CreateRow(0);

                row.CreateCell(0).SetCellValue("Unique Code");
                row.CreateCell(1).SetCellValue("Custom Type");
                row.CreateCell(2).SetCellValue("SKU");
                row.CreateCell(3).SetCellValue("Currency");
                row.CreateCell(4).SetCellValue("Qty");
                row.CreateCell(5).SetCellValue("List Price");
                row.CreateCell(6).SetCellValue("Mgr Price");
                row.CreateCell(7).SetCellValue("SDPrice");

                var collection = await _context.KLHKG_PriceBook.ToListAsync();
                int i = 1;
                foreach(var item in collection)
                {
                    row = excelSheet.CreateRow(i);
                    row.CreateCell(0).SetCellValue(item.UniqueCode);
                    row.CreateCell(1).SetCellValue(item.CustType);
                    row.CreateCell(2).SetCellValue(item.SKU);
                    row.CreateCell(3).SetCellValue(item.Currency);
                    row.CreateCell(4).SetCellValue(item.Qty.ToString());
                    row.CreateCell(5).SetCellValue(item.ListPrice.ToString());
                    row.CreateCell(6).SetCellValue(item.MgrPrice.ToString());
                    row.CreateCell(7).SetCellValue(item.SDPrice.ToString());
                    i++;
                }

                workbook.Write(fs);
            }
            using (var stream = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sFileName);
        }
        [HttpPost]
        public async Task<JsonResult> ImportExcel()
        {
            string result = string.Empty, info = string.Empty;

            var files = Request.Form.Files;
            long filesize = files.Sum(f => f.Length);

            if (filesize > 1024 * 1024 * 30)
            {
                result = "Error";
                info = "File size must be less than 30 MB.";
            }

            if (files.Count > 0 && filesize > 0)
            {
                foreach (var f in files)
                {
                    string fileExtend = new FileInfo(f.FileName).Extension;
                    var filePath = _hostingEnvironment.WebRootPath + @"\assets\upload\hkprice\" + DateTime.Now.ToString("yyyyMMddhhmmssfff") + fileExtend;

                    if (f.Length > 0)
                    {
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await f.CopyToAsync(stream);
                        }
                    }


                    FileInfo file = new FileInfo(filePath);
                    try
                    {

                        using (ExcelPackage package = new ExcelPackage(file))
                        {
                            StringBuilder sb = new StringBuilder();
                            string sheetname = package.Workbook.Worksheets[1].Name;
                            ExcelWorksheet worksheet = package.Workbook.Worksheets[sheetname];
                            int rowCount = worksheet.Dimension.Rows;
                            int ColCount = worksheet.Dimension.Columns;

                            KLHKG_PriceBook[] klprice = new KLHKG_PriceBook[rowCount - 1];

                            int i = 0;
                            int maxid = _context.KLHKG_PriceBook.Max(m => m.ID);

                            for (int row = 2; row <= rowCount; row++)
                            {
                                string uniqueCode = worksheet.Cells[row, 2].Value.ToString().Trim();
                                uniqueCode += worksheet.Cells[row, 3].Value.ToString().Trim();
                                uniqueCode += ((int)Convert.ToDecimal(worksheet.Cells[row, 4].Value.ToString().Trim())).ToString();
                                uniqueCode += worksheet.Cells[row, 1].Value != null ? worksheet.Cells[row, 1].Value.ToString().Trim() : "";

                                KLHKG_PriceBook checkmodel = _context.KLHKG_PriceBook.Where(m => m.UniqueCode ==uniqueCode).SingleOrDefault();
                                if (checkmodel != null)
                                {

                                    checkmodel.ListPrice = Convert.ToDecimal(worksheet.Cells[row, 5].Value.ToString().Trim());
                                    checkmodel.MgrPrice = Convert.ToDecimal(worksheet.Cells[row, 6].Value.ToString().Trim());
                                    checkmodel.SDPrice = Convert.ToDecimal(worksheet.Cells[row, 7].Value.ToString().Trim());
                                    await _context.SaveChangesAsync();
                                }
                                else
                                {
                                    klprice[i] = new KLHKG_PriceBook
                                    {
                                        ID = maxid + (i + 1),
                                        KLHKG_PriceBookID=Guid.NewGuid().ToString(),
                                        UniqueCode=uniqueCode,
                                        CustType= worksheet.Cells[row, 1].Value != null ? worksheet.Cells[row, 1].Value.ToString().Trim() : "",
                                        SKU= worksheet.Cells[row, 2].Value.ToString().Trim(),
                                        Currency= worksheet.Cells[row, 3].Value.ToString().Trim(),
                                        Qty= Convert.ToDecimal(worksheet.Cells[row, 4].Value.ToString().Trim()),
                                        ListPrice=Convert.ToDecimal(worksheet.Cells[row, 5].Value.ToString().Trim()),
                                        MgrPrice=Convert.ToDecimal(worksheet.Cells[row, 6].Value.ToString().Trim()),
                                        SDPrice=Convert.ToDecimal(worksheet.Cells[row, 7].Value.ToString().Trim())
                                    };
                                    i++;
                                }
                            }
                            if (i > 0)
                            {
                                KLHKG_PriceBook[] klprice2 = new KLHKG_PriceBook[i];
                                for (int i1 = 0; i1 < i; i1++)
                                {
                                    klprice2[i1] = klprice[i1];
                                }
                                _context.KLHKG_PriceBook.AddRange(klprice2);

                            }
                            await _context.SaveChangesAsync();
                            //return Content(sb.ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        result = "Error";
                        info = ex.Message; break;

                        // Content(ex.Message);
                    }
                }
                if (string.IsNullOrEmpty(result))
                {
                    result = "Ok";
                    info = "Update Successful.";
                }
            }
            else
            {
                result = "Error";
                info = "Please choose upload file!";
            }
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("result", result);
            dic.Add("info", info);

            return Json(dic);
        }
        // GET: KLHKG_PriceBook/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kLHKG_PriceBook = await _context.KLHKG_PriceBook
                .SingleOrDefaultAsync(m => m.ID == id);
            if (kLHKG_PriceBook == null)
            {
                return NotFound();
            }

            return View(kLHKG_PriceBook);
        }

        // GET: KLHKG_PriceBook/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: KLHKG_PriceBook/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("KLHKG_PriceBookID,UniqueCode,CustType,SKU,Currency,Qty,ListPrice,MgrPrice,SDPrice,ID")] KLHKG_PriceBook kLHKG_PriceBook)
        {
            if (ModelState.IsValid)
            {
                _context.Add(kLHKG_PriceBook);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(kLHKG_PriceBook);
        }

        // GET: KLHKG_PriceBook/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kLHKG_PriceBook = await _context.KLHKG_PriceBook.SingleOrDefaultAsync(m => m.ID == id);
            if (kLHKG_PriceBook == null)
            {
                return NotFound();
            }
            return View(kLHKG_PriceBook);
        }

        // POST: KLHKG_PriceBook/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("KLHKG_PriceBookID,UniqueCode,CustType,SKU,Currency,Qty,ListPrice,MgrPrice,SDPrice,ID")] KLHKG_PriceBook kLHKG_PriceBook)
        {
            if (id != kLHKG_PriceBook.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(kLHKG_PriceBook);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KLHKG_PriceBookExists(kLHKG_PriceBook.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(kLHKG_PriceBook);
        }

        // GET: KLHKG_PriceBook/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kLHKG_PriceBook = await _context.KLHKG_PriceBook
                .SingleOrDefaultAsync(m => m.ID == id);
            if (kLHKG_PriceBook == null)
            {
                return NotFound();
            }

            return View(kLHKG_PriceBook);
        }

        // POST: KLHKG_PriceBook/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var kLHKG_PriceBook = await _context.KLHKG_PriceBook.SingleOrDefaultAsync(m => m.ID == id);
            _context.KLHKG_PriceBook.Remove(kLHKG_PriceBook);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool KLHKG_PriceBookExists(int id)
        {
            return _context.KLHKG_PriceBook.Any(e => e.ID == id);
        }
    }
}
