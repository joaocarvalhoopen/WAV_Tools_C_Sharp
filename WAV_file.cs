/*
Copyright (c) 2015, Joao Nuno Carvalho

All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
Neither the author nor the names of any contributors may be used to endorse or promote products derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace WAV_Tools_C_Sharp
{
    public class WAV_file
    {
        public enum NUM_CHANNELS
        {
            NOT_DEFINED = 0,    
            ONE         = 1,
            TWO         = 2
        };

        public enum BITS_PER_SAMPLE
        {
            NOT_DEFINED = 0,
            BPS_8_BITS  = 8,
            BPS_16_BITS = 16
        };

        const int C_HEADER_BYTE_SIZE = 44;

        // WAV file header fields.
        private byte[] chunk_id = new Byte[4];       // This are char[] in the C sense, one byte each.
        private System.UInt32 chunk_size;
        private byte[] format = new Byte[4];         //    "
        private byte[] fmtchunk_id = new Byte[4];    //    "
        private System.UInt32 fmtchunk_size;
        private System.UInt16 audio_format;
        private System.UInt16 num_channels;
        private System.UInt32 sample_rate;
        private System.UInt32 byte_rate;
        private System.UInt16 block_align;
        private System.UInt16 bps;                    //Bits per sample 
        private byte[] datachunk_id = new Byte[4];    // This are char[] in the C sense, one byte each.
        private System.UInt32 datachunk_size;


        // WAV object fields.
        private string _file_name;
        public string File_name
        {
            get { return _file_name; }
            set
            {
                if (!value.ToUpper().EndsWith("WAV"))
                    throw new ArgumentException("WAV file name must end with 'wav', or 'WAV' extension!");
                _file_name = value;
            }
        }

        private string _path;
        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }

        private System.UInt32 _numberOfSamples;
        public System.UInt32 NumberOfSamples
        {
            get { return _numberOfSamples; }
            set { _numberOfSamples = value; }
        }

        private System.Byte[]  bufferInternal_uint8 = null;
        private System.Int16[] bufferInternal_int16 = null;
   

        // Constructor
        public WAV_file()
        {

        }

        public WAV_file(string path, string file_name)
        {
            Path = path;
            File_name = file_name;
        }

        // Properties.
        public NUM_CHANNELS NumOfChannels
        {
            get { return (NUM_CHANNELS) num_channels; }
            set { num_channels = (System.UInt16)value; }
        }

        public System.UInt32 SampleRate
        {
            get { return sample_rate; }
            set { sample_rate = value; }
        }

        private System.UInt32 ByteRate
        {
            get { return byte_rate; }
            set { byte_rate = value; }
        }

        public BITS_PER_SAMPLE BitsPerSample
        {
            get { return (BITS_PER_SAMPLE)bps; }
            set { bps = (System.UInt16)value; }
        }

        public bool isStereo()
        {
            if (num_channels == 2)
                return true;
            else
                return false;
        }

        public void initializeWaveHeaderStructBeforeWriting()
        {
            // Initial fill of waveHeader data structure, byte[].
            chunk_id = System.Text.Encoding.ASCII.GetBytes("RIFF");

            // waveHeader.chunk_size = ;
            format = System.Text.Encoding.ASCII.GetBytes("WAVE");

            fmtchunk_id = System.Text.Encoding.ASCII.GetBytes("fmt ");
            fmtchunk_size = 16;
            audio_format = 1;
            // num_channels;
            if (num_channels != (uint) NUM_CHANNELS.ONE
                && num_channels != (uint)NUM_CHANNELS.TWO)
            {
                throw new ApplicationException("ERROR: The number of channels wasn't set to 1 or 2 ");
            }
            // sample_rate;
            if (sample_rate == 0)
            {
                throw new ApplicationException("ERROR: The sample rate wasn't set to ex: 8000 S/s or ex:44100");
            }
            byte_rate = (System.UInt32) (sample_rate * num_channels * (bps / 8));
            block_align = (System.UInt16) (num_channels * (bps / 8));
            // bps;   // BitsPerSample
            if (bps != (uint)BITS_PER_SAMPLE.BPS_8_BITS
                && bps != (uint)BITS_PER_SAMPLE.BPS_16_BITS)
            {
                throw new ApplicationException("ERROR: The bits per sample wasn't set to 8 or 16 Bits ");
            }

            // Fill the data subchunk.
            datachunk_id = System.Text.Encoding.ASCII.GetBytes("data");
            datachunk_size = (System.UInt32) (_numberOfSamples * num_channels * (bps / 8));

            // Fill of the the first chunck size. It has to be made out of order.
            chunk_size = 36 + datachunk_size;
            // No error.
        }

        public string toWAVHeaderString()
        {
            StringBuilder strBuf = new StringBuilder();

            strBuf.AppendLine("################");
            strBuf.AppendLine("WAV Header dump.");
            strBuf.AppendLine("################");
            strBuf.AppendLine(">> RIFF header.");
            strBuf.Append("chunk_id     [char 4]:  "); strBuf.AppendLine(System.Text.Encoding.ASCII.GetString(chunk_id)); //Copy the char[].
            strBuf.AppendFormat("chunk_size     [uint32]:  {0}", chunk_size ); strBuf.AppendLine(); // Just for \n.
            strBuf.Append("format       [char 4]:  "); strBuf.AppendLine(System.Text.Encoding.ASCII.GetString(format));  //Copy the char[].

            strBuf.AppendLine(">> fmt header.");
            strBuf.Append("fmtchunk_id  [char 4]:  "); strBuf.AppendLine(System.Text.Encoding.ASCII.GetString(fmtchunk_id));  //Copy the char[].
            strBuf.AppendFormat("fmtchunk_size  [uint32]:  {0}", fmtchunk_size); strBuf.AppendLine(); // Just for \n.
            strBuf.AppendFormat("audio_format   [uint16]:  {0}", audio_format);  strBuf.AppendLine(); // Just for \n. 
            strBuf.AppendFormat("num_channels   [uint16]:  {0}", num_channels);  strBuf.AppendLine(); // Just for \n.
            strBuf.AppendFormat("sample_rate    [uint32]:  {0}", sample_rate);   strBuf.AppendLine(); // Just for \n.
            strBuf.AppendFormat("byte_rate      [uint32]:  {0}", byte_rate);     strBuf.AppendLine(); // Just for \n.
            strBuf.AppendFormat("block_align    [uint16]:  {0}", block_align);   strBuf.AppendLine(); // Just for \n.
            strBuf.AppendFormat("bps            [uint16]:  {0}", bps);           strBuf.AppendLine(); // Just for \n.
            strBuf.AppendLine(">> data header.");
            strBuf.Append("datachunk_id [char 4]:      "); strBuf.AppendLine(System.Text.Encoding.ASCII.GetString(datachunk_id));  //Copy the char[].
            strBuf.AppendFormat("datachunk_size [uint32]:  {0}", datachunk_size); strBuf.AppendLine(); // Just for \n.

            return strBuf.ToString();
        }

        public bool loadFile()
        {
            string file_path = System.IO.Path.Combine(_path, _file_name);

            if (!File.Exists(file_path))
            {
                throw new ApplicationException("ERROR file " + file_path + " doesn't exists. ");
            }
            if ( (new FileInfo(file_path)).Length < C_HEADER_BYTE_SIZE)
            {
                throw new ApplicationException("ERROR: File " + file_path + " is smalller then the header of a WAV file");
            }

            using (BinaryReader reader = new BinaryReader(File.Open(file_path, FileMode.Open)))
            {
                // Read WAV file header fields.
                chunk_id       = reader.ReadBytes(4);   // Byte[]
                chunk_size     = reader.ReadUInt32();
                format         = reader.ReadBytes(4);   // Byte[]
                fmtchunk_id    = reader.ReadBytes(4);   // Byte[]
                fmtchunk_size  = reader.ReadUInt32();
                audio_format   = reader.ReadUInt16();
                num_channels   = reader.ReadUInt16();
                sample_rate    = reader.ReadUInt32();
                byte_rate      = reader.ReadUInt32();
                block_align    = reader.ReadUInt16();
                bps            = reader.ReadUInt16();   //Bits per sample 
                datachunk_id   = reader.ReadBytes(4);   // Byte[]
                datachunk_size = reader.ReadUInt32();

                // File type validations.
                if (System.Text.Encoding.ASCII.GetString(chunk_id) != "RIFF"
                    || System.Text.Encoding.ASCII.GetString(format) != "WAVE")
                {
                    throw new ApplicationException("ERROR: File " + file_path + " is not a WAV file");
                }
                if (audio_format != 1)
                {
                    throw new ApplicationException("ERROR: File " + file_path + " the API only supports PCM format in WAV.");
                }

                switch ((BITS_PER_SAMPLE)bps)
                {
                    case BITS_PER_SAMPLE.BPS_8_BITS:
                        bufferInternal_uint8 = reader.ReadBytes((int)datachunk_size);
                        _numberOfSamples = datachunk_size / num_channels;
                        break;

                    case BITS_PER_SAMPLE.BPS_16_BITS:
                        // Note: To make the following convertion from byte[] to int16[] I could make unsafe code
                        //       and a simple cast but i'm trying to not make unsafe code.
                        int num_int16 = (int)(datachunk_size / sizeof(System.Int16));
                        bufferInternal_int16 = new System.Int16[num_int16];
                        byte[] two_byte_buf_to_int16;
                        for (int i = 0; i < num_int16; i++)
                        {
                            two_byte_buf_to_int16 = reader.ReadBytes(2);
                            bufferInternal_int16[i] = BitConverter.ToInt16(two_byte_buf_to_int16, 0);
                        }
                        _numberOfSamples = (datachunk_size / 2) / num_channels;
                        break;

                    default:
                        throw new ApplicationException("ERROR: Incorret bits per sample in file " + file_path);
                }
                return true;
            }
        }   // END loadFile().

        public bool writeFile()
        {
            string file_path = System.IO.Path.Combine(_path, _file_name);
            
            // TODO: Test if the file could be open for writing.

            using (BinaryWriter writer = new BinaryWriter(File.Open(file_path, FileMode.Create)))
            {
                // Write all fields of WAV the header.
                writer.Write(chunk_id);        // Byte[4]
                writer.Write(chunk_size);      // UInt32
                writer.Write(format);          // Byte[4]
                writer.Write(fmtchunk_id);     // Byte[4]
                writer.Write(fmtchunk_size);   // UInt32
                writer.Write(audio_format);    // UInt16
                writer.Write(num_channels);    // UInt16
                writer.Write(sample_rate);     // UInt32
                writer.Write(byte_rate);       // UInt32
                writer.Write(block_align);     // UInt16
                writer.Write(bps);             // UInt16
                writer.Write(datachunk_id);    // Byte[4]
                writer.Write(datachunk_size);  // UInt32

                switch ((BITS_PER_SAMPLE)bps)
                {
                    case BITS_PER_SAMPLE.BPS_8_BITS:
                        if (bufferInternal_uint8 == null)
                        {
                            throw new ApplicationException("ERROR: Data buffer uint8 is NULL!");
                        }
                        // Write WAV data buffer.
                        writer.Write(bufferInternal_uint8);
                       break;

                    case BITS_PER_SAMPLE.BPS_16_BITS:
                        if (bufferInternal_int16 == null)
                        {
                            throw new ApplicationException("ERROR: Data buffer int16 is NULL!");
                        }
                        // Note: To make the following convertion from byte[] to int16[] I could make unsafe code
                        //       and a simple cast but i'm trying to not make unsafe code.
                        int num_int16 = (int)(datachunk_size / sizeof(System.Int16));
                        byte[] two_bytes_buf_from_int16;
                        for (int i = 0; i < num_int16; i++)
                        {
                            two_bytes_buf_from_int16 = BitConverter.GetBytes( bufferInternal_int16[i] ); 
                            writer.Write(two_bytes_buf_from_int16);
                        }
                        break;

                    default:
                        throw new ApplicationException("ERROR: Incorret bits per sample to write file " + file_path);
                }
            }

            return true;
        } // END writeFile().

        /////////////////
        // Get Buffers
        ////////////////

        // NOTE: If no file have been readed it return an allocated and inicialized buffer.
        public int getBuffer_8_bits_mono( out System.Byte[] p_new_buffer_8_bits_mono)
        {
            p_new_buffer_8_bits_mono = null;
            if (num_channels == (int)NUM_CHANNELS.TWO)
            {
                throw new ApplicationException("ERROR: getBuffer_8_bits_mono() can't be used for a stereo WAV file!");
            }
            if (bufferInternal_uint8 == null)
            {
                // The buffer hasn't been created, we allocate memory and fill it with center value.
                bufferInternal_uint8 = new System.Byte[datachunk_size];
                System.Byte medium_value = System.Byte.MaxValue / 2;
                for (int i = 0; i < bufferInternal_uint8.Length; i++)
                {
                    bufferInternal_uint8[i] = medium_value;
                }
            }
            p_new_buffer_8_bits_mono = new System.Byte[datachunk_size];
            Array.Copy(bufferInternal_uint8, p_new_buffer_8_bits_mono, datachunk_size);
            return (int)datachunk_size;
        } // END getBuffer_8_bits_mono()

        public int getBuffer_8_bits_stereo(out System.Byte[] newBuffer_1_8_bits_left, out System.Byte[] newBuffer_2_8_bits_right)
        {
            newBuffer_1_8_bits_left  = null;
            newBuffer_2_8_bits_right = null;
            if (num_channels == (int)NUM_CHANNELS.ONE)
            {
                throw new ApplicationException("ERROR: getBuffer_8_bits_stereo() can't be used for a mono WAV file!");
            }
            if (bufferInternal_uint8 == null)
            {
                // The buffer hasn't been created, we allocate memory and fill it with center value.
                bufferInternal_uint8 = new System.Byte[datachunk_size];
                System.Byte medium_value = System.Byte.MaxValue / 2;
                for (int i = 0; i < bufferInternal_uint8.Length; i++)
                {
                    bufferInternal_uint8[i] = medium_value;
                }
            }
            newBuffer_1_8_bits_left  = new System.Byte[datachunk_size / 2];
            newBuffer_2_8_bits_right = new System.Byte[datachunk_size / 2];
                        for (System.UInt32 i = 0; i < datachunk_size; i += 2)
            {
                newBuffer_1_8_bits_left[i / 2]  = bufferInternal_uint8[i];
                newBuffer_2_8_bits_right[i / 2] = bufferInternal_uint8[i + 1];
            }
            return (int) (datachunk_size / 2);
        }  // END getBuffer_8_bits_stereo().

        // NOTE: If no file have been readed it return an allocated and inicialized buffer.
        public int getBuffer_16_bits_mono(out System.Int16[] p_new_buffer_16_bits_mono)
        {
            p_new_buffer_16_bits_mono = null;
            if (num_channels == (int)NUM_CHANNELS.TWO)
            {
                throw new ApplicationException("ERROR: getBuffer_16_bits_mono() can't be used for a stereo WAV file!");
            }
            if (bufferInternal_int16 == null)
            {
                // The buffer hasn't been created, we allocate memory and fill it with center value.
                bufferInternal_int16 = new System.Int16[datachunk_size / 2];
                // Note: The buffer is already initialized to zero. 
            }
            p_new_buffer_16_bits_mono = new System.Int16[datachunk_size / 2];
            Array.Copy(bufferInternal_int16, p_new_buffer_16_bits_mono, datachunk_size / 2);
            return (int) (datachunk_size / 2);
        } // END getBuffer_16_bits_mono()

        public int getBuffer_16_bits_stereo(out System.Int16[] newBuffer_1_16_bits_left, out System.Int16[] newBuffer_2_16_bits_right)
        {
            newBuffer_1_16_bits_left  = null;
            newBuffer_2_16_bits_right = null;
            if (num_channels == (int)NUM_CHANNELS.ONE)
            {
                throw new ApplicationException("ERROR: getBuffer_8_bits_stereo() can't be used for a mono WAV file!");
            }
            if (bufferInternal_int16 == null)
            {
                // The buffer hasn't been created, we allocate memory and fill it with center value (zero).
                bufferInternal_int16 = new System.Int16[datachunk_size  / sizeof(System.UInt16)];
            }
            newBuffer_1_16_bits_left  = new System.Int16[(datachunk_size / 2) / sizeof(System.UInt16)];
            newBuffer_2_16_bits_right = new System.Int16[(datachunk_size / 2) / sizeof(System.UInt16)];
            for (System.UInt32 i = 0; i < datachunk_size / 2; i += 2)
            {
                newBuffer_1_16_bits_left[i / 2]  = bufferInternal_int16[i];
                newBuffer_2_16_bits_right[i / 2] = bufferInternal_int16[i + 1];
            }
            return (int) (datachunk_size / 4);
        }  // END getBuffer_16_bits_stereo().

        /////////////////
        // Set Buffers
        ////////////////

        public int setBuffer_8_bits_mono( System.Byte[] p_new_buffer_8_bits_mono)
        {
            if (p_new_buffer_8_bits_mono == null)
            {
                throw new ApplicationException("ERROR: setBuffer_8_bits_mono() p_new_buffer_8_bits_mono is NULL!");
            }
            if (_numberOfSamples == 0)
            {
                throw new ApplicationException("WARNING: setBuffer_8_bits_mono() the length of the buffer is zero!");
            }
            if (_numberOfSamples != p_new_buffer_8_bits_mono.Length)
            {
                throw new ApplicationException("WARNING: setBuffer_8_bits_mono() the length of the buffer is different from numberOfSamples!");
            }
            if (num_channels == (int)NUM_CHANNELS.TWO)
            {
                throw new ApplicationException("ERROR: setBuffer_8_bits_mono() can't be used for a stereo WAV file!");
            }
            if (bufferInternal_uint8 != null)
            {
                bufferInternal_uint8 = null;
            }
            bufferInternal_uint8 = new System.Byte[_numberOfSamples];
            Array.Copy(p_new_buffer_8_bits_mono, bufferInternal_uint8, _numberOfSamples);
            return (int) _numberOfSamples;
        }  // END setBuffer_8_bits_mono().

        public int setBuffer_8_bits_stereo(System.Byte[] p_new_buffer_1_8_bits_left, System.Byte[] p_new_buffer_2_8_bits_right )
        {
            if (p_new_buffer_1_8_bits_left == null)
            {
                throw new ApplicationException("ERROR: setBuffer_8_bits_stereo() p_new_buffer_1_8_bits_left is NULL!");
            }
            if (p_new_buffer_2_8_bits_right == null)
            {
                throw new ApplicationException("ERROR: setBuffer_8_bits_stereo() p_new_buffer_1_8_bits_right is NULL!");
            }
            if (_numberOfSamples == 0)
            {
                throw new ApplicationException("WARNING: setBuffer_8_bits_stereo() the length of the buffer is zero!");
            }
            if (   _numberOfSamples != p_new_buffer_1_8_bits_left.Length
                || _numberOfSamples != p_new_buffer_2_8_bits_right.Length)
            {
                throw new ApplicationException("WARNING: setBuffer_8_bits_stereo() the length of the buffers is different from numberOfSamples!");
            }
            if (num_channels == (int)NUM_CHANNELS.ONE)
            {
                throw new ApplicationException("ERROR: setBuffer_8_bits_stereo() can't be used for a mono WAV file!");
            }
            if (bufferInternal_uint8 != null)
            {
                bufferInternal_uint8 = null;
            }
            bufferInternal_uint8 = new System.Byte[_numberOfSamples * 2];
            for (uint i = 0; i < (_numberOfSamples * 2); i += 2)
            {
                bufferInternal_uint8[i]     = p_new_buffer_1_8_bits_left[i / 2];
                bufferInternal_uint8[i + 1] = p_new_buffer_2_8_bits_right[i / 2];
            }
            return (int) (_numberOfSamples / 2);
        }  // END setBuffer_8_bits_stereo().

        public int setBuffer_16_bits_mono(System.Int16[] p_new_buffer_16_bits_mono)
        {
            if (p_new_buffer_16_bits_mono == null)
            {
                throw new ApplicationException("ERROR: setBuffer_16_bits_mono() p_new_buffer_16_bits_mono is NULL!");
            }
            if (_numberOfSamples == 0)
            {
                throw new ApplicationException("WARNING: setBuffer_16_bits_mono() the length of the buffer is zero!");
            }
            if (_numberOfSamples != p_new_buffer_16_bits_mono.Length)
            {
                throw new ApplicationException("WARNING: setBuffer_16_bits_mono() the length of the buffer is different from numberOfSamples!");
            }
            if (num_channels == (int)NUM_CHANNELS.TWO)
            {
                throw new ApplicationException("ERROR: setBuffer_16_bits_mono() can't be used for a stereo WAV file!");
            }
            if (bufferInternal_int16 != null)
            {
                bufferInternal_int16 = null;
            }
            bufferInternal_int16 = new System.Int16[_numberOfSamples];
            Array.Copy(p_new_buffer_16_bits_mono, bufferInternal_int16, _numberOfSamples);
            return (int)_numberOfSamples;
        }   // END setBuffer_16_bits_mono().

        public int setBuffer_16_bits_stereo(System.Int16[] p_new_buffer_1_16_bits_left, System.Int16[] p_new_buffer_2_16_bits_right)
        {
            if (p_new_buffer_1_16_bits_left == null)
            {
                throw new ApplicationException("ERROR: setBuffer_16_bits_stereo() p_new_buffer_1_16_bits_left is NULL!");
            }
            if (p_new_buffer_2_16_bits_right == null)
            {
                throw new ApplicationException("ERROR: setBuffer_16_bits_stereo() p_new_buffer_2_8_bits_right is NULL!");
            }
            if (_numberOfSamples == 0)
            {
                throw new ApplicationException("WARNING: setBuffer_8_bits_stereo() the length of the buffer is zero!");
            }
            if (   _numberOfSamples != p_new_buffer_1_16_bits_left.Length
                || _numberOfSamples != p_new_buffer_2_16_bits_right.Length)
            {
                throw new ApplicationException("WARNING: setBuffer_16_bits_stereo() the length of the buffers is different from numberOfSamples!");
            }
            if (num_channels == (int)NUM_CHANNELS.ONE)
            {
                throw new ApplicationException("ERROR: setBuffer_16_bits_stereo() can't be used for a mono WAV file!");
            }
            if (bufferInternal_int16 != null)
            {
                bufferInternal_int16 = null;
            }
            bufferInternal_int16 = new System.Int16[_numberOfSamples * 2];

            for (uint i = 0; i < (_numberOfSamples * 2); i += 2)
            {
                bufferInternal_int16[i]     = p_new_buffer_1_16_bits_left[i / 2];
                bufferInternal_int16[i + 1] = p_new_buffer_2_16_bits_right[i / 2];
            }
            return (int) (_numberOfSamples / 2);
        }  // END setBuffer_16_bits_stereo().


    }   // End of class WAV_file. 
}     // End of namespace.
