﻿using Dapper;
using ECP_V2.Business.UnitOfWork;
using ECP_V2.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECP_V2.Business.Repository
{
    public class pccc_TrangThaiRepository : RepositoryBase<pccc_TrangThai>
    {
        public string Connectstr { get; set; }
        static string DBName { get; set; }
        public pccc_TrangThaiRepository(string connect)
            : base()
        {
            this.Connectstr = connect;
            try
            {
                var connection = new SqlConnection(Context.Database.Connection.ConnectionString);
                DBName = connection.Database;
            }
            catch (Exception ex)
            { }
        }

        public pccc_TrangThaiRepository()
            : base()
        {
        }

        public pccc_TrangThaiRepository(WorkUnit unit)
            : base(unit)
        {
        }

        public override string Delete(object entityId, ref string strError)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                var entity = Context.pccc_TrangThai.SingleOrDefault(o => o.ID == id);
                Context.pccc_TrangThai.Remove(entity);
                Context.SaveChanges();
                strError = "";
                return "success";
            }
            catch (Exception ex) { strError = ex.Message; return "error"; }
        }

        public override object Create(pccc_TrangThai entity, ref string strError)
        {
            try
            {
                Context.Entry(entity).State = EntityState.Added;
                Context.SaveChanges();
                return entity.ID;
            }
            catch (Exception ex)
            {
                strError = ex.Message;
                return 0;
            }
        }

        public override pccc_TrangThai GetById(object entityId)
        {
            try
            {
                var id = int.Parse(entityId.ToString());
                return Context.pccc_TrangThai.SingleOrDefault(p => p.ID == id);
            }
            catch { return null; }
        }

        public override List<pccc_TrangThai> List()
        {
            try
            {
                return Context.pccc_TrangThai.ToList();
            }
            catch (Exception ex) { return null; }
        }

        public override List<pccc_TrangThai> ListByQuery(string strQuery)
        {
            throw new NotImplementedException();
        }

        public override object Update(pccc_TrangThai entity, ref string strError)
        {
            throw new NotImplementedException();
        }

        public override List<pccc_TrangThai> GetTableListById(object entityId)
        {
            throw new NotImplementedException();
        }

        public int GetIdByName(string Name)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(Connectstr))
                {
                    string query = "select ID from pccc_TrangThai where UPPER(Name) like N'%" + Name.ToUpper() + "%'";
                    var q = db.Query<int>(query);
                    return q.FirstOrDefault();
                }
            }
            catch (Exception ex) { return new int(); }
        }

    }
}
