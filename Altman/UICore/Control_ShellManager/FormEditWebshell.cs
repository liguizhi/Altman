﻿using System;
using System.Windows.Forms;
using Altman.LogicCore;
using Altman.ModelCore;

namespace Altman.UICore.Control_ShellManager
{
    public delegate void WebshellWatchEventHandler(object sender, EventArgs e);

    public partial class FormEditWebshell : Form
    {
        private string Id;
        private ShellManager _shellManager = null;
        public FormEditWebshell()
        {
            InitializeComponent();
            InitComboBox_ScriptType();

            _shellManager = new ShellManager();

            this.button_Add.Enabled = true;
            this.button_Alter.Enabled = false;
        }

        public FormEditWebshell(ShellStruct shellStructArray)
        {
            InitializeComponent();
            InitComboBox_ScriptType();

            _shellManager = new ShellManager();

            this.button_Add.Enabled = false;
            this.button_Alter.Enabled = true;
            this.Id = shellStructArray.Id;
            this.textBox_TargetID.Text = shellStructArray.TargetId;
            this.comboBox_TargetLevel.Text = shellStructArray.TargetLevel;

            this.textBox_ShellPath.Text = shellStructArray.ShellUrl;
            this.textBox_ShellPass.Text = shellStructArray.ShellPwd;

            this.richTextBox_Setting.Text = shellStructArray.ShellExtraSetting;

            this.textBox_Remark.Text = shellStructArray.Remark;

            this.comboBox_ScritpType.Text = shellStructArray.ShellType;
            this.comboBox_ServerCoding.Text = shellStructArray.ServerCoding;
            this.comboBox_WebCoding.Text = shellStructArray.WebCoding;
            this.comboBox_Area.Text = shellStructArray.Area;

            comboBox_types_init();
            comboBox_items_init();
        }

        private void button_Add_Click(object sender, EventArgs e)
        {
            ShellStruct shellStruct = new ShellStruct();

            shellStruct.Id = this.Id;
            shellStruct.TargetId = this.textBox_TargetID.Text.Trim();
            shellStruct.TargetLevel = this.comboBox_TargetLevel.Text.Trim();

            shellStruct.ShellUrl = this.textBox_ShellPath.Text.Trim();
            shellStruct.ShellPwd = this.textBox_ShellPass.Text.Trim();

            shellStruct.ShellExtraSetting = this.richTextBox_Setting.Text.Trim();
            shellStruct.Remark = this.textBox_Remark.Text.Trim();

            shellStruct.ShellType = this.comboBox_ScritpType.Text.Trim();
            shellStruct.ServerCoding = this.comboBox_ServerCoding.Text.Trim();
            shellStruct.WebCoding = this.comboBox_WebCoding.Text.Trim();
            shellStruct.Area = this.comboBox_Area.Text.Trim();

            string time = DateTime.Now.Date.ToShortDateString();
            if (time.Contains("/"))
            {
                time = time.Replace("/", "-");
            }
            shellStruct.AddTime = time;

            _shellManager.Insert(shellStruct);
            OnWebshellChange(EventArgs.Empty);
            this.Close();
        }

        private void button_Alter_Click(object sender, EventArgs e)
        {
            ShellStruct shellStruct = new ShellStruct();

            shellStruct.Id = this.Id;
            shellStruct.TargetId = this.textBox_TargetID.Text.Trim();
            shellStruct.TargetLevel = this.comboBox_TargetLevel.Text.Trim();

            shellStruct.ShellUrl = this.textBox_ShellPath.Text.Trim();
            shellStruct.ShellPwd = this.textBox_ShellPass.Text.Trim();

            shellStruct.ShellExtraSetting = this.richTextBox_Setting.Text.Trim();
            shellStruct.Remark = this.textBox_Remark.Text.Trim();

            shellStruct.ShellType = this.comboBox_ScritpType.Text.Trim();
            shellStruct.ServerCoding = this.comboBox_ServerCoding.Text.Trim();
            shellStruct.WebCoding = this.comboBox_WebCoding.Text.Trim();
            shellStruct.Area = this.comboBox_Area.Text.Trim();

            string time = DateTime.Now.Date.ToShortDateString();
            if (time.Contains("/"))
            {
                time = time.Replace("/", "-");
            }
            shellStruct.AddTime = time;


            _shellManager.Update(int.Parse(shellStruct.Id), shellStruct);
            OnWebshellChange(EventArgs.Empty);
            this.Close();
        }

        public event WebshellWatchEventHandler WebshellWatchEvent;
        private void OnWebshellChange(EventArgs e)
        {
            if (WebshellWatchEvent != null)
            {
                WebshellWatchEvent(this, e);
            }
        }

        /// <summary>
        /// 初始化可选择的脚本类型
        /// </summary>
        private void InitComboBox_ScriptType()
        {
            //获取可用的CustomShellType
            foreach (string type in InitUI.GetCustomShellTypeNameList())
            {
                comboBox_ScritpType.Items.Add(type);
            }
        }

        private void comboBox_types_init()
        {
            string[] types = {"PostData", "DbConnStr"};
            comboBox_types.Items.AddRange(types);
        }

        private void comboBox_items_init()
        {
            string[] items =
            {
                @"<type>access_oledb</type><conn>Provider=Microsoft.Jet.OLEDB.4.0; Data Source=my.mdb; Jet OLEDB:Database Password=passwd;</conn>",
                @"<type>mssql_oledb</type><conn>Provider=sqloledb;User ID=sa;Password=passwd;Initial Catalog=master;Data Source=(local);</conn>",
                @"<type>oracle_oledb</type><conn>Provider=OraOLEDB.Oracle; Data Source=MyOracleDB; User Id=user; Password=passwd;</conn>",
                @"<type>mysql_oledb</type><conn>Provider=MySQLProv; Data Source=mydb; User Id=user; Password=passwd;</conn>"
            };
            //comboBox_items.Items.AddRange(items);
        }


        private void comboBox_types_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_types.Text == "DbConnStr")
            {
                comboBox_items.Items.AddRange(InitUI.GetDbNodeFuncCodeNameList(comboBox_ScritpType.Text));
            }
        }
    }
}
