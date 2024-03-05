using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Enigmatic.Experemental.FolderColorize
{
    [CreateAssetMenu(fileName = "Folder Colors Setup", menuName = "Enigmatic/FolderColorize/FolderColorsSetup")]
    public class FolderColorsSetup : ScriptableObject
    {
        [SerializeField] private List<FolderColorSettings> m_FolderColorSettings = new List<FolderColorSettings>();

        public void AddSettings(DefaultAsset folder, Texture2D icon)
        {
            if (folder == null && icon == null)
                throw new ArgumentNullException();

            if (ContanceSettings(folder, out FolderColorSettings settings))
                m_FolderColorSettings.Remove(settings);

            m_FolderColorSettings.Add(new FolderColorSettings(folder, icon));
        }

        public void RemoveSettings(DefaultAsset folder) 
        {
            if (ContanceSettings(folder, out FolderColorSettings settings))
                m_FolderColorSettings.Remove(settings);
        }

        public Texture2D GetIcon(DefaultAsset folder)
        {
            if(folder == null)
                throw new ArgumentNullException();
            
            if(ContanceSettings(folder, out FolderColorSettings settings) == false)
                return null;
            
            return settings.FolderIcon;
        }

        public bool ContanceSettings(DefaultAsset folder, out FolderColorSettings settings)
        {
            foreach (FolderColorSettings FolderColorSettings in m_FolderColorSettings)
            {
                if(FolderColorSettings.Folder == folder)
                {
                    settings = FolderColorSettings;
                    return true;
                }
            }

            settings = null;
            return false;
        }
    }
}
