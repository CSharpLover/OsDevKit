using OsDevKit.UI.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OsDevKit.UI
{
    public partial class ProjectView : Form
    {
        public ProjectView()
        {
            InitializeComponent();
        }

        ProjectFile buffer = new ProjectFile();

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (Global.CurrentProjectFile != null)
            {
                if (buffer != Global.CurrentProjectFile)
                {
                    treeView1.Nodes.Clear();
                    treeView1.Nodes.Add(Global.CurrentProjectFile.Name);
                    foreach (var i in Global.CurrentProjectFile.Files)
                    {
                        if (!i.Contains("\\"))
                        {
                            treeView1.Nodes[0].Nodes.Add(i);
                        }
                        else
                        {

                            ProcessPath(i.Split('\\'), treeView1.Nodes[0].Nodes);
                        }

                    }
                    buffer = Global.CurrentProjectFile;
                    treeView1.ExpandAll();
                }
            }

        }

        public void ProcessPath(IEnumerable<String> path, TreeNodeCollection nodes)
        {
            if (!path.Any())
                return;
            var node = nodes.Cast<TreeNode>().FirstOrDefault(n => n.Text == path.First());
            if (node == null)
            {
                node = new TreeNode(text: path.First());
                nodes.Add(node);
            }
            ProcessPath(path.Skip(1), node.Nodes);
        }

        private void treeView1_DoubleClick(object sender, EventArgs e)
        {
            bool FoundMdiEditor = false;
            var sel = treeView1.SelectedNode.FullPath.Replace(Global.CurrentProjectFile.Name + "\\", "");

            if (!sel.Contains("."))
            {
                return;
            }
            foreach (var i in this.MdiParent.MdiChildren)
            {
                if (i is Editor)
                {
                    if ((i as Editor).FileName == sel)
                    {
                        i.BringToFront();
                        FoundMdiEditor = true;
                    }

                }
            }
            if (!FoundMdiEditor)
            {
                var f = new Editor();
                f.FileName = sel;
                f.MdiParent = this.MdiParent;
                f.Show();
            }


        }

        private void treeView1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // Select the clicked node
                treeView1.SelectedNode = treeView1.GetNodeAt(e.X, e.Y);

                if (treeView1.SelectedNode != null)
                {
                    contextMenuStrip1.Show(treeView1, e.Location);
                }
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
               


                AskDlg dlg = new AskDlg();
                dlg.Quiestion = "Name:";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    var sel1 = treeView1.SelectedNode.Parent;
                    var sel2 = treeView1.SelectedNode;
                    var sel = "";
                    if (sel1 != null)
                    {
                        if (dlg.Awnser.Contains("."))
                        {
                            sel = sel2.FullPath.Replace(Global.CurrentProjectFile.Name + "\\", "");

                        }
                        else
                        {
                            sel = sel1.FullPath.Replace(Global.CurrentProjectFile.Name + "\\", "");
                        }
                    }

                    if (sel == "")
                    {
                        Global.CurrentProjectFile.Files.Add(dlg.Awnser);
                    }
                    else
                    {
                        Global.CurrentProjectFile.Files.Add(sel + "\\" + dlg.Awnser);
                    }

                    var p = Path.Combine(Global.CurrentProjectFilePath, "files", sel, dlg.Awnser);

                    if (!dlg.Awnser.Contains("."))
                    {
                        Directory.CreateDirectory(p);
                    }
                    else
                    {
                        File.WriteAllText(p," ");
                    }
                }

            }
            catch (Exception ee)
            {

            }
            Global.Save();
            buffer = null;
        }



        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {

                var sel = treeView1.SelectedNode.FullPath.Replace(Global.CurrentProjectFile.Name + "\\", "");

                if (!sel.Contains("."))
                {
                    foreach (var i in Global.CurrentProjectFile.Files.ToArray().ToList())
                    {
                        if (new FileInfo(i).Directory.Name == sel)
                        {
                            Global.CurrentProjectFile.Files.RemoveAt(Global.CurrentProjectFile.Files.IndexOf(i));
                        }
                    }
                    foreach (var i in Directory.EnumerateFiles(Path.Combine(Global.CurrentProjectFilePath, "files", sel)))
                    {
                        File.Delete(i);
                    }

                    Directory.Delete(Path.Combine(Global.CurrentProjectFilePath, "files", sel));
                }
                else
                {
                    foreach (var i in Global.CurrentProjectFile.Files.ToArray().ToList())
                    {
                        if (i == sel)
                        {
                            Global.CurrentProjectFile.Files.RemoveAt(Global.CurrentProjectFile.Files.IndexOf(i));
                            File.Delete(Path.Combine(Global.CurrentProjectFilePath, "files", i));
                        }
                    }

                }
            }
            catch (Exception ee)
            {

                

            }
            Global.Save();
            buffer = null;

        }

        private void ProjectView_Load(object sender, EventArgs e)
        {
            treeView1.Sorted = true;
        }
    }
}
