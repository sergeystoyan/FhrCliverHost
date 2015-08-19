﻿using System;
using System.Collections.Generic;
using System.Data;
//using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cliver.ProductOffice.Models;
using System.Configuration;
using System.Text.RegularExpressions;

namespace Cliver.ProductOffice.Controllers
{
    [Authorize]
    public class ProductIdentifierConfigurationController : Controller
    {
        private ProductOfficeEntities db = new ProductOfficeEntities();

        //List<object> CompanySelect
        //{
        //    get
        //    {
        //        var cs = (from r in db.Companies select new { Value = r.Id, Name = r.Name }).ToList<object>();
        //        cs.Insert(0, new { Value = -1, Name = "-- COMMON --" });
        //        return cs;
        //    }
        //}

        //public ActionResult Index()
        //{
        //    ViewBag.CompanyId = new SelectList(CompanySelect, "Value", "Name");
        //    return View();
        //}

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TableJson([ModelBinder(typeof(DataTables.AspNet.Mvc5.ModelBinder))] DataTables.AspNet.Core.IDataTablesRequest request)
        {
            JqueryDataTable.Field[] fields = new JqueryDataTable.Field[] { 
                new JqueryDataTable.Field("Id"),
                new JqueryDataTable.Field("Name", true),
            };
            JsonResult jr = JqueryDataTable.Index(request, db.Database.Connection, "FROM Companies", fields);
            List<object[]> cs = (List<object[]>)(((dynamic)jr.Data).Data);
            cs.Insert(0, new object[] { ProductIdentifier.Configuration.NO_COMPANY_DEPENDENT, "-- DEFAULT --" });
            return jr;
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            ViewBag.CompanyId = id;
            string synonyms;
            string word_weights;
            string ignored_words;
            ProductIdentifier.Engine e = new ProductIdentifier.Engine();
            e.Configuration.Get((int)id).GetConfigurationAsString(out word_weights, out ignored_words, out synonyms);
            ViewBag.WordWeights = word_weights;
            ViewBag.IgnoredWords = ignored_words;
            ViewBag.Synonyms = synonyms;
            if (Request.IsAjaxRequest())
                return PartialView();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int company_id, string synonyms, string word_weights, string ignored_words)
        {
            ProductIdentifier.Engine e = new ProductIdentifier.Engine();
            e.Configuration.Get(company_id).SetConfigurationFromString(word_weights, ignored_words, synonyms);
            e.Configuration.Get(company_id).Save();

            if (Request.IsAjaxRequest())
                return Content(null);
            return RedirectToAction("Index");
        }
    }
}