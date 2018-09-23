using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace SpriteRotate
{
    class Program
    {
        private static readonly byte[] memdmp = File.ReadAllBytes("memdmp.bin");

        static void Main(string[] args)
        {
            Bitmap bmpNewTiles = new Bitmap(@"..\..\newtiles.png");

            FileStream fs = new FileStream("TILES.MAC", FileMode.Create);
            StreamWriter writer = new StreamWriter(fs);
            writer.WriteLine("; START OF TILES.MAC");
            writer.WriteLine();
            writer.WriteLine("; Блок тайлов поезда, фаза на крыше, 416. байт, 13 строк по 32 тайла");
            writer.WriteLine("\t.EVEN");
            DumpBytes(writer, 11808, 416, 9);  // 027040
            writer.WriteLine();
            writer.WriteLine("; Блок тайлов поезда, фаза в вагоне");
            writer.WriteLine("\t.EVEN");
            DumpBytes(writer, 12224, 256);  // 027700
            writer.WriteLine(";");
            writer.WriteLine("; Тайлы локомотива");
            writer.WriteLine("Z30300::");
            writer.WriteLine("\t;NOTE: Тайлы локомотива убраны -- нехватает места");
            writer.WriteLine(";");
            DumpBytes(writer, 12928, 32);  // 031200
            DumpBytes(writer, 12960, 20);  // 031240
            writer.WriteLine(";");
            writer.WriteLine("; Экран описания -- 24 строки по 32 символа");
            writer.WriteLine("Z31264:");
            writer.WriteLine("\t;NOTE: Экран с описанием игры убран, для экономии памяти");
            writer.WriteLine();

            writer.WriteLine("; Блок тайлов");
            writer.WriteLine("\t.EVEN");
            RotatePatternTiles(writer, bmpNewTiles, 0x20, 9, "TILE20");
            RotateNewTiles(writer, bmpNewTiles, 0x30, 16, "TILE30");
            RotateNewTiles(writer, bmpNewTiles, 0x40, 27, "TILE40");
            RotateNewTiles(writer, bmpNewTiles, 0x60, 32, "TILE60");
            RotateNewTiles(writer, bmpNewTiles, 0x80, 128, "TILE80");
            writer.WriteLine();

            writer.WriteLine("; Блок тайлов для фазы в вагоне");
            writer.WriteLine("\t.EVEN");
            writer.WriteLine("TILEEX:");
            RotateExtraTiles(writer, bmpNewTiles, 8, 0x88, 8);
            RotateExtraTiles(writer, bmpNewTiles, 8, 0x98, 6);
            RotateExtraTiles(writer, bmpNewTiles, 10, 0xA8, 7);
            RotateExtraTiles(writer, bmpNewTiles, 9, 0xB8, 7);
            RotateExtraTiles(writer, bmpNewTiles, 13, 0xCA, 4);
            RotateExtraTiles(writer, bmpNewTiles, 12, 0xDA, 4);
            RotateExtraTiles(writer, bmpNewTiles, 12, 0xEB, 4);
            RotateExtraTiles(writer, bmpNewTiles, 12, 0xFB, 4);
            writer.WriteLine("\t.BYTE\t000,000");
            writer.WriteLine();

            writer.WriteLine("; Сжатая демо-последовательность");
            writer.WriteLine("Z44274::");
            writer.WriteLine("\t;NOTE: Демо-последовательность убрана -- нехватает места");
            writer.WriteLine();

            writer.WriteLine("\t.EVEN");
            writer.WriteLine("; END OF TILES.MAC");

            writer.Flush();
        }

        static void RotateNewTiles(StreamWriter writer, Bitmap bmpTiles, int tileoffset, int tilecount, string label)
        {
            writer.WriteLine("{0}:", label);

            for (int tile = tileoffset; tile < tileoffset + tilecount; tile++)
            {
                writer.Write("\t.WORD\t");
                int basex = 10 + (tile % 16) * 10;
                int basey = 10 + (tile / 16) * 10;

                RotateNewTile(writer, bmpTiles, tile, basex, basey);

                writer.WriteLine("\t; {0}", EncodeOctalString((byte)tile));
            }
        }

        static void RotatePatternTiles(StreamWriter writer, Bitmap bmpTiles, int tileoffset, int tilecount, string label)
        {
            writer.WriteLine("{0}:", label);

            for (int tile = tileoffset; tile < tileoffset + tilecount; tile++)
            {
                if (tile % 8 == 0)
                    writer.Write("\t.WORD\t");
                int basex = 10 + (tile % 16) * 10;
                int basey = 10 + (tile / 16) * 10;

                int bb = 0;
                for (int x = 0; x < 8; x++)
                {
                    Color color = bmpTiles.GetPixel(basex + (7 - x), basey);
                    int index = ColorToIndex(color);
                    bb = bb << 2;
                    bb |= index;
                }

                writer.Write(EncodeOctalString2(bb));
                if (tile % 8 < 7)
                    writer.Write(",");
                else
                    writer.WriteLine();

                //writer.WriteLine("\t; {0}", EncodeOctalString((byte)tile));
            }
        }

        static void RotateExtraTiles(StreamWriter writer, Bitmap bmpTiles, int skiptiles, int tileoffset, int tilecount)
        {
            writer.WriteLine("\t.BYTE\t{0},{1}", EncodeOctalString((byte)(skiptiles * 8)), EncodeOctalString((byte)tilecount));

            for (int tile = tileoffset; tile < tileoffset + tilecount; tile++)
            {
                writer.Write("\t.WORD\t");
                int basex = 90 + (tile % 16) * 10;
                int basey = 10 + (tile / 16) * 10;

                RotateNewTile(writer, bmpTiles, tile, basex, basey);

                writer.WriteLine("\t; {0}", EncodeOctalString((byte)tile));
            }
        }

        static void RotateNewTile(StreamWriter writer, Bitmap bmpTiles, int tile, int basex, int basey)
        {
            for (int i = 0; i < 8; i++)
            {
                int bb = 0;
                for (int x = 0; x < 8; x++)
                {
                    Color color = bmpTiles.GetPixel(basex + (7 - x), basey + i);
                    int index = ColorToIndex(color);
                    bb = bb << 2;
                    bb |= index;
                }

                if ((tile < 91 && tile >= 0x30) && (i != 2 && i != 4 && i != 7) ||
                    (tile < 0x30 || tile >= 91) && ((i & 1) == 0))
                {
                    writer.Write(EncodeOctalString2(bb));
                    if (i < 6)
                        writer.Write(",");
                }
            }
        }

        static int ColorToIndex(Color color)
        {
            if ((color.ToArgb() & 0xffffff) == 0xff0000) return 3;  // Red
            if ((color.ToArgb() & 0xffffff) == 0x00ff00) return 2;  // Green
            if ((color.ToArgb() & 0xffffff) == 0x0000ff) return 1;  // Blue
            if ((color.ToArgb() & 0xffffff) == 0xffff00) return 3;  // Yellow -> Red
            if ((color.ToArgb() & 0xffffff) == 0xffffff) return 3;  // White -> Red
            return 0;
        }

        static void DumpBytes(StreamWriter writer, int address, int length, int numlines = -1)
        {
            string saddress = "Z" + EncodeOctalString2(address).Substring(1);
            writer.Write("{0}::", saddress);
            for (int i = 0; i < length; i++)
            {
                if (i == 0)
                    writer.Write(".BYTE\t");
                else if (i % 16 == 0)
                    writer.Write("\t.BYTE\t");
                else
                    writer.Write(",");

                byte b = memdmp[address + i];
                writer.Write(EncodeOctalString(b));

                if (numlines >= 0 && i % 32 == 15)
                {
                    writer.Write("\t; {0}", numlines);
                    numlines++;
                }
                if (i % 16 == 15)
                    writer.WriteLine();
            }
            if (length % 16 != 0)
                writer.WriteLine();
        }

        static string EncodeOctalString(byte value)
        {
            //convert to int, for cleaner syntax below. 
            int x = (int)value;

            return string.Format(
                @"{0}{1}{2}",
                ((x >> 6) & 7),
                ((x >> 3) & 7),
                (x & 7)
            );
        }

        static string EncodeOctalString2(int x)
        {
            return string.Format(
                @"{0}{1}{2}{3}{4}{5}",
                ((x >> 15) & 7),
                ((x >> 12) & 7),
                ((x >> 9) & 7),
                ((x >> 6) & 7),
                ((x >> 3) & 7),
                (x & 7)
            );
        }
    }
}
