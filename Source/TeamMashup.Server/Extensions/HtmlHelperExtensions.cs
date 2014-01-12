using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc.Html;
using TeamMashup.Membership;

namespace System.Web.Mvc
{
    public static class HtmlHelperExtensions
    {
        public static string BoolToVisibilityConverter(this HtmlHelper helper, bool visible)
        {
            if (visible)
                return "visible";

            return "hidden";
        }

        public static MvcHtmlString ValidationPopover(this HtmlHelper helper, string key)
        {
            if (!helper.ViewData.ModelState.IsValidField(key))
            {
                var context = helper.ViewContext.Controller.ControllerContext;
                if (context == null)
                    return new MvcHtmlString(string.Empty);

                helper.ViewBag.ValidationKey = key;

                using (var sw = new StringWriter())
                {
                    var viewResult = ViewEngines.Engines.FindPartialView(context, "_ValidationPopover");
                    var viewContext = new ViewContext(context, viewResult.View, helper.ViewData, new TempDataDictionary(), sw);
                    viewResult.View.Render(viewContext, sw);
                    return new MvcHtmlString(sw.GetStringBuilder().ToString());
                }
            }

            return new MvcHtmlString(string.Empty);
        }

        public static string VisibleIfHasClaims(this HtmlHelper helper, params string[] claims)
        {
            var hasClaims = WebSecurity.AccessControl.UserHasClaims(WebSecurity.CurrentUserId, claims);

            return hasClaims ? string.Empty : "hide";
        }

        public static MvcHtmlString DropDownListForEnum<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string optionLabel, object htmlAttributes)
        {
            if (!typeof(TProperty).IsEnum)
                throw new Exception("This helper can be used only with enum types");

            var enumType = typeof(TProperty);
            var fields = enumType.GetFields(
                BindingFlags.Static | BindingFlags.GetField | BindingFlags.Public
            );

            var values = Enum.GetValues(enumType).OfType<TProperty>();
            var items = from value in values
                        from field in fields
                        let descriptionAttribute = field.GetCustomAttributes(typeof(DisplayAttribute), true).OfType<DisplayAttribute>().FirstOrDefault()
                        let resourceType = descriptionAttribute.ResourceType
                        let displayName = (descriptionAttribute != null) ? 
                            resourceType.GetProperty(descriptionAttribute.Name, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public).GetValue(null, null) :
                            value.ToString()
                        where value.ToString() == field.Name
                        select new { Id = value, Name = displayName };

            var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            var enumObj = metadata;
            var selectList = new SelectList(items, "Id", "Name", metadata.Model);

            return SelectExtensions.DropDownListFor(htmlHelper, expression, selectList, optionLabel, htmlAttributes);
        }
    }
}