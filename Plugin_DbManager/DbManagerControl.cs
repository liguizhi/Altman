﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Altman.Common.AltData;
using Altman.ModelCore;
using PluginFramework;

namespace Plugin_DbManager
{
    public partial class DbManagerControl : UserControl
    {
        private HostService _hostService;
        private ShellStruct _shellData;

        private DbManagerService dbManagerService;
        private XmlNode node = null;
        public DbManagerControl(HostService hostService, ShellStruct data)
        {
            InitializeComponent();
            this.Dock = System.Windows.Forms.DockStyle.Fill;

            _hostService = hostService;
            _shellData = data;

            //初始化ExtraSettingXml
            InitExtraSettingXml();

            //绑定事件
            dbManagerService = new DbManagerService(_hostService, _shellData, GetDbType());
            dbManagerService.GetDbNameCompletedToDo += dbManagerService_GetDbNameCompletedToDo;
            dbManagerService.GetDbTableNameCompletedToDo += dbManagerService_GetTableNameCompletedToDo;
            dbManagerService.GetColumnTypeCompletedToDo += dbManagerService_GetColumnTypeCompletedToDo;
            dbManagerService.ExecuteReaderCompletedToDo += dbManagerService_ExecuteReaderCompletedToDo;
            dbManagerService.ExecuteNonQueryCompletedToDo += dbManagerService_ExecuteNonQueryCompletedToDo;
            
            treeView_Dbs.AfterSelect += treeView_Dbs_AfterSelect;           

            //获取数据库
            //dbManagerService.GetDbName(_shellData.ShellExtraSetting);

            dbManagerService.GetDbName(GetConnStr());
        }

        private void dbManagerService_ExecuteNonQueryCompletedToDo(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                _hostService.ShowMsgInStatusBar(e.Error.Message);
            }
            else if (e.Result is DataTable)
            {
                dataGridView_result.DataSource = e.Result;
            }
        }
        private void dbManagerService_ExecuteReaderCompletedToDo(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                _hostService.ShowMsgInStatusBar(e.Error.Message);
            }
            else if (e.Result is DataTable)
            {
                dataGridView_result.DataSource = e.Result;
            }
        }
        private void dbManagerService_GetColumnTypeCompletedToDo(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                _hostService.ShowMsgInStatusBar(e.Error.Message);
            }
            else if (e.Result is string[])
            {
                string[] columns = e.Result as string[];
                if (columns.Length > 0)
                {
                    RefreshColumnsInDbTree(columns, treeView_Dbs.SelectedNode);
                }
                else
                {
                    //更改table为失败的图标
                    treeView_Dbs.SelectedNode.ImageIndex = 4;
                    treeView_Dbs.SelectedNode.SelectedImageIndex = 4;
                }
            }
        }
        private void dbManagerService_GetTableNameCompletedToDo(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                _hostService.ShowMsgInStatusBar(e.Error.Message);
            }
            else if (e.Result is string[])
            {
                string[] tables = e.Result as string[];
                if (tables.Length > 0)
                {
                    RefreshTablesInDbTree(tables, treeView_Dbs.SelectedNode);
                }
                else
                {
                    //更改db为失败的图标
                    treeView_Dbs.SelectedNode.ImageIndex = 1;
                    treeView_Dbs.SelectedNode.SelectedImageIndex = 1;
                }
            }
        }
        private void dbManagerService_GetDbNameCompletedToDo(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                _hostService.ShowMsgInStatusBar(e.Error.Message);
                //更改root为失败的图标
                RefreshRootInDbTree(false);
            }
            else if (e.Result is string[])
            {
                string[] dbs = e.Result as string[];
                if (dbs.Length > 0)
                {
                    RefreshRootInDbTree(true);
                    RefreshDbsInDbTree(dbs, treeView_Dbs.Nodes[0]);
                }
                else
                {
                    //更改root为失败的图标
                    RefreshRootInDbTree(false);
                }
            }
        }

        private void RefreshRootInDbTree(bool isSuccess)
        {
            treeView_Dbs.Nodes.Clear();
            int index = isSuccess ? 0 : 1;
            string root = "(local)";
            TreeNode node = new TreeNode(root, index, index)
            {
                Name = root,
                Tag = "root"
            };
            treeView_Dbs.Nodes.Add(node);
        }

        private void RefreshDbsInDbTree(string[] dbs, TreeNode selected)
        {
            selected.Nodes.Clear();
            foreach (string db in dbs)
            {
                TreeNode node = new TreeNode(db, 2, 2)
                {
                    Name = db,
                    Tag = "db"
                };
                selected.Nodes.Add(node);
            }
            selected.Expand();
            //刷新数据库选择框
            toolStripComboBox_dbs.Items.Clear();
            toolStripComboBox_dbs.Items.AddRange(dbs);
        }

        private void RefreshTablesInDbTree(string[] tables, TreeNode selected)
        {
            selected.Nodes.Clear();
            foreach (string table in tables)
            {
                TreeNode node = new TreeNode(table, 3, 3)
                {
                    Name = table,
                    Tag = "table"
                };
                selected.Nodes.Add(node);
            }
            selected.Expand();
        }

        private void RefreshColumnsInDbTree(string[] columns, TreeNode selected)
        {
            selected.Nodes.Clear();
            foreach (string column in columns)
            {
                TreeNode node = new TreeNode(column, 5, 5)
                {
                    Name = column,
                    Tag = "column"
                };
                selected.Nodes.Add(node);
            }
            selected.Expand();
        }

        private void treeView_Dbs_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            string name = treeView_Dbs.SelectedNode.Text;
            string type = (string)(treeView_Dbs.SelectedNode.Tag ?? "");
            if (name != "")
            {
                if (type == "root")
                {
                    //dbManagerService.GetDbName(_shellData.ShellExtraSetting);
                    dbManagerService.GetDbName(GetConnStr());
                }
                else if (type == "db")
                {
                    //dbManagerService.GetTableName(_shellData.ShellExtraSetting, name);
                    dbManagerService.GetTableName(GetConnStr(),name);
                }
                else if (type == "table")
                {
                    string dbname = treeView_Dbs.SelectedNode.Parent.Text;
                    //dbManagerService.GetColumnType(_shellData.ShellExtraSetting, dbname, name);

                    dbManagerService.GetColumnType(GetConnStr(), dbname, name);
                }
            }
        }

        private void treeView_Dbs_AfterSelect(object sender, TreeViewEventArgs e)
        {
            int index = treeView_Dbs.SelectedNode.Index;
            string type = (string)(treeView_Dbs.SelectedNode.Tag ?? "");
            if (type == "db")
            {
                toolStripComboBox_dbs.SelectedIndex = index;
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            string sql = tbx_sql.Text;
            if (string.IsNullOrWhiteSpace(sql))
            {
                MessageBox.Show("查询语句不能为空");
            }
            else if (toolStripComboBox_dbs.SelectedIndex == -1)
            {
                MessageBox.Show("请选择数据库");
            }
            else
            {
                //执行前先清空之前结果
                dataGridView_result.DataSource = null;

                string dbName = (string)toolStripComboBox_dbs.SelectedItem;
                //dbManagerService.ExecuteReader(_shellData.ShellExtraSetting, dbName, sql);


                if (sql.ToLower().StartsWith("select"))
                {
                    dbManagerService.ExecuteReader(GetConnStr(), dbName, sql);
                }
                else
                {
                    dbManagerService.ExecuteNonQuery(GetConnStr(), dbName, sql);
                }
                
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            /**
             * <connection>
             *  <type></type>
             *  <conn></conn>
             *  <host></host>
             *  <user></user>
             *  <pass></pass>
             *  <language></language>
             * </connection>
             */
        }

        
        private void InitExtraSettingXml()
        {
            //创建ExtraSetting节点
            node = new XmlDocument().CreateElement("ExtraSetting");
            node.InnerXml = _shellData.ShellExtraSetting;
        }
        private string GetDbType()
        {
            string type = string.Empty;
            if (node != null)
            {
                XmlNode typeNode = node.SelectSingleNode("/connection/type");
                if (typeNode != null)
                    type = typeNode.InnerText;              
            }
            return type;
        }
        private string GetConnStr()
        {
            string conn = string.Empty;
            if (node != null)
            {
                //获取type
                string scriptType = _shellData.ShellType;

                if (scriptType.StartsWith("php"))
                {
                    string host = string.Empty;
                    string user = string.Empty;
                    string pass = string.Empty;
                    string language = string.Empty;
                    XmlNode hostNode = node.SelectSingleNode("/connection/host");
                    if (hostNode != null) host = hostNode.InnerText;
                    XmlNode userNode = node.SelectSingleNode("/connection/user");
                    if (userNode != null) user = userNode.InnerText;
                    XmlNode passNode = node.SelectSingleNode("/connection/pass");
                    if (passNode != null) pass = passNode.InnerText;
                    XmlNode lanNode = node.SelectSingleNode("/connection/language");
                    if (lanNode != null) language = lanNode.InnerText;

                    conn = string.Format("{0};{1};{2};{3};", host, user, pass, language);
                }
                else
                {

                    XmlNode connNode = node.SelectSingleNode("/connection/conn");
                    if (connNode != null) 
                        conn = connNode.InnerText;
                }
            }
            return conn;           
        }
    }
}
