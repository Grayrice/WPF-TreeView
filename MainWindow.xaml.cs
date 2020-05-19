using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;


namespace WpfTreeView
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        #endregion

        #region On Load

        /// <summary>
        /// When the application first open
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Get every logical drive on the machine
            foreach (var drive in Directory.GetLogicalDrives())
            {
                // Create a new item for it
                var item = new TreeViewItem()
                {
                    // Set the header 
                    Header = drive,
                    //Add the full path
                    Tag = drive
                };

                // Add a dummy item
                item.Items.Add(null);

                // Listen  out for item being expanded
                item.Expanded += Folder_Expanded;

                // Add it to the main TreeView
                FolderView.Items.Add(item);
            }
        }

        #endregion

        #region Folder Expanded

        /// <summary>
        /// When a folder is expander, find the sub folders/files
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Folder_Expanded(object sender, RoutedEventArgs e)
        {
            #region Initial Checks
            var item = (TreeViewItem)sender;

            // If the item only contains the dummy data
            if (item.Items.Count != 1 || item.Items[0] != null)
                return;

            // Clear dummy date
            item.Items.Clear();

            // Get full path
            var fullPath = (string)item.Tag;

            #endregion

            #region Get Folders

            // Create a blank list for directories
            var directories = new List<string>();

            // Try and get directories from the folder
            try
            {
                var dirs = Directory.GetDirectories(fullPath);

                if (dirs.Length > 0)
                    directories.AddRange(dirs); 
                
            }
            catch { }

            // For each directory...
            directories.ForEach(directoryPath =>
            {
                // Create directory item
                var subItem = new TreeViewItem()
                {
                    // Set header as folder name
                    Header = GetFileFolderName(directoryPath),
                    // Add tag as full path
                    Tag = directoryPath
                };

                // Add dummy item so we can expand folder
                subItem.Items.Add(null);

                // Handler expanding
                subItem.Expanded += Folder_Expanded;

                // Add this item to the parent 
                item.Items.Add(subItem);
            });

            #endregion

            #region Get Files

            // Create a blank list for files
            var files = new List<string>();

            // Try and get files from the folder
            try
            {
                var fs = Directory.GetFiles(fullPath);

                if (fs.Length > 0)
                    files.AddRange(fs);

            }
            catch { }

            // For each file...
            files.ForEach(filepath =>
            {
                // Create directory item
                var subItem = new TreeViewItem()
                {
                    // Set header as file name
                    Header = GetFileFolderName(filepath),
                    // Add tag as full path
                    Tag = filepath
                };

                // Add this item to the parent 
                item.Items.Add(subItem);
            });

            #endregion
        }

        #endregion

        #region Helper

        /// <summary>
        /// Find the file or folder name from a full path
        /// </summary>
        /// <param name="directoryPath">The full path</param>
        /// <returns></returns>
        public static string GetFileFolderName(string path)
        {
            // C:\Sonthiing\ a folder
            // C:\Sonthiing\ a file.png

            // If we have no path, return empty
            if (string.IsNullOrEmpty(path))
                return string.Empty;

            // Make all slashes back slashes
            var normalizeDPath = path.Replace('/', '\\');

            // Find the last  backslash in the path
            var lastIndex = normalizeDPath.LastIndexOf('\\');

            // If we don't find a backslash, return the path itself
            if (lastIndex <= 0)
                return path;

            // Return the name after the lst back slash
            return path.Substring(lastIndex + 1);
        }

        #endregion
    }
}
