using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.IO;

namespace Game.Objects.Planes
{
    public class Plane
    {
        public Plane()
        {
            LoadModel();
            CalculateNormals();
        }
     
        public string filePath = "C:\\Users\\deakt\\Documents\\yoda.obj";

        public uint[] _indices;
        public float[] _vertices;
        public List<uint> _indices_main = new List<uint>();
        public List<float> _vertices_main = new List<float>();
        public List<float> _vertices4 = new List<float>();
        private float[] _vertices2;
        private uint[] _indices2;

        public void LoadModel2()
        {
            try
            {
                // Fájl sorainak beolvasása egy tömbbe
                string[] lines = File.ReadAllLines(filePath);

                foreach (string line in lines)
                {
                    if (line.StartsWith("v "))
                    {
                        string line2 = line.Substring(2);
                        string[] line3 = line2.Split(' ');
                        foreach (var item in line3)
                        {
                            string fixedItem = item.Replace('.', ',');
                            if (float.TryParse(fixedItem, out float x))
                            {
                                _vertices4.Add(x);
                            }
                            else
                            {
                                Console.WriteLine($"Nem sikerült konvertálni a következő értéket: {item}");
                            }
                        }
                    }
                    else if (line.StartsWith("f "))
                    {
                        string line2 = line.Substring(2);
                        string[] line3 = line2.Split(' ');
                        _indices_main.Add(uint.Parse(line3[0]) - 1);
                        _indices_main.Add(uint.Parse(line3[1]) - 1);
                        _indices_main.Add(uint.Parse(line3[2]) - 1);
                        _indices_main.Add(uint.Parse(line3[2]) - 1);
                        _indices_main.Add(uint.Parse(line3[3]) - 1);
                        _indices_main.Add(uint.Parse(line3[0]) - 1);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Hiba történt: " + ex.Message);
            }

            _indices2 = _indices_main.ToArray();
            _vertices2 = _vertices4.ToArray();
        }
        public void LoadModel()
        {
            try
            {

                // Fájl sorainak beolvasása egy tömbbe
                string[] lines = File.ReadAllLines(filePath);

                // Jegyzettömb kiíratása

                foreach (string line in lines)
                {
                    //  Console.WriteLine(line);
                    if (line.StartsWith('f'))
                    {
                        string line2 = line.Remove(0, 2);
                        string[] line3 = line2.Split(' ');
                        for (int i = 0; i < line3.Length; i++)
                        {
                            uint x = uint.Parse(line3[i]) - 1;
                            _indices_main.Add(x);
                        }
                    }
                    if (line.StartsWith('v'))
                    {
                        string line2 = line.Remove(0, 2);
                        string[] line3 = line2.Split(' ');
                        for (int i = 0; i < line3.Length; i++)
                        {
                            line3[i] = line3[i].Replace('.', ',');
                            float x = float.Parse(line3[i]);
                            _vertices4.Add(x);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Hiba történt: " + ex.Message);
            }


            _indices2 = _indices_main.ToArray();
            _vertices2 = _vertices4.ToArray();
        }

        public void CalculateNormals()
        {
            Vector3[] normals = new Vector3[_indices2.Length / 3];

            for (int i = 0; i < _indices2.Length; i += 3)
            {
                uint index1 = _indices2[i];
                uint index2 = _indices2[i + 1];
                uint index3 = _indices2[i + 2];

                Vector3 v1 = new Vector3(_vertices2[index1 * 3], _vertices2[index1 * 3 + 1], _vertices2[index1 * 3 + 2]);
                Vector3 v2 = new Vector3(_vertices2[index2 * 3], _vertices2[index2 * 3 + 1], _vertices2[index2 * 3 + 2]);
                Vector3 v3 = new Vector3(_vertices2[index3 * 3], _vertices2[index3 * 3 + 1], _vertices2[index3 * 3 + 2]);

                Vector3 edge1 = v2 - v1;
                Vector3 edge2 = v3 - v1;

                Vector3 normal = Vector3.Cross(edge1, edge2);
                normal = Vector3.Normalize(normal);

                normals[i / 3] = normal;
            }

            for (int i = 0; i < _indices2.Length; i += 3)
            {
                uint index1 = _indices2[i];
                uint index2 = _indices2[i + 1];
                uint index3 = _indices2[i + 2];

                Vector3 normal = normals[i / 3];

                _vertices_main.Add(_vertices2[index1 * 3]);
                _vertices_main.Add(_vertices2[index1 * 3 + 1]);
                _vertices_main.Add(_vertices2[index1 * 3 + 2]);
                _vertices_main.Add(normal.X);
                _vertices_main.Add(normal.Y);
                _vertices_main.Add(normal.Z);

                _vertices_main.Add(_vertices2[index2 * 3]);
                _vertices_main.Add(_vertices2[index2 * 3 + 1]);
                _vertices_main.Add(_vertices2[index2 * 3 + 2]);
                _vertices_main.Add(normal.X);
                _vertices_main.Add(normal.Y);
                _vertices_main.Add(normal.Z);

                _vertices_main.Add(_vertices2[index3 * 3]);
                _vertices_main.Add(_vertices2[index3 * 3 + 1]);
                _vertices_main.Add(_vertices2[index3 * 3 + 2]);
                _vertices_main.Add(normal.X);
                _vertices_main.Add(normal.Y);
                _vertices_main.Add(normal.Z);
            }

            _vertices = _vertices_main.ToArray();
        }
    }
}
