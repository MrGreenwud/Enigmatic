using System;
using UnityEditor;
using UnityEngine;

namespace Enigmatic.Experemental.FolderColorize
{
    [Serializable]
    public class FolderColorSettings
    {
        [SerializeField] private DefaultAsset m_Folder;
        [SerializeField] private Texture2D m_FolderIcon;

        public DefaultAsset Folder => m_Folder;
        public Texture2D FolderIcon => m_FolderIcon;

        public FolderColorSettings(DefaultAsset folder, Texture2D folderIcon)
        {
            m_Folder = folder;
            m_FolderIcon = folderIcon;
        }
    }
}
