# Split NMEA and RTCM

This application takes an input file containing mixed and interleaved NMEA (UTF-8) and RTCM3 (binary) content and separates it into a `.nmea` file and a `.rtcm3` file.

Useful for Sparkfun's [RTK Everywhere firmware](https://github.com/sparkfun/SparkFun_RTK_Everywhere_Firmware/) for [Sparkfun RTK Postcard](https://www.sparkfun.com/sparkfun-rtk-postcard.html) + [Sparkfun Portability Shield](https://www.sparkfun.com/sparkfun-portability-shield.html).

Although many applications can read the txt file directly, there are problems with data loss at the intersection between the RTCM3 binary content and the first NMEA sentence after the binary content. This leads to regular and predictable data loss of a single NMEA sentence for every output segment.

This code snippet reads the txt file as a binary file, looking for RTCM3 binary content, then using the RTCM content length to read exactly the correct number of bytes. That allows it to precisely separate NMEA and RTCM3 content on the condition that the RTCM3 binary data isn't corrupted.

How to run: 
- Option 1: drag and drop a file on the executable
- Option 2: run the exacutable with the file as the first argument: .\SplitNmeaAndRtcm.exe input.txt
- Option 3: run the code with the file as the first argument: dotnet run input.txt


# Development information

How to build the executable:
1. Install .NET 9 SDK: https://dotnet.microsoft.com/en-us/download/dotnet/9.0
2. Ensure you have the required prerequisites for Native AOT compilation: https://aka.ms/nativeaot-prerequisites (use the tabs to select your OS)
2. Run the following command: dotnet publish -c Release -o executable

This a C# console app written in .NET 9, using Native AOT compilation, meaning that the executable is small, fast and does not depend in on .NET runtime being installed on the user's machine.

Should work on any platform where you can build .NET applications, such as Windows, Linux and MacOS.