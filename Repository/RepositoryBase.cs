using GEPV.Domain.Entities;
using GEPV.Domain.Interfaces.Repository;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;

using System.Threading.Tasks;

namespace GEPV.Domain.Repository
{
    public class RepositoryBase<TEntity> : IRepositoryBase<TEntity> where TEntity : class
    {
        protected GEPVEntities db = new GEPVEntities();

        public virtual void Add(TEntity obj, bool? saveChanges = true)
        {
            db.Set<TEntity>().Add(obj);
            if (saveChanges == true)
                SaveChanges();
        }

        public virtual void Delete(TEntity obj, bool? saveChanges = true)
        {
            db.Entry(obj).State = EntityState.Deleted;

            if (saveChanges == true)
                SaveChanges();
        }

        public virtual void Delete(int id)
        {
            Delete(GetById(id));
        }

        public void Dispose()
        {
            db.Dispose();
        }

        public virtual bool Exists(int id)
        {
            return db.Set<TEntity>().Find(id) != null;
        }

        public virtual TEntity GetById(int id)
        {
            return db.Set<TEntity>().Find(id);
        }

        public virtual List<TEntity> List()
        {
            return db.Set<TEntity>().ToList();
        }

        public virtual void SaveChanges()
        {
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {


                Console.WriteLine("Property: {0} Error: {1}", ex.InnerException?.Source, ex.InnerException?.Message);


                throw ex;
            }
        }

        public virtual void Update(TEntity obj, bool? saveChanges = true)
        {
            db.Entry(obj).State = EntityState.Modified;
            if (saveChanges == true)
                SaveChanges();
        }
    }
}
