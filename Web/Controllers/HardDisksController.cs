using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Web.Data;
using Web.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using OfficeOpenXml;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Web.Controllers
{
    public class HardDisksController : Basic
    {
        private readonly ToolkitContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;

        private readonly ILogger<HardDisksController> _logger;
        public HardDisksController(ToolkitContext context, IHostingEnvironment hostingEnvironment, ILogger<HardDisksController> logger)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
        }

        // GET: HardDisks
        public async Task<IActionResult> Index(string menu)
        {
            ViewData["menu"] = menu;

            return View(await _context.HardDisk.ToListAsync());
        }
        [HttpPost("HardDisks")]
        public async Task<JsonResult> ImportExcel()
        {
            string result=string.Empty, info=string.Empty;

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
                    var filePath = _hostingEnvironment.WebRootPath + @"\assets\upload\harddisk\" + DateTime.Now.ToString("yyyyMMddhhmmssfff") + fileExtend;

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

                            HardDisk[] hardDisk = new HardDisk[rowCount - 1];

                            int i = 0;
                            for (int row = 2; row <= rowCount; row++)
                            {
                                HardDisk checkmodel = _context.HardDisk.Where(m => m.BarCode == worksheet.Cells[row, 1].Value.ToString().Trim()).SingleOrDefault();
                                if (checkmodel != null)
                                {

                                    checkmodel.CreateDate = DateTime.Now;
                                    checkmodel.BarCode = worksheet.Cells[row, 1].Value.ToString().Trim();
                                    checkmodel.Bank = worksheet.Cells[row, 2].Value.ToString().Trim();
                                    checkmodel.Tag = worksheet.Cells[row, 3].Value.ToString().Trim();
                                    checkmodel.Category = worksheet.Cells[row, 4].Value.ToString().Trim();
                                    checkmodel.Date = Convert.ToDateTime(worksheet.Cells[row, 5].Value.ToString().Trim());
                                    await _context.SaveChangesAsync();
                                }
                                else
                                {
                                    hardDisk[i] = new HardDisk
                                    {
                                        CreateDate = DateTime.Now,
                                        BarCode = worksheet.Cells[row, 1].Value.ToString().Trim(),
                                        Bank = worksheet.Cells[row, 2].Value.ToString().Trim(),
                                        Tag = worksheet.Cells[row, 3].Value.ToString().Trim(),
                                        Category = worksheet.Cells[row, 4].Value.ToString().Trim(),
                                        Date = Convert.ToDateTime(worksheet.Cells[row, 5].Value.ToString().Trim())
                                    };
                                    i++;
                                }
                            }
                            if (i > 0)
                            {
                                HardDisk[] hardDisk2 = new HardDisk[i];
                                for (int i1 = 0; i1 < i; i1++)
                                {
                                    hardDisk2[i1] = hardDisk[i1];
                                }
                                _context.HardDisk.AddRange(hardDisk2);

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

        // GET: HardDisks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hardDisk = await _context.HardDisk
                .SingleOrDefaultAsync(m => m.ID == id);
            if (hardDisk == null)
            {
                return NotFound();
            }

            return View(hardDisk);
        }

        // GET: HardDisks/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: HardDisks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,BarCode,Bank,Tag,CreateDate")] HardDisk hardDisk)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hardDisk);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(hardDisk);
        }

        // GET: HardDisks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hardDisk = await _context.HardDisk.SingleOrDefaultAsync(m => m.ID == id);
            if (hardDisk == null)
            {
                return NotFound();
            }
            return View(hardDisk);
        }

        // POST: HardDisks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,BarCode,Bank,Tag,CreateDate")] HardDisk hardDisk)
        {
            if (id != hardDisk.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hardDisk);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HardDiskExists(hardDisk.ID))
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
            return View(hardDisk);
        }

        // GET: HardDisks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hardDisk = await _context.HardDisk
                .SingleOrDefaultAsync(m => m.ID == id);
            if (hardDisk == null)
            {
                return NotFound();
            }

            return View(hardDisk);
        }

        // POST: HardDisks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hardDisk = await _context.HardDisk.SingleOrDefaultAsync(m => m.ID == id);
            _context.HardDisk.Remove(hardDisk);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<JsonResult> DeleteMulitple(string barcode)
        {
            if(!string.Equals(barcode,"all"))
            barcode = barcode.Substring(0, barcode.Length - 1);
            string result = string.Empty, info = string.Empty;
            try
            {
                string query = @"delete from [HardDisk]";
                if (!string.Equals(barcode, "all"))
                    query += " where barcode in (" + barcode + ")";

                int count = await _context.Database.ExecuteSqlCommandAsync(query);

                result = "Successful";
                info = "Delete Successful !";
            }
            catch (Exception e)
            {
                result = "Error";
                info = e.Message;
            }
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("result", result);
            dic.Add("info", info);

            return Json(dic);
        }
        private bool HardDiskExists(int id)
        {
            return _context.HardDisk.Any(e => e.ID == id);
        }
    }
}
