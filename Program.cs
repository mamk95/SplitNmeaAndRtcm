using System.Text;

string inputFile = args.First();
string nmeaFile = $"{inputFile}.nmea";
string rtcmFile = $"{inputFile}.rtcm3";

using var fs = new FileStream(inputFile, FileMode.Open, FileAccess.Read);
using var nmeaWriter = new StreamWriter(nmeaFile, false, Encoding.UTF8);
using var rtcmWriter = new FileStream(rtcmFile, FileMode.Create, FileAccess.Write);

var buffer = new byte[3]; // Small buffer to check RTCM preamble
var rtcmBuffer = new MemoryStream();

while (fs.Position < fs.Length)
{
    int b = fs.ReadByte();
    if (b == -1) break; // End of file

    var isNmea = b == '$'; // Check for NMEA start
    var isRtcm = b == 0xD3; // Check for RTCM3 Preamble (0xD3)

    if (isNmea)
    {
        string line = ((char)b).ToString();
        while (fs.Position < fs.Length)
        {
            int nextByte = fs.ReadByte();
            if (nextByte == -1 || nextByte == '\n' || nextByte == '\r') break;
            line += (char)nextByte;
        }
        nmeaWriter.WriteLine(line);
    }
    else if (isRtcm)
    {
        // Read next two bytes for length check
        buffer[0] = (byte)b;
        fs.ReadExactly(buffer, 1, 2);
        int rtcmLength = ((buffer[1] & 0x03) << 8) | buffer[2]; // RTCM3 length

        rtcmBuffer.SetLength(0);
        rtcmBuffer.Write(buffer, 0, 3);

        byte[] rtcmPayload = new byte[rtcmLength + 3]; // Include CRC bytes
        int bytesRead = fs.Read(rtcmPayload, 0, rtcmPayload.Length);

        if (bytesRead == rtcmPayload.Length)
        {
            rtcmBuffer.Write(rtcmPayload, 0, bytesRead);
            rtcmWriter.Write(rtcmBuffer.ToArray(), 0, (int)rtcmBuffer.Length);
        }
    }
}
