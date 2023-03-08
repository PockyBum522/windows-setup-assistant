using System;
using System.IO;

namespace WindowsSetupAssistant.Logic;

public class FileSearcher
{
    private static string _foundFullPath = "";
    
    public static string ReverseWalkDirectoriesFind(string pathToSearchIn, string fileName, int maxLevels)
    {
        _foundFullPath = "";

        ReverseWalkDirectoriesBacker(pathToSearchIn, fileName, maxLevels);

        // Check if we didn't find anything
        if (_foundFullPath.Equals("")) throw new FileNotFoundException();
        
        var fullPathToReturn = DeepCopyString(_foundFullPath);

        // Reset this for next run
        _foundFullPath = "";
            
        // then we already found it. Stop this. There are more efficient places to do this but this is cleaner code-wise
        return fullPathToReturn;
    }

    /// <summary>
    /// Runs the recursive search, walking backwards up the directory tree
    /// </summary>
    /// <param name="pathToSearchIn">The deepest path to start in, moving backwards up the tree from</param>
    /// <param name="fileName">File name to match (Case ignored)</param>
    /// <param name="maxLevels">How far back up the tree to go before giving up</param>
    private static void ReverseWalkDirectoriesBacker(string pathToSearchIn, string fileName, int maxLevels)
    {
        var depthOfDeepestFolder = 
            pathToSearchIn.Split(Path.DirectorySeparatorChar).Length;

        var nextDirectoryPath = pathToSearchIn;
        
        for (var i = depthOfDeepestFolder; i > 0; i--)
        {
            if ((depthOfDeepestFolder - i) > maxLevels) break;
            
            // Search all subdirs and files in there
            RecursivelySearchDirectory(nextDirectoryPath, fileName);
            
            // If we find, return
            if (!_foundFullPath.Equals("")) break;
            
            // If not, get dir above that
            nextDirectoryPath = Path.Join(nextDirectoryPath, "..");
        }
    }
    
    /// <summary>
    /// This is what actually does the searching
    /// </summary>
    /// <param name="targetDirectory">Folder to recursively search in</param>
    /// <param name="fileNameToLookFor">Filename to match (Case ignored)</param>
    private static void RecursivelySearchDirectory(string targetDirectory, string fileNameToLookFor)
    {
        if (!_foundFullPath.Equals(""))
        {
            // then we already found it. Stop this. There are more efficient places to do this but this is cleaner code-wise
            return;
        }
        
        // Process the list of files found in the directory.
        var fileEntries = Directory.GetFiles(targetDirectory);

        foreach (var fileFullPath in fileEntries)
        {
            //Console.WriteLine($"Comparing: {fileFullPath} to: {fileNameToLookFor}");
            
            if (fileFullPath.EndsWith(fileNameToLookFor, StringComparison.OrdinalIgnoreCase)) 
                _foundFullPath = fileFullPath;
        }

        // Recurse into subdirectories of this directory.
        var subdirectoryEntries = Directory.GetDirectories(targetDirectory);
        
        foreach(var subdirectory in subdirectoryEntries)
            RecursivelySearchDirectory(subdirectory, fileNameToLookFor);
    }
    
    private static string DeepCopyString(string foundFullPath)
    {
        var stringToReturn = "";

        foreach (var character in foundFullPath)
        {
            stringToReturn += character;
        }

        return stringToReturn;
    }
}