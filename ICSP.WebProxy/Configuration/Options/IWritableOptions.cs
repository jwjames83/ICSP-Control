using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;

namespace ICSP.WebProxy.Configuration.Options
{
  public interface IWritableOptions<out T> : IOptionsSnapshot<T> where T : class, new()
  {
    void Update(Action<T> applyChanges);

    Task UpdateAsync(int millisecondsDelay, Action<T> applyChanges);
  }
}
