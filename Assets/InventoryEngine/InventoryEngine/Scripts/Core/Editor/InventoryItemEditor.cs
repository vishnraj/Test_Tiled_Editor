using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UI;

namespace MoreMountains.InventoryEngine
{	
	[CustomEditor(typeof(InventoryItem),true)]
	/// <summary>
	/// Custom editor for the InventoryItem component
	/// </summary>
	public class InventoryItemEditor : Editor 
	{
		/// <summary>
		/// Gets the target inventory component.
		/// </summary>
		/// <value>The inventory target.</value>
		public InventoryItem ItemTarget 
		{ 
			get 
			{ 
				return (InventoryItem)target;
			}
		} 
	   
	   /// <summary>
	   /// Custom editor for the inventory panel.
	   /// </summary>
		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			if (!ItemTarget.Equippable && !ItemTarget.Usable)
			{
				Editor.DrawPropertiesExcluding(serializedObject, new string[] { "TargetEquipmentInventoryName", "EquippedSound", "UsedSound" });
			}
			if (ItemTarget.Equippable && !ItemTarget.Usable)
			{
				Editor.DrawPropertiesExcluding(serializedObject, new string[] {"UsedSound" });
			}
			if (ItemTarget.Usable && !ItemTarget.Equippable)
			{
				Editor.DrawPropertiesExcluding(serializedObject, new string[] { "TargetEquipmentInventoryName", "EquippedSound" });
			}
			if (ItemTarget.Equippable && ItemTarget.Usable)
			{
				DrawDefaultInspector();
			}
			serializedObject.ApplyModifiedProperties();
		}
	}
}