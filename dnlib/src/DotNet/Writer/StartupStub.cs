﻿#region

using System.IO;
using dnlib.IO;
using dnlib.PE;

#endregion

namespace dnlib.DotNet.Writer
{
    /// <summary>
    ///     Stores the instruction that jumps to _CorExeMain/_CorDllMain
    /// </summary>
    public sealed class StartupStub : IChunk
    {
        private uint length;
        private uint padding;

        /// <summary>
        ///     Gets/sets the <see cref="ImportDirectory" />
        /// </summary>
        public ImportDirectory ImportDirectory
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets/sets the <see cref="PEHeaders" />
        /// </summary>
        public PEHeaders PEHeaders
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets the address of the JMP instruction
        /// </summary>
        public RVA EntryPointRVA => RVA + padding;

        /// <summary>
        ///     Gets the address of the operand of the JMP instruction
        /// </summary>
        public RVA RelocRVA => EntryPointRVA + 2;

        /// <inheritdoc />
        public FileOffset FileOffset
        {
            get;
            private set;
        }

        /// <inheritdoc />
        public RVA RVA
        {
            get;
            private set;
        }

        /// <inheritdoc />
        public void SetOffset(FileOffset offset, RVA rva)
        {
            FileOffset = offset;
            RVA = rva;

            padding = rva.AlignUp(4) - rva + 2;
            length = padding + 6;
        }

        /// <inheritdoc />
        public uint GetFileLength()
        {
            return length;
        }

        /// <inheritdoc />
        public uint GetVirtualSize()
        {
            return GetFileLength();
        }

        /// <inheritdoc />
        public void WriteTo(BinaryWriter writer)
        {
            writer.WriteZeros((int) padding);
            writer.Write((byte) 0xFF);
            writer.Write((byte) 0x25);
            writer.Write((uint) PEHeaders.ImageBase + (uint) ImportDirectory.IatCorXxxMainRVA);
        }
    }
}