﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ModEditor.Controls
{
    public class EditorManager
    {
        static int preferedHeight = 17;
        [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
        public class ModEditorAttribute : Attribute
        {
            public virtual ModEditor.Controls.EditorManager.FieldEditor GenerateEditor(System.Reflection.FieldInfo fieldInfo, ModContents.Item item)
            {
                return null;
            }
        }

        public class LocStringToken : ModEditorAttribute
        {
            public override ModEditor.Controls.EditorManager.FieldEditor GenerateEditor(System.Reflection.FieldInfo fieldInfo, ModContents.Item item)
            {
                return new FieldEditorLocString(fieldInfo, item);
            }
        }


        public class ObjectReference : ModEditorAttribute
        {
            public string groupName;
            public ObjectReference(string group)
            {
                groupName = group;
            }
            public override ModEditor.Controls.EditorManager.FieldEditor GenerateEditor(System.Reflection.FieldInfo fieldInfo, ModContents.Item item)
            {
                return null;
            }
        }

        static public Dictionary<System.Type, Func<System.Reflection.FieldInfo, ModContents.Item, FieldEditor>> editors = new Dictionary<System.Type, Func<System.Reflection.FieldInfo, ModContents.Item, FieldEditor>>();

        /// <summary>
        /// Register editors for basic types
        /// </summary>
        static public void Init()
        {
            editors.Add(typeof(string), (System.Reflection.FieldInfo field, ModContents.Item obj) =>
            {
                return new FieldEditorString(field, obj);
            });

            editors.Add(typeof(Single), (System.Reflection.FieldInfo field, ModContents.Item obj) =>
            {
                return new FieldEditorFloat(field, obj);
            });

            editors.Add(typeof(Int16), (System.Reflection.FieldInfo field, ModContents.Item obj) =>
            {
                return new FieldEditorInt<Int16>(field, obj);
            });

            editors.Add(typeof(Int32), (System.Reflection.FieldInfo field, ModContents.Item obj) =>
            {
                return new FieldEditorInt<Int32>(field, obj);
            });

            editors.Add(typeof(Byte), (System.Reflection.FieldInfo field, ModContents.Item obj) =>
            {
                return new FieldEditorInt<Byte>(field, obj);
            });

            editors.Add(typeof(Boolean), (System.Reflection.FieldInfo field, ModContents.Item obj) =>
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
        static public FieldEditor generateControl(System.Reflection.FieldInfo fieldInfo, ModContents.Item item)
        {            
            if (fieldInfo.FieldType.IsEnum)
            {
                return new FieldEditorEnum(fieldInfo, item);
            }

            Func<System.Reflection.FieldInfo, ModContents.Item, FieldEditor> factory = null;
            if(editors.ContainsKey(fieldInfo.FieldType))
                factory = editors[fieldInfo.FieldType];
            else
                return new FieldEditorReadOnly(fieldInfo, item);

            return factory(fieldInfo, item);
        }

        public class FieldEditor
        {
            System.Reflection.FieldInfo fieldInfo;
            public ModContents.Item item;

            public FieldEditor(System.Reflection.FieldInfo field, ModContents.Item item)
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

        public class FieldEditorFloat : FieldEditor
        {
            TextBox control;
            public FieldEditorFloat(System.Reflection.FieldInfo fieldInfo, ModContents.Item item)
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
                control.Text =  ReadValue().ToString();
            }
        }

        public class FieldEditorEnum : FieldEditor
        {
            ComboBox control;

            public FieldEditorEnum(System.Reflection.FieldInfo fieldInfo, ModContents.Item item)
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
            public FieldEditorReadOnly(System.Reflection.FieldInfo fieldInfo, ModContents.Item item)
                :base(fieldInfo, item)
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
                control.Text = ReadValue().ToString();
            }
        }
        /// <summary>
        /// Edit int field
        /// </summary>
        public class FieldEditorInt<TargetInt> : FieldEditor
        {
            NumericUpDown control;
            public FieldEditorInt(System.Reflection.FieldInfo fieldInfo, ModContents.Item item)
                :base(fieldInfo, item)
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
            public FieldEditorBool(System.Reflection.FieldInfo fieldInfo, ModContents.Item item)
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
            public FieldEditorString(System.Reflection.FieldInfo fieldInfo, ModContents.Item item)
                :base(fieldInfo, item)
            {
                control = new TextBox();
                control.BorderStyle = BorderStyle.None;

                control.Height = preferedHeight;
                if(item.IsBase())
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
                control.Text = (string)ReadValue();
            }
        }

        public class FieldEditorLocString : FieldEditor
        {
            StringEditor control;            
            public FieldEditorLocString(System.Reflection.FieldInfo fieldInfo, ModContents.Item item)
                : base(fieldInfo, item)
            {
                control = new StringEditor();
                
                /*
                if (item.IsBase())
                    control.ReadOnly = true;
                control.TextChanged += (object sender, EventArgs e) =>
                {
                    WriteValue(control.Text);
                };*/
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
                int index = (int)value;
                control.TokenBox.Text = index.ToString();
                control.ValueBox.Text = ModContents.GetLocString(index) ;
            }
        }
    }
}
