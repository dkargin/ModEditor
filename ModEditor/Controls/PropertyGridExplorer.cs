using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ModEditor.Controls
{
    public partial class PropertyGridExplorer : TableLayoutPanel
    {
        private int rows = 0;
        //static int rowHeight = 25;

        public Dictionary<string, FieldEditor> editors = new Dictionary<string, FieldEditor>();

        protected void AddRow(string name, FieldEditor editor, Control control)
        {
            var label = new Label()
            {
                Text = name,
                TextAlign = ContentAlignment.MiddleLeft,
                //Dock= DockStyle.Fill, 
                AutoSize = true,
                BackColor = SystemColors.ControlLight,
            };
            control.Dock = DockStyle.Fill;
            this.Controls.Add(label, 0, rows);
            this.Controls.Add(control, 1, rows);
            /*
            dataTable.RowStyles[rows].Height = rowHeight;
            dataTable.RowStyles[rows].SizeType = SizeType.Absolute;*/
            editors.Add(name, editor);
            rows++;
        }

        protected FieldEditor GenerateFieldEditor(System.Reflection.FieldInfo fieldInfo, ModEditor.Item item)
        {
            foreach (ModEditorAttribute attrib in item.controller.GetFieldAttribute(fieldInfo))
            {
                // Attribute can return null - thats ok, meaning that this field should be skipped
                var editor = attrib.GenerateEditor(fieldInfo, item);
                return editor;
            }
            // No overriden editor was found - use default editors
            return generateControl(fieldInfo, item);
        }

        public void UpdateData()
        {
            foreach (var editor in editors)
            {
                editor.Value.UpdateValue();
            }
        }  

        public void SetSource(System.Type type, ModEditor.Item item)
        {
            foreach (var member in type.GetFields())
            {
                try
                {
                    //var value = member.GetValue(item.target);
                    //if (value != null)
                    {
                        FieldEditor editor = GenerateFieldEditor(member, item);

                        if (editor == null)
                            continue;
                        Control control = editor.GetControl();

                        //Control control = item.controller.GenerateControl(member, data);

                        if (control != null)
                            AddRow(member.Name, editor, control);
                    }
                }
                catch (Exception e)
                {
                    MainForm.LogString(e.Message);
                }
            }
        }

        static int preferedHeight = 17;
       
        static public Dictionary<System.Type, Func<System.Reflection.FieldInfo, ModEditor.Item, FieldEditor>> editorFactories = new Dictionary<System.Type, Func<System.Reflection.FieldInfo, ModEditor.Item, FieldEditor>>();

        /// <summary>
        /// Register editors for basic types
        /// </summary>
        static public void Init()
        {
            editorFactories.Add(typeof(string), (System.Reflection.FieldInfo field, ModEditor.Item obj) =>
            {
                return new FieldEditorString(field, obj);
            });

            editorFactories.Add(typeof(Single), (System.Reflection.FieldInfo field, ModEditor.Item obj) =>
            {
                return new FieldEditorFloat(field, obj);
            });

            editorFactories.Add(typeof(Int16), (System.Reflection.FieldInfo field, ModEditor.Item obj) =>
            {
                return new FieldEditorInt<Int16>(field, obj);
            });

            editorFactories.Add(typeof(Int32), (System.Reflection.FieldInfo field, ModEditor.Item obj) =>
            {
                return new FieldEditorInt<Int32>(field, obj);
            });

            editorFactories.Add(typeof(Byte), (System.Reflection.FieldInfo field, ModEditor.Item obj) =>
            {
                return new FieldEditorInt<Byte>(field, obj);
            });

            editorFactories.Add(typeof(Boolean), (System.Reflection.FieldInfo field, ModEditor.Item obj) =>
            {
                return new FieldEditorBool(field, obj);
            });
        }
        /// <summary>
        /// Generate appropriate controll for specified field
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        static public FieldEditor generateControl(System.Reflection.FieldInfo fieldInfo, ModEditor.Item item)
        {
            if (fieldInfo.FieldType.IsEnum)
            {
                return new FieldEditorEnum(fieldInfo, item);
            }

            Func<System.Reflection.FieldInfo, ModEditor.Item, FieldEditor> factory = null;
            if (editorFactories.ContainsKey(fieldInfo.FieldType))
                factory = editorFactories[fieldInfo.FieldType];
            else
                return new FieldEditorReadOnly(fieldInfo, item);

            return factory(fieldInfo, item);
        }

        

        public class FieldEditorFloat : FieldEditor
        {
            TextBox control;
            public FieldEditorFloat(System.Reflection.FieldInfo fieldInfo, ModEditor.Item item)
                : base(fieldInfo, item)
            {
                control = new TextBox();
                control.BorderStyle = BorderStyle.None;
                control.Height = preferedHeight;
                if (item.IsBase())
                    control.ReadOnly = true;
                control.Validating += control_Validating;
                control.TextChanged += control_TextChanged;
            }

            void control_TextChanged(object sender, EventArgs e)
            {
                Single value;
                if (Single.TryParse(control.Text, out value))
                {
                    WriteValue(value);
                }
            }

            void control_Validating(object sender, System.ComponentModel.CancelEventArgs e)
            {
                Single value;
                if (!Single.TryParse(control.Text, out value))
                {
                    e.Cancel = true;
                }
            }

            public override Control GetControl()
            {
                return control;
            }
            public override void UpdateValue()
            {
                control.Text = ReadValue().ToString();
            }
        }

        public class FieldEditorEnum : FieldEditor
        {
            ComboBox control;

            public FieldEditorEnum(System.Reflection.FieldInfo fieldInfo, ModEditor.Item item)
                : base(fieldInfo, item)
            {
                control = new ComboBox();
                //control.Bo = BorderStyle.None;
                control.Items.AddRange(Enum.GetNames(fieldInfo.FieldType));
                //control.DataSource = Enum.GetValues(fieldInfo.FieldType);
                control.DropDownStyle = ComboBoxStyle.DropDownList;
                control.SelectedIndexChanged += control_SelectedIndexChanged;
            }

            void control_SelectedIndexChanged(object sender, EventArgs e)
            {
                //throw new NotImplementedException();
                object value = Enum.Parse(FieldInfo.FieldType, control.SelectedItem.ToString());
                if (item.IsBase())
                    UpdateValue();
                else
                    WriteValue(value);
            }

            public override Control GetControl()
            {
                return control;
            }
            public override void UpdateValue()
            {
                object value = ReadValue();
                control.SelectedItem = value.ToString();
                //control.Text = fieldInfo.GetValue(item).ToString();
            }
        }
        /// <summary>
        /// Only show field .ToString contents
        /// </summary>
        public class FieldEditorReadOnly : FieldEditor
        {
            TextBox control;
            public FieldEditorReadOnly(System.Reflection.FieldInfo fieldInfo, ModEditor.Item item)
                : base(fieldInfo, item)
            {
                control = new TextBox();
                control.Height = preferedHeight;
                control.BorderStyle = BorderStyle.None;
                control.ReadOnly = true;
            }
            public override Control GetControl()
            {
                return control;
            }
            public override void UpdateValue()
            {
                object value = ReadValue();
                if (value != null)
                    control.Text = value.ToString();
                else
                    control.Text = "";
            }
        }
        /// <summary>
        /// Edit int field
        /// </summary>
        public class FieldEditorInt<TargetInt> : FieldEditor
        {
            NumericUpDown control;
            public FieldEditorInt(System.Reflection.FieldInfo fieldInfo, ModEditor.Item item)
                : base(fieldInfo, item)
            {
                control = new NumericUpDown();
                control.BorderStyle = BorderStyle.None;
                if (item.IsBase())
                    control.ReadOnly = true;
                control.ValueChanged += control_ValueChanged;

                SetLimits();
            }

            void SetLimits()
            {
                if (typeof(TargetInt).Equals(typeof(short)))
                {
                    control.Minimum = short.MinValue;
                    control.Maximum = short.MaxValue;
                }
                if (typeof(TargetInt).Equals(typeof(Byte)))
                {
                    control.Minimum = Byte.MinValue;
                    control.Maximum = Byte.MaxValue;
                }
                if (typeof(TargetInt).Equals(typeof(Int32)))
                {
                    control.Minimum = Int32.MinValue;
                    control.Maximum = Int32.MaxValue;
                }
            }

            void control_ValueChanged(object sender, EventArgs e)
            {
                try
                {
                    TargetInt value = (TargetInt)Convert.ChangeType(control.Value, typeof(TargetInt));
                    if (item.IsBase())
                        UpdateValue();
                    else
                        WriteValue(value);
                }
                catch (Exception)
                { }
            }

            public override Control GetControl()
            {
                return control;
            }
            public override void UpdateValue()
            {
                try
                {
                    object objValue = ReadValue();// fieldInfo.GetValue(item);
                    control.Value = (Decimal)Convert.ChangeType(objValue, typeof(Decimal)); //Convert.ToDecimal(objValue);
                }
                catch (Exception)
                { }
            }
        }

        public class FieldEditorBool : FieldEditor
        {
            CheckBox control;
            public FieldEditorBool(System.Reflection.FieldInfo fieldInfo, ModEditor.Item item)
                : base(fieldInfo, item)
            {
                control = new CheckBox();
                control.Height = preferedHeight;
                //
                control.CheckedChanged += control_ValueChanged;
                if (item.IsBase())
                    control.Enabled = false;
            }

            void control_ValueChanged(object sender, EventArgs e)
            {
                WriteValue(control.Checked);
            }

            public override Control GetControl()
            {
                return control;
            }
            public override void UpdateValue()
            {
                Boolean objValue = (Boolean)ReadValue();

                control.Checked = objValue;
            }
        }
        /// <summary>
        /// Edit string field
        /// </summary>
        public class FieldEditorString : FieldEditor
        {
            TextBox control;
            public FieldEditorString(System.Reflection.FieldInfo fieldInfo, ModEditor.Item item)
                : base(fieldInfo, item)
            {
                control = new TextBox();
                control.BorderStyle = BorderStyle.None;

                control.Height = preferedHeight;
                if (item.IsBase())
                    control.ReadOnly = true;
                control.TextChanged += (object sender, EventArgs e) =>
                {
                    WriteValue(control.Text);
                };
            }

            void result_ValueChanged(object sender, EventArgs e)
            {
                throw new NotImplementedException();
            }

            public override Control GetControl()
            {
                return control;
            }

            public override void UpdateValue()
            {
                object value = ReadValue();
                if (value != null)
                    control.Text = value.ToString();
                else
                    control.Text = "";
            }
        }

        // Field is represented by a string, containing name of referenced object.
        public class FieldEditorReference : FieldEditor
        {
            ReferenceEditor control;
            string groupName;
            public FieldEditorReference(System.Reflection.FieldInfo fieldInfo, ModEditor.Item item, string groupName)
                : base(fieldInfo, item)
            {
                control = new ReferenceEditor();
                control.pick.Click += pick_Click;
                this.groupName = groupName;
            }

            void pick_Click(object sender, EventArgs e)
            {
                string value = (string)ReadValue();                
                if(value == null)
                    value = "";

                FormSelect form = new FormSelect(false);
                Controller controller = item.controller.Mod.GetGroupController(groupName);
                if(controller == null)
                    return;
                var localItems = controller.GetLocalItems();

                form.AddItem(new FormSelect.Item()
                {
                    name="(Empty)",
                    fromBase = true,
                    fromCurrentMod = true,
                    fromMods = true
                });
                
                foreach(var rec in controller.GetItems())
                {
                    form.AddItem(new FormSelect.Item()
                    {
                        name = rec.Key,
                        fromBase = rec.Value.IsBase(),
                        fromCurrentMod = (localItems.ContainsKey(rec.Key)),
                        selected = (rec.Key == value)
                    });
                }

                if (form.ShowDialog() == DialogResult.OK)
                {
                    FormSelect.Item selected = form.selected;
                    if (selected != null)
                    {
                        if (selected.name.Equals("(Empty)"))
                            WriteValue("");
                        WriteValue(selected.name);
                        UpdateValue();
                    }
                }
            }

            public override Control GetControl()
            {
                return control;
            }
            bool empty = false;
            public override void UpdateValue()
            {
                string value = (string)ReadValue();
                if (value == null || value.Length == 0)
                {
                    empty = true;
                    control.refName.Text = "(Empty)";
                }
                else
                {
                    empty = false;
                    control.refName.Text = value;
                }
            }
        }

        // Represented by int token, index of string in localization dictionary
        public class FieldEditorLocString : FieldEditor
        {
            StringEditor control;
            public FieldEditorLocString(System.Reflection.FieldInfo fieldInfo, ModEditor.Item item)
                : base(fieldInfo, item)
            {
                control = new StringEditor();

                control.EditButton.Click += EditButton_Click;
                control.TokenBox.Validating += control_Validating;
                control.TokenBox.TextChanged += control_TextChanged;
                control.ValueBox.TextChanged += ValueBox_TextChanged;

                /*
                if (item.IsBase())
                    control.ReadOnly = true;
                control.TextChanged += (object sender, EventArgs e) =>
                {
                    WriteValue(control.Text);
                };*/
            }

            void ValueBox_TextChanged(object sender, EventArgs e)
            {
                if (GetToken() != 0)
                    Controllers.StringsController.SetLocString(GetToken(), control.ValueBox.Text);
            }

            int GetToken()
            {
                object value = ReadValue();
                try
                {
                    int index = (int)Convert.ChangeType(value, typeof(int));
                    return index;
                }
                catch (Exception e)
                {
                    MainForm.LogString(e.Message);
                }
                return 0;
            }
            void EditButton_Click(object sender, EventArgs e)
            {
                FormEditString form = new FormEditString(GetToken());
                if (form.ShowDialog() == DialogResult.OK)
                {
                    WriteValue(form.token);
                    UpdateValue();
                }
            }

            void result_ValueChanged(object sender, EventArgs e)
            {
                throw new NotImplementedException();
            }

            public override Control GetControl()
            {
                return control;
            }

            void control_Validating(object sender, System.ComponentModel.CancelEventArgs e)
            {
                int value;
                if (!int.TryParse(control.Text, out value))
                {
                    e.Cancel = true;
                }
            }

            void control_TextChanged(object sender, EventArgs e)
            {
                int value;
                if (int.TryParse(control.Text, out value))
                {
                    WriteValue(value);
                }
            }

            public override void UpdateValue()
            {
                int index = GetToken();
                control.TokenBox.Text = index.ToString();
                control.ValueBox.Text = ModContents.GetLocString(index);
            }
        }
    }

    public class FieldEditor
    {
        System.Reflection.FieldInfo fieldInfo;
        public ModEditor.Item item;

        public FieldEditor(System.Reflection.FieldInfo field, ModEditor.Item item)
        {
            fieldInfo = field;
            this.item = item;
        }

        public virtual Control GetControl()
        {
            return null;
        }

        /// <summary>
        ///  write value from object to editor
        /// </summary>
        public virtual void UpdateValue()
        {
        }

        public System.Reflection.FieldInfo FieldInfo
        {
            get
            {
                return fieldInfo;
            }
        }
        protected object ReadValue()
        {
            return ModContents.ReadItemValue(item, fieldInfo);
        }

        protected void WriteValue(object value)
        {
            ModContents.ChangeItemValue(item, fieldInfo, value);
        }
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class ModEditorAttribute : Attribute
    {
        public virtual FieldEditor GenerateEditor(System.Reflection.FieldInfo fieldInfo, ModEditor.Item item)
        {
            return null;
        }
    }

    public class IgnoreByEditor : ModEditorAttribute
    {
        public override FieldEditor GenerateEditor(System.Reflection.FieldInfo fieldInfo, ModEditor.Item item)
        {
            return null;
        }
    }

    public class LocStringToken : ModEditorAttribute
    {
        public override FieldEditor GenerateEditor(System.Reflection.FieldInfo fieldInfo, ModEditor.Item item)
        {
            return new PropertyGridExplorer.FieldEditorLocString(fieldInfo, item);
        }
    }


    public class ObjectReference : ModEditorAttribute
    {
        public string groupName;
        public ObjectReference(string group)
        {
            groupName = group;
        }
        public override FieldEditor GenerateEditor(System.Reflection.FieldInfo fieldInfo, ModEditor.Item item)
        {
            return new PropertyGridExplorer.FieldEditorReference(fieldInfo, item, groupName);
        }
    }
}
