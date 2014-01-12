using System.Web.Optimization;

namespace TeamMashup.Server.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include
             (
                 "~/Scripts/jquery-{version}.js",
                 "~/Scripts/jquery.validate.*",
                 "~/Scripts/jquery.extensions.js",
                 "~/Scripts/jquery.unobtrusive-ajax.js",
                 "~/Scripts/jquery.typewatch.js",
                 "~/Scripts/jquery-ui-*",
                 "~/Scripts/mustache.js",
                 "~/Scripts/jquery.tm.search-box.js",
                 "~/Scripts/hogan-{version}.js",
                 "~/Scripts/typeahead.js"
             ));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/bootstrap-modalmanager.js",
                      "~/Scripts/bootstrap-modal.js",
                      "~/Scripts/bootstrap-datepicker.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/teammashup").Include(
                   "~/Scripts/team.mashup.core.js",
                   "~/Scripts/team.mashup.ui.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/datatables").Include(
                        "~/Scripts/DataTables-1.9.4/media/js/jquery.dataTables.js",
                        "~/Scripts/DataTables-1.9.4/extensions/fnReloadAjax.js",
                        "~/Scripts/team.mashup.data-table.js",
                        "~/Scripts/jquery.dataTables.rowReordering.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/siganlr").Include(
                    "~/Scripts/jquery.signalR-{version}.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/reports").Include(
                    "~/Scripts/flot/jquery.flot.js",
                    "~/Scripts/flot/jquery.flot.time.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/moment").Include(
                    "~/Scripts/moment.*"
            ));

            bundles.Add(new StyleBundle("~/Content/styles").Include(
                        "~/Content/bootstrap/bootstrap.css",
                        "~/Content/bootstrap/bootstrap-modal-bs3patch.css",
                        "~/Content/bootstrap/bootstrap-modal.css",
                        "~/Content/bootstrap/bootstrap-overrides.css",
                        "~/Content/bootstrap/datepicker.css",
                        "~/Content/styles.css",
                        "~/Content/elusive-webfont.css",
                        "~/Content/typeahead.css"
            ));

            bundles.Add(new StyleBundle("~/Content/datatables").Include(
                    "~/Content/DataTables-1.9.4/media/css/jquery.dataTables.css",
                    "~/Content/team.mashup.data-table.css"
            ));
        }
    }
}