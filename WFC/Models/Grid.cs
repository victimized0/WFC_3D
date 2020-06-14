using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace WFC {
    interface IGrid {
        public int GetCountX();
        public int GetCountY();
        public int GetCountZ();
        public int[] GetCells();
        public int Index(int x, int y, int z);
        public (int, int, int) Coords(int index);
        public bool IsValid(int x, int y, int z);
        public bool IsValid(int index);
        public void ToModel(Preset preset);
    }

    class UniformGrid : IGrid {
        public int CountX { get; protected set; }
        public int CountY { get; protected set; }
        public int CountZ { get; protected set; }
        public float CellSize { get; protected set; }
        public int[] Cells { get; set; }

        public UniformGrid(int x, int y, int z, float cellSize) {
            CountX      = x;
            CountY      = y;
            CountZ      = z;
            CellSize    = cellSize;
            Cells       = new int[CountX * CountY * CountZ];

            //for (int xi = 0; xi < CountX; xi++) {
            //    Cells[xi] = new int[CountY][];
            //    for (int yi = 0; yi < CountX; yi++) {
            //        Cells[xi][yi] = new int[CountZ];
            //    }
            //}
        }

        public int GetCountX() {
            return CountX;
        }

        public int GetCountY() {
            return CountY;
        }

        public int GetCountZ() {
            return CountZ;
        }

        public int[] GetCells() {
            return Cells;
        }

        public int Index(int x, int y, int z) {
            return x + CountX * (y + CountY * z);
        }

        public (int, int, int) Coords(int index) {
            int z = index / (CountX * CountY);
            index -= (z * CountX * CountY);
            int y = index / CountX;
            int x = index % CountX;
            return (x, y, z);
        }

        public bool IsValid(int x, int y, int z) {
            return (x >= 0 && x < CountX && y >= 0 && y < CountY && z >= 0 && z < CountZ);
        }

        public bool IsValid(int index) {
            var xyz = Coords(index);
            int x = xyz.Item1;
            int y = xyz.Item2;
            int z = xyz.Item3;
            return IsValid(x, y, z);
        }

        public void ToModel(Preset preset) {
            string path = @$"Export\Models\";
            string filePath = $@"{path}{preset.Name}.obj";
            System.IO.Directory.CreateDirectory(path);
            Model m = new Model(filePath);

            for (int i = 0; i < Cells.Length; ++i) {
                var coords = Coords(i);
                Vector3 pivot = new Vector3(coords.Item1 * CellSize, coords.Item2 * CellSize, coords.Item3 * CellSize);
                m.Append(preset.Models[Cells[i]], pivot);
            }

            m.Export(filePath);
        }
    }

    /// <summary>
    /// TODO: Complete me / Exclude me
    /// </summary>
    class NonUniformGrid : IGrid {
        NonUniformGrid() {

        }

        public int GetCountX() {
            throw new NotImplementedException();
        }

        public int GetCountY() {
            throw new NotImplementedException();
        }

        public int GetCountZ() {
            throw new NotImplementedException();
        }

        public int[] GetCells() {
            throw new NotImplementedException();
        }

        public int Index(int x, int y, int z) {
            throw new NotImplementedException();
        }

        public (int, int, int) Coords(int index) {
            throw new NotImplementedException();
        }

        public bool IsValid(int x, int y, int z) {
            throw new NotImplementedException();
        }

        public bool IsValid(int index) {
            throw new NotImplementedException();
        }

        public void ToModel(Preset preset) {
            throw new NotImplementedException();
        }
    }
}
