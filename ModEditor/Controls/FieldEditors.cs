using ModEditor.Controllers;
using ModEditor.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
namespace ModEditor
{
    // Used by any TypeEditor to access assigned object
    public interface ObjectAccessor
    {
        System.Type GetTargetType();

        // get field name ( or anything simular)
        string GetName();

        bool ReadOnly();
        // obtain value
        object ReadValue();
        // set new value
        void WriteValue(object value);

        List<ModEditorAttribute> GetAttributes();
    }
    
    public interface FieldAccessor
    {
        System.Type GetTargetType();
        // get overrides for specific field
        //List<ModEditorAttribute> GetAttributes(System.Reflection.FieldInfo fieldInfo);
        // if value is locked
        bool ReadOnly();
        // obtain value
        object ReadValue(System.Reflection.FieldInfo fieldInfo);
        // set new value
        void WriteValue(System.Reflection.FieldInfo fieldInfo, object value);
    }

    public class ListObjectAccessor : ModEditor.ObjectAccessor
    {
        public int index;
        public string name;

        IList list;
        Type targetType;
        bool readOnly;
        
        List<ModEditorAttribute> attributes;

        public ListObjectAccessor(IList list, int index, Type targetType, string name, bool readOnly, List<ModEditorAttribute> attrib)
        {
            this.list = list;
            this.index = index;
            this.targetType = targetType;
            this.readOnly = readOnly;
            this.name = name;
            this.attributes = attrib;
        }

        public bool ReadOnly()
        {
            return readOnly;
        }

        public System.Type GetTargetType()
        {
            return targetType;
        }

        // get field name ( or anything simular)
        public string GetName()
        {
            return name;
        }

        // obtain value
        public object ReadValue()
        {
            return list[index];
        }

        // set new value
        public void WriteValue(object value)
        {
            list[index] = value;
        }

        public List<ModEditorAttribute> GetAttributes()
        {
            return attributes;
        }
    }

    public class GenericObjectAccessor : ModEditor.ObjectAccessor
    {
        Object data;
        Type targetType;
        bool readOnly;
        string name;
        List<ModEditorAttribute> attributes;

        public GenericObjectAccessor(Object data, Type targetType, string name, bool readOnly, List<ModEditorAttribute> attrib)
        {
            this.data = data;
            this.targetType = targetType;
            this.readOnly = readOnly;
            this.name = name;
            this.attributes = attrib;
        }

        public bool ReadOnly()
        {
            return readOnly;
        }

        public System.Type GetTargetType()
        {
            return targetType;
        }

        // get field name ( or anything simular)
        public string GetName()
        {
            return name;
        }

        // obtain value
        public object ReadValue()
        {
            return data;
        }

        // set new value
        public void WriteValue(object value)
        {
            data = value;
        }

        public List<ModEditorAttribute> GetAttributes()
        {
            return attributes;
        }
    }
    // Accessor for specific field from other object
    public class ObjectFieldAccessor : ModEditor.ObjectAccessor
    {
        public ModEditor.FieldAccessor parent;
        public System.Reflection.FieldInfo field;

        public ObjectFieldAccessor(ModEditor.FieldAccessor parent, System.Reflection.FieldInfo field)
        {
            this.parent = parent;
            this.field = field;
        }

        public bool ReadOnly()
        {
            return parent.ReadOnly();
        }

        public System.Type GetTargetType()
        {
            return field.FieldType;
        }

        // get field name ( or anything simular)
        public string GetName()
        {
            return field.Name;
        }

        // obtain value
        public object ReadValue()
        {
            return parent.ReadValue(field);
        }

        // set new value
        public void WriteValue(object value)
        {
            parent.WriteValue(field, value);
        }

        public List<ModEditorAttribute> GetAttributes()
        {
            return FieldEditorManager.GetAttributes(field, parent.GetTargetType());
        }
    }

    // Plain accessor without overloading Read/Write methods
    public class GenericFieldAccessor : ModEditor.FieldAccessor
    {
        object item;
        bool readOnly;
        System.Type targetType;

        public GenericFieldAccessor(object item, System.Type type, bool readOnly)
        {
            this.item = item;
            this.readOnly = readOnly;
            this.targetType = type;
        }

        public bool ReadOnly()
        {
            return readOnly;
        }

        public System.Type GetTargetType()
        {
            return targetType;
        }

        public object ReadValue(System.Reflection.FieldInfo field)
        {
            return field.GetValue(item);
        }

        public void WriteValue(System.Reflection.FieldInfo field, object value)
        {
            field.SetValue(item, value);
        }
    }

    // Report for item problems
    public class ItemReport
    {
        public Item item; 
        public string path; /// path to specific field

        /// check if the same error persists
        public virtual bool Check()
        {
            return false;
        }

        public virtual string Message()
        {
            return "Something is wrong";
        }
    }

    public class ItemReport_WrongReferenceField : ItemReport
    {
        //public object storage;
        public string value;
        public string group;

        public virtual string GetReference()
        {
            return value;
        }

        public override bool Check()
        {
            return base.Check();
        }

        public override string Message()
        {
            return String.Format("'{0}' is invalid name for group '{1}'", value, group);
        }
    }

    #region Attributes
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class ModEditorAttribute : Attribute
    {
        public virtual TypeEditor GenerateEditor(ObjectAccessor item)
        {
            return null;
        }
    }

    public class IgnoreByEditor : ModEditorAttribute
    {
        public override TypeEditor GenerateEditor(ObjectAccessor item)
        {
            return null;
        }
    }

    public class LocStringToken : ModEditorAttribute
    {
        public override TypeEditor GenerateEditor(ObjectAccessor item)
        {
            return new FieldEditorLocString(item);
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
        public override TypeEditor GenerateEditor(ObjectAccessor item)
        {
            return new FieldEditorReference(item, groupName, allowEmpty);
        }
    }
    #endregion

    public class TypeEditor
    {        
        public ObjectAccessor callback;

        public TypeEditor(ObjectAccessor callback)
        {
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

        public Type TargetType
        {
            get
            {
                return callback.GetTargetType();
            }
        }

        protected object ReadValue()
        {
            return callback.ReadValue();
        }

        protected void WriteValue(object value)
        {
            callback.WriteValue(value);
        }
    }

    public class FieldEditorFloat : TypeEditor
    {
        TextBox control;
        public FieldEditorFloat(ObjectAccessor item)
            : base(item)
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

    public class FieldEditorEnum : TypeEditor
    {
        ComboBox control;

        public FieldEditorEnum(ObjectAccessor item)
            : base(item)
        {
            control = new ComboBox();
            //control.Bo = BorderStyle.None;
            control.Items.AddRange(Enum.GetNames(TargetType));
            //control.DataSource = Enum.GetValues(fieldInfo.FieldType);
            control.DropDownStyle = ComboBoxStyle.DropDownList;
            control.SelectedIndexChanged += control_SelectedIndexChanged;
        }

        void control_SelectedIndexChanged(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            object value = Enum.Parse(TargetType, control.SelectedItem.ToString());
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
    public class FieldEditorReadOnly : TypeEditor
    {
        TextBox control;
        public FieldEditorReadOnly(ObjectAccessor item)
            : base(item)
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
    public class FieldEditorInt<TargetInt> : TypeEditor
    {
        NumericUpDown control;
        public FieldEditorInt(ObjectAccessor item)
            : base(item)
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

    public class FieldEditorBool : TypeEditor
    {
        CheckBox control;
        public FieldEditorBool(ObjectAccessor item)
            : base(item)
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
    public class FieldEditorString : TypeEditor
    {
        TextBox control;
        public FieldEditorString(ObjectAccessor item)
            : base(item)
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

    public class FieldEditorList : TypeEditor
    {
        Type targetType;
        Button control;
        public FieldEditorList(ObjectAccessor item, Type type)
            : base(item)
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

            //FormEditContainer form = new FormEditContainer(targetType, source, callback.GetAttributes());
            FormSimpleEditContainer form = new FormSimpleEditContainer(targetType, source, callback.GetAttributes());

            form.Text = String.Format("Edit {0}", callback.GetName());

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
    public class FieldEditorReference : TypeEditor
    {
        ModEditor.Controls.ReferenceEditor control;
        string groupName;
        bool allowEmpty;
        public FieldEditorReference(ObjectAccessor item, string groupName, bool allowEmpty)
            : base(item)
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
    public class FieldEditorLocString : TypeEditor
    {
        ModEditor.Controls.StringEditor control;
        public FieldEditorLocString(ObjectAccessor item)
            : base(item)
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
                PanelErrors.LogErrorString(e.Message);
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

    public class FieldEditorManager
    {
        static Dictionary<System.Type, FieldOverrides> typeTable = new Dictionary<Type, FieldOverrides>();

        static public FieldOverrides GetFieldOverrides(Type type)
        {
            FieldOverrides overrides = null;
            if (typeTable.ContainsKey(type))
                overrides = typeTable[type];
            else
            {
                overrides = new FieldOverrides(type);
                typeTable.Add(type, overrides);
            }
            return overrides;
        }

        static public bool HasAttributeOverrides(Type type)
        {
            if (typeTable.ContainsKey(type))
                return typeTable[type].HasOverrides();
            return false;
        }

        static public void AddOverridedAttribute(Type type, string fieldName, Attribute attrib)
        {
            GetFieldOverrides(type).AddAttribute(fieldName, attrib);
        }

        static public List<ModEditorAttribute> GetAttributes(System.Reflection.FieldInfo fieldInfo, Type ownerType)
        {
            List<ModEditorAttribute> result = new List<ModEditorAttribute>();

            if (FieldEditorManager.HasAttributeOverrides(ownerType))
            {
                var overrides = FieldEditorManager.GetFieldOverrides(ownerType);
                var list = overrides.GetFieldAttribute(fieldInfo);
                // Get attributes from local override list
                foreach (ModEditorAttribute attribute in overrides.GetFieldAttribute(fieldInfo))
                    result.Add(attribute);
            }

            // Get attributes from class definition                
            foreach (ModEditorAttribute attribute in fieldInfo.GetCustomAttributes(true))
                result.Add(attribute);

            return result;
        }

        static public bool IsPrimitiveType(System.Type type)
        {
            if (type.IsEnum)
            {
                return true;
            }

            if (type.IsGenericType)
                return false;

            if (editorFactories.ContainsKey(type))
                return true;
            return false;
        }

        static public TypeEditor GenerateEditor(ObjectAccessor callback)
        {
            // No overriden editor was found - use default editors

            Type type = callback.GetTargetType();
            if (type.IsEnum)
            {
                return new FieldEditorEnum(callback);
            }

            if (type.IsGenericType)
            {
                // If list
                
                if (type.Name.Equals("List`1"))
                {
                    Type storedType = type.GetGenericArguments()[0];
                    // check custom attributes for list
                    return new FieldEditorList(callback, storedType);
                }
            }

            foreach (ModEditorAttribute attrib in callback.GetAttributes()/*GetAttributes(fieldInfo, callback.GetTargetType())*/)
            {
                // Attribute can return null - thats ok, meaning that this field should be skipped
                var editor = attrib.GenerateEditor(callback);
                return editor;
            }

            Func<ObjectAccessor, TypeEditor> factory = null;
            if (editorFactories.ContainsKey(type))
                factory = editorFactories[type].factory;
            else
                return new FieldEditorReadOnly(callback);

            return factory(callback);
        }

        class TypeEditorDesc
        {
            public object defaultValue;                         // used to create new items of primitive types
            public Func<ObjectAccessor, TypeEditor> factory;    // factory for TypeEditor
        }
        static Dictionary<System.Type, TypeEditorDesc> editorFactories = new Dictionary<System.Type, TypeEditorDesc>();

        static public void AddTypeEditorFactory(Type type, Func<ObjectAccessor, TypeEditor> factory, object defaultValue)
        {
            editorFactories.Add(type, new TypeEditorDesc() {factory = factory, defaultValue = defaultValue} );
        }
        /// <summary>
        /// Register editors for basic types
        /// </summary>
        static public void InitBasicTypes()
        {
            AddTypeEditorFactory(typeof(string), (ObjectAccessor obj) =>
            {
                return new FieldEditorString(obj);
            }, "");

            AddTypeEditorFactory(typeof(Single), (ObjectAccessor obj) =>
            {
                return new FieldEditorFloat(obj);
            }, 0);

            AddTypeEditorFactory(typeof(Int16), (ObjectAccessor obj) =>
            {
                return new FieldEditorInt<Int16>(obj);
            }, 0);

            AddTypeEditorFactory(typeof(Int32), (ObjectAccessor obj) =>
            {
                return new FieldEditorInt<Int32>(obj);
            }, 0);

            AddTypeEditorFactory(typeof(Byte), (ObjectAccessor obj) =>
            {
                return new FieldEditorInt<Byte>(obj);
            }, 0);

            AddTypeEditorFactory(typeof(Boolean), (ObjectAccessor obj) =>
            {
                return new FieldEditorBool(obj);
            }, false);
        }

        public static object CreateObject(Type type)
        {
            if (editorFactories.ContainsKey(type))
                return editorFactories[type].defaultValue;
            else
            {
                return System.Activator.CreateInstance(type, new object[] { });
            }
        }

        public static void InitGameTypes()
        {
            FieldOverrides overrides = null;

            overrides = GetFieldOverrides(typeof(Ship_Game.ModInformation));
            overrides.OverrideFieldObjectReference("ModImagePath_1920x1280", "Textures", true);
            overrides.OverrideFieldObjectReference("PortraitPath", "Textures", true);

            overrides = GetFieldOverrides(typeof(Ship_Game.Gameplay.Weapon));
            overrides.OverrideFieldObjectReference("BeamTexture", "Textures", false);
            overrides.OverrideFieldObjectReference("ProjectileTexturePath", "Textures", false);
            overrides.IgnoreField("moduleAttachedTo");
            overrides.IgnoreField("owner");
            overrides.IgnoreField("drowner");


            overrides = GetFieldOverrides(typeof(Ship_Game.Technology.LeadsToTech));
            overrides.OverrideFieldObjectReference("UID", TechSpec.Name, true);

            overrides = GetFieldOverrides(typeof(Ship_Game.Technology.UnlockedBuilding));
            overrides.OverrideFieldObjectReference("Name", BuildingSpec.Name, true);

            overrides = GetFieldOverrides(typeof(Ship_Game.Technology.UnlockedMod));
            overrides.OverrideFieldObjectReference("ModuleUID", ModuleSpec.Name, true);

            overrides = GetFieldOverrides(typeof(Ship_Game.Technology.UnlockedTroop));
            overrides.OverrideFieldObjectReference("Name", TroopSpec.Name, true);

            overrides = GetFieldOverrides(typeof(Ship_Game.Technology.UnlockedHull));
            //overrides.OverrideFieldObjectReference("Name", "Hulls", true);

            overrides = GetFieldOverrides(typeof(Ship_Game.Technology));
            overrides.OverrideFieldLocString("DescriptionIndex");
            overrides.OverrideFieldLocString("NameIndex");

            overrides = GetFieldOverrides(typeof(Ship_Game.Troop));
            overrides.OverrideFieldObjectReference("TexturePath", "Textures", true);

            overrides = GetFieldOverrides(typeof(Ship_Game.Building));
            overrides.OverrideFieldLocString("NameTranslationIndex");
            overrides.OverrideFieldLocString("DescriptionIndex");
            overrides.OverrideFieldLocString("ShortDescriptionIndex");
            overrides.OverrideFieldObjectReference("Weapon", WeaponGroup.Name, false);

            overrides = GetFieldOverrides(typeof(Ship_Game.Artifact));
            overrides.OverrideFieldLocString("DescriptionIndex");
            overrides.OverrideFieldLocString("NameIndex");

            overrides = GetFieldOverrides(typeof(Ship_Game.Gameplay.ShipModule));
            overrides.OverrideFieldLocString("DescriptionIndex");
            overrides.OverrideFieldLocString("NameIndex");
            overrides.IgnoreField("ModuleCenter");
            overrides.IgnoreField("installedSlot");
            overrides.IgnoreField("hangarShip");
            overrides.IgnoreField("LinkedModulesList");
            overrides.IgnoreField("Center");
            overrides.IgnoreField("moduleCenter");
            overrides.IgnoreField("Parent");
            overrides.IgnoreField("ParentOfDummy");
            overrides.OverrideFieldObjectReference("WeaponType", WeaponGroup.Name, false);

            overrides = GetFieldOverrides(typeof(Ship_Game.Outcome));
            overrides.OverrideFieldObjectReference("TroopsGranted", TroopSpec.Name, true);
            overrides.OverrideFieldObjectReference("TroopsToSpawn", TroopSpec.Name, true);
            overrides.OverrideFieldObjectReference("UnlockTech", TechSpec.Name, true);

            overrides.OverrideFieldObjectReference("FriendlyShipsToSpawn", ShipsGroup.Name, true);
            overrides.OverrideFieldObjectReference("RemnantShipsToSpawn", ShipsGroup.Name, true);

            overrides = GetFieldOverrides(typeof(Ship_Game.ShipData));
            overrides.IgnoreField("ThrusterList");

            overrides = GetFieldOverrides(typeof(Ship_Game.Gameplay.Ship));
            overrides.IgnoreField("AreaOfOperation");
            overrides.IgnoreField("BombBays");
            overrides.IgnoreField("ExternalSlots");
            overrides.IgnoreField("dying");
            overrides.IgnoreField("disabled");
            overrides.IgnoreField("Deleted");
            overrides.IgnoreField("fleet");
            overrides.IgnoreField("FleetCombatStatus");
            overrides.IgnoreField("FleetOffset");
            overrides.IgnoreField("guid");
            overrides.IgnoreField("inborders");
            overrides.IgnoreField("InCombat");
            overrides.IgnoreField("InCombatTimer");
            overrides.IgnoreField("InFrustum");
            overrides.IgnoreField("Inhibited");
            overrides.IgnoreField("InhibitedTimer");
            overrides.IgnoreField("InhibitionRadius");
            overrides.IgnoreField("inSensorRange");
            overrides.IgnoreField("isCloaked");
            overrides.IgnoreField("isColonyShip");
            overrides.IgnoreField("isDecloaking");
            overrides.IgnoreField("isJumping");
            overrides.IgnoreField("isThrusting");
            overrides.IgnoreField("isTurning");
            overrides.IgnoreField("loyalty");
            overrides.IgnoreField("Mothership");
            overrides.IgnoreField("OrbitalBeams");
            overrides.IgnoreField("Weapons");
            overrides.IgnoreField("TroopList");

            overrides = GetFieldOverrides(typeof(Ship_Game.Gameplay.ModuleSlot));
            overrides.IgnoreField("Parent");
            overrides.IgnoreField("module");
        }
    }
}