using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence
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
            //_dataContext.Histories.RemoveRange(_dataContext.Histories.Select(h => h));  
            _dataContext.Database.ExecuteSqlCommand("DELETE FROM HISTORY");
            var historySeq = _dataContext.Sequences.FirstOrDefault(s => s.Name == nameof(History));
            if(historySeq != null)
            {
                historySeq.Seq = 0;
            }
            _dataContext.SaveChanges();
        }
    }
}
