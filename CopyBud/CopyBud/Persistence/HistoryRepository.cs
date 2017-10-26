using System;
using System.Linq;

namespace CopyBud.Persistence
{
    public class HistoryRepository
    {
        private readonly DataContext _dataContext;
        public HistoryRepository(DataContext ctx)
        {
            _dataContext = ctx;
        }

        public void AddHistory(string historyStr)
        {
            var history = new History { ClipString = historyStr,DateTimeTaken = DateTime.Now };
            _dataContext.Histories.Add(history);
            _dataContext.SaveChanges();
        }

        public IQueryable<History> GetRecentHistory()
        {
            return _dataContext.Histories.OrderBy(h => h.Id).Take(5);
        }

        public bool DoesHistoryExist(string historyStr)
        {
            return _dataContext.Histories.Any(h => Equals(h.ClipString,historyStr));
        }

        public void DeleteAll()
        {
            _dataContext.Database.ExecuteSqlCommand("DELETE FROM HISTORY");
            _dataContext.SaveChanges();
        }
    }
}
