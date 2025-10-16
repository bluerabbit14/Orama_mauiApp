using Orama.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orama.Services
{
    public class NavigationService: INavigationService
    {
        public async Task NavigateToAsync(string route)
        {
            await Shell.Current.GoToAsync(route);
        }

        public async Task NavigateToAsync(string route, IDictionary<string, object> parameters)
        {
            // Convert parameters to query string
            var query = string.Join("&", parameters.Select(kvp => $"{kvp.Key}={kvp.Value}"));
            await Shell.Current.GoToAsync($"{route}?{query}");
        }

        public async Task GoBackAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
