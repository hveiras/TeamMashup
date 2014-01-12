using System.Web.Mvc.Html;

namespace System.Web.Mvc.Ajax
{
    public static class AjaxHelperExtensions
    {
        public static MvcForm CustomBeginForm(this AjaxHelper ajaxHelper, string actionName, string formName, object htmlAttributes)
        {
            return CustomBeginFormInternal(ajaxHelper, actionName, null, null, formName, htmlAttributes);
        }

        public static MvcForm CustomBeginForm(this AjaxHelper ajaxHelper, string actionName, string controllerName, string formName, object htmlAttributes)
        {
            return CustomBeginFormInternal(ajaxHelper, actionName, controllerName, null, formName, htmlAttributes);
        }

        public static MvcForm CustomBeginForm(this AjaxHelper ajaxHelper, string actionName, string controllerName, object routeValues, string formName, object htmlAttributes)
        {
            return CustomBeginFormInternal(ajaxHelper, actionName, controllerName, routeValues, formName, htmlAttributes);
        }

        private static MvcForm CustomBeginFormInternal(this AjaxHelper ajaxHelper, string actionName, string controllerName, object routeValues, string formName, object htmlAttributes)
        {
            var onCompleteCallback = string.Format("{0}Complete", formName);
            var successCallback = string.Format("{0}Success", formName);
            var validationErrorsCallback = string.Format("{0}ValidationErrors", formName);

            var onComplete = string.Format("onAjaxFormComplete(xhr, status, {0})", onCompleteCallback);
            var onSuccess = string.Format("onAjaxFormSuccess(data, status, xhr, {0})", successCallback);
            var onFailure = string.Format("onAjaxFormFailure(xhr, status, error, {0})", validationErrorsCallback);

            return ajaxHelper.BeginForm(actionName, controllerName, routeValues, new AjaxOptions { OnComplete = onComplete, OnSuccess = onSuccess, OnFailure = onFailure }, htmlAttributes);
        }
    }
}