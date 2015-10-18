﻿/*
Copyright (c) 2015, Joao Nuno Carvalho

All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
Neither the author nor the names of any contributors may be used to endorse or promote products derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;
using System.Collections.Generic;

namespace WAV_Tools_C_Sharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("&&&&&&&&&&&&&&&&&&");
            Console.WriteLine("&&& Wave Tools &&&");
            Console.WriteLine("&&&&&&&&&&&&&&&&&&");

            Console.WriteLine();
            Console.WriteLine(">>>> BEGIN test1_ReadWriteWAVFile()");
            test1_ReadWrite8BitsMonoWAVFile();
            Console.WriteLine(">>>> END test1_ReadWriteWAVFile()");
            Console.WriteLine();

            Console.WriteLine(">>>> BEGIN test2_generate8BitsMono440HzWAVFile()");
            test2_generate8BitsMono440HzWAVFile();
            Console.WriteLine(">>>> END test2_generate8BitsMono440HzWAVFile()");
            Console.WriteLine();

            Console.WriteLine(">>>> BEGIN test3_generate16BitsMono440HzWAVFile()");
            test3_generate16BitsMono440HzWAVFile();
            Console.WriteLine(">>>> END test3_generate16BitsMono440HzWAVFile()");
            Console.WriteLine();

            Console.WriteLine(">>>> BEGIN test4_Read8BitsMonoWrite8bitsStereoWAVFile()");
            test4_Read8BitsMonoWrite8bitsStereoWAVFile();
            Console.WriteLine(">>>> END test4_Read8BitsMonoWrite8bitsStereoWAVFile()()");
            Console.WriteLine();

            Console.WriteLine(">>>> BEGIN test5_Read16BitsMonoWrite16bitsStereoWAVFile()");
            test5_Read16BitsMonoWrite16bitsStereoWAVFile();
            Console.WriteLine(">>>> END test5_Read16BitsMonoWrite16bitsStereoWAVFile()");
            Console.WriteLine();

            Console.WriteLine(">>>> BEGIN test6_ReadWrite8BitsStereoWAVFile()");
            test6_ReadWrite8BitsStereoWAVFile();
            Console.WriteLine(">>>> END test6_ReadWrite8BitsStereoWAVFile()");
            Console.WriteLine();

            Console.WriteLine(">>>> BEGIN test7_ReadWrite16BitsStereoWAVFile()");
            test7_ReadWrite16BitsStereoWAVFile();
            Console.WriteLine(">>>> END test7_ReadWrite16BitsStereoWAVFile()");
            Console.WriteLine();
        }

        private static int test1_ReadWrite8BitsMonoWAVFile()
        {
            WAV_file my_wav_file = new WAV_file();
            my_wav_file.File_name = "dog_bark.wav";
            Console.WriteLine( "Nome: " + my_wav_file.File_name );
            my_wav_file.Path = ".\\";
            // my_wav_file.Path = ".\\Debug\\wav_files\\";
            Console.WriteLine( "Path: " + my_wav_file.Path );
            my_wav_file.loadFile();
            Console.Write(my_wav_file.toWAVHeaderString());

            System.Byte[] buffer_8_bits_Mono;
            uint numberOfSamples = (uint) my_wav_file.getBuffer_8_bits_mono( out buffer_8_bits_Mono);

            // Buffer processing.
            Console.WriteLine();
            Console.WriteLine();
            for (uint i = 0; i < numberOfSamples; i++)
            {
                //		Console.Write( buffer_8_bits_Mono[i].toString() +  ".");
            }
            Console.WriteLine();
            Console.WriteLine();

            // We are going to save the file on the hard drive with another name.
            my_wav_file.File_name = "test1_dog_bark_saved.wav";
            my_wav_file.initializeWaveHeaderStructBeforeWriting();
            my_wav_file.setBuffer_8_bits_mono(buffer_8_bits_Mono);
            my_wav_file.writeFile();
            return 0;
        }  // END

        private static int test2_generate8BitsMono440HzWAVFile()
        {
            WAV_file my_wav_file = new WAV_file();
            my_wav_file.File_name = "test2_A440_8_Bits_Mono.wav";
            Console.WriteLine("Nome: " + my_wav_file.File_name);
            my_wav_file.Path = ".\\";
            // my_wav_file.Path = ".\\Debug\\wav_files\\";
            Console.WriteLine("Path: " + my_wav_file.Path);
                        
            const System.UInt32 C_SAMPLES_PER_SEC = 8000;
            const System.UInt32 C_TIME_SEC = 5;

            my_wav_file.BitsPerSample   = WAV_file.BITS_PER_SAMPLE.BPS_8_BITS;
            my_wav_file.NumOfChannels   = WAV_file.NUM_CHANNELS.ONE;
            my_wav_file.SampleRate      = C_SAMPLES_PER_SEC;
            my_wav_file.NumberOfSamples = C_SAMPLES_PER_SEC * C_TIME_SEC; // 5 segundos

            my_wav_file.initializeWaveHeaderStructBeforeWriting();

            System.Byte[] buffer_8_bits_Mono;
            // Allocate the memory for the buffer.
            my_wav_file.getBuffer_8_bits_mono(out buffer_8_bits_Mono);


            // Fills the buffer with a sinusoidal.
            float alphaAngle = 0;
            const System.UInt32 C_SIGNAL_FREQUENCY = 440; // 440 Hz La (A)
            for (uint i = 0; i < my_wav_file.NumberOfSamples; i++)
            {
                buffer_8_bits_Mono[i] = (System.Byte)  ( ( (Math.Sin(alphaAngle) * (System.Byte.MaxValue / 2)) + (System.Byte.MaxValue / 2) ) );
                alphaAngle += (2 * (float) (Math.PI) * C_SIGNAL_FREQUENCY) / C_SAMPLES_PER_SEC;
            }

            my_wav_file. setBuffer_8_bits_mono(buffer_8_bits_Mono);
            my_wav_file.writeFile();
            return 0;
        }  // END 

        private static int test3_generate16BitsMono440HzWAVFile()
        {
            WAV_file my_wav_file = new WAV_file();
            my_wav_file.File_name = "test3_A440_16_Bits_Mono.wav";
            Console.WriteLine("Nome: " + my_wav_file.File_name);
            my_wav_file.Path = ".\\";
            // my_wav_file.Path = ".\\Debug\\wav_files\\";
            Console.WriteLine("Path: " + my_wav_file.Path);
                        
            const System.UInt32 C_SAMPLES_PER_SEC = 8000;
            const System.UInt32 C_TIME_SEC = 5;

            my_wav_file.BitsPerSample   = WAV_file.BITS_PER_SAMPLE.BPS_16_BITS;
            my_wav_file.NumOfChannels   = WAV_file.NUM_CHANNELS.ONE;
            my_wav_file.SampleRate      = C_SAMPLES_PER_SEC;
            my_wav_file.NumberOfSamples = C_SAMPLES_PER_SEC * C_TIME_SEC; // 5 segundos

            my_wav_file.initializeWaveHeaderStructBeforeWriting();

            System.Int16[] buffer_16_bits_Mono;
            // Allocate the memory for the buffer.
            my_wav_file.getBuffer_16_bits_mono(out buffer_16_bits_Mono);


            // Fills the buffer with a sinusoidal.
            float alphaAngle = 0;
            const System.UInt32 C_SIGNAL_FREQUENCY = 440; // 440 Hz La (A)
            for (uint i = 0; i < my_wav_file.NumberOfSamples; i++)
            {
                buffer_16_bits_Mono[i] = (System.Int16) (Math.Round(Math.Sin(alphaAngle) * System.Int16.MaxValue));
                alphaAngle += (2 * (float)(Math.PI) * C_SIGNAL_FREQUENCY) / C_SAMPLES_PER_SEC;
            }

            my_wav_file.setBuffer_16_bits_mono(buffer_16_bits_Mono);
            my_wav_file.writeFile();
            return 0;
        }  // END 

        private static int test4_Read8BitsMonoWrite8bitsStereoWAVFile()
        {
            WAV_file my_wav_file = new WAV_file();
            my_wav_file.File_name = "test2_A440_8_Bits_Mono.wav";
            Console.WriteLine("Nome: " + my_wav_file.File_name);
            my_wav_file.Path = ".\\";
            // my_wav_file.Path = ".\\Debug\\wav_files\\";
            Console.WriteLine("Path: " + my_wav_file.Path);
            my_wav_file.loadFile();
            Console.Write(my_wav_file.toWAVHeaderString());

            System.Byte[] buffer_8_bits_Mono;
            uint numberOfSamples = (uint)my_wav_file.getBuffer_8_bits_mono(out buffer_8_bits_Mono);

            // Buffer processing.
            Console.WriteLine();
            Console.WriteLine();
            for (uint i = 0; i < numberOfSamples; i++)
            {
                //		Console.Write( buffer_8_bits_Mono[i].toString() +  ".");
            }
            Console.WriteLine();
            Console.WriteLine();

            // We are going to save the file on the hard drive with another name.
            my_wav_file.File_name = "test4_A440_8_Bits_Stereo.wav";
            my_wav_file.NumOfChannels = WAV_file.NUM_CHANNELS.TWO;
            my_wav_file.initializeWaveHeaderStructBeforeWriting();
            my_wav_file.setBuffer_8_bits_stereo(buffer_8_bits_Mono, buffer_8_bits_Mono);
            my_wav_file.writeFile();
            return 0;
        }  // END 

        private static int test5_Read16BitsMonoWrite16bitsStereoWAVFile()
        {
            WAV_file my_wav_file = new WAV_file();
            my_wav_file.File_name = "test3_A440_16_Bits_Mono.wav";
            Console.WriteLine("Nome: " + my_wav_file.File_name);
            my_wav_file.Path = ".\\";
            // my_wav_file.Path = ".\\Debug\\wav_files\\";
            Console.WriteLine("Path: " + my_wav_file.Path);
            my_wav_file.loadFile();
            Console.Write(my_wav_file.toWAVHeaderString());

            System.Int16[] buffer_16_bits_Mono;
            uint numberOfSamples = (uint)my_wav_file.getBuffer_16_bits_mono(out buffer_16_bits_Mono);

            // Buffer processing.
            Console.WriteLine();
            Console.WriteLine();
            for (uint i = 0; i < numberOfSamples; i++)
            {
                //		Console.Write( buffer_16_bits_Mono[i].toString() +  ".");
            }
            Console.WriteLine();
            Console.WriteLine();

            // We are going to save the file on the hard drive with another name.
            my_wav_file.File_name = "test5_A440_16_Bits_Stereo.wav";
            my_wav_file.NumOfChannels = WAV_file.NUM_CHANNELS.TWO;
            my_wav_file.initializeWaveHeaderStructBeforeWriting();
            my_wav_file.setBuffer_16_bits_stereo(buffer_16_bits_Mono, buffer_16_bits_Mono);
            my_wav_file.writeFile();
            return 0;
        }  // END 

        private static int test6_ReadWrite8BitsStereoWAVFile()
        {
            WAV_file my_wav_file = new WAV_file();
            my_wav_file.File_name = "test4_A440_8_Bits_Stereo.wav";
            Console.WriteLine("Nome: " + my_wav_file.File_name);
            my_wav_file.Path = ".\\";
            // my_wav_file.Path = ".\\Debug\\wav_files\\";
            Console.WriteLine("Path: " + my_wav_file.Path);
            my_wav_file.loadFile();
            Console.Write(my_wav_file.toWAVHeaderString());

            System.Byte[] buffer_8_bits_stereo_left;
            System.Byte[] buffer_8_bits_stereo_right;
            uint numberOfSamples = (uint)my_wav_file.getBuffer_8_bits_stereo(out buffer_8_bits_stereo_left, out buffer_8_bits_stereo_right);

            // Buffer processing.
            Console.WriteLine();
            Console.WriteLine();
            for (uint i = 0; i < numberOfSamples; i++)
            {
                //		Console.Write( buffer_8_bits_stereo_left[i].toString() +  ".");
                //		Console.Write( buffer_8_bits_stereo_right[i].toString() +  ".");

            }
            Console.WriteLine();
            Console.WriteLine();

            // We are going to save the file on the hard drive with another name.
            my_wav_file.File_name = "test6_A440_8_Bits_Stereo_saved.wav";
            my_wav_file.initializeWaveHeaderStructBeforeWriting();
            my_wav_file.setBuffer_8_bits_stereo(buffer_8_bits_stereo_left, buffer_8_bits_stereo_right);
            my_wav_file.writeFile();
            return 0;
        }  // END .

        private static int test7_ReadWrite16BitsStereoWAVFile()
        {
            WAV_file my_wav_file = new WAV_file();
            my_wav_file.File_name = "test5_A440_16_Bits_Stereo.wav";
            Console.WriteLine("Nome: " + my_wav_file.File_name);
            my_wav_file.Path = ".\\";
            // my_wav_file.Path = ".\\Debug\\wav_files\\";
            Console.WriteLine("Path: " + my_wav_file.Path);
            my_wav_file.loadFile();
            Console.Write(my_wav_file.toWAVHeaderString());

            System.Int16[] buffer_16_bits_stereo_left;
            System.Int16[] buffer_16_bits_stereo_right;
            uint numberOfSamples = (uint)my_wav_file.getBuffer_16_bits_stereo(out buffer_16_bits_stereo_left, out buffer_16_bits_stereo_right);

            // Buffer processing.
            Console.WriteLine();
            Console.WriteLine();
            for (uint i = 0; i < numberOfSamples; i++)
            {
                //		Console.Write( buffer_16_bits_stereo_left[i].toString() +  ".");
                //		Console.Write( buffer_16_bits_stereo_right[i].toString() +  ".");

            }
            Console.WriteLine();
            Console.WriteLine();

            // We are going to save the file on the hard drive with another name.
            my_wav_file.File_name = "test7_A440_16_Bits_Stereo_saved.wav";
            my_wav_file.initializeWaveHeaderStructBeforeWriting();
            my_wav_file.setBuffer_16_bits_stereo(buffer_16_bits_stereo_left, buffer_16_bits_stereo_right);
            my_wav_file.writeFile();
            return 0;
        } // END

    }  // END Class Program.
}
