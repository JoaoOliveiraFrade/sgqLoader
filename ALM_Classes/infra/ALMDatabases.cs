using System;
using System.Data;
using Oracle.DataAccess.Client;
using System.Configuration;
using System.Collections.Generic;
using System.Reflection;
using sgq;

namespace sgq.alm {
    public static class Databases {
        public static alm.Database ALM11 = new alm.Database() {
            name = "ALM11",
            prefix = "QC11PRD1.",
            dominio = "PRJ",
            sqlListProjects = "projectsALM11.sql",
            connectionString = ConfigurationManager.ConnectionStrings["ALM11"].ConnectionString,
            scheme = ConfigurationManager.AppSettings["ALM11_Scheme"]
        };

        public static alm.Database ALM12 = new alm.Database() {
            name = "ALM12",
            prefix = "",
            dominio = "QUALIDADE_TI",
            sqlListProjects = "projectsALM12.sql",
            connectionString = ConfigurationManager.ConnectionStrings["ALM12"].ConnectionString,
            scheme = ConfigurationManager.AppSettings["ALM12_Scheme"]
        };

        public static List<alm.Database> List = new List<alm.Database>() {
            ALM11,
            ALM12
        };
    }
}

