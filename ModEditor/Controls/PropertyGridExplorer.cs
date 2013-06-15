using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace ModEditor.Controls
{
    public partial class PropertyGridExplorer : TableLayoutPanel
    {
        static int rowHeight = 21;
        static int firstColumnWidth = 160;

        public PropertyGridExplorer()
        {
            AutoScroll = true;
            AutoSize = false;
            CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
        }
        
        static Dictionary<System.Type, FieldOverrides> typeTable = new Dictionary<Type,FieldOverrides>();

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

        public class Column
        {
            public Dictionary<string, FieldEditor> editors = new Dictionary<string, FieldEditor>();
            object source;
        }
        
        public Dictionary<string, FieldEditor> editors = new Dictionary<string, FieldEditor>();

        void AddRow(string name, FieldEditor editor, Control control)
        {
            RowStyle rowStyle = new RowStyle()
            {
                Height = rowHeight,
                SizeType = SizeType.AutoSize
            };

            int row = RowStyles.Add(rowStyle);

            var label = new Label()
            {
                Text = name,
                Anchor = (AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom),
                TextAlign = ContentAlignment.MiddleLeft,
                //Dock= DockStyle.Right, 
                //AutoSize = false,
                BackColor = SystemColors.ControlLightLight,
                Padding = new Padding(0),
                Margin = new Padding(1),
                //Height = rowHeight,
                Width = firstColumnWidth,
            };

            control.Anchor = (AnchorStyles.Left | AnchorStyles.Right);//DockStyle.Fill;
            
            this.Controls.Add(control, 1, row);
            this.Controls.Add(label, 0, row);
            /*
            dataTable.RowStyles[rows].Height = rowHeight;
            dataTable.RowStyles[rows].SizeType = SizeType.Absolute;*/
            editors.Add(name, editor);
        }

        public List<ModEditorAttribute> GetAttributes(System.Reflection.FieldInfo fieldInfo, Type ownerType)
        {
            List<ModEditorAttribute> result = new List<ModEditorAttribute>();

            if (PropertyGridExplorer.HasAttributeOverrides(ownerType))
            {
                var overrides = PropertyGridExplorer.GetFieldOverrides(ownerType);
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

        FieldEditor GenerateFieldEditor(System.Reflection.FieldInfo fieldInfo, FieldCallback callback)
        {
            foreach (ModEditorAttribute attrib in GetAttributes(fieldInfo, callback.GetTargetType()))
            {
                // Attribute can return null - thats ok, meaning that this field should be skipped
                var editor = attrib.GenerateEditor(fieldInfo, callback);
                return editor;
            }
            // No overriden editor was found - use default editors

            Type type = fieldInfo.FieldType;
            if (type.IsEnum)
            {
                return new FieldEditorEnum(fieldInfo, callback);
            }

            if(type.IsGenericType)
            {
                // If list
                if(type.GetInterfaces().Contains(typeof(IList)))
                {
                }
                if (type.Name.Equals("List`1"))
                {
                    Type storedType = type.GetGenericArguments()[0];
                    return new FieldEditorList(fieldInfo, callback, storedType);
                }
            }
            Func<System.Reflection.FieldInfo, FieldCallback, FieldEditor> factory = null;
            if (editorFactories.ContainsKey(type))
                factory = editorFactories[type];
            else
                return new FieldEditorReadOnly(fieldInfo, callback);

            return factory(fieldInfo, callback);
        }

        public void UpdateData()
        {
            foreach (var editor in editors)
            {
                editor.Value.UpdateValue();
            }
        }

        public void Clear()
        {
            editors.Clear();
            this.RowStyles.Clear();
            this.Controls.Clear();
            this.RowCount = 0;
        }

        public void SetSource(System.Type type, FieldCallback callback)
        {
            SuspendLayout();
            Clear();

            if (callback != null)
            {
                foreach (var member in type.GetFields())
                {
                    try
                    {
                        FieldEditor editor = GenerateFieldEditor(member, callback);

                        if (editor == null)
                            continue;
                        Control control = editor.GetControl();

                        //Control control = item.controller.GenerateControl(member, data);

                        if (control != null)
                            AddRow(member.Name, editor, control);
                    }
                    catch (Exception e)
                    {
                        MainForm.LogErrorString(e.Message);
                    }
                }
            }
            UpdateData();
            // Setup row and column styles
            foreach (RowStyle row in this.RowStyles)
            {
                row.Height = rowHeight;
                row.SizeType = SizeType.AutoSize;
            }

            RowStyle rowStyle = new RowStyle()
            {
                Height = rowHeight,
                SizeType = SizeType.AutoSize
            };

            RowStyles.Add(rowStyle);
            this.RowCount = RowStyles.Count;

            if (this.ColumnCount > 0)
            {
                this.ColumnStyles[0].Width = firstColumnWidth;
                this.ColumnStyles[0].SizeType = SizeType.Absolute;
                this.ColumnStyles[1].SizeType = SizeType.AutoSize;
            }
            ResumeLayout();
        }        
        
        static public Dictionary<System.Type, Func<System.Reflection.FieldInfo, FieldCallback, FieldEditor>> editorFactories = new Dictionary<System.Type, Func<System.Reflection.FieldInfo, FieldCallback, FieldEditor>>();

        private void PropertyGridExplorer_MouseHover(object sender, EventArgs e)
        {
            //int width = this.ColumnStyles[0].Width
        }
        /// <summary>
        /// Register editors for basic types
        /// </summary>
        static public void InitBasicTypes()
        {
            editorFactories.Add(typeof(string), (System.Reflection.FieldInfo field, FieldCallback obj) =>
            {
                return new FieldEditorString(field, obj);
            });

            editorFactories.Add(typeof(Single), (System.Reflection.FieldInfo field, FieldCallback obj) =>
            {
                return new FieldEditorFloat(field, obj);
            });

            editorFactories.Add(typeof(Int16), (System.Reflection.FieldInfo field, FieldCallback obj) =>
            {
                return new FieldEditorInt<Int16>(field, obj);
            });

            editorFactories.Add(typeof(Int32), (System.Reflection.FieldInfo field, FieldCallback obj) =>
            {
                return new FieldEditorInt<Int32>(field, obj);
            });

            editorFactories.Add(typeof(Byte), (System.Reflection.FieldInfo field, FieldCallback obj) =>
            {
                return new FieldEditorInt<Byte>(field, obj);
            });

            editorFactories.Add(typeof(Boolean), (System.Reflection.FieldInfo field, FieldCallback obj) =>
            {
                return new FieldEditorBool(field, obj);
            });
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

            overrides = GetFieldOverrides(typeof(Ship_Game.Technology.LeadsToTech));
            overrides.OverrideFieldObjectReference("UID", "Technology", true);

            overrides = GetFieldOverrides(typeof(Ship_Game.Technology.UnlockedBuilding));
            overrides.OverrideFieldObjectReference("Name", "Buildings", true);

            overrides = GetFieldOverrides(typeof(Ship_Game.Technology.UnlockedMod));
            overrides.OverrideFieldObjectReference("ModuleUID", "ShipModules", true);

            overrides = GetFieldOverrides(typeof(Ship_Game.Technology.UnlockedTroop));
            overrides.OverrideFieldObjectReference("Name", "Troops", true);

            overrides = GetFieldOverrides(typeof(Ship_Game.Technology.UnlockedHull));
            overrides.OverrideFieldObjectReference("Name", "Hulls", true);

            overrides = GetFieldOverrides(typeof(Ship_Game.Technology));
            overrides.OverrideFieldLocString("DescriptionIndex");
            overrides.OverrideFieldLocString("NameIndex");

            overrides = GetFieldOverrides(typeof(Ship_Game.Troop));
            overrides.OverrideFieldObjectReference("TexturePath", "Textures", true);

            overrides = GetFieldOverrides(typeof(Ship_Game.Building));
            overrides.OverrideFieldLocString("NameTranslationIndex");
            overrides.OverrideFieldLocString("DescriptionIndex");
            overrides.OverrideFieldLocString("ShortDescriptionIndex");
            overrides.OverrideFieldObjectReference("Weapon", "Weapons", false);

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
            overrides.OverrideFieldObjectReference("WeaponType", "Weapons", false);
        }
 
    }
}
