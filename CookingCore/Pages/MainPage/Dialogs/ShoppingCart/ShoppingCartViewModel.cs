using Cooking.Commands;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using System;
using System.IO;
using System.Text;

namespace Cooking.Pages.Recepies
{
    public partial class ShoppingCartViewModel
    {
        public ShoppingCartViewModel(string list)
        {
            CloseCommand = new Lazy<DelegateCommand>(
                () => new DelegateCommand(async () => {
                    var current = await DialogCoordinator.Instance.GetCurrentDialogAsync<BaseMetroDialog>(this);
                    await DialogCoordinator.Instance.HideMetroDialogAsync(this, current);
                }));

            EvernoteExportCommand = new Lazy<DelegateCommand>(
                () => new DelegateCommand(() => {
                    var sb = new StringBuilder();
                    sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                    sb.AppendLine("<!DOCTYPE en-export SYSTEM \"http://xml.evernote.com/pub/evernote-export2.dtd\">");
                    sb.AppendLine($"<en-export export-date=\"{DateTime.Now.ToString("yyyyMMddTHHmmssZ")}\" application=\"Evernote/Windows\" version=\"6.x\">");
                    sb.AppendLine("<note>");
                    sb.AppendLine($"<title>Продукты на неделю</title>");
                    sb.AppendLine("		<content>");
                    sb.AppendLine("			<![CDATA[<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                    sb.AppendLine("			<!DOCTYPE en-note SYSTEM \"http://xml.evernote.com/pub/enml2.dtd\">");
                    sb.AppendLine("			<en-note>");

                    var split = List.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

                    for (int i = 0; i < split.Length; i++)
                    {
                        while (i < split.Length && string.IsNullOrWhiteSpace(split[i]))
                        {
                            i++;
                        }

                        if (i == split.Length) break;

                        if (split[i].StartsWith("\t"))
                        {
                            sb.AppendLine("				<div>");
                            sb.AppendLine($"					<en-todo/>{split[i].TrimStart(new[] { '\t' })}");
                            sb.AppendLine("				</div>");
                        }
                        else
                        {
                            if (split[i] == "---------------------------------------------")
                            {
                                sb.AppendLine("<hr/>");
                            }
                            else
                            {
                                sb.AppendLine("				<div>");
                                sb.AppendLine($"					<span style=\"font-weight: bold; \"><font style=\"font-size: 14pt;\">{split[i]}</font></span>");
                                sb.AppendLine("				</div>");
                            }
                            continue;
                        }
                        i++;

                        bool ulStarted = false;
                        if (!string.IsNullOrWhiteSpace(split[i]))
                        {
                            sb.AppendLine("				<ul>");
                            ulStarted = true;
                        }

                        while (i < split.Length && !string.IsNullOrWhiteSpace(split[i]))
                        {
                            sb.AppendLine($"<li>{split[i]}<br clear=\"none\"/></li>");
                            i++;
                        }

                        if (ulStarted)
                        {
                            sb.AppendLine("				</ul>");
                        }
                    }


                    sb.AppendLine("			</en-note>]]>");
                    sb.AppendLine("		</content>");
                    sb.AppendLine($"		<created>{DateTime.Now.ToString("yyyyMMddTHHmmssZ")}</created>");
                    sb.AppendLine($"		<updated>{DateTime.Now.ToString("yyyyMMddTHHmmssZ")}</updated>");
                    sb.AppendLine("		<note-attributes>");
                    sb.AppendLine("			<author>Cookbook</author>");
                    sb.AppendLine("		</note-attributes>");
                    sb.AppendLine("	</note>");
                    sb.AppendLine("</en-export>");
                    sb.Replace("\r\n", "");
                    sb.Replace("\t", "");

                    SaveFileDialog saveFileDialog = new SaveFileDialog();

                    saveFileDialog.FileName = "Продукты.enex";
                    saveFileDialog.FilterIndex = 2;
                    saveFileDialog.RestoreDirectory = true;

                    if (saveFileDialog.ShowDialog().Value)
                    {
                        File.WriteAllText(saveFileDialog.FileName, sb.ToString());
                    }
                }));

            List = list;
        }

        public Lazy<DelegateCommand> EvernoteExportCommand { get; }
        public Lazy<DelegateCommand> CloseCommand { get; }
        
        public string List { get; }
    }
}