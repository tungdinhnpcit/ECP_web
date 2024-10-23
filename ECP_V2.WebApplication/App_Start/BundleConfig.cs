using System.Web;
using System.Web.Optimization;

namespace ECP_V2.WebApplication
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
  "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryvalAjax").Include(
                        "~/Scripts/jquery.unobtrusive-ajax*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));           

            ///For Admin Template
            bundles.Add(new ScriptBundle("~/bundles/admin/jqueryIndex").Include(
                  "~/Scripts/AdminPanel/assets/vendor/jquery/jquery.js",
                  "~/Scripts/AdminPanel/assets/vendor/jquery-browser-mobile/jquery.browser.mobile.js",
                  "~/Scripts/AdminPanel/assets/vendor/bootstrap/js/bootstrap.js",
                  "~/Scripts/AdminPanel/assets/vendor/nanoscroller/nanoscroller.js",          
                  "~/Scripts/AdminPanel/assets/vendor/morris/morris.js",
                  "~/Scripts/AdminPanel/assets/vendor/nanoscroller/nanoscroller.js",
                  "~/Scripts/AdminPanel/assets/vendor/bootstrap-datepicker/js/bootstrap-datepicker.js",
                  "~/Scripts/AdminPanel/assets/vendor/magnific-popup/magnific-popup.js",
                  "~/Scripts/AdminPanel/assets/vendor/jquery-placeholder/jquery.placeholder.js",
                  "~/Scripts/AdminPanel/assets/vendor/jquery-ui/js/jquery-ui-1.10.4.custom.js",
                  "~/Scripts/AdminPanel/assets/vendor/jquery-ui-touch-punch/jquery.ui.touch-punch.js",
                  "~/Scripts/AdminPanel/assets/vendor/select2/select2.js",
                  "~/Scripts/AdminPanel/assets/vendor/pnotify/pnotify.custom.js",
                  "~/Scripts/AdminPanel/assets/vendor/jquery-appear/jquery.appear.js",
                  "~/Scripts/AdminPanel/assets/vendor/bootstrap-multiselect/bootstrap-multiselect.js",
                  "~/Scripts/AdminPanel/assets/vendor/jquery-maskedinput/jquery.maskedinput.js",
                  "~/Scripts/AdminPanel/assets/vendor/bootstrap-tagsinput/bootstrap-tagsinput.js",
                  "~/Scripts/AdminPanel/assets/vendor/bootstrap-colorpicker/js/bootstrap-colorpicker.js",
                  "~/Scripts/AdminPanel/assets/vendor/bootstrap-timepicker/js/bootstrap-timepicker.min.js",
                  "~/Scripts/AdminPanel/assets/vendor/bootstrap-fileupload/bootstrap-fileupload.min.js",
                  "~/Scripts/AdminPanel/assets/vendor/fuelux/js/spinner.js",
                  "~/Scripts/AdminPanel/assets/vendor/dropzone/dropzone.js",
                  "~/Scripts/AdminPanel/assets/vendor/bootstrap-markdown/js/markdown.js",
                  "~/Scripts/AdminPanel/assets/vendor/bootstrap-markdown/js/to-markdown.js",
                  "~/Scripts/AdminPanel/assets/vendor/bootstrap-maxlength/bootstrap-maxlength.js",
                  "~/Scripts/AdminPanel/assets/vendor/ios7-switch/ios7-switch.js",
                  "~/Scripts/AdminPanel/assets/vendor/bootstrap-confirmation/bootstrap-confirmation.js",
                  "~/Scripts/AdminPanel/assets/vendor/jquery-easypiechart/jquery.easypiechart.js",
                  "~/Scripts/AdminPanel/assets/vendor/jquery-sparkline/jquery.sparkline.js",
                  "~/Scripts/AdminPanel/assets/vendor/liquid-meter/liquid.meter.js",
                  "~/Scripts/AdminPanel/assets/vendor/jquery-datatables/media/js/jquery.dataTables.js",
                  "~/Scripts/AdminPanel/assets/vendor/jquery-datatables/extras/TableTools/js/dataTables.tableTools.min.js",
                  "~/Scripts/AdminPanel/assets/vendor/jquery-datatables-bs3/assets/js/datatables.js",
                  "~/Scripts/AdminPanel/assets/javascripts/theme.js",
                  "~/Scripts/AdminPanel/assets/javascripts/theme.custom.js",
                  "~/Scripts/AdminPanel/assets/javascripts/theme.init.js",
                  "~/Scripts/AdminPanel/assets/javascripts/tables/examples.datatables.default.js",
                  "~/Scripts/AdminPanel/assets/javascripts/tables/examples.datatables.row.with.details.js",
                  "~/Scripts/AdminPanel/assets/javascripts/tables/examples.datatables.tabletools.js"
                  ));

            bundles.Add(new StyleBundle("~/Content/themes/admindesign/css").Include(
                        "~/Content/AdminPanel/assets/vendor/bootstrap/css/bootstrap.css",
                       "~/Content/AdminPanel/assets/vendor/font-awesome/css/font-awesome.css",
                       "~/Content/AdminPanel/assets/vendor/magnific-popup/magnific-popup.css",
                       "~/Content/AdminPanel/assets/vendor/select2/select2.css",
                       "~/Content/AdminPanel/assets/vendor/pnotify/pnotify.custom.css",
                       "~/Content/AdminPanel/assets/vendor/jquery-datatables-bs3/assets/css/datatables.css",
                       "~/Content/AdminPanel/assets/vendor/bootstrap-datepicker/css/datepicker3.css",
                       "~/Content/AdminPanel/assets/vendor/bootstrap-fileupload/bootstrap-fileupload.min.css",
                       "~/Content/AdminPanel/assets/vendor/bootstrap-multiselect/bootstrap-multiselect.css",
                       "~/Content/AdminPanel/assets/vendor/bootstrap-tagsinput/bootstrap-tagsinput.css",
                       "~/Content/AdminPanel/assets/vendor/bootstrap-colorpicker/css/bootstrap-colorpicker.css",
                       "~/Content/AdminPanel/assets/vendor/bootstrap-timepicker/css/bootstrap-timepicker.css",
                       "~/Content/AdminPanel/assets/vendor/bootstrap-markdown/css/bootstrap-markdown.min.css",
                       "~/Content/AdminPanel/assets/vendor/dropzone/css/basic.css",
                       "~/Content/AdminPanel/assets/vendor/dropzone/css/dropzone.css",
                       "~/Content/AdminPanel/assets/vendor/morris/morris.css",
                       "~/Content/AdminPanel/assets/stylesheets/theme.css",
                       "~/Content/AdminPanel/assets/stylesheets/skins/default.css",
                       "~/Content/AdminPanel/assets/stylesheets/theme-custom.css"
                       ));

            bundles.Add(new StyleBundle("~/Content/jQuery-File-Upload").Include(
                   "~/Content/jQuery.FileUpload/css/jquery.fileupload.css",
                  "~/Content/jQuery.FileUpload/css/jquery.fileupload-ui.css",
                  "~/Content/blueimp-gallery2/css/blueimp-gallery.css",
                    "~/Content/blueimp-gallery2/css/blueimp-gallery-video.css",
                      "~/Content/blueimp-gallery2/css/blueimp-gallery-indicator.css"
                  ));

            bundles.Add(new ScriptBundle("~/bundles/jQuery-File-Upload").Include(
                     //<!-- The Templates plugin is included to render the upload/download listings -->
                     "~/Scripts/jQuery.FileUpload/vendor/jquery.ui.widget.js",
                       "~/Scripts/jQuery.FileUpload/tmpl.min.js",
                    //<!-- The Load Image plugin is included for the preview images and image resizing functionality -->
                    "~/Scripts/jQuery.FileUpload/load-image.all.min.js",
                    //<!-- The Canvas to Blob plugin is included for image resizing functionality -->
                    "~/Scripts/jQuery.FileUpload/canvas-to-blob.min.js",
                    //"~/Scripts/file-upload/jquery.blueimp-gallery.min.js",
                    //<!-- The Iframe Transport is required for browsers without support for XHR file uploads -->
                    "~/Scripts/jQuery.FileUpload/jquery.iframe-transport.js",
                    //<!-- The basic File Upload plugin -->
                    "~/Scripts/jQuery.FileUpload/jquery.fileupload.js",
                    //<!-- The File Upload processing plugin -->
                    "~/Scripts/jQuery.FileUpload/jquery.fileupload-process.js",
                    //<!-- The File Upload image preview & resize plugin -->
                    "~/Scripts/jQuery.FileUpload/jquery.fileupload-image.js",
                    //<!-- The File Upload audio preview plugin -->
                    "~/Scripts/jQuery.FileUpload/jquery.fileupload-audio.js",
                    //<!-- The File Upload video preview plugin -->
                    "~/Scripts/jQuery.FileUpload/jquery.fileupload-video.js",
                    //<!-- The File Upload validation plugin -->
                    "~/Scripts/jQuery.FileUpload/jquery.fileupload-validate.js",
                    //!-- The File Upload user interface plugin -->
                    "~/Scripts/jQuery.FileUpload/jquery.fileupload-ui.js",
                    //Blueimp Gallery 2 
                    "~/Scripts/blueimp-gallery2/js/blueimp-gallery.js",
                    "~/Scripts/blueimp-gallery2/js/blueimp-gallery-video.js",
                    "~/Scripts/blueimp-gallery2/js/blueimp-gallery-indicator.js",
                    "~/Scripts/blueimp-gallery2/js/jquery.blueimp-gallery.js"

                    ));
            bundles.Add(new ScriptBundle("~/bundles/Blueimp-Gallerry2").Include(//Blueimp Gallery 2 
                    "~/Scripts/blueimp-gallery2/js/blueimp-gallery.js",
                    "~/Scripts/blueimp-gallery2/js/blueimp-gallery-video.js",
                    "~/Scripts/blueimp-gallery2/js/blueimp-gallery-indicator.js",
                    "~/Scripts/blueimp-gallery2/js/jquery.blueimp-gallery.js"));


            /*
           *for  Datatable
           */
            bundles.Add(new ScriptBundle("~/bundles/datatables").Include(
                       "~/Scripts/DataTables/jquery.dataTables.min.js",
                       "~/Scripts/DataTables/dataTables.bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/datatables").Include(
                      "~/Content/DataTables/css/dataTables.bootstrap.css"));
            // Code removed for clarity.
            BundleTable.EnableOptimizations = true;
        }
    }
}
