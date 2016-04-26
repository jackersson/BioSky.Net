using System;
using System.IO;
using Iddk2000DotNet;

namespace Iddk2000Demo
{
  static class Iddk2000Utils
    {
	    public static readonly int SUCCESS  = 0;
      public static readonly int ERROR = -1;

        public static string ReadString(string message)
        {
            Console.Write(message);
            string result = null;
            try
            {
                result = Console.In.ReadLine();
            }
            catch(IOException e)
            {
                TextWriter errorWriter = Console.Error;
                errorWriter.WriteLine(e.Message);
                return null;
            }
            return result;
        }

        public static float ReadFloat(string message, float limitMin, float limitMax, float defaultValue)
        {
            string buffer;
            float result = 1.0f;
            bool rightChoice = false;

            Console.Write(message);

            while (!rightChoice)
            {
                rightChoice = true;
                try
                {
                    buffer = Console.In.ReadLine();
                    if (buffer == null) Environment.Exit(-1);

                    if (buffer.Length == 0)
                    {
                        Console.WriteLine(defaultValue);
                        return defaultValue;
                    }
                    
                    try
                    {
                        result = float.Parse(buffer);
                        if(result < limitMin || result > limitMax) throw new FormatException();
                    }
                    catch (FormatException e)
                    {
                        Console.Write("Invalid value. It should be on the range [" + limitMin + "," + limitMax + "]" + "\n" + message);
                        Console.WriteLine(e.Message);
                        rightChoice = false;
                    }
                }
                catch(IOException e)
                {
                    TextWriter errorWriter = Console.Error;
                    errorWriter.WriteLine(e.Message);
                    return 0;
                }
            }
            
            return result;
        }

        public static int ReadUint(string message, int limitMin, int limitMax, int defaultValue)
        {
            string buffer;
            int result = 1;
            bool rightChoice = false;

            Console.Write(message);

            while (!rightChoice)
            {
                rightChoice = true;
                try
                {
                    buffer = Console.In.ReadLine();
                    if (buffer == null) Environment.Exit(-1);

                    if (buffer.Length == 0)
                    {
                        Console.WriteLine(defaultValue);
                        return defaultValue;
                    }

                    try
                    {
                        result = int.Parse(buffer);
                        if (result < limitMin || result > limitMax) throw new FormatException();
                    }
                    catch (FormatException e)
                    {
                        Console.Write("Invalid value. It should be on the range [" + limitMin + "," + limitMax + "]" + "\n" + message);
                        Console.WriteLine(e.Message);
                        rightChoice = false;
                    }
                }
                catch (IOException e)
                {
                    TextWriter errorWriter = Console.Error;
                    errorWriter.WriteLine(e.Message);
                    return 0;
                }
            }

            return result;
        }

        public static int ChooseOption(int numberOfOpts, int defaultValue)
        {
            string buffer = "";
            int option = -1;
            bool rightChoice = false;

            while (!rightChoice)
            {
                try
                {
                    buffer = Console.In.ReadLine();
                    if (buffer == null) Environment.Exit(-1);
                    if (buffer.Length == 0)
                    {
                        if( defaultValue != -1) Console.Out.WriteLine(defaultValue);
                        return defaultValue;
                    }
                    else
                    {
                        try
                        {
                            option = int.Parse(buffer);
                            if (option > 0 && option <= numberOfOpts)
                                rightChoice = true;
                            else
                                throw new FormatException();
                        }
                        catch (FormatException e)
                        {
                            Console.Out.WriteLine("Invalid number. Please try again !");

                            Console.Out.Write("Select one option: ");
                            Console.Out.WriteLine(e.Message);
                        }
                    }
                }
                catch (IOException e)
                {
                    Console.Out.WriteLine(e.Message);
                }
            }
            return option;
        }

        public static int ChooseOption(int numberOfOpts)
        {
            string buffer = "";
            int option = -1;
            bool rightChoice = false;

            while (!rightChoice)
            {
                try
                {
                    buffer = Console.In.ReadLine();
                    if (buffer == null) Environment.Exit(-1);
                    if (buffer.Length == 0)
                    {
                        return -1;
                    }
                    else
                    {
                        try
                        {
                            option = int.Parse(buffer);
                            if (option > 0 && option <= numberOfOpts)
                                rightChoice = true;
                            else
                                throw new FormatException();
                        }
                        catch (FormatException e)
                        {
                            Console.Out.WriteLine("Invalid number. Please try again !");
                            Console.Out.Write("Select one option: ");
                            Console.WriteLine(e.Message);
                        }
                    }
                }
                catch (IOException e)
                {
                    Console.Out.WriteLine(e.Message);
                }
            }
            return option;
        }

        public static int DisplayMenu(string[] menus, int menuSize, int defaultIdx, string specialComment)
        {
            for (int i = 0; i < menuSize; i++)
            {
                if (i == defaultIdx)
                    Console.Out.Write("\t" + (i+1) + ". " + (menus[i]) + " (default)\n");
                else
                    Console.Out.Write("\t" + (i+1) + ". " + (menus[i]) + "\n");
            }
            Console.Out.Write("Enter your choice (" + specialComment + "): ");
            return ChooseOption(menuSize, defaultIdx);
        }

        public static int DisplayMenu(string[] menus, int menuSize, int defaultIdx)
        {
            for (int i = 0; i < menuSize; i++)
            {
                if (i == defaultIdx)
                    Console.Out.Write("\t" + (i + 1) + ". " + (menus[i]) + " (default)\n");
                else
                    Console.Out.Write("\t" + (i + 1) + ". " + (menus[i]) + "\n");
            }
            Console.Out.Write("Enter your choice: ");
            return ChooseOption(menuSize, defaultIdx);
        }


        public static void Wait(int milliseconds)
        {
            try
            {
                System.Threading.Thread.Sleep(milliseconds);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.Out.WriteLine(e.Message);
            }
        }

        public static bool SaveFile(string fileName, byte[] data)
        {
            BinaryWriter binWriter;
            bool result = false;

            if (fileName == null || fileName == "" || data == null)
                return false;

            try
            {
                binWriter = new BinaryWriter(File.Create(fileName));
                binWriter.Write(data);
                result = true;
                binWriter.Close();
            }
            catch (FileNotFoundException e)
            {
                Console.Out.WriteLine(e.Message);
            }
            return result;
        }

        public static void SaveResultImage(IddkImage image, IddkEyeSubtype eyeSubtype)
        {
            string fileName = "";
            string eyeLabel = eyeSubtype.ToString();
            string time = DateTime.Now.ToString("yyMMddHHmmss");
            
            if (image.ImageFormat == IddkImageFormat.MonoJpeg2000)
            {
                fileName = eyeLabel + "EyeImage_" + time + ".jp2";
            }
            else if (image.ImageFormat == IddkImageFormat.MonoRaw)
            {
                fileName = eyeLabel + "EyeImage_" + image.ImageWidth + "x" + image.ImageHeight + "_" + time + ".raw";
            }
            else if (image.ImageFormat == IddkImageFormat.IritechRaw)
            {
                fileName = eyeLabel + "EyeImage_" + time + "_raw.iri";
            }
            else if (image.ImageFormat == IddkImageFormat.IritechJpeg2000)
            {
                fileName = eyeLabel + "EyeImage_" + time + "_jp2.iri";
            }

            string eyeFilePath = Environment.CurrentDirectory + "\\" + fileName;

            if (Iddk2000Utils.SaveFile(eyeFilePath, image.ImageData))
            {
                Console.Out.Write("\t\tSaved .\\" + fileName + "\n");
            }
            else
            {
                Console.Out.Write("\t\tSaving .\\" + fileName + " failed.\n");
            }
        }

        public static bool ReadFile(string fileName, out IddkDataBuffer data)
        {
            BinaryReader binReader;
            bool result = false;
            data = new IddkDataBuffer();
            data.Data = null;
            try
            {
                binReader = new BinaryReader(File.OpenRead(fileName));
                data.Data = new byte[(int)binReader.BaseStream.Length];
                binReader.Read(data.Data, 0, data.Data.Length);
                result = true;
                binReader.Close();
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
            }
            return result;
        }

        public static void PrepareCaptureParams(out IddkCaptureMode captureMode, out IddkQualityMode qualityMode, out IddkEyeSubtype eyeSubtype, out bool autoLeds, out int iCount, bool isBino)
        {
            IddkConfig config = new IddkConfig();
            captureMode = new IddkCaptureMode();
            qualityMode = new IddkQualityMode();
            eyeSubtype = IddkEyeSubtype.Unknown;
            autoLeds = true;

            Iddk2000APIs.GetSdkConfig(config);

            Console.Out.WriteLine("\nParameters for capturing process");
            Console.Out.WriteLine("Capture mode: \n\t1. IDDK_TIMEBASED (default) \n\t2. IDDK_FRAMEBASED");
            Console.Out.Write("Enter your choice: ");
            switch(ChooseOption(2, 1))
            {
                case 1:
                    captureMode = IddkCaptureMode.TimeBased;
                    break;
                case 2:
                    captureMode = IddkCaptureMode.FrameBased;
                    break;
            }

            if(captureMode == IddkCaptureMode.TimeBased)
                iCount = ReadUint("\nEnter the duration to capture(from 1 to 600 in seconds, enter for default of 3 secs): ", 1, 600, 3);
            else
                iCount = ReadUint("\nEnter the number of frames to capture (from 1 to 600, enter for default of 3 frames):", 1, 600, 3);

            Console.Out.WriteLine("\nQuality mode: \n\t1. Normal (default)\n\t2. High \n\t3. Very High\n");
            Console.Out.Write("Enter your choice: ");
            switch(ChooseOption(3, 1))
            {
                case 1: 
                    qualityMode = IddkQualityMode.Normal;
                    break;
                case 2:
                    qualityMode = IddkQualityMode.High;
                    break;
                case 3:
                    qualityMode = IddkQualityMode.VeryHigh;
                    break;
            }

            Console.Out.Write("\nEnable auto led? \n\t1. Yes (default)\n\t2. No\n");
            Console.Out.Write("Enter your choice: ");
            switch (ChooseOption(2, 1))
            {
                case 1:
                    autoLeds = true;
                    break;
                case 2:
                    autoLeds = false;
                    break;
            }

            if (isBino)
            {

                Console.Out.WriteLine("\nSpecify eye subtype: \n\t1. Unknown eye(default)\n\t2. Left Eye(Binocular device only) \n\t3. Right Eye(Binocular device only)\n");
                Console.Out.Write("Enter your choice: ");
                switch (ChooseOption(3, 1))
                {
                    case 1:
                        eyeSubtype = IddkEyeSubtype.Both;
                        break;
                    case 2:
                        eyeSubtype = IddkEyeSubtype.Left;
                        break;
                    case 3:
                        eyeSubtype = IddkEyeSubtype.Right;
                        break;
                }
            }
        }
    }
}
