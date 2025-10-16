using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orama.Interfaces
{
    public interface INavigationService
    {
        
        Task NavigateToAsync(string route);  // Navigate to a route/page by route name
        Task NavigateToAsync(string route, IDictionary<string, object> parameters);   // Navigate to a route/page with parameters
        Task GoBackAsync();   // Navigate back
    }
}
