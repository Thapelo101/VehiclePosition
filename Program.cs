using System;
using System.Collections.Generic;
using System.IO;

namespace NearestVehiclePositions
{
    class VehiclePosition
    {
        public int VehicleId { get; set; }
        public string VehicleRegistration { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public ulong RecordedTimeUTC { get; set; }
    }

    class Coordinate
    {
        public int PositionNumber { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public VehiclePosition NearestVehicle { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // File paths
            string binaryFilePath = "C:/Users/thapelo.chuene/OneDrive - Smollan/Desktop/VehiclePosition/VehiclePositions_DataFile/VehiclePositions.dat";

            // Coordinates to find nearest vehicles for
            List<Coordinate> coordinates = new List<Coordinate>()
            {
                new Coordinate() { PositionNumber = 1, Latitude = 34.544909f, Longitude = -102.100843f },
                new Coordinate() { PositionNumber = 2, Latitude = 32.345544f, Longitude = -99.123124f },
                new Coordinate() { PositionNumber = 3, Latitude = 33.234235f, Longitude = -100.214124f },
                new Coordinate() { PositionNumber = 4, Latitude = 35.195739f, Longitude = -95.348899f },
                new Coordinate() { PositionNumber = 5, Latitude = 31.895839f, Longitude = -97.789573f },
                new Coordinate() { PositionNumber = 6, Latitude = 32.895839f, Longitude = -101.789573f },
                new Coordinate() { PositionNumber = 7, Latitude = 34.115839f, Longitude = -100.225732f },
                new Coordinate() { PositionNumber = 8, Latitude = 32.335839f, Longitude = -99.992232f },
                new Coordinate() { PositionNumber = 9, Latitude = 33.535339f, Longitude = -94.792232f },
                new Coordinate() { PositionNumber = 10, Latitude = 32.234235f, Longitude = -100.222222f }
            };

            // Load vehicle positions from binary file
            List<VehiclePosition> vehiclePositions = LoadVehiclePositions(binaryFilePath);

            // Find nearest vehicle for each coordinate
            foreach (var coordinate in coordinates)
            {
                FindNearestVehicle(coordinate, vehiclePositions);
            }

            // Print the results
            foreach (var coordinate in coordinates)
            {
                Console.WriteLine($"Position #{coordinate.PositionNumber}: " +
                                  $"Nearest Vehicle ID: {coordinate.NearestVehicle.VehicleId}, " +
                                  $"Distance: {CalculateDistance(coordinate, coordinate.NearestVehicle):F2}");
            }
        }

        static List<VehiclePosition> LoadVehiclePositions(string filePath)
        {
            List<VehiclePosition> vehiclePositions = new List<VehiclePosition>();

            using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
            using (BinaryReader reader = new BinaryReader(fileStream))
            {
                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    VehiclePosition position = new VehiclePosition();
                    position.VehicleId = reader.ReadInt32();
                    position.VehicleRegistration = ReadNullTerminatedString(reader);
                    position.Latitude = reader.ReadSingle();
                    position.Longitude = reader.ReadSingle();
                    position.RecordedTimeUTC = reader.ReadUInt64();

                    vehiclePositions.Add(position);
                }
            }

            return vehiclePositions;
        }

        static string ReadNullTerminatedString(BinaryReader reader)
        {
            List<byte> bytes = new List<byte>();
            byte b;

            while ((b = reader.ReadByte()) != 0)
            {
                bytes.Add(b);
            }

            return System.Text.Encoding.ASCII.GetString(bytes.ToArray());
        }

        static void FindNearestVehicle(Coordinate coordinate, List<VehiclePosition> vehiclePositions)
        {
            float minDistance = float.MaxValue;
            VehiclePosition nearestVehicle = null;

            foreach (var vehicle in vehiclePositions)
            {
                float distance = CalculateDistance(coordinate, vehicle);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestVehicle = vehicle;
                }
            }

            coordinate.NearestVehicle = nearestVehicle;
        }

        static float CalculateDistance(Coordinate coordinate, VehiclePosition vehiclePosition)
        {
            float latDiff = coordinate.Latitude - vehiclePosition.Latitude;
            float lonDiff = coordinate.Longitude - vehiclePosition.Longitude;

            return (float)Math.Sqrt(latDiff * latDiff + lonDiff * lonDiff);
        }
    }
}
