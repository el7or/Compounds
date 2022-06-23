using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Models
{
    public class PuzzleFileInfo
    {
        public int SizeInBytes { get; set; }
        public string FileName { get; set; }
        public string Path { get; set; }
        public string FileBase64 { get; set; }
    }
}
