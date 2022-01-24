/*
Copyright 2009-2022 Intel Corporation

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.IO;
using System.Net;
using System.Text;
using System.Globalization;
using System.Security.Cryptography;

namespace MeshAssistant
{

    class ExeHandler
    {
        private static byte[] exeJavaScriptGuid = ConvertHexStringToByteArray("B996015880544A19B7F7E9BE44914C18");
        private static byte[] exeMeshPolicyGuid = ConvertHexStringToByteArray("B996015880544A19B7F7E9BE44914C19");

        public static int GetMshLengthFromExecutable(string path)
        {
            FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            if (fs.Length < 20) { fs.Close(); return 0; }
            fs.Seek(-20, SeekOrigin.End);
            byte[] buf = new byte[20];
            fs.Read(buf, 0, 20);
            fs.Close();
            bool mshPresent = true;
            for (var i = 0; i < 16; i ++) { if (exeMeshPolicyGuid[i] != buf[i + 4]) { mshPresent = false; } }
            if (mshPresent == false) return 0;
            return IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buf, 0));
        }

        public static string GetMshFromExecutable(string path, out int mshLength)
        {
            mshLength = 0;
            FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            if (fs.Length < 20) { fs.Close(); return null; }
            fs.Seek(-20, SeekOrigin.End);
            byte[] buf = new byte[20];
            fs.Read(buf, 0, 20);
            bool mshPresent = true;
            for (var i = 0; i < 16; i++) { if (exeMeshPolicyGuid[i] != buf[i + 4]) { mshPresent = false; } }
            if (mshPresent == false) return null;
            mshLength = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buf, 0));
            fs.Seek(-1 * (20 + mshLength), SeekOrigin.End);
            byte[] mshBuf = new byte[mshLength];
            fs.Read(mshBuf, 0, mshLength);
            fs.Close();
            return UTF8Encoding.UTF8.GetString(mshBuf, 0, mshBuf.Length);
        }

        public class WindowsBinaryData
        {
            public enum BinaryTypeEnum { Unknown = 0, x32bit = 1, x64bit = 2 }
            public BinaryTypeEnum BinaryType;
            public int optionalHeaderSize;
            public int optionalHeaderSizeAddress;
            public int checkSumPos;
            public int sizeOfCode;
            public int sizeOfInitializedData;
            public int sizeOfUnInitializedData;

            public int rvaCount;
            public int CertificateTableAddress;
            public int CertificateTableSize;
            public int CertificateTableSizePos;
            public int rvaStartAddress;
            public byte[] certificate;
        }

        public static WindowsBinaryData ExecutableParser(string path)
        {
            WindowsBinaryData r = new WindowsBinaryData();

            FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            if (fs.Length < 64) { fs.Close(); return null; } // File too short
            fs.Seek(0, SeekOrigin.Begin);

            // Read DOS header
            byte[] dosHeader = new byte[64];
            fs.Read(dosHeader, 0, 64);
            if ((dosHeader[0] != 0x4d) || (dosHeader[1] != 0x5a)) { return null; } // Unrecognized binary format
            int ntHeaderPosition = BitConverter.ToInt32(dosHeader, 60);

            // Read NT header
            fs.Seek(ntHeaderPosition, SeekOrigin.Begin);
            byte[] ntHeader = new byte[24];
            fs.Read(ntHeader, 0, ntHeader.Length);
            if ((ntHeader[0] != 0x50) || (ntHeader[1] != 0x45) || (ntHeader[2] != 0x00) || (ntHeader[3] != 0x00)) { return null; } // Not a PE file
            int binaryType = BitConverter.ToInt16(ntHeader, 4);
            if (binaryType == 0x014C) { r.BinaryType = WindowsBinaryData.BinaryTypeEnum.x32bit; } // 32 bit
            else if (binaryType == 0x8664) { r.BinaryType = WindowsBinaryData.BinaryTypeEnum.x64bit; } // 64 bit

            // Read the optional header
            r.optionalHeaderSize = BitConverter.ToInt16(ntHeader, 20);
            r.optionalHeaderSizeAddress = BitConverter.ToInt16(dosHeader, 60) + 24;
            byte[] optHeader = new byte[r.optionalHeaderSize];
            fs.Seek(r.optionalHeaderSizeAddress, SeekOrigin.Begin);
            fs.Read(optHeader, 0, optHeader.Length);

            // Set values
            r.checkSumPos = ntHeaderPosition + 24 + 64;
            r.sizeOfCode = BitConverter.ToInt32(optHeader, 4);
            r.sizeOfInitializedData = BitConverter.ToInt32(optHeader, 8);
            r.sizeOfUnInitializedData = BitConverter.ToInt32(optHeader, 12);

            int optionalMagic = BitConverter.ToInt16(optHeader, 0);
            switch (optionalMagic)
            {
                case 0x010B: // 32bit
                    {
                        r.rvaCount = BitConverter.ToInt32(optHeader, 92);
                        r.CertificateTableAddress = BitConverter.ToInt32(optHeader, 128);
                        r.CertificateTableSize = BitConverter.ToInt32(optHeader, 132);
                        r.CertificateTableSizePos = r.optionalHeaderSizeAddress + 132;
                        r.rvaStartAddress = r.optionalHeaderSizeAddress + 96;
                        /*
					        if (ILibMemory_AllocateA_Size(optHeader) >= 132)
					        {
						        if (((unsigned int*)(optHeader + 128))[0] != 0)
						        {
							        endIndex = ((unsigned int*)(optHeader + 128))[0];
						        }
						        tableIndex = NTHeaderIndex + 24 + 128;
						        retVal = 0;
					        }
                        */
                        break;
                    }
                case 0x020B: // 64bit
                    {
                        r.rvaCount = BitConverter.ToInt32(optHeader, 108);
                        r.CertificateTableAddress = BitConverter.ToInt32(optHeader, 144);
                        r.CertificateTableSize = BitConverter.ToInt32(optHeader, 148);
                        r.CertificateTableSizePos = r.optionalHeaderSizeAddress + 148;
                        r.rvaStartAddress = r.optionalHeaderSizeAddress + 112;
                        break;
                    }
                default: // Unknown Value found for Optional Magic
                    {
                        return null;
                    }
            }

            if (r.CertificateTableAddress > 0)
            {
                // Read the authenticode certificate, only one cert (only the first entry)
                byte[] hdr = new byte[8];
                fs.Seek(r.CertificateTableAddress, SeekOrigin.Begin);
                fs.Read(hdr, 0, hdr.Length);
                int certLen = BitConverter.ToInt32(hdr, 0);
                r.certificate = new byte[certLen];
                fs.Seek(r.CertificateTableAddress + hdr.Length, SeekOrigin.Begin);
                fs.Read(r.certificate, 0, r.certificate.Length);
            }

            return r;
        }


        public static string HashExecutable(string path)
        {
            WindowsBinaryData r = ExecutableParser(path);

            byte[] selfHash;
            int checkSumIndex = r.checkSumPos;
            int tableIndex = r.CertificateTableSizePos - 4;
            int endIndex = 0;
            if (r.CertificateTableAddress != 0) { endIndex = r.CertificateTableAddress; }

            if (endIndex == 0)
            {
                // Hash the entire file except the .msh at the end if .msh is present
                int mshLen = GetMshLengthFromExecutable(path);
                if (mshLen > 0) { mshLen += 20; }
                using (SHA384 sha384 = SHA384Managed.Create())
                {
                    sha384.Initialize();
                    using (FileStream stream = File.OpenRead(path))
                    {
                        hashPortionOfStream(sha384, stream, 0, (int)stream.Length - mshLen); // Start --> end - (mshLen + 20)
                        sha384.TransformFinalBlock(new byte[0], 0, 0);
                        selfHash = sha384.Hash;
                    }
                }
                return BitConverter.ToString(selfHash).Replace("-", string.Empty).ToLower();
            }

            using (SHA384 sha384 = SHA384Managed.Create())
            {
                sha384.Initialize();
                using (FileStream stream = File.OpenRead(path))
                {
                    hashPortionOfStream(sha384, stream, 0, checkSumIndex); // Start --> checkSumIndex
                    sha384.TransformBlock(new byte[4], 0, 4, null, 0); // 4 zero bytes
                    hashPortionOfStream(sha384, stream, checkSumIndex + 4, tableIndex); // checkSumIndex + 4 --> tableIndex
                    sha384.TransformBlock(new byte[8], 0, 8, null, 0); // 8 zero bytes
                    hashPortionOfStream(sha384, stream, tableIndex + 8, endIndex); // tableIndex + 8 --> endIndex
                    sha384.TransformFinalBlock(new byte[0], 0, 0);
                    selfHash = sha384.Hash;
                }
            }
            return BitConverter.ToString(selfHash).Replace("-", string.Empty).ToLower();
        }

        private static void hashPortionOfStream(SHA384 sha384, FileStream stream, int start, int end)
        {
            stream.Seek(start, SeekOrigin.Begin);
            int fileLengthToHash = (end - start);
            byte[] buf = new byte[65535];
            while (fileLengthToHash > 0) {
                int l = stream.Read(buf, 0, (int)Math.Min(fileLengthToHash, buf.Length));
                fileLengthToHash -= l;
                sha384.TransformBlock(buf, 0, l, null, 0);
            }
        }

        public static byte[] ConvertHexStringToByteArray(string hexString)
        {
            if (hexString.Length % 2 != 0) return null;
            byte[] data = new byte[hexString.Length / 2];
            for (int index = 0; index < data.Length; index++)
            {
                string byteValue = hexString.Substring(index * 2, 2);
                data[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }
            return data;
        }

    }
}
