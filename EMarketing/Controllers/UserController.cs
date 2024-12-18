using EMarketing.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace EMarketing.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index(int? page)
        {
            dbemarketEntities2 db = new dbemarketEntities2();
            int pagesize = 9, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.tb1_category.Where(x => x.cat_satuts == 1).OrderByDescending(x => x.cat_id).ToList();
            IPagedList<tb1_category> stu = list.ToPagedList(pageindex, pagesize);
            return View(stu);
        }
        public ActionResult Signup()
        {
            return View();
        }
        [HttpPost]

        public ActionResult Signup(tbl_user avm, HttpPostedFileBase imgfile)
        {


            dbemarketEntities2 db = new dbemarketEntities2();


            string path = UploadImgFile(imgfile);
            if (path.Equals("-1"))
            {
                ViewBag.error = "Image could not be uploaded. Please try again.";
                return View(avm);
            }


            tbl_user u = new tbl_user
            {
                u_name = avm.u_name,
                u_email = avm.u_email,
                u_password = avm.u_password,
                u_image = path,
                u_contact = avm.u_contact
            };


            db.tbl_user.Add(u);
            db.SaveChanges();


            return RedirectToAction("Login");
        }


        public ActionResult login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult login(tbl_user avm)
        {
            dbemarketEntities2 db = new dbemarketEntities2();
            tbl_user da = db.tbl_user.Where(x => x.u_email == avm.u_email && x.u_password == avm.u_password).SingleOrDefault();
            if (da != null)
            {
                Session["u_id"] = da.u_id.ToString();
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.error = "invalid username password";
                return View();
            }





        }

        public string UploadImgFile(HttpPostedFileBase file)
        {
            Random r = new Random();
            string path = "-1"; // Default error state
            int random = r.Next();

            if (file != null && file.ContentLength > 0)
            {
                string extension = Path.GetExtension(file.FileName).ToLower();

                if (extension == ".jpg" || extension == ".jpeg" || extension == ".png")
                {
                    try
                    {
                        // Generate file path
                        string folderPath = Server.MapPath("~/Content/upload");
                        string fileName = random + Path.GetFileName(file.FileName);
                        path = Path.Combine(folderPath, fileName);

                        // Save the file
                        file.SaveAs(path);

                        // Return virtual path for accessing the image in views
                        path = "~/Content/upload/" + fileName;
                    }
                    catch (Exception ex)
                    {
                        // Log the exception if necessary (could be done via a logging library)
                        path = "-1";
                        ViewBag.error = "An error occurred while uploading the file.";
                    }
                }
                else
                {
                    ViewBag.error = "Only jpg, jpeg, and png files are allowed.";
                }
            }
            else
            {
                ViewBag.error = "Please select a file to upload.";
            }

            return path;
        }
        public ActionResult CreateAd()
        {
            dbemarketEntities2 db = new dbemarketEntities2();
            List<tb1_category> li = db.tb1_category.ToList();
            ViewBag.categorylist = new SelectList(li, "cat_id", "cat_name");
            return View();
        }
        [HttpPost]
        public ActionResult CreateAd(tbl_product pvm, HttpPostedFileBase imgfile)
        {
            if (Session["u_id"] == null)
            {

                return RedirectToAction("Login", "User");
            }

            using (dbemarketEntities2 db = new dbemarketEntities2())
            {

                string path = UploadImgFile(imgfile);
                if (path.Equals("-1"))
                {

                    ViewBag.error = "Image could not be uploaded. Please try again.";
                    return View(pvm);
                }


                tbl_product p = new tbl_product
                {
                    pro_name = pvm.pro_name,
                    pro_price = pvm.pro_price,
                    pro_image = path,
                    pro = pvm.pro,
                    pro_des = pvm.pro_des,
                    prod = Convert.ToInt32(Session["u_id"])
                };

                db.tbl_product.Add(p);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
        }
        public ActionResult Add(int? id, int? page)
        {
            dbemarketEntities2 db = new dbemarketEntities2 ();
            int pagesize = 9, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.tbl_product.Where(x => x.pro == id).OrderByDescending(x => x.pro_id).ToList();
            IPagedList<tbl_product> stu = list.ToPagedList(pageindex, pagesize);

            return View(stu);




        }
        public ActionResult ViewAdd(int? id)
        {
            dbemarketEntities2 db = new dbemarketEntities2();
            ViewAdd add = new ViewAdd();
        
            tbl_product p = db.tbl_product.Where(x => x.pro_id == id).SingleOrDefault();
            add.pro_id = p.pro_id;
            add.pro_name = p.pro_name;
            add.pro_image = p.pro_image;
            add.pro_price = p.pro_price;
            tb1_category cat = db.tb1_category.Where(x => x.cat_id == p.pro).SingleOrDefault();
            add.cat_name = cat.cat_name;
            tbl_user u = db.tbl_user.Where(x => x.u_id == p.prod).SingleOrDefault();
            add.u_name = u.u_name;
            add.u_image = u.u_image;
            add.u_contact = u.u_contact;
            add.prod = u.u_id;



            return View(add);
        }
        public ActionResult LogOut()
        {
            Session.RemoveAll();
            Session.Abandon();
            return RedirectToAction("Index");

        }
        public ActionResult Delete(int? id)
        {
            dbemarketEntities2 db = new dbemarketEntities2();
            tbl_product p = db.tbl_product.Where(x => x.pro_id == id).SingleOrDefault();
            db.tbl_product.Remove(p);
            db.SaveChanges();
            return RedirectToAction("Index");



        }
        [HttpPost]


        public ActionResult Add(int? id, int? page, string search)
        {

            dbemarketEntities2 db = new dbemarketEntities2();

            int pagesize = 9;
            int pageindex = page.HasValue ? page.Value : 1;


            var list = db.tbl_product
                         .Where(x => string.IsNullOrEmpty(search) || x.pro_name.Contains(search))
                         .OrderByDescending(x => x.pro_id);


            IPagedList<tbl_product> pagedList = list.ToPagedList(pageindex, pagesize);

            return View(pagedList);
        }

    }






}