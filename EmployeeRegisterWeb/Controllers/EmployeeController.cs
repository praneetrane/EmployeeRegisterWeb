using EmployeeRegisterWeb.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmployeeRegisterWeb.Controllers
{
    public class EmployeeController : Controller
    {
        [HttpGet]
        // GET: Employee
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult ViewAll()
        {
            return View(GetAllEmployees());
        }

        IEnumerable<Employee> GetAllEmployees()
        {
            using (EmployeeRegisterEntities db = new EmployeeRegisterEntities())
            {
                return db.Employees.ToList<Employee>();
            }
        } 

        [HttpGet]
        public ActionResult AddorEdit(int id=0)
        {
            Employee emp = new Employee();

            if (id != 0)
            {
                using (EmployeeRegisterEntities db = new EmployeeRegisterEntities())
                {
                    emp = db.Employees.Where(x => x.EmployeeID == id).FirstOrDefault<Employee>();
                }
            }
            return View(emp);
        }
        [HttpPost]
        public ActionResult AddOrEdit(Employee emp)
        {
            try
            {
                if (emp.ImageUpload != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(emp.ImageUpload.FileName);
                    string extension = Path.GetExtension(emp.ImageUpload.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    emp.ImagePath = "~/AppFiles/Images/" + fileName;
                    emp.ImageUpload.SaveAs(Path.Combine(Server.MapPath("~/AppFiles/Images/"), fileName));
                }
                using (EmployeeRegisterEntities db = new EmployeeRegisterEntities())
                {
                    if (emp.EmployeeID == 0)
                    {
                        db.Employees.Add(emp);
                        db.SaveChanges();
                    }
                    else
                    {
                        db.Entry(emp).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                return Json(new { success = true, html = GlobalClass.RenderRazorViewToString(this, "ViewAll", GetAllEmployees()), message = "Submitted Successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Delete(int id)
        {
            try
            {
                using (EmployeeRegisterEntities db = new EmployeeRegisterEntities())
                {
                    Employee emp = db.Employees.Where(x => x.EmployeeID == id).FirstOrDefault<Employee>();
                    db.Employees.Remove(emp);
                    db.SaveChanges();
                }
                return Json(new { success = true, html = GlobalClass.RenderRazorViewToString(this, "ViewAll", GetAllEmployees()), message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}