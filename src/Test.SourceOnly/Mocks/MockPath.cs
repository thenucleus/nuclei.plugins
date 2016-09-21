//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.IO.Abstractions;
using System.Reflection;
using Nuclei;

namespace Test.Mocks
{
    internal sealed class MockPath : PathBase
    {
        public override string ChangeExtension(string path, string extension)
        {
            throw new NotImplementedException();
        }

        public override string Combine(string path1, string path2)
        {
            return Path.Combine(path1, path2);
        }

        public override string GetDirectoryName(string path)
        {
            throw new NotImplementedException();
        }

        public override string GetExtension(string path)
        {
            throw new NotImplementedException();
        }

        public override string GetFileName(string path)
        {
            throw new NotImplementedException();
        }

        public override string GetFileNameWithoutExtension(string path)
        {
            throw new NotImplementedException();
        }

        public override string GetFullPath(string path)
        {
            throw new NotImplementedException();
        }

        public override char[] GetInvalidFileNameChars()
        {
            throw new NotImplementedException();
        }

        public override char[] GetInvalidPathChars()
        {
            throw new NotImplementedException();
        }

        public override string GetPathRoot(string path)
        {
            throw new NotImplementedException();
        }

        public override string GetRandomFileName()
        {
            return "abcde.fgh";
        }

        public override string GetTempFileName()
        {
            var path = GetTempPath();
            return Path.Combine(path, GetRandomFileName());
        }

        public override string GetTempPath()
        {
            return Assembly.GetExecutingAssembly().LocalDirectoryPath();
        }

        public override bool HasExtension(string path)
        {
            throw new NotImplementedException();
        }

        public override bool IsPathRooted(string path)
        {
            throw new NotImplementedException();
        }

        public override char AltDirectorySeparatorChar
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override char DirectorySeparatorChar
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete]
        public override char[] InvalidPathChars
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override char PathSeparator
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override char VolumeSeparatorChar
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
