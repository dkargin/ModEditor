using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace ModEditor.Controllers
{
    public class StringsController : ModEditor.Controller
    {
        static StringsController lastController;
        static DataTable globalStrings = new DataTable();
        static List<string> globalLanguages = new List<string>();

        DataTable localStrings = new DataTable();

        DataColumn localColumnTokens;
        static string tokensColumnName = "token";
        public StringsController(ModContents mod)
            : base(mod)
        {
            lastController = this;
            rootItem.name = "Strings";
            groupName = "Localization";

            InitTable(globalStrings);

            localColumnTokens = InitTable(localStrings);
            bindingSource.DataSource = localStrings;
            localStrings.TableNewRow += localStrings_TableNewRow;
            localStrings.ColumnChanged += localStrings_ColumnChanged;
        }

        void localStrings_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {            
            int newToken = (int)e.Row[0];
            if (e.Column.ColumnName == tokensColumnName)
            return;
            DataRow row = globalStrings.Rows.Find(newToken);
            DataColumn column = globalStrings.Columns[e.Column.ColumnName];
            
            if (row != null && column != null)
            {
                row[column] = e.Row[e.Column];
            }
        }

        void localStrings_TableNewRow(object sender, DataTableNewRowEventArgs e)
        {
            int newToken = (int)e.Row[0];
            DataRow row = globalStrings.Rows.Find(newToken);

            if (row == null)
            {
                row = globalStrings.NewRow();                
                //row.BeginEdit();

                globalStrings.Rows.Add(row);
                //row.EndEdit();
            }
            else
            {
                //row[columnValue] = entry.Text;
            }
            //throw new NotImplementedException();
        }

        // Initialises strings table
        DataColumn InitTable(DataTable sourceTable)
        {
            if (sourceTable.Columns.Contains(tokensColumnName))
                return sourceTable.Columns[tokensColumnName];
            
            var columnTokens = new DataColumn(tokensColumnName, typeof(int));
            columnTokens.Unique = true;
            columnTokens.AutoIncrementStep = 1;
            columnTokens.AutoIncrement = true;
            sourceTable.Columns.Add(columnTokens);
            sourceTable.PrimaryKey = new DataColumn[] { columnTokens };
            return columnTokens;            
        }

        static string defaultLanguage = "English";

        public static string GetLocString(int token)
        {
            return GetLocString(token, defaultLanguage);
        }

        public static string GetLocString(int token, string language)
        {
            if (globalStrings.Columns.Contains(language))
            {
                DataColumn column = globalStrings.Columns[language];
                DataColumn tokenColumn = globalStrings.Columns[language];
                DataRow row = globalStrings.Rows.Find(token);
                if(row != null)
                    return row[column].ToString();
            }
            return "";
        }

        public override void ClearCache()
        {

        }

        bool StringOverriden(int token, string language)
        {
            DataRow rowGlobal = globalStrings.Rows.Find(token);
            if (rowGlobal == null)
                return false;
            DataRow rowLocal = localStrings.Rows.Find(token);
            
            DataColumn colGlobal = globalStrings.Columns.Contains(language) ? globalStrings.Columns[language] : null;
            DataColumn colLocal = localStrings.Columns.Contains(language) ? localStrings.Columns[language] : null;
            if (colGlobal == null && colLocal == null)
                return false;
            if (colGlobal == null || colLocal == null)
                return true;
            if (!rowGlobal[colGlobal].Equals(rowLocal[colLocal]))
                return true;
                
            
            return false;
        }

        private void CustomCellFormatting(object sender, System.Windows.Forms.DataGridViewCellFormattingEventArgs e)
        {         
            DataColumn columnToken = localStrings.Columns[tokensColumnName];
            DataColumn columnData = localStrings.Columns[e.ColumnIndex];

            if (columnData.ColumnName.Equals(tokensColumnName))
                return;
            try
            {
                if(e.RowIndex <= localStrings.Rows.Count)
                {
                    DataRow row = localStrings.Rows[e.RowIndex];
                    int token = (int)row[columnToken];
                    if (StringOverriden(token, columnData.ColumnName))
                        e.CellStyle.BackColor = Color.LightGreen;
                }
            }
            catch(Exception )
            {
            }
        }

        public override ContextMenuStrip GenerateContextMenu(ModEditor.Item item)
        {
            ContextMenuStrip result = new ContextMenuStrip();
            if (item.Equals(rootItem))
            {
                result.Items.Add("New Language", null, OnNewItem);
                //result.Items.Add("Select languages", null, OnSelectLanguages);
            }
            else
            {
                result.Items.Add("Delete language", null, OnDeleteItem);
            }
            return result;
        }

        private void OnDeleteItem(object sender, EventArgs e)
        {

        }

        // Show dialog with selecting active languages
        private void OnSelectLanguages(object sender, EventArgs e)
        {
            using (FormSelect form = new FormSelect(true))
            {
                foreach (DataColumn column in localStrings.Columns)
                {
                    form.AddItem(new FormSelect.Item()
                    {
                        name = column.ColumnName,
                        fromBase = false,
                        fromMods = true,
                        fromCurrentMod = true,
                    });
                }
                DialogResult result = form.ShowDialog();
            }
        }

        private void OnNewItem(object sender, EventArgs e)
        {
        }
        private BindingSource bindingSource = new BindingSource();
        public override Control GenerateControl(ModEditor.Item item)
        {

            DataGridView result = new DataGridView();
            //result.DataSource = bindingSource;
            result.DataSource = localStrings;           
            
            result.Dock = DockStyle.Fill;            
            result.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            result.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.CustomCellFormatting);
            result.VirtualMode = true;
            //result.CellValueNeeded += result_CellValueNeeded;
            //result.CellValuePushed += result_CellValuePushed;
            //result.NewRowNeeded += result_NewRowNeeded;
            //result.RowsAdded += result_RowsAdded;
            //result.UserAddedRow += result_UserAddedRow;
            //result.Columns["text"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            //result.MasterTemplate.AddNewBoundRowBeforeEdit = true;
            //result.DataSourceChanged
            return result;
        }

        void result_NewRowNeeded(object sender, DataGridViewRowEventArgs e)
        {
            DataGridView control = sender as DataGridView;
            //control.CommitEdit(DataGridViewDataErrorContexts.CurrentCellChange);
            // control.EndEdit();
            //e.RowIndex
            //int index = (int)e.Row.HeaderCell.RowIndex;
            /*
            int token = (int)control.Rows[e.RowIndex].Cells[0].Value;
            int count = localStrings.Rows.Count;*/
            
            var row = localStrings.NewRow();
            localStrings.Rows.Add(row);            
        }


        // Save new data from strings view to localStrings
        void result_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            
        }

        void result_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (e.RowIndex < localStrings.Rows.Count)
            {
                var row = localStrings.Rows[e.RowIndex];
                e.Value = row[e.ColumnIndex];
            }
        }       
        
        public void CreateNewString(int token)
        {
            DataRow row = globalStrings.Rows.Find(token);
            if (row == null)
            {
                row = globalStrings.NewRow();
                row[tokensColumnName] = token;
            }
        }

        private Ship_Game.LocalizationFile LoadLanguage(string language)
        {
            FileInfo[] textList = Tools.GetFilesFromDirectory(language);
            XmlSerializer serializer = new XmlSerializer(typeof(Ship_Game.LocalizationFile));
            FileInfo[] array = textList;
            Ship_Game.LocalizationFile result = null;
            for (int i = 0; i < array.Length; i++)
            {
                FileInfo FI = array[i];
                FileStream stream = FI.OpenRead();
                Ship_Game.LocalizationFile data = (Ship_Game.LocalizationFile)serializer.Deserialize(stream);
                stream.Close();
                stream.Dispose();
                result = data;
                //Ship_Game.ResourceManager.LanguageFile = data;
            }
            //Ship_Game.Localizer.FillLocalizer();
            return result;
        }

        private void ProcessLanguage(Ship_Game.LocalizationFile data, string language, DataTable sourceTable)
        {
            DataColumn columnValue = null;

            if (!sourceTable.Columns.Contains(language))
            {
                columnValue = sourceTable.Columns.Add(language, typeof(string));
            }
            else
                columnValue = sourceTable.Columns[language];

            DataColumn columnTokens = sourceTable.Columns[tokensColumnName];

            int rowsCount = sourceTable.Rows.Count;

            foreach (var entry in data.TokenList)
            {
                //string expression = String.Format("token Like  {0}", entry.Index);
                DataRow row = sourceTable.Rows.Find(entry.Index);

                if (row == null)
                {
                    row = sourceTable.NewRow();
                    //row.BeginEdit();
                    row[columnValue] = entry.Text;
                    row[columnTokens] = entry.Index;
                    sourceTable.Rows.Add(row);
                    //row.EndEdit();
                }
                else
                {
                    row[columnValue] = entry.Text;
                }
            }
        }

        public override void ObtainModData(string basePath, bool isBase)
        {
            this.isBase = isBase;
            foreach (var dir in Tools.GetDirectoriesFromDirectory(Ship_Game.ResourceManager.WhichModPath + "/Localization/"))
            {
                var data = LoadLanguage(dir.FullName);
                if (data != null)
                {
                    if (!globalLanguages.Contains(dir.Name))
                        globalLanguages.Add(dir.Name);
                    ProcessLanguage(data, dir.Name, localStrings);
                    ProcessLanguage(data, dir.Name, globalStrings);
                }
            }
            // Add the rest language columns
            foreach (var language in globalLanguages)
            {
                if (!localStrings.Columns.Contains(language))
                {
                    DataColumn column = localStrings.Columns.Add(language, typeof(string));
                    column.DefaultValue = "";
                }
                /*
                if (!globalStrings.Columns.Contains(language))
                {
                    globalStrings.Columns.Add(language, typeof(string));
                }*/
            }
        }

        Ship_Game.LocalizationFile GenerateLocFile(DataColumn column)
        {
            Ship_Game.LocalizationFile dump = new Ship_Game.LocalizationFile();
            foreach (DataRow row in this.localStrings.Rows)
            {
                int token = (int)row[localColumnTokens];
                string text = row[column].ToString();
                dump.TokenList.Add(new Ship_Game.Token() { Index = token, Text = text });
            }
            return dump;
        }

        public override void PopulateModOverview(TreeNodeCollection root)
        {
            TreeNode groupNode = null;
            string folder = "Strings";
            if (root.ContainsKey(folder))
                groupNode = root[folder];
            else
            {
                groupNode = new TreeNode(folder);
                root.Add(groupNode);
            }
            groupNode.Tag = rootItem;
            rootItem.node = groupNode;
        }

        public override void SaveAll(string dir)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Ship_Game.LocalizationFile));
            foreach (DataColumn column in this.localStrings.Columns)
            {
                if (column.ColumnName.Equals("token"))
                    continue;
                string outputDir = dir + "/Localization/" + column.ColumnName;
                Directory.CreateDirectory(outputDir);
                Tools.SafeWriter writer = new Tools.SafeWriter(new FileInfo(outputDir + "/GameText.xml"));
                serializer.Serialize(writer.Stream, GenerateLocFile(column));
                writer.Finish();
            }
        }

        public override void UpdateItems()
        {
        }

        public static DataTable LoadDataTable(int token)
        {
            DataTable result = new DataTable();
            DataColumn column = result.Columns.Add("Language", typeof(string));
            column.ReadOnly = true;
            result.Columns.Add("Value", typeof(string));
            foreach (var language in globalLanguages)
            {
                var row = result.NewRow();
                row[0] = language;
                row[1] = GetLocString(token, language);
                result.Rows.Add(row);
            }
            return result;
        }

        public static bool TokenExists(int token)
        {
            return globalStrings.Rows.Find(token) != null;
        }

        public static void SetLocString(int token, string value)
        {
            SetLocString(token, value, defaultLanguage);
        }

        public static void SetLocString(int token, string value, string language)
        {
            if (lastController != null)
            {
                // 1. update local string
                DataColumn column = lastController.localStrings.Columns[language];
                DataColumn tokenColumn = lastController.localStrings.Columns[language];
                DataRow row = lastController.localStrings.Rows.Find(token);
                if (row != null)
                {
                    row[column] = value; ;
                }
                // 2. update global string
                    // global string is updated due to event attached to localStrings table
            }
        }
    }
}
