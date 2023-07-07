﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Abstract
{
    public interface IGenericDAL<TEntity> where TEntity : class, new()
    {
        void Add(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        TEntity GetOne(Expression<Func<TEntity, bool>> filter);
        IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> filter = null);
    }
}
