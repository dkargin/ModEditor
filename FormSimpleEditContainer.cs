using ModEditor.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ModEditor
{
    public partial class FormSimpleEditContainer : Form
    {
        private void FormSimpleEditContainer_Load(object sender, EventArgs e)
        {

        }

        class Record
        {
            public ListObjectAccessor primitiveAccessor;
            public TypeEditor editor;            
            public Control control;            
        }

        // cached controls for each list item
        List<Record> records = new List<Record>();
        List<ListItemCaption> captions = new List<ListItemCaption>();

        public System.Type targetType;
        public List<object> data = new List<object>();
        // Attributes used to override targetType. Mostly for overriding primitive types
        public List<ModEditorAttribute> attributes;

        protected Button button;
        
        public FormSimpleEditContainer(System.Type targetType, System.Collections.IList source, List<ModEditorAttribute> attributes)
        {
            InitializeComponent();

            this.targetType = targetType;
            this.attributes = attributes;
            this.contents.RowStyles.Clear();
            button = new Button()
            {
                Text = "New Item",
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top,
            };
            button.Click += btnAdd_Click;

            int count = 0;
            foreach(var obj in source)
            {
                InternalInsertItem(obj, count++);                
            }
            
            contents.GrowStyle = TableLayoutPanelGrowStyle.AddRows;
            contents.CellBorderStyle = TableLayoutPanelCellBorderStyle.None;
            contents.AutoScroll = true;
            //contents.AutoSize = true;
            
            /*
            this.contents.Controls.SetChildIndex(control, rowIndex * 2);
            this.contents.Controls.SetChildIndex(caption, rowIndex * 2);*/

            // Row for last button
            this.contents.RowStyles.Add(new RowStyle()
            {
                Height = 25,
                SizeType = SizeType.Absolute,
            });
            
            // Row for space filler
            /*
            this.contents.RowStyles.Add(new RowStyle()
            {
                //Height = rowHeight,
                SizeType = SizeType.AutoSize
            });
            */
            contents.Controls.Add(button);
            contents.SetColumnSpan(button, 2);
            //contents.AutoSize = true;

            if (this.contents.ColumnCount > 0)
            {
                this.contents.ColumnStyles[0].Width = firstColumnWidth;
                this.contents.ColumnStyles[0].SizeType = SizeType.Absolute;
                this.contents.ColumnStyles[1].SizeType = SizeType.AutoSize;
            }

            UpdateData();            
        }

        bool IsPrimitive()
        {
            return FieldEditorManager.IsPrimitiveType(targetType);
        }

        void InternalInsertItem(object obj, int index)
        {
            data.Insert(index, obj);
            Record record = new Record();

            {
                ListItemCaption newCaption = new ListItemCaption();
                int lastIndex = data.Count - 1;
                newCaption.itemName.Text = String.Format("Item {0}", lastIndex);
                newCaption.cmdCopy.Click += (object handler, EventArgs args) => { onCopyItem(lastIndex); };
                newCaption.cmdDelete.Click += (object handler, EventArgs args) => { onRemoveItem(lastIndex); };
                captions.Add(newCaption);
            }

            ListItemCaption caption = captions[index];

            if (IsPrimitive())
            {
                record.primitiveAccessor = new ListObjectAccessor(data, index, targetType, caption.itemName.Text, false, attributes);
            }

            records.Insert(index, record);

            contents.RowStyles.Insert(index, new RowStyle()
            {
                //Height = rowHeight,
                SizeType = SizeType.AutoSize
            });

            AddRow(obj, index);
            // Inserted object has index+1 position

            // Update indexes for each accessor. It is not neccesary for complex types
            for (int i = index + 1; i < data.Count; i++)
            {
                //records[i].caption.itemName.Text = String.Format("Item {0}", i);
                if (records[i].primitiveAccessor != null)
                {
                    records[i].primitiveAccessor.index = i;
                    records[i].primitiveAccessor.name = captions[i].itemName.Text;
                }
            }
        }

        int firstColumnWidth = 180;

        // This editor does not bother undo/redo, so we can use generic accessors 
        void AddRow(object obj, int rowIndex)
        {
            this.SuspendLayout();

            Control control = null;

            Record record = records[rowIndex];  // record should be created earlier
            // for primitive types we should access through list index. Caution, after add/remove index order can change            
            if (IsPrimitive())
            {
                record.editor = FieldEditorManager.GenerateEditor(record.primitiveAccessor);
                control = record.editor.GetControl();// record.editor.GetControl();
                control.Anchor = AnchorStyles.Left | AnchorStyles.Right;                
            }
            else
            {
                GenericFieldAccessor callback = new GenericFieldAccessor(obj, targetType, false);
                PropertyGridExplorer explorer = new PropertyGridExplorer();
                explorer.RowFiller = false;
                explorer.AutoSize = true;
                explorer.AutoScroll = false;
                explorer.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
                //explorer.Width = CustomList.Width;

                explorer.GrowStyle = TableLayoutPanelGrowStyle.AddRows;
                //explorer.Dock = DockStyle.Fill;
                explorer.Anchor = AnchorStyles.Left | AnchorStyles.Right;
                explorer.SetSource(this.targetType, callback);
                control = explorer;
            }

            Control oldControl = records[rowIndex].control;
            records[rowIndex].control = control;

            contents.Controls.Add(captions[captions.Count - 1], 0, captions.Count - 1);
            contents.Controls.Add(control, 1, rowIndex);
            
            this.contents.RowCount = this.contents.RowStyles.Count;
            // 2. Sync moved records: all indexes from [index, last] are changed, also should update all table rows
            contents.SetCellPosition(button, new TableLayoutPanelCellPosition(0, data.Count));
            contents.SetColumnSpan(button, 2);

            for (int i = data.Count - 1; i >= rowIndex; i--)
            {                
                contents.SetRow(captions[i], i);
                if (records[i].control != null)
                    contents.SetRow(records[i].control, i);
            }

            
            this.ResumeLayout();   
        }

        void UpdateData()
        {
            foreach (var rec in records)
            {
                if (rec.editor != null)
                    rec.editor.UpdateValue();
            }
        }

        void UpdateFooterPosition()
        {
        }
        
        /*
        // Completeyl rebuild list
        void RebuildList()
        {
            this.SuspendLayout();      
      
            

            //SyncRecords(data.Count);

            int rows = 0;
            foreach (var obj in data)
            {
                AddRow(obj, rows++);
            }

            // Setup row and column styles
            foreach (RowStyle row in this.contents.RowStyles)
            {
                //row.Height = rowHeight;
                row.SizeType = SizeType.AutoSize;
            }

            UpdateData();
            // Add last row            
            this.contents.RowCount = this.contents.RowStyles.Count;
            
            this.ResumeLayout();
        }*/

        private void FormEditContainer_Load(object sender, EventArgs e)
        {

        }

        public static void RemoveTableRow(TableLayoutPanel tableLayoutPanel, int rowNumber)
        {
            // remove all controls at specific row
            foreach (Control control in tableLayoutPanel.Controls)
            {
                int row = tableLayoutPanel.GetRow(control);
                if (row == rowNumber)
                {
                    tableLayoutPanel.Controls.Remove(control);
                }
            }

            tableLayoutPanel.RowStyles.RemoveAt(rowNumber);

            foreach (Control control in tableLayoutPanel.Controls)
            {
                int row = tableLayoutPanel.GetRow(control);
                if (row > rowNumber)
                {
                    tableLayoutPanel.SetRow(control, row - 1);
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                object item = FieldEditorManager.CreateObject(targetType);

                if (item != null)
                {
                    InternalInsertItem(item, data.Count);
                    //AddRow(item, data.Count - 1);
                    UpdateData();
                }
            }
            catch(Exception ex)
            {
                PanelErrors.LogErrorString(ex.Message);
            }                     
        }
        
        private void onCopyItem(int index)
        {
            object selected = data[index];
            object item = Tools.Clone(selected, targetType);
            if (item != null)
            {                 
                // 1. Add new item after cloned item
                InternalInsertItem(item, index);                              
                // 3. Add new row
                //AddRow(item, index);
                
                UpdateData();
            }            
        }

        private void onRemoveItem(int index)
        {            
            data.RemoveAt(index);
            Record record = records[index];
            
            contents.Controls.Remove(captions[captions.Count-1]);
            contents.Controls.Remove(record.control);

            records.RemoveAt(index);
            captions.RemoveAt(captions.Count - 1);
            // All data has moved one position up
            for (int i = data.Count - 1; i >= index; i--)
            {
                int localIndex = i;
                if (records[i].primitiveAccessor != null)
                {
                    records[i].primitiveAccessor.name = captions[i].itemName.Text;
                    records[i].primitiveAccessor.index = i;
                }
                contents.SetRow(captions[i], i);                
                contents.SetRow(records[i].control, i);
            }            
            
            contents.RowStyles.RemoveAt(index);
            contents.RowCount = contents.RowStyles.Count;
            //SyncRecords(data.Count);
            //UpdateData();
            //RebuildList();              
        }
    }
}
