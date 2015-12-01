﻿using System;
using System.Collections.Generic;
using System.Data;
//using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cliver.Fhr.ProductOffice.Models;
using System.Configuration;
using System.Text.RegularExpressions;

namespace Cliver.ProductOffice.Controllers
{
    [Authorize]
    public class ProductIdentifierConfigurationController : Controller
    {
        private DbApi db = Fhr.ProductOffice.Models.DbApi.Create();
        
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
            Bot.DbSettings ss = new Bot.DbSettings(Cliver.Bot.DbConnection.CreateFromNativeConnection(db.Database.Connection));
            DateTime t = ss.Get<DateTime>(Cliver.ProductIdentifier.SettingsKey.SCOPE, Cliver.ProductIdentifier.SettingsKey.TRAINING_TIME);
            if (t != default(DateTime))
                ViewBag.SelfTrainingDate = t.ToShortDateString();
            else
                ViewBag.SelfTrainingDate = "no training was done yet.";
            ViewBag.Companies = db.Companies;
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
            e.Configuration.Get(company_id).SetWordWeightsFromString(word_weights, ignored_words);
            e.Configuration.Get(company_id).SaveWordWeights();
            e.Configuration.Get(company_id).SetSynonymsFromString(synonyms);
            e.Configuration.Get(company_id).SaveSynonyms();

            if (Request.IsAjaxRequest())
                return Content(null);
            return RedirectToAction("Index");
        }

        public ActionResult TableJson2([ModelBinder(typeof(DataTables.AspNet.Mvc5.ModelBinder))] DataTables.AspNet.Core.IDataTablesRequest request)
        {
            Bot.DbConnection dbc = Cliver.Bot.DbConnection.CreateFromNativeConnection(db.Database.Connection);

            //dbc.Get("DROP TABLE [#ProductIdentifierConfig2]").Execute();
            string fs = "";
            if(db.Companies.Count() > 0)
                fs = ", [" + string.Join("] CHAR(2) NOT NULL DEFAULT '', [", db.Companies.Select(c => c.Name)) + "] CHAR(2) NOT NULL DEFAULT '' ";
            string sql = @"IF OBJECT_ID('tempdb..[#ProductIdentifierConfig2]') IS NOT NULL 
DROP TABLE [#ProductIdentifierConfig2]
ELSE
CREATE TABLE [#ProductIdentifierConfig2] ([C1IdC2Id] NVARCHAR (100) NOT NULL" + fs + ")";
            dbc.Get(sql).Execute();

            Bot.DbSettings ss = new Bot.DbSettings(dbc);
            List<string> keys = ss.GetKeys(Cliver.ProductIdentifier.SettingsKey.SCOPE, Cliver.ProductIdentifier.SettingsKey.CATEGORY_MAP + "%");
            foreach(string k in keys)
            {
                Match m = Regex.Match(k, @"(\d+),(\d+)$", RegexOptions.Singleline);
                int c1_i = int.Parse(m.Groups[1].Value);
                int c2_i = int.Parse(m.Groups[2].Value);
                string c1_n = db.Companies.Where(c => c.Id == c1_i).Select(c => c.Name).FirstOrDefault();
                if (c1_n == null)
                    continue;
                string c2_n = db.Companies.Where(c => c.Id == c2_i).Select(c => c.Name).FirstOrDefault();
                if (c2_n == null)
                    continue;
                dbc.Get(@"INSERT INTO [#ProductIdentifierConfig2] (C1IdC2Id,[" + c1_n + "],[" + c2_n + "]) VALUES('" + m.Groups[0].Value + "','+','+')").Execute();
            }

            List<JqueryDataTable.Field> fields = new List<JqueryDataTable.Field> { new JqueryDataTable.Field("C1IdC2Id", false) };
            fields.AddRange(from c in db.Companies.ToList() select new JqueryDataTable.Field("[" + c.Name + "]", true, 0, null));
            return JqueryDataTable.Index(request, db.Database.Connection, "FROM #ProductIdentifierConfig2", fields.ToArray());
        }

        public ActionResult Edit2([Bind(Prefix = "Id")]string c1_i_c2_i)
        {
            ViewBag.Company1IdCompany2Id = c1_i_c2_i;

            Match m = Regex.Match(c1_i_c2_i, @"(\d+),(\d+)$", RegexOptions.Singleline);
            int c1_i = int.Parse(m.Groups[1].Value);
            int c2_i = int.Parse(m.Groups[2].Value);
            ViewBag.Company1Name = db.Companies.Where(c => c.Id == c1_i).Select(c => c.Name).FirstOrDefault();
            ViewBag.Company2Name = db.Companies.Where(c => c.Id == c2_i).Select(c => c.Name).FirstOrDefault();

            string mapped_categories;
            ProductIdentifier.Engine e = new ProductIdentifier.Engine();
            e.Configuration.GetMappedCategoriesAsString(c1_i, c2_i, out mapped_categories);
            ViewBag.MappedCategories = mapped_categories;

            if (Request.IsAjaxRequest())
                return PartialView();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit2(string c1_i_c2_i, string mapped_categories)
        {
            Match m = Regex.Match(c1_i_c2_i, @"(\d+),(\d+)$", RegexOptions.Singleline);
            int c1_i = int.Parse(m.Groups[1].Value);
            int c2_i = int.Parse(m.Groups[2].Value);

            ProductIdentifier.Engine e = new ProductIdentifier.Engine();
            e.Configuration.SaveMappedCategoriesFromString(c1_i, c2_i, mapped_categories);

            if (Request.IsAjaxRequest())
                return Content(null);
            return RedirectToAction("Index");
        }

        [ValidateAntiForgeryToken]
        public ActionResult PerformSelfTraining()
        {
            ProductIdentifier.Engine e = new ProductIdentifier.Engine();
            e.PerformSelfTrainingAnalysisOfLinks();

            if (Request.IsAjaxRequest())
                return Content("Done!");
            return RedirectToAction("Index");
        }
    }
}