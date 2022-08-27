using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Task.Models;
using Task.Report;
using Task.Report.CustomerDataSetTableAdapters;
using Task.ViewModels;

namespace Task.Controllers
{
    public class CustomerController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: Customer
        public ActionResult Index()
        {
            return View(db.Customers.ToList());
        }

        // GET: Customer/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }

            var result = db.Products.Select(x => new
            {
                x.Id,
                x.Name,
                isChecked = db.CustomersProducts.Where(cp =>
                cp.CustomerId == id & cp.ProductId == x.Id).Count() > 0
            });
            var customerViewModel = new CustomerViewModel();
            customerViewModel.Id = id.Value;
            customerViewModel.Name = customer.Name;

            var customerCheckBoxList = new List<CheckBoxViewModel>();
            foreach (var item in result)
            {
                customerCheckBoxList.Add(new CheckBoxViewModel { Id = item.Id, Name = item.Name, isChecked = item.isChecked });
            }
            customerViewModel.Products = customerCheckBoxList;
            return View(customerViewModel);
        }

        // GET: Customer/Create
        public ActionResult Create()
        {
            var result = db.Products.ToList();
            var customerViewModel = new CustomerViewModel();
            customerViewModel.Id = 0;
            customerViewModel.Name ="";

            var customerCheckBoxList = new List<CheckBoxViewModel>();
            foreach (var item in result)
            {
                customerCheckBoxList.Add(new CheckBoxViewModel { Id = item.Id, Name = item.Name, isChecked = false });
            }
            customerViewModel.Products = customerCheckBoxList;
            return View(customerViewModel);
            //return View();
        }

        // POST: Customer/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CustomerViewModel customerViewModel)
        {
            if (ModelState.IsValid)
            {
                var customer = new Customer { Name = customerViewModel.Name };
                var result=db.Customers.Add(customer);
                db.SaveChanges();
                foreach (var item in customerViewModel.Products)
                {
                    if (item.isChecked)
                    {
                        db.CustomersProducts.Add(new CustomerProduct { CustomerId = result.Id, ProductId = item.Id });
                    }
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(customerViewModel);
        }

        // GET: Customer/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            //var result=from p in db.Products
            //           select new()

            var result = db.Products.Select(x => new
            {
                x.Id,
                x.Name,
                isChecked = db.CustomersProducts.Where(cp =>
                cp.CustomerId == id & cp.ProductId == x.Id).Count() > 0
            });
            var customerViewModel = new CustomerViewModel();
            customerViewModel.Id = id.Value;
            customerViewModel.Name = customer.Name;

            var customerCheckBoxList = new List<CheckBoxViewModel>();
            foreach (var item in result)
            {
                customerCheckBoxList.Add(new CheckBoxViewModel { Id=item.Id ,Name=item.Name,isChecked=item.isChecked });
            }
            customerViewModel.Products=customerCheckBoxList;
            return View(customerViewModel);
        }

        // POST: Customer/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CustomerViewModel customerViewModel)
        {
            if (ModelState.IsValid)
            {
                var customer = db.Customers.Find(customerViewModel.Id);
                customer.Name = customerViewModel.Name;
                foreach (var item in db.CustomersProducts)
                {
                    if (item.CustomerId==customerViewModel.Id)
                    {
                        db.Entry(item).State = EntityState.Deleted;

                    }
                }
                foreach (var item in customerViewModel.Products)
                {
                    if (item.isChecked)
                    {
                        db.CustomersProducts.Add(new CustomerProduct { CustomerId = customerViewModel.Id, ProductId = item.Id });
                    }
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(customerViewModel);
        }

        // GET: Customer/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Customer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Customer customer = db.Customers.Find(id);
            db.Customers.Remove(customer);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        #region CustomerReport

        public ActionResult CustomerReport(int customerId)
        {
            dynamic Report = new CustomerReport();
            CustomerDataSet DataSet = new CustomerDataSet();
            SP_CustomerProducts_ReportTableAdapter TableAdapter = new SP_CustomerProducts_ReportTableAdapter();

            DataSet.EnforceConstraints = false;//من اجل عدم ظهور خطاء اذا لم يوجد داتا

            TableAdapter.Fill(DataSet.SP_CustomerProducts_Report, customerId);
            if (DataSet.SP_CustomerProducts_Report == null ||
                DataSet.SP_CustomerProducts_Report.Count == 0)
                Report.DataSource = new CustomerDataSet.SP_CustomerProducts_ReportDataTable();
            else
                Report.DataSource = DataSet.SP_CustomerProducts_Report;

            Report.DataMember = DataSet.SP_CustomerProducts_Report.TableName;

            Report.Parameters["customerId"].Value = customerId;
            ReportViewerExtension.ExportTo(Report, Request);
            return View(Report);
        }


        #endregion
    }
}
