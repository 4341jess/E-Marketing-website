using EMarketing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Microsoft.Ajax.Utilities;
using PagedList;
using System.Web.UI;
using EMarketing.Models;


namespace Emarketing .Controllers
{
    public class AdminController : Controller
    {

        public ActionResult login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult login(tbl_admin avm)
        {
            dbemarketEntities2 db = new dbemarketEntities2();
            tbl_admin da = db.tbl_admin.Where(x => x.ad_username == avm.ad_username && x.ad_password == avm.ad_password).SingleOrDefault();
            if (da != null)
            {
                Session["ad_id"] = da.ad_id.ToString();
                return RedirectToAction("Create");
            }
            else
            {
                ViewBag.error = "invalid username password";
                return View();
            }




        }
        public ActionResult Create()
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("Create");
            }
            return View();
        }

        [HttpPost]
        public ActionResult Create(tb1_category cvm, HttpPostedFileBase imgfile)
        {
            dbemarketEntities2 db = new dbemarketEntities2();
            string path = UploadImgFile(imgfile);
            if (path.Equals("-1"))
            {
                ViewBag.Error = "Image colud not uploaded...";
            }
            else
            {
                tb1_category cat = new tb1_category();
                cat.cat_name = cvm.cat_name;
                cat.cat_image = path;
                cat.cat_satuts = 1;
                cat.cat = Convert.ToInt32(Session["ad_id"].ToString());
                db.tb1_category.Add(cat);
                db.SaveChanges();
                return RedirectToAction("ViewCategory");

            }


            return View();
        }
        public string UploadImgFile(HttpPostedFileBase file)
        {
            Random r = new Random();
            string path = "-1";
            int random = r.Next();

            if (file != null && file.ContentLength > 0)
            {
                string extension = Path.GetExtension(file.FileName).ToLower();

                if (extension == ".jpg" || extension == ".jpeg" || extension == ".png")
                {
                    try
                    {
                        string folderPath = Server.MapPath("~/Content/upload");
                        string fileName = random + Path.GetFileName(file.FileName);
                        path = Path.Combine(folderPath, fileName);


                        if (!Directory.Exists(folderPath))
                        {
                            Directory.CreateDirectory(folderPath);
                        }


                        file.SaveAs(path);

                        path = "~/Content/upload/" + fileName;
                    }
                    catch (Exception ex)
                    {

                        path = "-1";
                    }
                }
                else
                {
                    // Invalid file extension
                    return "Invalid file format. Only jpg, jpeg, and png files are allowed.";
                }
            }
            else
            {
                // No file selected
                return "No file selected.";
            }

            return path; // Return the final path or error state
        }
        public ActionResult ViewCategory(int? page)
        {
            dbemarketEntities2 db = new dbemarketEntities2();
            int pagesize = 9, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.tb1_category.Where(x => x.cat_satuts == 1).ToList();
            IPagedList<tb1_category> stu = list.ToPagedList(pageindex, pagesize);
            return View(stu);

            return View();
        }
    }
}