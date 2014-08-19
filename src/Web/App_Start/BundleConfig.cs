using System.Web;
using System.Web.Optimization;

namespace Web
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/angular").Include(
                "~/Scripts/vendor/angular-1.2.22/angular.js",
                "~/Scripts/vendor/angular-1.2.22/angular-animate.js",
                "~/Scripts/vendor/angular-1.2.22/angular-cookies.js",
                "~/Scripts/vendor/angular-1.2.22/angular-loader.js",
                "~/Scripts/vendor/angular-1.2.22/angular-resource.js",
                "~/Scripts/vendor/angular-1.2.22/angular-route.js",
                "~/Scripts/vendor/angular-1.2.22/angular-sanitize.js",
                "~/Scripts/vendor/angular-1.2.22/angular-touch.js",
                "~/Scripts/vendor/lodash-2.4.1/lodash.compat.js",
                "~/Scripts/vendor/angular-spinner-0.5.1/angular-spinner.js",
                "~/Scripts/vendor/spin.js/spin.js",
                "~/Scripts/vendor/nsPopover-0.5.8/nsPopover.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/site").Include(
                "~/Scripts/site/iscrimmage.api.js",
                "~/Scripts/site/iscrimmage.app.js",
                "~/Scripts/site/iscrimmage.home.controller.js",
                "~/Scripts/site/iscrimmage.account.controller.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/site.css",
                "~/Scripts/vendor/nsPopover-0.5.8/nsPopover.css"));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/jquery.ui.core.css",
                        "~/Content/themes/base/jquery.ui.resizable.css",
                        "~/Content/themes/base/jquery.ui.selectable.css",
                        "~/Content/themes/base/jquery.ui.accordion.css",
                        "~/Content/themes/base/jquery.ui.autocomplete.css",
                        "~/Content/themes/base/jquery.ui.button.css",
                        "~/Content/themes/base/jquery.ui.dialog.css",
                        "~/Content/themes/base/jquery.ui.slider.css",
                        "~/Content/themes/base/jquery.ui.tabs.css",
                        "~/Content/themes/base/jquery.ui.datepicker.css",
                        "~/Content/themes/base/jquery.ui.progressbar.css",
                        "~/Content/themes/base/jquery.ui.theme.css"));
        }
    }
}