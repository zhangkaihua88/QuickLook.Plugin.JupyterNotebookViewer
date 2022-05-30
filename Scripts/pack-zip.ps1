Remove-Item ..\QuickLook.Plugin.JupyterNotebookViewer.qlplugin -ErrorAction SilentlyContinue

# Copy-Item -Path ..\QuickLook.Plugin.HtmlViewer\runtimes\ -Destination ..\bin\Release\ -Recurse
$files = Get-ChildItem -Path ..\bin\Release\ -Exclude *.pdb,*.xml
Compress-Archive $files ..\QuickLook.Plugin.JupyterNotebookViewer.zip
Move-Item ..\QuickLook.Plugin.JupyterNotebookViewer.zip ..\QuickLook.Plugin.JupyterNotebookViewer.qlplugin