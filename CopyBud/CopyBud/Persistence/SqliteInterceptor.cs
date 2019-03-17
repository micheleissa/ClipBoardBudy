using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;
using System.Text.RegularExpressions;

namespace CopyBud.Persistence
{
    public class SqliteInterceptor : IDbCommandInterceptor
    {
        private static readonly Regex ReplaceRegex = new Regex(@"\(CHARINDEX\((.*?),\s?(.*?)\)\)\s*?>\s*?0");

        public void NonQueryExecuted(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
        }

        public void NonQueryExecuting(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
        }

        public void ReaderExecuted(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
        }

        public void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            ReplaceCharIndexFunc(command);
        }

        public void ScalarExecuted(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
        }

        public void ScalarExecuting(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            ReplaceCharIndexFunc(command);
        }

        private void ReplaceCharIndexFunc(DbCommand command)
        {
            var isMatch = false;
            var text = ReplaceRegex.Replace(command.CommandText, (match) =>
            {
                if (match.Success)
                {
                    var paramsKey = match.Groups[1].Value;
                    var paramsColumnName = match.Groups[2].Value;
                    //replaceParams
                    foreach (DbParameter param in command.Parameters)
                    {
                        if (param.ParameterName == paramsKey.Substring(1))
                        {
                            param.Value = $"%{param.Value}%";
                            break;
                        }
                    }
                    isMatch = true;
                    return $"{paramsColumnName} LIKE {paramsKey}";
                }

                return match.Value;
            });
            if (isMatch)
                command.CommandText = text;
        }
    }
}
