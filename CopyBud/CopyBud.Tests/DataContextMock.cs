using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Moq;

namespace CopyBud.Tests
{
    public class DataContextMock<T> : Mock<T> where T : DbContext
        {
        public DataContextMock<T> WithDbSetFrom<TResult>(Expression<Func<T, DbSet<TResult>>> dbSet, IList<TResult> collection) where TResult : class
            {
            var dbSetMock = new Mock<DbSet<TResult>>();
            dbSetMock.As<IQueryable<TResult>>().Setup(m => m.Provider).Returns(collection.AsQueryable().Provider);
            dbSetMock.As<IQueryable<TResult>>().Setup(m => m.Expression).Returns(collection.AsQueryable().Expression);
            dbSetMock.As<IQueryable<TResult>>().Setup(m => m.ElementType).Returns(collection.AsQueryable().ElementType);
            dbSetMock.As<IQueryable<TResult>>().Setup(m => m.GetEnumerator()).Returns(collection.GetEnumerator());
            dbSetMock.Setup(m => m.Add(It.IsAny<TResult>())).Callback<TResult>(collection.Add).Returns<TResult>(val => val);
            dbSetMock.Setup(m => m.AddRange(It.IsAny<IEnumerable<TResult>>())).Callback<IEnumerable<TResult>>(c => collection.AddRange(c)).Returns<IEnumerable<TResult>>(val => val);
            dbSetMock.Setup(m => m.RemoveRange(It.IsAny<IEnumerable<TResult>>())).Callback<IEnumerable<TResult>>(cs =>
                {
                foreach (var c in cs.ToList()) collection.Remove(c);
                }).Returns<IEnumerable<TResult>>(val => val);

            dbSetMock.Setup(m => m.Remove(It.IsAny<TResult>())).Callback<TResult>(x => collection.Remove(x));
            dbSetMock.Setup(m => m.AsNoTracking()).Returns(() => dbSetMock.Object);
            dbSetMock.Setup(m => m.Include(It.IsAny<string>())).Returns(() => dbSetMock.Object);
            dbSetMock.Setup(m => m.Attach(It.IsAny<TResult>())).Returns<TResult>((val) => val);
            dbSetMock.As<ICollection<TResult>>().Setup(m => m.Count).Returns(() => collection.Count());
            dbSetMock.As<ICollection<TResult>>()
                .Setup(m => m.CopyTo(It.IsAny<TResult[]>(), It.IsAny<int>()))
                .Callback<TResult[], int>(collection.CopyTo);
            Setup(dbSet).Returns(dbSetMock.Object);
            return this;

            }

        public Mock<T> WithDbQueryFrom<TResult>(Expression<Func<T, DbQuery<TResult>>> dbQuery, IList<TResult> collection) where TResult : class
            {
            var dbQueryMock = new Mock<DbSet<TResult>>();
            dbQueryMock.As<IQueryable<TResult>>().Setup(m => m.Provider).Returns(collection.AsQueryable().Provider);
            dbQueryMock.As<IQueryable<TResult>>().Setup(m => m.Expression).Returns(collection.AsQueryable().Expression);
            dbQueryMock.As<IQueryable<TResult>>().Setup(m => m.ElementType).Returns(collection.AsQueryable().ElementType);
            dbQueryMock.As<IQueryable<TResult>>().Setup(m => m.GetEnumerator()).Returns(collection.GetEnumerator());

            Setup(dbQuery).Returns(dbQueryMock.Object);
            return this;

            }

        public Mock<ICollection<ET>> RelationMock<ET>(DbSet<ET> dbset, Expression<Func<ET, bool>> sel) where ET : class
            {
            Func<IQueryable<ET>> q = () => dbset.Where(sel);
            var cm = new Mock<ICollection<ET>>();
            cm.As<ICollection<ET>>().Setup(m => m.Count).Returns(() => dbset.Where(sel).Count());
            cm.As<ICollection<ET>>().Setup(m => m.CopyTo(It.IsAny<ET[]>(), It.IsAny<int>()))
                .Callback<ET[], int>((a, i) => {
                    q().ToList().CopyTo(a, i);
                    });
            cm.As<ICollection<ET>>().Setup(m => m.Add(It.IsAny<ET>())).Callback<ET>(v => dbset.Add(v));
            cm.As<IEnumerable<ET>>().Setup(m => m.GetEnumerator()).Returns(() => q().GetEnumerator());
            cm.As<IQueryable<ET>>().Setup(m => m.Provider).Returns(() => q().Provider);
            cm.As<IQueryable<ET>>().Setup(m => m.Expression).Returns(() => q().Expression);
            cm.As<IQueryable<ET>>().Setup(m => m.ElementType).Returns(() => q().ElementType);
            cm.As<IQueryable<ET>>().Setup(m => m.GetEnumerator()).Returns(() => q().GetEnumerator());
            return cm;
            }

        }
}
