using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using QuickLook.Common.Plugin;
using QuickLook.Plugin.HtmlViewer;
using UtfUnknown;

namespace QuickLook.Plugin.JupyterNotebookViewer
{
    public class Plugin : IViewer
    {
        private WebpagePanel _panel;

        public int Priority => 0;

        public void Init()
        {
        }

        public bool CanHandle(string path)
        {
            return !Directory.Exists(path) && new[] { ".ipynb" }.Any(path.ToLower().EndsWith);
        }

        public void Prepare(string path, ContextObject context)
        {
            context.PreferredSize = new Size(1000, 600);
        }

        public void View(string path, ContextObject context)
        {
            _panel = new WebpagePanel();
            context.ViewerContent = _panel;
            context.Title = Path.GetFileName(path);

            _panel.NavigateToHtml(GenerateNotebookHtml(path));
            _panel.Dispatcher.Invoke(() => { context.IsBusy = false; }, DispatcherPriority.Loaded);
        }

        public void Cleanup()
        {
            GC.SuppressFinalize(this);

            _panel?.Dispose();
            _panel = null;
        }

        private string GenerateNotebookHtml(string path)
        {
            var html = Resources.json2html;
            html = html.Replace("{{prism_min_css}}", Resources.prism_min_css);
            html = html.Replace("{{marked_min_js}}", Resources.marked_min_js);
            html = html.Replace("{{prism_min_js}}", Resources.prism_min_js);
            html = html.Replace("{{prism_min_css}}", Resources.prism_min_css);
            html = html.Replace("{{prism_python_min_js}}", Resources.prism_python_min_js);
            html = html.Replace("{{katex_min_css}}", Resources.katex_min_css);
            html = html.Replace("{{katex_min_js}}", Resources.katex_min_js);
            html = html.Replace("{{nbv_js}}", Resources.nbv_js);

            var bytes = File.ReadAllBytes(path);
            var encoding = CharsetDetector.DetectFromBytes(bytes).Detected?.Encoding ?? Encoding.Default;

            var nbjson = encoding.GetString(bytes);
            nbjson = WebUtility.HtmlEncode(nbjson);

            html = html.Replace("{{content}}", nbjson);

            return html;
        }
    }
}