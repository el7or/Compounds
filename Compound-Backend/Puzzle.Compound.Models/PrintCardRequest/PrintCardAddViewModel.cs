using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Models.PrintCardRequest
{
    public class PrintCardAddViewModel
    {
        public string Name { get; set; }
        public string Details { get; set; }
        public byte[] Picture { get; set; }
        public string PictureName { get; set; }
        public Guid CompoundUnitId { get; set; }
        public string QrCode { get; set; }
    }
}
