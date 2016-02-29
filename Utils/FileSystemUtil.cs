using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace QuantumConcepts.Common.Utils
{
    public static class FileSystemUtil
    {
        /// <summary>
        ///     This method is used to calculate a hint path. Hint paths are useful if the location of one file is directly
        ///     related to the location of another file. Instead of using absolute paths - which would necessarily work if
        ///     you were to move the containing folders to another drive - hint paths allow for greater flexibility. A good
        ///     example of where a hint path would be preferable is if you have a project file which references files
        ///     related to the project.
        ///     <example>
        ///         sourcePath:         D:\Project\MyProject\Database\Scripts\1.0.0.0.sql
        ///         destinationPath:    D:\Project\MyProject\Archive\Database\Backups\1.0.0.0.bak
        ///         
        ///         There is a valid hint path for this input because they reside on the same drive.
        ///         
        ///         The hint path for this input is calculated buy first matching each portion of the paths to each other
        ///         and finding the point where the path changes. In this case, that would be after the MyProject directory.
        ///         If the path does change, once we know the directory at which the path changes, we can calculate how many
        ///         directories we would need to "go back" from the sourcePath to get to the change point. In this case,
        ///         we would have to go back two directories (Scripts back to Database, Database back to MyProject). At this
        ///         point the hint path is simply "..\..\" and all that is left to do is append the remaining portion of the
        ///         destinationPath. So the resulting hint path for this input is:
        ///         
        ///         ..\..\Archive\Database\Backups\1.0.0.0.bak
        ///     </example>
        /// </summary>
        /// <param name="referencePath">The path to base the hint path off of. This must either be a path ending in '\' or a full file path.</param>
        /// <param name="destinationFileName">The full path for the hint path to lead to.</param>
        /// <returns>The hint path, if one exists, otherwise the destinationPath.</returns>
        public static string AbsolutePathToHintPath(string referencePath, string destinationFileName)
        {
            //If the root paths aren't the same (drive), then there is no hint path.
            if (!string.Equals(Path.GetPathRoot(referencePath), Path.GetPathRoot(Path.GetDirectoryName(destinationFileName))))
                return destinationFileName;
            else
            {
                //Split the paths into "parts."
                string[] referencePathParts = Path.GetDirectoryName(referencePath).Split('\\');
                string[] destinationPathParts = destinationFileName.Split('\\');
                int lastMatchIndex = 0;
                StringBuilder hintPath = new StringBuilder();

                //Determine the point at which the path changes (if any).
                while (referencePathParts.GetUpperBound(0) > lastMatchIndex && destinationPathParts.GetUpperBound(0) > lastMatchIndex)
                {
                    if (string.Equals(referencePathParts[lastMatchIndex + 1], destinationPathParts[lastMatchIndex + 1]))
                        lastMatchIndex++;
                    else
                        break;
                }

                //For each remaining directory in the sourcePath, we need to go back one level.
                for (int i = 0; i < (referencePathParts.GetUpperBound(0) - lastMatchIndex); i++)
                    hintPath.Append(@"..\");

                //Now, append the remainder of the destinationPath.
                hintPath.Append(string.Join(@"\", destinationPathParts.Skip(lastMatchIndex + 1).ToArray()));

                return hintPath.ToString();
            }
        }

        /// <summary>
        ///     This method is used to calculate an absolute path from a hint path.
        /// </summary>
        /// <param name="referencePath">The path to base the hint path off of. This must either be a path ending in '\' or a full file path.</param>
        /// <param name="destinationFileName">The hint path.</param>
        /// <returns>The calculated absolute path.</returns>
        public static string HintPathToAbsolutePath(string referencePath, string hintPath)
        {
            string[] referencePathPathParts = Path.GetDirectoryName(referencePath).Split('\\');
            string[] hintPathParts = hintPath.Split('\\');
            int parentCount = 0;
            StringBuilder absolutePath = new StringBuilder();

            while (hintPathParts.GetUpperBound(0) > parentCount && hintPathParts[parentCount].Equals(".."))
                parentCount++;

            absolutePath.Append(string.Join(@"\", referencePathPathParts.Take(referencePathPathParts.Length - parentCount).ToArray()));
            absolutePath.Append(@"\");
            absolutePath.Append(string.Join(@"\", hintPathParts.Skip(parentCount).ToArray()));

            return absolutePath.ToString();
        }
    }
}
