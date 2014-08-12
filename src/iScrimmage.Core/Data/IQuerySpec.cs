using System.Data;

namespace iScrimmage.Core.Data
{
    public interface IQuerySpec<out T> where T : class
    {
        T Execute(IDataContext context);
    }
}
