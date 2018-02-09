using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Razor;

namespace BuildingBlocks.Mvc
{
    public class FeaturesLocationExpander : IViewLocationExpander

    {
        public void PopulateValues(ViewLocationExpanderContext context)
        {
            // nothing
        }

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            return new[]
            {
                "/Features/{1}/{0}.cshtml",   // feature specific content
                "/Features/Shared/{0}.cshtml" // shared
            };
        }
    }
}
