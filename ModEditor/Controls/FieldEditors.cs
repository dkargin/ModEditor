using System;
using System.Collections;
using System.Windows.Forms;
namespace ModEditor
{
    public interface FieldCallback
    {
        // get overrides for specific field
        System.Type GetTargetType();
        //List<ModEditorAttribute> GetAttributes(System.Reflection.FieldInfo fieldInfo);
        // if value is locked
        bool ReadOnly();
        // obtain value
        object ReadValue(System.Reflection.FieldInfo fieldInfo);
        // set new value
        void WriteValue(System.Reflection.FieldInfo fieldInfo, object value);
    }

    #region Attributes
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class ModEditorAttribute : Attribute
    {
        public virtual FieldEditor GenerateEditor(System.Reflection.FieldInfo fieldInfo, FieldCallback item)
        {
            return null;
        }
    }

    public class IgnoreByEditor : ModEditorAttribute
    {
        public override FieldEditor GenerateEditor(System.Reflection.FieldInfo fieldInfo, FieldCallback item)
        {
            return null;
        }
    }

    public class LocStringToken : ModEditorAttribute
    {
        public override FieldEditor GenerateEditor(System.Reflection.FieldInfo fieldInfo, FieldCallback item)
        {
            return new FieldEditorLocString(fieldInfo, item);
        }
    }
    
    public class ObjectReference : ModEditorAttribute
    {
        public string groupName;
        public bool allowEmpty;
        public ObjectReference(string group, bool allowEmpty)
        {
            this.groupName = group;
            this.allowEmpty = allowEmpty;
        }
        public override FieldEditor GenerateEditor(System.Reflection.FieldInfo fieldInfo, FieldCallback item)
        {
            return new FieldEditorReference(fieldInfo, item, groupName, allowEmpty);
        }
    }
    #endregion

    public class FieldEditor
    {
        System.Reflection.FieldInfo fieldInfo;
        public FieldCallback callback;

        public FieldEditor(System.Reflection.FieldInfo field, FieldCallback callback)
        {
            fieldInfo = field;
            this.callback = callback;
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
            return callback.ReadValue(fieldInfo);
        }

        protected void WriteValue(object value)
        {
            callback.WriteValue(fieldInfo, value);
        }
    }

    public class FieldEditorFloat : FieldEditor
    {
        TextBox control;
        public FieldEditorFloat(System.Reflection.FieldInfo fieldInfo, FieldCallback item)
            : base(fieldInfo, item)
        {
            control = new TextBox();
            control.BorderStyle = BorderStyle.None;
            //control.Height = preferedHeight;
            if (item.ReadOnly())
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

        public FieldEditorEnum(System.Reflection.FieldInfo fieldInfo, FieldCallback item)
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
            if (callback.ReadOnly())
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
        public FieldEditorReadOnly(System.Reflection.FieldInfo fieldInfo, FieldCallback item)
            : base(fieldInfo, item)
        {
            control = new TextBox();
            //control.Height = preferedHeight;
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
        public FieldEditorInt(System.Reflection.FieldInfo fieldInfo, FieldCallback item)
            : base(fieldInfo, item)
        {
            control = new NumericUpDown();
            control.BorderStyle = BorderStyle.None;
            if (item.ReadOnly())
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
                if (callback.ReadOnly())
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
        public FieldEditorBool(System.Reflection.FieldInfo fieldInfo, FieldCallback item)
            : base(fieldInfo, item)
        {
            control = new CheckBox();
            //control.Height = preferedHeight;
            //
            control.CheckedChanged += control_ValueChanged;
            if (item.ReadOnly())
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
        public FieldEditorString(System.Reflection.FieldInfo fieldInfo, FieldCallback item)
            : base(fieldInfo, item)
        {
            control = new TextBox();
            control.BorderStyle = BorderStyle.None;

            //control.Height = preferedHeight;
            if (item.ReadOnly())
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

    public class FieldEditorList : FieldEditor
    {
        Type targetType;
        Button control;
        public FieldEditorList(System.Reflection.FieldInfo fieldInfo, FieldCallback item, Type type)
            : base(fieldInfo, item)
        {
            this.targetType = type;
            control = new Button();
            control.Text = "Collection";
            control.Click += control_Click;
        }

        void control_Click(object sender, EventArgs e)
        {            
            IList source = ReadValue() as IList;
            if (source == null)
                return;

            FormEditContainer form = new FormEditContainer(targetType, source);
            form.Text = String.Format("Edit {0}", this.FieldInfo.Name);

            if (form.ShowDialog() == DialogResult.OK)
            {
                source.Clear();
                foreach (var item in form.data)
                {
                    object converted = Convert.ChangeType(item, targetType);                    
                    if (converted != null)
                    {
                        source.Add(converted);
                    }
                }
            }
        }

        public override Control GetControl()
        {
            return control;
        }
    }

    // Field is represented by a string, containing name of referenced object.
    public class FieldEditorReference : FieldEditor
    {
        ModEditor.Controls.ReferenceEditor control;
        string groupName;
        bool allowEmpty;
        public FieldEditorReference(System.Reflection.FieldInfo fieldInfo, FieldCallback item, string groupName, bool allowEmpty)
            : base(fieldInfo, item)
        {
            control = new ModEditor.Controls.ReferenceEditor();
            //control.MaximumSize = new Size(0, rowHeight);
            control.pick.Click += pick_Click;
            this.groupName = groupName;
            this.allowEmpty = allowEmpty;
        }

        void pick_Click(object sender, EventArgs e)
        {
            string value = (string)ReadValue();
            if (value == null)
                value = "";

            FormSelect form = new FormSelect(false);

            ModContents mod = ModContents.GetMod();
            if (mod == null)
                return;

            Controller controller = mod.GetGroupController(groupName);
            if (controller == null)
                return;
            var localItems = controller.GetLocalItems();

            form.AddItem(new FormSelect.Item()
            {
                name = "(Empty)",
                fromBase = true,
                fromCurrentMod = true,
                fromMods = true,
                selected = empty,
            });

            foreach (var rec in controller.GetItems())
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
        ModEditor.Controls.StringEditor control;
        public FieldEditorLocString(System.Reflection.FieldInfo fieldInfo, FieldCallback item)
            : base(fieldInfo, item)
        {
            control = new ModEditor.Controls.StringEditor();

            control.EditButton.Click += EditButton_Click;
            control.TokenBox.Validating += control_Validating;
            control.TokenBox.TextChanged += control_TextChanged;
            control.ValueBox.TextChanged += ValueBox_TextChanged;

            //control.MaximumSize = new Size(0, rowHeight);// = rowHeight;

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
                MainForm.LogErrorString(e.Message);
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