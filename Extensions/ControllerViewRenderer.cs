using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CuzdanUygulamasi.Extensions
{
    public static class ControllerViewRenderer
    {
        public static async Task<string> RenderViewAsync<TModel>(
            this Controller controller,
            string viewName,
            TModel model,
            bool isPartial = false)
        {
            if (string.IsNullOrWhiteSpace(viewName))
                viewName = controller.ControllerContext.ActionDescriptor.ActionName;

            var httpContext = controller.HttpContext;
            var serviceProvider = httpContext.RequestServices;

            var viewEngine = serviceProvider.GetRequiredService<IRazorViewEngine>();
            var tempDataProvider = serviceProvider.GetRequiredService<ITempDataProvider>();

            // ActionContext hazırla
            var actionContext = new ActionContext(
                httpContext,
                controller.RouteData,
                new ActionDescriptor()
            );

            // ViewData/TempData hazırla
            var viewData = new ViewDataDictionary(
                metadataProvider: new EmptyModelMetadataProvider(),
                modelState: controller.ModelState)
            {
                Model = model
            };
            var tempData = new TempDataDictionary(httpContext, tempDataProvider);

            // View bul (hem FindView hem GetView dene)
            var findViewResult = viewEngine.FindView(actionContext, viewName, !isPartial);
            IView view;

            if (findViewResult.Success)
            {
                view = findViewResult.View;
            }
            else
            {
                var getViewResult = viewEngine.GetView(executingFilePath: null, viewPath: viewName, isMainPage: !isPartial);
                if (getViewResult.Success)
                {
                    view = getViewResult.View;
                }
                else
                {
                    var searched = findViewResult.SearchedLocations
                                  .Concat(getViewResult.SearchedLocations ?? Array.Empty<string>());
                    var msg = $"View '{viewName}' bulunamadı.\nAranan konumlar:\n" + string.Join("\n", searched);
                    throw new FileNotFoundException(msg);
                }
            }

            await using var sw = new StringWriter();
            var viewContext = new ViewContext(
                actionContext,
                view,
                viewData,
                tempData,
                sw,
                new HtmlHelperOptions()
            );

            await view.RenderAsync(viewContext);
            return sw.ToString();
        }
    }
}
