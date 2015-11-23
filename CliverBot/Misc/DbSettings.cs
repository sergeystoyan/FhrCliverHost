//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//        26 September 2006
//Copyright: (C) 2006-2013, Sergey Stoyan
//********************************************************************************************
using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Web.Script.Serialization;

namespace Cliver.Bot
{
    public partial class DbSettings
    {
        public DbSettings(DbConnection dbc)
        {
            this.dbc = dbc;
            create_tables();
        }
        
        readonly DbConnection dbc;

        void create_tables()
        {
            lock (dbc)
            {
                dbc.Get(@"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Settings' and xtype='U') CREATE TABLE [dbo].[Settings] (
    [Scope] NVARCHAR (100) NOT NULL,
    [Key]   NVARCHAR (100) NOT NULL,
    [Value] NTEXT          NOT NULL,
    CONSTRAINT [PK_Settings] PRIMARY KEY CLUSTERED ([Scope] ASC, [Key] ASC)
);
"
                    ).Execute();
            }
        }

        public const string DEFAULT_KEY = "__DEFAULT__";

        public List<string> GetScopes(string scope_template)
        {
            Recordset scopes = dbc["SELECT Scope FROM Settings WHERE Scope LIKE @Scope"].GetRecordset("@Scope", scope_template);
            return scopes.Select(k => (string)k["Scope"]).ToList();
        }

        public List<string> GetKeys(string scope, string key_template)
        {
            Recordset keys = dbc["SELECT [Key] FROM Settings WHERE Scope=@Scope AND [Key] LIKE @Key"].GetRecordset("@Scope", scope, "@Key", key_template);
            return keys.Select(k => (string)k["Key"]).ToList();
        }

        public T Get<T>(string scope)
        {
            return Get<T>(scope, DEFAULT_KEY);
        }

        public T Get<T>(string scope, string key)
        {
            string json = (string)dbc["SELECT Value FROM Settings WHERE Scope=@Scope AND [Key]=@Key"].GetSingleValue("@Scope", scope, "@Key", key);
            if (json == null)
                return default(T);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Deserialize<T>(json);
        }

        //public T Get<T>(string scope, string key, string json_path)
        //{
        //    dynamic o = Get(scope, key);
        //    foreach (string p in json_path.Split('\\', '/'))
        //        o = o[p];
        //    return (T)o;
        //}

        public void Save(string scope, Dictionary<string, object> value)
        {
            Save(scope, DEFAULT_KEY, value);
        }

        public void Save(string scope, string key, object value)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string json = serializer.Serialize(value);
            if (dbc.Get("UPDATE Settings SET Value=@Value WHERE Scope=@Scope AND [Key]=@Key").Execute("@Value", json, "@Scope", scope, "@Key", key) < 1)
                dbc.Get("INSERT INTO Settings (Value,Scope,[Key]) VALUES(@Value,@Scope,@Key)").Execute("@Value", json, "@Scope", scope, "@Key", key);
        }

        //public void Save(string scope, string key, string json_path, object value)
        //{
        //    dynamic o = Get(scope, key);
        //    dynamic po = o;
        //    string[] ps = json_path.Split('\\', '/');
        //    for (int i = 0; i < ps.Length - 1; i++)
        //        po = po[ps[i]];
        //    po[ps[ps.Length - 1]] = value;
        //    Save(scope, key, po);
        //}

        public int Delete(string scope, string key_template)
        {
            return dbc.Get("DELETE FROM Settings WHERE Scope=@Scope AND [Key] LIKE @Key").Execute("@Scope", scope, "@Key", key_template);
        }
    }
}