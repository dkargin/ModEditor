using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ModEditor.Controls
{
    public class EditorManager
    {
        static public Dictionary<System.Type, Func<System.Reflection.FieldInfo, Object, FieldEditor>> editors = new Dictionary<System.Type, Func<System.Reflection.FieldInfo, object, FieldEditor>>();

        static public void Init()
        {            
            editors.Add(typeof(string), (System.Reflection.FieldInfo field, Object obj) =>
            {
                return new FieldEditorString(field, obj);
            });

            editors.Add(typeof(Single), (System.Reflection.FieldInfo field, Object obj) =>
            {
                return new FieldEditorFloat(field, obj);
            });

            editors.Add(typeof(Int16), (System.Reflection.FieldInfo field, Object obj) =>
            {
                return new FieldEditorInt<Int16>(field, obj);
            });            

            editors.Add(typeof(Int32), (System.Reflection.FieldInfo field, Object obj) =>
            {
                return new FieldEditorInt<Int32>(field, obj);
            });

            editors.Add(typeof(Byte), (System.Reflection.FieldInfo field, Object obj) =>
            {
                return new FieldEditorInt<Byte>(field, obj);
            });

            editors.Add(typeof(Boolean), (System.Reflection.FieldInfo field, Object obj) =>
            {
                return new FieldEditorBool(field, obj);
            });
        }

        static public FieldEditor generateControl(System.Reflection.FieldInfo fieldInfo, Object item)
        {            
            if (fieldInfo.FieldType.IsEnum)
            {
                return new FieldEditorEnum(fieldInfo, item);
            }
            
            Func<System.Reflection.FieldInfo, Object, FieldEditor> factory = null;
            try
            {
                factory = editors[fieldInfo.FieldType];
            }
            catch (Exception ex)
            {
                return new FieldEditorReadOnly(fieldInfo, item);
            }

            return factory(fieldInfo, item);
        }

        public class FieldEditor
        {
            public System.Reflection.FieldInfo fieldInfo;
            public Object item;

            public FieldEditor(System.Reflection.FieldInfo field, Object item)
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
        }

        public class FieldEditorFloat : FieldEditor
        {
            TextBox control;
            public FieldEditorFloat(System.Reflection.FieldInfo fieldInfo, Object item)
                : base(fieldInfo, item)
            {
                control = new TextBox();
                control.Validating += control_Validating;
                control.TextChanged += control_TextChanged;
            }

            void control_TextChanged(object sender, EventArgs e)
            {
                Single value;
                if (Single.TryParse(control.Text, out value))
                {
                    fieldInfo.SetValue(item, value);
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
                control.Text = fieldInfo.GetValue(item).ToString();
            }
        }

        public class FieldEditorEnum : FieldEditor
        {
            ComboBox control;
            
            public FieldEditorEnum(System.Reflection.FieldInfo fieldInfo, Object item)
                : base(fieldInfo, item)
            {
                control = new ComboBox();
                control.Items.AddRange(Enum.GetNames(fieldInfo.FieldType));
                //control.DataSource = Enum.GetValues(fieldInfo.FieldType);
                control.DropDownStyle = ComboBoxStyle.DropDownList;
                control.SelectedIndexChanged += control_SelectedIndexChanged;
            }

            void control_SelectedIndexChanged(object sender, EventArgs e)
            {
                //throw new NotImplementedException();
                object value = Enum.Parse(fieldInfo.FieldType, control.SelectedItem.ToString());
                fieldInfo.SetValue(item, value);
            }

            public override Control GetControl()
            {
                return control;
            }
            public override void UpdateValue()
            {
                object value = fieldInfo.GetValue(item);
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
            public FieldEditorReadOnly(System.Reflection.FieldInfo fieldInfo, Object item)
                :base(fieldInfo, item)
            {
                control = new TextBox();
                control.ReadOnly = true;
            }
            public override Control GetControl()
            {
                return control;
            }
            public override void UpdateValue()
            {
                control.Text = fieldInfo.GetValue(item).ToString();
            }
        }
        /// <summary>
        /// Edit int field
        /// </summary>
        public class FieldEditorInt<TargetInt> : FieldEditor
        {
            NumericUpDown control;
            public FieldEditorInt(System.Reflection.FieldInfo fieldInfo, Object item)
                :base(fieldInfo, item)
            {
                control = new NumericUpDown();
                control.ValueChanged += control_ValueChanged;

                SetLimits();
            }

            void SetLimits()
            {
                if(typeof(TargetInt).Equals(typeof(short)))
                {
                    control.Minimum = short.MinValue;
                    control.Maximum = short.MaxValue;
                }
                if(typeof(TargetInt).Equals(typeof(Byte)))
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
                try{
                    TargetInt value = (TargetInt)Convert.ChangeType(control.Value, typeof(TargetInt));
                    fieldInfo.SetValue(item, value);
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
                    object objValue = fieldInfo.GetValue(item);
                    control.Value = (Decimal)Convert.ChangeType(objValue, typeof(Decimal)); //Convert.ToDecimal(objValue);
                }
                catch (Exception)
                { }
            }
        }

        public class FieldEditorBool : FieldEditor
        {
            CheckBox control;
            public FieldEditorBool(System.Reflection.FieldInfo fieldInfo, Object item)
                : base(fieldInfo, item)
            {
                control = new CheckBox();
                control.CheckedChanged += control_ValueChanged;
            }

            void control_ValueChanged(object sender, EventArgs e)
            {                
                fieldInfo.SetValue(item, control.Checked);
            }

            public override Control GetControl()
            {
                return control;
            }
            public override void UpdateValue()
            {
                Boolean objValue = (Boolean)fieldInfo.GetValue(item);

                control.Checked = objValue;
            }
        }
        /// <summary>
        /// Edit string field
        /// </summary>
        public class FieldEditorString : FieldEditor
        {         
            TextBox control;
            public FieldEditorString(System.Reflection.FieldInfo fieldInfo, Object item)
                :base(fieldInfo, item)
            {
                control = new TextBox();
                //control.ReadOnly = true;
                control.TextChanged += (object sender, EventArgs e) =>
                {
                    fieldInfo.SetValue(item, control.Text);
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
                control.Text = (string)fieldInfo.GetValue(item);
            }
        }
    }
}
