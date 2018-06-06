using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Web.Data;
using Web.Models;

namespace Web.Controllers
{
    public class EBooksController : Basic
    {
        private readonly KnowledgeBaseContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        public EBooksController(KnowledgeBaseContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: EBooks
        public async Task<IActionResult> Index(string menu, string p, string c)
        {
            if (object.Equals(p, null) && object.Equals(c, null))
                c = "1";
            // return View(await Paging(menu, string.Empty,"1"));
            paging.TotalPage = await _context.EBook.CountAsync();
            paging.CurrentPage = Convert.ToInt32(c);

            if (string.Equals(p, "next") && paging.CurrentPage + 1 <= paging.Pages)
            {
                paging.CurrentPage = paging.CurrentPage + 1;
            }
            else if (string.Equals(p, "prev") && paging.CurrentPage - 1 >= 1)
            {
                paging.CurrentPage = paging.CurrentPage - 1;
            }

            var result = await _context.EBook.Skip((paging.CurrentPage - 1) * paging.DisplayCount).Take(paging.DisplayCount).ToListAsync();

            ViewData["paging"] = paging;
            ViewData["menu"] = menu;
            return View(result);
        }
        public async Task<List<EBook>> Paging(string menu,string p,string c)
        {

            return null;
        }
        public async Task<JsonResult> UploadFiles()
        {
            string result = string.Empty, info = string.Empty;

            var files = Request.Form.Files;
            long filesize = files.Sum(f => f.Length);

            if (filesize > 1024 * 1024 * 6)
            {
                result = "Error";
                info = "File size must be less than 6 MB.";
            }
            else
            {
                if (files.Count > 0 && filesize > 0)
                {
                    foreach (var f in files)
                    {
                        string fileExtend = new FileInfo(f.FileName).Extension;
                        string filepureName = f.FileName.Substring(f.FileName.LastIndexOf("\\") + 1);
                        if (fileExtend == ".pdf")
                        {
                            var filePath = _hostingEnvironment.WebRootPath + @"\assets\upload\ebooks\" + filepureName;

                            var checkMod=_context.EBook.Where(m => m.BookName == filepureName).ToList();
                            if (checkMod != null && checkMod.Count > 0)
                            {
                                result = "Error";
                                info = "This book already exist.";
                            }
                            else
                            {
                                if (f.Length > 0)
                                {
                                    using (var stream = new FileStream(filePath, FileMode.Create))
                                    {
                                        await f.CopyToAsync(stream);
                                    }

                                    try
                                    {
                                        EBook eBook = new EBook()
                                        {
                                            Author = HttpContext.Request.Cookies["displayname"],
                                            BookName = filepureName,
                                            Classification = Request.Form["classification"].ToString(),
                                            Datetime = DateTime.Now,
                                            Path = @"\assets\upload\ebooks\" + filepureName
                                        };
                                        _context.EBook.Add(eBook);
                                        await _context.SaveChangesAsync();
                                    }
                                    catch (Exception e)
                                    {
                                        result = "Error"; info = e.Message;
                                    }
                                }
                            }
                        }
                        else
                        {
                            result = "Error"; info = "Just allow upload PDF file.";
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
            }
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("result", result);
            dic.Add("info", info);

            return Json(dic);
        }
        // GET: EBooks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eBook = await _context.EBook
                .SingleOrDefaultAsync(m => m.ID == id);
            if (eBook == null)
            {
                return NotFound();
            }

            return View(eBook);
        }

        // GET: EBooks/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EBooks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookName,Picture,Path,Datetime,ID,Author")] EBook eBook)
        {
            if (ModelState.IsValid)
            {
                _context.Add(eBook);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(eBook);
        }

        // GET: EBooks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eBook = await _context.EBook.SingleOrDefaultAsync(m => m.ID == id);
            if (eBook == null)
            {
                return NotFound();
            }
            return View(eBook);
        }

        // POST: EBooks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookName,Picture,Path,Datetime,ID,Author")] EBook eBook)
        {
            if (id != eBook.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(eBook);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EBookExists(eBook.ID))
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
            return View(eBook);
        }

        // GET: EBooks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eBook = await _context.EBook
                .SingleOrDefaultAsync(m => m.ID == id);
            if (eBook == null)
            {
                return NotFound();
            }

            return View(eBook);
        }

        // POST: EBooks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var eBook = await _context.EBook.SingleOrDefaultAsync(m => m.ID == id);
            _context.EBook.Remove(eBook);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EBookExists(int id)
        {
            return _context.EBook.Any(e => e.ID == id);
        }
    }
}
